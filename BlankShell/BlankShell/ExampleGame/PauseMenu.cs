//------------------------------------------------------------------------------
// Filename: PauseMenu.cs
// Author: nholmes
// Summary: pauses the game and gives the player chance to alter options etc
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
using BlankShell.Frontend;

namespace BlankShell.ExampleGame
{
    //------------------------------------------------------------------------------
    // Class: PauseMenu
    // Author: nholmes
    // Summary: appears over the top of the game. player can choose to resume, edit
    //          options or exit the game#
    //------------------------------------------------------------------------------
    class PauseMenu : Menu
    {
        //------------------------------------------------------------------------------
        // Function: PauseMenu
        // Author: nholmes
        // Summary: constructor - creates the menu and adds all the menu items
        //------------------------------------------------------------------------------
        public PauseMenu(Game game, PlayerIndex? controllingPlayer)
            : base(game, controllingPlayer, Color.White)
        {
            // set the title and it's alignment
            title = "Pause";
            alignment = Alignment.centre;

            // set this menu to occupy the full screen
            SetCentered(250,180);

            // create our menu items
            ItemButton resumeMenuItem = new ItemButton(this, game, "Resume", 20, 170, Alignment.centre, textFont, Color.White, Alignment.centre, textYOffset);
            ItemButton optionsMenuItem = new ItemButton(this, game, "Options", 55, 170, Alignment.centre, textFont, Color.White, Alignment.centre, textYOffset);
            ItemButton quitMenuItem = new ItemButton(this, game, "Quit", 90, 170, Alignment.centre, textFont, Color.White, Alignment.centre, textYOffset);
            
            // hook up the menu event handlers
            resumeMenuItem.Selected += OnCancel;
            optionsMenuItem.Selected += OptionsMenuItemSelected;
            quitMenuItem.Selected += QuitMenuItemSelected;

            // add our items to the menu
            menuItems.Add(resumeMenuItem);
            menuItems.Add(optionsMenuItem);
            menuItems.Add(quitMenuItem);
        }

        //------------------------------------------------------------------------------
        // Function: OptionsMenuItemSelected
        // Author: nholmes
        // Summary: event handler for when the Options menu item is selected
        //------------------------------------------------------------------------------
        void OptionsMenuItemSelected(object sender, MenuItemEventArgs e)
        {
            // add the exit game stete to check if the user is really sure they want to exit
            gameStateManager.AddGameState(new OptionsMenu(game, e.PlayerIndex));
        }

        //------------------------------------------------------------------------------
        // Function: QuitMenuItemSelected
        // Author: nholmes
        // Summary: event handler for when the Quit menu item is selected
        //------------------------------------------------------------------------------
        void QuitMenuItemSelected(object sender, MenuItemEventArgs e)
        {
            // create a question box game state
            QuestionBox questionBox = new QuestionBox(game, e.PlayerIndex, "EXIT GAME", "Are you sure?", "No", "Yes");

            // hook up event handlers for the question box
            questionBox.OptionSelected += QuestionBoxOptionSelected;
            
            // add a qustion box state to check if the user is really sure they want to exit
            gameStateManager.AddGameState(questionBox);
        }

        //------------------------------------------------------------------------------
        // Function: QuestionBoxOptionSelected
        // Author: nholmes
        // Summary: event handler for when the question box selection is made 
        //------------------------------------------------------------------------------
        void QuestionBoxOptionSelected(object sender, QuestionBoxEventArgs e)
        {
            // which option was selected?
            if (e.SelectedOption == SelectedOption.optionA)
            {
                // user chose not to exit, do nothing
            }
            else
            {
                // exiting, trgger the frontend to load
                gameStateManager.LoadNewStates(game, LoadingTexture, null, new Background(game, null), new MainMenu(game, null));
            }
        }

        //------------------------------------------------------------------------------
        // Function: Draw
        // Author: nholmes
        // Summary: darkens the game down before drawing the menu as normal
        //------------------------------------------------------------------------------
        public override void Draw()
        {
            // darken the game screen behind the pause menu
            gameStateManager.DarkenBackground(TransitionAlpha * 2 / 3);

            // call the base draw functionality
            base.Draw();
        }
    }
}
