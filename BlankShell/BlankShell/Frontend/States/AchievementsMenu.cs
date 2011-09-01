//------------------------------------------------------------------------------
// Filename: AchievementsMenu.cs
// Author: nholmes
// Summary: the achieveemnts menu - allows user to view the game's achievements
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
using Microsoft.Xna.Framework.Input;
using IndieCityXna.Common;
using ICAchievements;

namespace BlankShell.Frontend
{
    //------------------------------------------------------------------------------
    // Class: AchievementsMenu
    // Author: nholmes
    // Summary: gives the user a chance to view the game's achievement list
    //------------------------------------------------------------------------------
    class AchievementsMenu : Menu
    {
        // the menu items that this menu uses
        private ItemButton backMenuItem;
        
        // indiecity achivement list display class
        ICAchievementList achievementList;

        //------------------------------------------------------------------------------
        // Function: AchievementsMenu
        // Author: nholmes
        // Summary: constructor - creates the menu and adds all of the menu items
        //------------------------------------------------------------------------------
        public AchievementsMenu(Game game, PlayerIndex? controllingPlayer)
            : base(game, controllingPlayer, Color.White)
        {
            // set the title and it's alignment
            title = "Achievements";
            alignment = Alignment.centre;

            // set this menu to the required size
            SetCentered(640, 600);

            // create our menu items
            backMenuItem = new ItemButton(this, game, "Back", 500, 200, Alignment.centre, textFont, Color.White, Alignment.centre, textYOffset);

            // hook up the menu event handlers
            backMenuItem.Selected += OnCancel;

            // add items to the menu
            menuItems.Add(backMenuItem);

            // get the achievment list display service
            achievementList = (ICAchievementList)game.Services.GetService(typeof(ICAchievementList));
        }

        //------------------------------------------------------------------------------
        // Function: Draw
        // Author: nholmes
        // Summary: constructor - creates the menu and adds all of the menu items
        //------------------------------------------------------------------------------
        public override void Draw()
        {
            // call the base draw functionality
            base.Draw();

            // only draw the achievement list when the menu is fully transitioned on
            if (transitionAmount != 0) return;

            // set the area of the screen that we wish the achievement list to be drawn into
            // for this example it's the same area as the menu occupies, with bit cut off at the top and bottom 
            // to make room for the title and the back button :)
            Rectangle displayArea = ItemArea;
            displayArea.Y += 32;
            displayArea.Height -= 64;

            // display achievement list and allow user to browse up and down through it
            if (achievementList != null)
            {
                achievementList.Draw(displayArea, 500, ICAchievementColorMode.inverted);
                if (inputManager.WasKeyPressed(Keys.Up, null)) achievementList.PageUp();
                if (inputManager.WasKeyPressed(Keys.Down, null)) achievementList.PageDown();
            }
        }
    }
}
