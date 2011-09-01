//------------------------------------------------------------------------------
// Filename: TileMap.cs
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

namespace IndieCityXna.TileMap
{
    //------------------------------------------------------------------------------
    // Class: TileMap
    // Author: nholmes
    // Summary: class for loading and holding map data for a single tile layer
    //------------------------------------------------------------------------------
    public class TileMap
    {
        // the name of the tile map - useful for the editor, load and saving levels and for debugging
        private string name;

        // the width and height of the map data
        private int width;
        private int height;

        // the actual data
        private int[] mapData;

        // constants for setting and checking flips and rotations for tiles in the map
        public const int FLIP_X             = (1 << 27);
        public const int FLIP_Y             = (1 << 28);
        public const int ROTATION_90        = (1 << 29);
        public const int ROTATION_180       = (1 << 30);
        public const int STRIP_TILE_INDEX   = ~(FLIP_X + FLIP_Y + ROTATION_90 + ROTATION_180);
        public const int ROTATION_STRIP     = (ROTATION_90 + ROTATION_180);
        public const int ROTATION_SHIFT     = (29);
        public const int ROTATION_CLEAR     = ~(ROTATION_90 + ROTATION_180);

        //------------------------------------------------------------------------------
        // Function: TileMap
        // Author: nholmes
        // Summary: creates the tile map
        //------------------------------------------------------------------------------
        public TileMap(string mapName, int mapWidth, int mapHeight)
        {
            // store the name of the map
            name = mapName;

            // hacky contructor that just bulds some default data so we can get the layer code written
            width = mapWidth;
            height = mapHeight;
            
            // initialise the map data
            mapData = new int[width * height];
        }

        //------------------------------------------------------------------------------
        // Function: ClearMapData
        // Author: nholmes
        // Summary: clears the entire map
        //------------------------------------------------------------------------------
        public void ClearMapData(int tileIndex)
        {
            // set every tile index in the map to zero
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    mapData[(y * width) + x] = tileIndex;
                }
            }
        }

        //------------------------------------------------------------------------------
        // Function: GetTileIndex
        // Author: nholmes
        // Summary: gets the index of the tile at a given x and y position in the map
        //------------------------------------------------------------------------------
        public int GetTileIndex(int x, int y)
        {
            // check that the position is within the map
            if (x < 0 || x >= width) return 0;
            if (y < 0 || y >= height) return 0;

            // read from the map data
            return mapData[(y * width) + x];
        }

        //------------------------------------------------------------------------------
        // Function: SetTileIndex
        // Author: nholmes
        // Summary: sets the index of the tile at a given x and y position in the map
        //------------------------------------------------------------------------------
        public void SetTileIndex(int x, int y, int tileIndex)
        {
            // check that the position is within the map
            if (x < 0 || x >= width) return;
            if (y < 0 || y >= height) return;
 
            // write to the map data
            mapData[(y * width) + x] = tileIndex;
        }

        //------------------------------------------------------------------------------
        // Function: FlipTileX
        // Author: nholmes
        // Summary: flips the tile in x - takes into account it's current flipped status
        //------------------------------------------------------------------------------
        public void FlipTileX(int x, int y)
        {
            // is the tile already flipped in X?
            if ((mapData[(y * width) + x] & FLIP_X) == 0)
            {
                // nope, flip it now
                mapData[(y * width) + x] |= FLIP_X;
            }
            else
            {
                // yup, un-flip it
                mapData[(y * width) + x] &= ~FLIP_X;
            }
        }

        //------------------------------------------------------------------------------
        // Function: FlipTileX
        // Author: nholmes
        // Summary: flips the tile in x - takes into account it's current flipped status
        //------------------------------------------------------------------------------
        public int FlipTileX(int tile)
        {
            // is the tile already flipped in X?
            if ((tile & FLIP_X) == 0)
            {
                // nope, flip it now
                tile |= FLIP_X;
            }
            else
            {
                // yup, un-flip it
                tile &= ~FLIP_X;
            }

            // return the flipped result
            return tile;
        }

        //------------------------------------------------------------------------------
        // Function: FlipTileY
        // Author: nholmes
        // Summary: flips the tile in y - takes into account it's current flipped status
        //------------------------------------------------------------------------------
        public void FlipTileY(int x, int y)
        {
            // is the tile already flipped in Y?
            if ((mapData[(y * width) + x] & FLIP_Y) == 0)
            {
                // nope, flip it now
                mapData[(y * width) + x] |= FLIP_Y;
            }
            else
            {
                // yup, un-flip it
                mapData[(y * width) + x] &= ~FLIP_Y;
            }
        }

        //------------------------------------------------------------------------------
        // Function: FlipTileY
        // Author: nholmes
        // Summary: flips the tile in y - takes into account it's current flipped status
        //------------------------------------------------------------------------------
        public int FlipTileY(int tile)
        {
            // is the tile already flipped in Y?
            if ((tile & FLIP_Y) == 0)
            {
                // nope, flip it now
                tile |= FLIP_Y;
            }
            else
            {
                // yup, un-flip it
                tile &= ~FLIP_Y;
            }

            // returnt the flipped result
            return tile;
        }

        //------------------------------------------------------------------------------
        // Function: Width
        // Author: nholmes
        // Summary: returns the width of the map (in tiles)
        //------------------------------------------------------------------------------
        public int Width
        {
            get { return width; }
        }

        //------------------------------------------------------------------------------
        // Function: Height
        // Author: nholmes
        // Summary: returns the height of the map (in tiles)
        //------------------------------------------------------------------------------
        public int Height
        {
            get { return height; }
        }

        //------------------------------------------------------------------------------
        // Function: Name
        // Author: nholmes
        // Summary: returns the name of the tile map
        //------------------------------------------------------------------------------
        public string Name
        {
            get { return name; }
        }
    }
}