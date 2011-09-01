//------------------------------------------------------------------------------
// Filename: OptionsMenu.cs
// Author: nholmes
// Summary: the options menu - allows user to change game settings
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

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using IndieCityXna.Common;

namespace BlankShell.Frontend
{
    //------------------------------------------------------------------------------
    // Class: OptionsMenu
    // Author: nholmes
    // Summary: gives the user a chance to configure the game
    //------------------------------------------------------------------------------
    class OptionsMenu : Menu
    {
        // the menu items that this menu uses
        private ItemSelector screenModeItem;
        private ItemSlider musicVolumeItem;
        private ItemSlider sfxVolumeItem;
        private ItemButton backMenuItem;

        // screeb mode selection strings
        private string[] screenModeList = { "Windowed", "Full Screen" };
        
        //------------------------------------------------------------------------------
        // Function: OptionsMenu
        // Author: nholmes
        // Summary: constructor - creates the menu and adds all of the menu items
        //------------------------------------------------------------------------------
        public OptionsMenu(Game game, PlayerIndex? controllingPlayer)
            : base(game, controllingPlayer, Color.White)
        {
            // set the title and it's alignment
            title = "Options";
            alignment = Alignment.centre;

            // set this menu to the required size
            SetCentered(320, 210);

            // create our menu items
            screenModeItem = new ItemSelector(this, game, screenModeList, 0, 20, 200, Alignment.centre, textFont, Color.White, textYOffset);
            musicVolumeItem = new ItemSlider(this, game, 0, 100, 75, 55, 200, Alignment.centre, textFont, Color.White, textYOffset);
            sfxVolumeItem = new ItemSlider(this, game, 0, 100, 75, 90, 200, Alignment.centre, textFont, Color.White, textYOffset);
            backMenuItem = new ItemButton(this, game, "Back", 125, 200, Alignment.centre, textFont, Color.White, Alignment.centre, textYOffset);

            // hook up the menu event handlers
            backMenuItem.Selected += OnCancel;
            screenModeItem.NextOptionWanted += IncreaseScreenMode;
            screenModeItem.PreviousOptionWanted += DecreaseScreenMode;
            musicVolumeItem.ValueIncreased += IncreaseMusicVolume;
            musicVolumeItem.ValueDecreased += DecreaseMusicVolume;
            sfxVolumeItem.ValueIncreased += IncreaseSfxVolume;
            sfxVolumeItem.ValueDecreased += DecreaseSfxVolume;

            // add items to the menu
            menuItems.Add(screenModeItem);
            menuItems.Add(musicVolumeItem);
            menuItems.Add(sfxVolumeItem);
            menuItems.Add(backMenuItem);
        }

        //------------------------------------------------------------------------------
        // Function: IncreaseScreenMode
        // Author: nholmes
        // Summary: increase the currently selected screen mode
        //------------------------------------------------------------------------------
        public void IncreaseScreenMode(object sender, MenuItemEventArgs e)
        {
            // increase the option and get the new selection
            int currentSelection = screenModeItem.IncreaseSelection();

            // set full screen or windowed mode according to the current selection
            if ((ScreenMode)currentSelection == ScreenMode.FullScreen)
            {
                displayManager.SetScreenMode(ScreenMode.FullScreen);
            }
            else
            {
                displayManager.SetScreenMode(ScreenMode.Windowed);
            }
        }

        //------------------------------------------------------------------------------
        // Function: DecreaseScreenMode
        // Author: nholmes
        // Summary: decrease the currently selected screen mode
        //------------------------------------------------------------------------------
        public void DecreaseScreenMode(object sender, MenuItemEventArgs e)
        {
            // increase the option and get the new selection
            int currentSelection = screenModeItem.IncreaseSelection();

            // set full screen or windowed mode according to the current selection
            if ((ScreenMode)currentSelection == ScreenMode.FullScreen)
            {
                displayManager.SetScreenMode(ScreenMode.FullScreen);
            }
            else
            {
                displayManager.SetScreenMode(ScreenMode.Windowed);
            }
        }

        //------------------------------------------------------------------------------
        // Function: IncreaseMusicVolume
        // Author: nholmes
        // Summary: increase the music volume
        //------------------------------------------------------------------------------
        public void IncreaseMusicVolume(object sender, MenuItemEventArgs e)
        {
            // increase the option and set the new music volume
            int currentMusicVolume = musicVolumeItem.IncreaseValue();

            // todo - set the correct music mode
        }

        //------------------------------------------------------------------------------
        // Function: DecreaseMusicVolume
        // Author: nholmes
        // Summary: decrease the music volume
        //------------------------------------------------------------------------------
        public void DecreaseMusicVolume(object sender, MenuItemEventArgs e)
        {
            // decrease the option and set the new music volume
            int currentMusicVolume = musicVolumeItem.DecreaseValue();

            // todo - set the correct music mode
        }

        //------------------------------------------------------------------------------
        // Function: IncreaseSfxVolume
        // Author: nholmes
        // Summary: increase the music volume
        //------------------------------------------------------------------------------
        public void IncreaseSfxVolume(object sender, MenuItemEventArgs e)
        {
            // increase the option and set the new music volume
            int currentSfxVolume = sfxVolumeItem.IncreaseValue();

            // todo - set the correct music mode
        }

        //------------------------------------------------------------------------------
        // Function: DecreaseSfxVolume
        // Author: nholmes
        // Summary: decrease the music volume
        //------------------------------------------------------------------------------
        public void DecreaseSfxVolume(object sender, MenuItemEventArgs e)
        {
            // decrease the option and set the new music volume
            int currentSfxVolume = sfxVolumeItem.DecreaseValue();

            // todo - set the correct music mode
        }
    }
}
