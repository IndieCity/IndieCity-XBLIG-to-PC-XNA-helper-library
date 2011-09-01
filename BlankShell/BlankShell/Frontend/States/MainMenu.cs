//------------------------------------------------------------------------------
// Filename: MainMenu.cs
// Author: nholmes
// Summary: the main menu for the game - options, start game etc
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
using BlankShell.ExampleGame;

namespace BlankShell.Frontend
{
    //------------------------------------------------------------------------------
    // Class: MainMenu
    // Author: nholmes
    // Summary: responds to input from any connected gamepad, but whichever player 
    //          makes a selection is given control over all subsequent screens
    //------------------------------------------------------------------------------
    class MainMenu : Menu
    {
        //------------------------------------------------------------------------------
        // Function: MainMenu
        // Author: nholmes
        // Summary: constructor - creates the menu and all of the items it contains
        //------------------------------------------------------------------------------
        public MainMenu(Game game, PlayerIndex? controllingPlayer)
            : base(game, controllingPlayer, Color.White)
        {
            // set the title and it's alignment
            title = "Main Menu";
            alignment = Alignment.centre;

            // set this menu to be a fixed size in the middle of the screen
            SetArbitary((int)(displayManager.GameResolutionX * 0.5f) - 125, 360, 250, 250);

            // create our menu items
            ItemButton playGameMenuItem = new ItemButton(this, game, "Play Game", 20, 170, Alignment.centre, textFont, Color.White, Alignment.centre, textYOffset);
            ItemButton achievementsMenuItem = new ItemButton(this, game, "Achievements", 55, 170, Alignment.centre, textFont, Color.White, Alignment.centre, textYOffset);
            ItemButton leaderboardsMenuItem = new ItemButton(this, game, "Leaderboards", 90, 170, Alignment.centre, textFont, Color.White, Alignment.centre, textYOffset);
            ItemButton optionsMenuItem = new ItemButton(this, game, "Options", 125, 170, Alignment.centre, textFont, Color.White, Alignment.centre, textYOffset);
            ItemButton exitMenuItem = new ItemButton(this, game, "Exit", 160, 170, Alignment.centre, textFont, Color.White, Alignment.centre, textYOffset);

            // hook up the menu item event handlers
            playGameMenuItem.Selected += PlayGameMenuItemSelected;
            achievementsMenuItem.Selected += AchievementsMenuItemSelected;
            leaderboardsMenuItem.Selected += LeaderboardsMenuItemSelected;
            optionsMenuItem.Selected += OptionsMenuItemSelected;
            exitMenuItem.Selected += OnCancel;

            // add the items to the menu
            menuItems.Add(playGameMenuItem);
            menuItems.Add(achievementsMenuItem);
            menuItems.Add(leaderboardsMenuItem);
            menuItems.Add(optionsMenuItem);
            menuItems.Add(exitMenuItem);
        }

        //------------------------------------------------------------------------------
        // Function: PlayGameMenuItemSelected
        // Author: nholmes
        // Summary: event handler for when the Play Game menu item is selected
        //------------------------------------------------------------------------------
        void PlayGameMenuItemSelected(object sender, MenuItemEventArgs e)
        {
            // load the main game!
            gameStateManager.LoadNewStates(game, LoadingTexture, e.PlayerIndex, new MainGame(game, e.PlayerIndex));
        }

        //------------------------------------------------------------------------------
        // Function: AchievementsMenuItemSelected
        // Author: nholmes
        // Summary: event handler for when the achievements menu item is selected
        //------------------------------------------------------------------------------
        void AchievementsMenuItemSelected(object sender, MenuItemEventArgs e)
        {
            // add the options menu state to the top of the stack
            gameStateManager.AddGameState(new AchievementsMenu(game, e.PlayerIndex));
        }

        //------------------------------------------------------------------------------
        // Function: LeaderboardsMenuItemSelected
        // Author: nholmes
        // Summary: event handler for when the leaderboards menu item is selected
        //------------------------------------------------------------------------------
        void LeaderboardsMenuItemSelected(object sender, MenuItemEventArgs e)
        {
            // add the options menu state to the top of the stack
            gameStateManager.AddGameState(new LeaderboardsMenu(game, e.PlayerIndex));
        }

        //------------------------------------------------------------------------------
        // Function: OptionsMenuItemSelected
        // Author: nholmes
        // Summary: event handler for when the Options menu item is selected
        //------------------------------------------------------------------------------
        void OptionsMenuItemSelected(object sender, MenuItemEventArgs e)
        {
            // add the options menu state to the top of the stack
            gameStateManager.AddGameState(new OptionsMenu(game, e.PlayerIndex));
        }

        //------------------------------------------------------------------------------
        // Function: OnCancel
        // Author: nholmes
        // Summary: if the user cancels the main menu, exit the game
        //------------------------------------------------------------------------------
        protected override void OnCancel(PlayerIndex playerIndex)
        {
            game.Exit();
        }
    }
}
