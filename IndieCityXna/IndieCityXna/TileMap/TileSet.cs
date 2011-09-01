//------------------------------------------------------------------------------
// Filename: TileMapData.cs
// Author: nholmes
// Summary: everthing needed to load and hold the map data for a tile layer
//------------------------------------------------------------------------------

//**********************************************************************************
//**********************************************************************************
// NOTE this code is WIP - it is designed to be used with my map editor which is not
// currently ready for public release. as i get it finished the source code for it
// will also be released as part of this package :)
//**********************************************************************************
//**********************************************************************************

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using System;

namespace IndieCityXna.TileMap
{
    //------------------------------------------------------------------------------
    // Class: TileSet
    // Author: nholmes
    // Summary: class for loading and holding a set of tile graphics
    //------------------------------------------------------------------------------
    public class TileSet
    {
        // the name of the tile set - useful for the editor, load and saving levels and for debugging
        private string name;

        // the file path where the tile set graphic is stored - useful for the editor, load and saving levels and for debugging
        private string path;

        // the width and height of the map data
        private Rectangle tile;
        
        // the actual data
        private Texture2D texture;

        // the number of tiles per row in the texture
        private int tilesPerRow;

        //------------------------------------------------------------------------------
        // Function: TileSet
        // Author: nholmes
        // Summary: default constructor - loads the texture for a tile set and pre-calcs
        //          rows and columns etc
        //------------------------------------------------------------------------------
        public TileSet(ContentManager content, string filePath, string fileName, int tileSize)
        {
            // store the name of the tileset
            name = fileName;

            // store the file path of the tileset
            path = filePath;

            // store the tile width and height
            tile.Width = tileSize;
            tile.Height = tileSize;

            // load the tile set texture
            try
            {
                // strip the file type extension - content files are all xnb format and do not preserve their extensions
                fileName = fileName.Remove(fileName.LastIndexOf("."));

                // attempt to load the texture for the tile set
                texture = content.Load<Texture2D>(filePath + fileName);
            }
            catch //(Exception e)
            {
                // dont report this error unless we are debugging - failing here is expected behaviour when loading
                // a level into the editor as we will force the creation of the xnb files after this stage and load
                // them at that point. In game this should always pass as the assets are created at compile time by
                // the normal content build process
                // System.Windows.Forms.MessageBox.Show(e.Message);
                
                // failed to load the tileset texture - set tiles per row to zero
                // and null the texture so we can catch the error later
                tilesPerRow = 0;
                texture = null;
                return;
            }

            // calculate how many tiles there are in each row within the texture
            tilesPerRow = (int)(texture.Width / tile.Width);
        }

        //------------------------------------------------------------------------------
        // Function: VerifyTileSet
        // Author: nholmes
        // Summary: checks if the tileset was correctly initialised
        //------------------------------------------------------------------------------
        public bool VerifyTileSet()
        {
            // check if the texture pointer is null and return false if it is
            if (texture == null) return false;

            // tileset is valud, return true
            return true;
        }

        //------------------------------------------------------------------------------
        // Function: GetTile
        // Author: nholmes
        // Summary: gets the rectangle for a specific tile index from the texture
        //------------------------------------------------------------------------------
        public Rectangle GetTile(int index)
        {
            // figure out which row the tile is on within the texture
            int row = index / tilesPerRow;
            
            // set the x and y position of the rectangle that defines the tile
            tile.X = (index - (row * tilesPerRow)) * tile.Width;
            tile.Y = row * tile.Height;
            
            // return the tile's rectangle
            return tile;
        }

        //------------------------------------------------------------------------------
        // Function: TileSize
        // Author: nholmes
        // Summary: returns the size of the tiles in this tile set
        //------------------------------------------------------------------------------
        public int TileSize
        {
            get { return tile.Width; }
        }

        //------------------------------------------------------------------------------
        // Function: Texture
        // Author: nholmes
        // Summary: returns the texture that contains all of the tile graphics
        //------------------------------------------------------------------------------
        public Texture2D Texture
        {
            get { return texture; }
        }

        //------------------------------------------------------------------------------
        // Function: Name
        // Author: nholmes
        // Summary: returns the name of the tile set
        //------------------------------------------------------------------------------
        public string Name
        {
            get { return name; }
        }

        //------------------------------------------------------------------------------
        // Function: Path
        // Author: nholmes
        // Summary: returns the file path of the tile set
        //------------------------------------------------------------------------------
        public string Path
        {
            get { return path; }
        }
    }
}