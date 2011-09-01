//------------------------------------------------------------------------------
// Filename: TileLayer.cs
// Author: nholmes
// Summary: everthing needed to display a single 2d tile mapped layer
//------------------------------------------------------------------------------

//**********************************************************************************
//**********************************************************************************
// NOTE this code is WIP - it is designed to be used with my map editor which is not
// currently ready for public release. as i get it finished the source code for it
// will also be released as part of this package :)
//**********************************************************************************
//**********************************************************************************

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using IndieCityXna.Common;

namespace IndieCityXna.TileMap
{
    // enumeration for different tile layer position modes
    public enum TileLayerMode
    {
        Static = 0,
        Forced,
        Follow,
        Relative,
        Scaled
    }

    //------------------------------------------------------------------------------
    // Class: TileLayer
    // Author: nholmes
    // Summary: class for creating and dsplaying a 2D tile mapped layer
    //------------------------------------------------------------------------------
    public class TileLayer
    {
        // data that is saved

        // the name of the tile layer - useful for the editor, load and saving levels and for debugging
        private string name;

        // name of and reference to the tile set that contains all of the tiles
        private string tileSetName;
        private TileSet tileSet;

        // name of and reference to the tile map for this layer
        private string tileMapName;
        private TileMap tileMap;
        
        // the layer's mode (how to interpret the target's position to calculate this layer's position)
        private TileLayerMode mode;

        // the layer that this one uses to calculate it's position
        private TileLayer target;

        // color to tint the layer by
        private Color tintColor;

        // scale value to apply to parents position if layer mode is set to 'Scaled' also used by the 'forced'
        // mode as movement speed
        private Vector2 positionScale;

        // positional offset of the layer. used by all targeted layer modes
        private Vector2 positionOffset;

        // the current position of this layer (top left corner)
        private Vector2 position;

        // data that is not saved

        // the update status of the layer - used when calculating multi-layer scrolling updates
        bool updated;

        // the current scale being applied to this layer
        private float displayScale;

        // the width and height of the display area for this tile layer (usually the screen size)
        private Vector2 displaySize;

        // the number of tiles to display in x and y to fill the display area (plus 1 extra row of tiles in each direction for scrolling)
        private int displayTilesX;
        private int displayTilesY;

        // vertex and triangle lists used to render the layer
        VertexPositionColorTexture[,] renderVertices;

        // handle to the display manager service
        private DisplayManager displayManager;
 
        //------------------------------------------------------------------------------
        // Function: TileLayer
        // Author: nholmes
        // Summary: constructor to use to create a blank tile layer
        //------------------------------------------------------------------------------
        public TileLayer(Game game, string layerName)
        {
            // get a handle to the display manager service
            displayManager = (DisplayManager)game.Services.GetService(typeof(DisplayManager));
            
            // store the layer's name
            name = layerName;
            
            // store the name of the tile set and a reference to them
            tileSetName = "";
            tileSet = null;

            // store the name of the tile map and a reference to them
            tileMapName = "";
            tileMap = null;

            // store the layer mode
            mode = TileLayerMode.Follow;

            // set the target layer to be undefined
            target = null;

            // default tint color is white
            tintColor = new Color(255, 255, 255, 255);

            // clear the updated status
            updated = false;

            // store the scale
            displayScale = 1.0f;

            // store the position scale (this will be ignored if position mode is anything other than 'Scaled'
            positionScale = new Vector2(1.0f, 1.0f);

            // store the position offset (used by all targeted layer modes)
            positionOffset = new Vector2(0.0f, 0.0f);
        }

        //------------------------------------------------------------------------------
        // Function: TileLayer
        // Author: nholmes
        // Summary: constructor to use when data is available - sets the map data to use,
        //          graphics to use, the size of the tiles and how the layer moves in 
        //          relation to it's parent position
        //------------------------------------------------------------------------------
        public TileLayer(string layerName, TileSet tileSet, TileMap tileMap, TileLayerMode layerMode, TileLayer targetLayer, float displayScale, Vector2 positionScale, Vector2 positionOffset, Vector2 displaySize, Color tintColor)
        {
            // store the layer's name
            this.name = layerName;

            // store the name of the tile set and a reference to them
            tileSetName = tileSet.Name;
            this.tileSet = tileSet;

            // store the name of the tile map and a reference to them
            tileMapName = tileMap.Name;
            this.tileMap = tileMap;

            // store the layer mode
            this.mode = layerMode;

            // store the target layer
            target = targetLayer;

            // clear the updated status
            updated = false;

            // store the scale
            this.displayScale = displayScale;

            // store the position scale (this will be ignored if position mode is anything other than 'Scaled'
            this.positionScale = positionScale;

            // store the position offset (used by all targeted layer modes)
            this.positionOffset = positionOffset;

            // precalculate values used for displaying the tile layer
            SetDisplaySize(displaySize);

            // set the tint color
            this.tintColor = tintColor;
        }

        //------------------------------------------------------------------------------
        // Function: SetDisplaySize
        // Author: nholmes
        // Summary: precalculates the number of tiles that need to be drawn to fill the
        //          specified display area. this must be called whenever the display
        //          area is changed
        //------------------------------------------------------------------------------
        public void SetDisplaySize(Vector2 displaySize)
        {
            // store the display size for future reference
            this.displaySize = displaySize;

            // calculate the number of tiles we need to draw to fill the display area 
            displayTilesX = (int)(displaySize.X / (tileSet.TileSize * displayScale));
            displayTilesY = (int)(displaySize.Y / (tileSet.TileSize * displayScale));

            // if the x or y display size is not exactly divisible by the tile size then we need to add an additional row to ensure the screen is filled
            if ((displaySize.X % tileSet.TileSize) > 0) displayTilesX += 1;
            if ((displaySize.Y % tileSet.TileSize) > 0) displayTilesY += 1;

            // need to add an additional row of tiles to allow for sub tile position in the scrolling
            displayTilesX += 1;
            displayTilesY += 1;

            // create the list of vertices we will use to render the tile layer 
            renderVertices = new VertexPositionColorTexture[(displayTilesX * displayTilesY),4];

            // pre-bake the color tint into all of the vertices
            for (int vertexGroup = 0; vertexGroup < (displayTilesX * displayTilesY); vertexGroup++)
            {
                for (int vertex = 0; vertex < 4; vertex ++) renderVertices[vertexGroup, vertex].Color = tintColor;
            }
        }

        //------------------------------------------------------------------------------
        // Function: DisplayScale
        // Author: nholmes
        // Summary: gets and sets the display scale to use, automatically updates the display size as well
        //------------------------------------------------------------------------------
        public float DisplayScale
        {
            // set the new display scale
            get { return displayScale; }
            set { displayScale = value; SetDisplaySize(displaySize); }
        }

        //------------------------------------------------------------------------------
        // Function: Position
        // Author: nholmes
        // Summary: allows direct setting of the tile layer's position. useful for the 
        //          initial layer (where the player needs to control the position of the
        //          layer directly) and also for custom effects etc
        //------------------------------------------------------------------------------
        public Vector2 Position
        {
            get { return position; }
            set
            {
                // set the position
                position = value;

                // clamp the layer within allowed bounds (even directly setting position can't
                // let it go outside of the valid area!)
                ClampLayerPosition();
            }
        }

        //------------------------------------------------------------------------------
        // Function: PositionX
        // Author: nholmes
        // Summary: gets the X position of the layer
        //------------------------------------------------------------------------------
        public float PositionX
        {
            get { return position.X; }
            set
            {
                // set the position
                position.X = value;

                // clamp the layer within allowed bounds (even directly setting position can't
                // let it go outside of the valid area!)
                ClampLayerPosition();
            }
        }

        //------------------------------------------------------------------------------
        // Function: PositionY
        // Author: nholmes
        // Summary: gets the Y position of the layer
        //------------------------------------------------------------------------------
        public float PositionY
        {
            get { return position.Y; }
            set
            {
                // set the position
                position.Y = value;

                // clamp the layer within allowed bounds (even directly setting position can't
                // let it go outside of the valid area!)
                ClampLayerPosition();
            }
        }

        //------------------------------------------------------------------------------
        // Function: Update
        // Author: nholmes
        // Summary: helper function to update the positions of all other tile layers
        //          relative to the master tile layer. NOTE: assumes that you only call
        //          this function on the master tile layer!
        //------------------------------------------------------------------------------
        public void UpdateAllLayers(TileLayer[] tileLayers, int masterLayerIndex)
        {
            int tileLayer;

            // clear the update status of all tile layers
            for (tileLayer = 0; tileLayer < tileLayers.Length; tileLayer++) tileLayers[tileLayer].updated = false;

            // set the updated status of the master tileset to true
            tileLayers[masterLayerIndex].updated = true;
            
            // continue to process throught all the layers until they have all been updated (allows for crazy out-of-order map layers!)
            while (true)
            {
                // spin through all of the supplied layers and update any that refer to the master layer
                for (tileLayer = 0; tileLayer < tileLayers.Length; tileLayer++)
                {
                    // if this layer has been updated, skip to the next one
                    if (tileLayers[tileLayer].updated == true) continue;

                    // not updated yet, see if this layer has a target
                    if (tileLayers[tileLayer].target != null)
                    {
                        // has the target been updated already? bail out if not..
                        if (tileLayers[tileLayer].target.updated == false) continue;

                        // ok, time to update this layer!
                        switch (tileLayers[tileLayer].mode)
                        {
                            case TileLayerMode.Static:

                                // no nothing :)
                                break;

                            case TileLayerMode.Forced:

                                // use the position scale values to move the layer by a fixed amount each frame
                                tileLayers[tileLayer].position = tileLayers[tileLayer].PositionScale;
                                break;

                            case TileLayerMode.Follow:

                                // exactly match the target position
                                tileLayers[tileLayer].position = tileLayers[tileLayer].target.Position;
                                break;

                            case TileLayerMode.Relative:

                                // move relative to the target's position taking into account the size of both layers
                                tileLayers[tileLayer].position.X = tileLayers[tileLayer].target.Position.X * ((float)tileLayers[tileLayer].tileMap.Width / (float)tileLayers[tileLayer].target.tileMap.Width);
                                tileLayers[tileLayer].position.Y = tileLayers[tileLayer].target.Position.Y * ((float)tileLayers[tileLayer].tileMap.Height / (float)tileLayers[tileLayer].target.tileMap.Height);
                                break;

                            case TileLayerMode.Scaled:

                                // move as a scaled amount of the target's position
                                tileLayers[tileLayer].position = tileLayers[tileLayer].target.Position * tileLayers[tileLayer].positionScale;
                                break;
                        }

                        // clamp the layer to it's allowed bounds
                        tileLayers[tileLayer].ClampLayerPosition();

                        // flag this layer as updated
                        tileLayers[tileLayer].updated = true;
                    }
                    else
                    {
                        // check to see if this layer is a forced scrolling layer
                        if (mode == TileLayerMode.Forced)
                        {
                            // move the tile layer by the scale X and scale Y amounts
                            tileLayers[tileLayer].Position += tileLayers[tileLayer].PositionScale;

                            // clamp the layer's new position
                            tileLayers[tileLayer].ClampLayerPosition();
                        }

                        // if we get here then this layer has been update succesfully - flag it
                        tileLayers[tileLayer].updated = true;
                    }
                }

                // check the status of all the layers to see if we have finished
                for (tileLayer = 0; tileLayer < tileLayers.Length; tileLayer++)
                {
                    // bail out if we find a layer that has not yet been updated
                    if (tileLayers[tileLayer].updated == false) break;
                }

                // are they all completed? if so, break out of the while loop!
                if (tileLayer == tileLayers.Length) break;
            }
        }

        //------------------------------------------------------------------------------
        // Function: ClampLayerPosition
        // Author: nholmes
        // Summary: ensure that the layer position does not result in us trying to draw
        //          outside the bounds of the layer
        //------------------------------------------------------------------------------
        private void ClampLayerPosition()
        {
            int scaledTileSize = (int)(tileSet.TileSize * displayScale);

            //TODO add support for wrap clamping!

            // clamp to right edge first
            if ((position.X + displaySize.X) > (tileMap.Width * scaledTileSize))
                position.X = (tileMap.Width * scaledTileSize) - displaySize.X;

            // clamp to left edge second to ensure Y position never go less than 0
            if (position.X < 0) position.X = 0;

            // clamp to bottom edge next
            if ((position.Y + displaySize.Y) > (tileMap.Height * scaledTileSize))
                position.Y = (tileMap.Height * scaledTileSize) - displaySize.Y;

            // clamp to top edge last to ensure Y position never goes less than 0
            if (position.Y < 0) position.Y = 0;
        }

        //------------------------------------------------------------------------------
        // Function: Draw
        // Author: nholmes
        // Summary: draws the tile layer :)
        //------------------------------------------------------------------------------
        public void Draw(SpriteBatch spriteBatch)
        {
            // TODO add support for wrap drawing and positionOffset!
            
            // precalculate ready for rendering
            Vector2 origin = new Vector2(tileSet.TileSize / 2, tileSet.TileSize / 2);
            int scaledTileSize = (int)(tileSet.TileSize * displayScale);
            int scrollOffsetX = (((int)position.X) % scaledTileSize) - (scaledTileSize / 2);
            int scrollOffsetY = (((int)position.Y) % scaledTileSize) - (scaledTileSize / 2);
            int mapOffsetX = (int)(position.X / scaledTileSize);
            int mapOffsetY = (int)(position.Y / scaledTileSize);

            // create source and destination rectangles for drawing the tiles
            Rectangle sourceRect;
            Rectangle destRect = new Rectangle(0, 0, scaledTileSize, scaledTileSize);

            // begin drawing
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, displayManager.TransformMatrix);

            // draw all of the tiles
            for (int y = 0; (y < displayTilesY) && ((y + mapOffsetY) < tileMap.Height); y++)
            {
                for (int x = 0; (x < displayTilesX) && ((x + mapOffsetX) < tileMap.Width); x++)
                {
                    // get the tile data from the tile map
                    int tileData = tileMap.GetTileIndex(mapOffsetX + x, mapOffsetY + y);
                    
                    // get the tile graphic from the tile set texture (strip off any flip and rotation flags from the file data)
                    sourceRect = tileSet.GetTile(tileData & TileMap.STRIP_TILE_INDEX);

                    // set the dest rectangle
                    destRect.X = (int)((x * scaledTileSize) - scrollOffsetX);
                    destRect.Y = (int)((y * scaledTileSize) - scrollOffsetY);

                    // calculate the rotation of this tile
                    float rotation = ((tileData & TileMap.ROTATION_STRIP) >> TileMap.ROTATION_SHIFT) * (90.0f * 0.0174532925f);
                    
                    // is the tile flipped in X?
                    if ((tileData & TileMap.FLIP_X) == 0)
                    {
                        // is the tile flipped in Y?
                        if ((tileData & TileMap.FLIP_Y) == 0)
                        {
                            // not flipped at all - draw normally
                            spriteBatch.Draw(tileSet.Texture, destRect, sourceRect, tintColor, rotation, origin, SpriteEffects.None, 0);
                        }
                        else
                        {
                            // only flipped in Y - draw flipped vertically
                            spriteBatch.Draw(tileSet.Texture, destRect, sourceRect, tintColor, rotation, origin, SpriteEffects.FlipVertically, 0);
                        }
                    }
                    else
                    {
                        // is the tile flipped in Y?
                        if ((tileData & TileMap.FLIP_Y) == 0)
                        {
                            // only flipped in X - draw flipped horizontally
                            spriteBatch.Draw(tileSet.Texture, destRect, sourceRect, tintColor, rotation, origin, SpriteEffects.FlipHorizontally, 0);
                        }
                        else
                        {
                            // flipped in X and Y - draw flipped horizontally and vertically
                            spriteBatch.Draw(tileSet.Texture, destRect, sourceRect, tintColor, rotation, origin, SpriteEffects.FlipHorizontally | SpriteEffects.FlipVertically, 0);
                        }
                    }
                }
            }

            // finsih drawing
            spriteBatch.End();
        }

        //------------------------------------------------------------------------------
        // Function: Name
        // Author: nholmes
        // Summary: returns the name of the tile layer
        //------------------------------------------------------------------------------
        public string Name
        {
            get { return name; }
        }

        //------------------------------------------------------------------------------
        // Functgion: TileSet
        // Author: nholmes
        // Summary: gets and sets the tile set that this layer uses
        //------------------------------------------------------------------------------
        public TileSet TileSet
        {
            get { return tileSet; }
            set { tileSet = value; if (tileSet != null) tileSetName = tileSet.Name; }
        }

        //------------------------------------------------------------------------------
        // Functgion: TileMap
        // Author: nholmes
        // Summary: gets the tile map data
        //------------------------------------------------------------------------------
        public TileMap TileMap
        {
            get { return tileMap; }
            set { tileMap = value; if (tileMap != null) tileMapName = tileMap.Name; }
        }

        //------------------------------------------------------------------------------
        // Functgion: LayerMode
        // Author: nholmes
        // Summary: gets and sets the layer mode
        //------------------------------------------------------------------------------
        public TileLayerMode LayerMode
        {
            set { mode = value; }
            get { return mode; }
        }

        //------------------------------------------------------------------------------
        // Functgion: Target
        // Author: nholmes
        // Summary: gets and sets the targeted layer that this layer bases it's movement on
        //------------------------------------------------------------------------------
        public TileLayer Target
        {
            set { target = value; }
            get { return target; }
        }

        //------------------------------------------------------------------------------
        // Function: TintColor
        // Author: nholmes
        // Summary: gets and sets the layer's tint color
        //------------------------------------------------------------------------------
        public Color TintColor
        {
            get { return tintColor; }
            set { tintColor = value; }
        }

        //------------------------------------------------------------------------------
        // Functgion: PositionScale
        // Author: nholmes
        // Summary: gets and sets the position scale
        //------------------------------------------------------------------------------
        public Vector2 PositionScale
        {
            get { return positionScale; }
            set { positionScale = value; }
        }

        //------------------------------------------------------------------------------
        // Functgion: PositionOffset
        // Author: nholmes
        // Summary: gets and sets the position offset
        //------------------------------------------------------------------------------
        public Vector2 PositionOffset
        {
            get { return positionOffset; }
            set { positionOffset = value; }
        }

        //------------------------------------------------------------------------------
        // Functgion: positionOffsetX
        // Author: nholmes
        // Summary: gets and sets the X position offset
        //------------------------------------------------------------------------------
        public float PositionOffsetX
        {
            get { return positionOffset.X; }
            set { positionOffset.X = value; }
        }

        //------------------------------------------------------------------------------
        // Functgion: positionOffsetY
        // Author: nholmes
        // Summary: gets and sets the Y position offset
        //------------------------------------------------------------------------------
        public float PositionOffsetY
        {
            get { return positionOffset.Y; }
            set { positionOffset.Y = value; }
        }

        //------------------------------------------------------------------------------
        // Functgion: PositionScaleX
        // Author: nholmes
        // Summary: gets and sets the X position scale
        //------------------------------------------------------------------------------
        public float PositionScaleX
        {
            get { return positionScale.X; }
            set { positionScale.X = value; }
        }

        //------------------------------------------------------------------------------
        // Functgion: PositionScaleX
        // Author: nholmes
        // Summary: gets and sets the Y position scale
        //------------------------------------------------------------------------------
        public float PositionScaleY
        {
            get { return positionScale.Y; }
            set { positionScale.Y = value; }
        }
    }
}
