//------------------------------------------------------------------------------
// Filename: MenuEventArgs.cs
// Author: nholmes
// Summary: custom event argument handler for the menu system
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

namespace BlankShell.Frontend
{
    //------------------------------------------------------------------------------
    // Class: MenuItemEventArgs
    // Author: nholmes
    // Summary: custom event argument which includes the index of the player who
    //          triggered the event. used by the MenuItem.Selected event
    //------------------------------------------------------------------------------
    public class MenuItemEventArgs : EventArgs
    {
        // local storage for the player index being passed
        PlayerIndex playerIndex;

        //------------------------------------------------------------------------------
        // Function: MenuItemEventArgs
        // Author: nholmes
        // Summary: constructor
        //------------------------------------------------------------------------------
        public MenuItemEventArgs(PlayerIndex playerIndex)
        {
            this.playerIndex = playerIndex;
        }

        //------------------------------------------------------------------------------
        // Function: PlayerIndex
        // Author: nholmes
        // Summary: gets the index of the player who triggered this event
        //------------------------------------------------------------------------------
        public PlayerIndex PlayerIndex
        {
            get { return playerIndex; }
        }
    }
}
