//------------------------------------------------------------------------------
// Filename: SaveData.cs
// Author: nholmes
// Summary: class containing all data that the game wishes to save
//------------------------------------------------------------------------------

//**********************************************************************************
//**********************************************************************************
// NOTE this code is provided only as an example of how to use the indie city XNA 
// code. it is not part of the code base and is probably dirty, hacky and full of
// horrible bugs :P feel free to browse it, but do not judge it - most of it was
// written at this years global game jam with little or no sleep and no time to make
// it sensible or easy to follow. we'll improve this example code as and when we can
//**********************************************************************************
//**********************************************************************************

using System;
using Microsoft.Xna.Framework;

namespace BlankShell.ExampleGame
{
    //------------------------------------------------------------------------------
    // Class: SaveData
    // Author: nholmes
    // Summary: class containing all data that the game wishes to save
    //------------------------------------------------------------------------------
    [Serializable]
    public class SaveData
    {
        // NOTE: versionID MUST be updated if you make any changes this class or any other classes it uses
        // NOTE: all data in the section up to [NonSerialized] will be saved

        public PlayerData playerData = new PlayerData();
 
//        [NonSerialized]

        // NOTE: all data following [NonSerialized] will not be saved

        //------------------------------------------------------------------------------
        // Function: SaveData
        // Author: nholmes
        // Summary: constructs a new save data class and sets all data to default values
        //------------------------------------------------------------------------------
        public SaveData()
        {
            // set all save data to defaults by calling the reset function
            Reset();
        }

        //------------------------------------------------------------------------------
        // Function: SaveData
        // Author: nholmes
        // Summary: sets all data to default values
        //------------------------------------------------------------------------------
        public void Reset()
        {
            // set data defaults
            playerData.Reset();
        }
    }
}
