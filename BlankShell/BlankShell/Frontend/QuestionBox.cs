//------------------------------------------------------------------------------
// Filename: ExitGame.cs
// Author: nholmes
// Summary: menu that appears when the needs to make a choice
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
using Microsoft.Xna.Framework.Graphics;

namespace BlankShell.Frontend
{
    // enum that defines the options that can be selected
    enum SelectedOption
    {
        optionA,
        optionB,
    };

    //------------------------------------------------------------------------------
    // Class: QuestionBox
    // Author: nholmes
    // Summary: displays a question and allows the user to select from two responses 
    //------------------------------------------------------------------------------
    class QuestionBox : Menu
    {
        // event raised when an option is selected
        public event EventHandler<QuestionBoxEventArgs> OptionSelected;
        
        //------------------------------------------------------------------------------
        // Class: QuestionBox
        // Author: nholmes
        // Summary: constructor - creates the menu as a pop-up and adds all the items
        //------------------------------------------------------------------------------
        public QuestionBox(Game game, PlayerIndex? controllingPlayer, 
            string title, string question, string optionA, string optionB)
            : base(game, controllingPlayer, Color.White)
        {
            // set the title and it's alignment
            this.title = title;
            alignment = Alignment.centre;

            // figure out the width of the menu
            int width = (int)textFont.MeasureString(question).X + 80;
            
            // check if the width is below the minimum allowed and set it to the minimum if it is
            if (width < 300)
            {
                width = 300;
            }

            // set this menu to be centered at a fixed size
            SetCentered(width, 180);

            // create our menu items
            ItemText questionTextItem = new ItemText(this, game, question, 20, Alignment.centre, textFont, Color.White, textYOffset, false);
            ItemButton continueMenuItem = new ItemButton(this, game, optionA, 55, 170, Alignment.centre, textFont, Color.Black, Alignment.centre, textYOffset);
            ItemButton exitMenuItem = new ItemButton(this, game, optionB, 90, 170, Alignment.centre, textFont, Color.Black, Alignment.centre, textYOffset);

            // hook up the menu item event handlers
            continueMenuItem.Selected += OptionASelected;
            exitMenuItem.Selected += OptionBSelected;

            // add the items to the menu
            menuItems.Add(questionTextItem);
            menuItems.Add(continueMenuItem);
            menuItems.Add(exitMenuItem);
        }

        //------------------------------------------------------------------------------
        // Class: OptionASelected
        // Author: nholmes
        // Summary: triggered if the user selcects option A
        //------------------------------------------------------------------------------
        void OptionASelected(object sender, MenuItemEventArgs e)
        {
            // call the question box option selected event handler with option A selected
            OptionSelected(this, new QuestionBoxEventArgs(SelectedOption.optionA));

            // close the question box
            CloseState();
        }

        //------------------------------------------------------------------------------
        // Class: OptionBSelected
        // Author: nholmes
        // Summary: triggered if the user selcects option B
        //------------------------------------------------------------------------------
        void OptionBSelected(object sender, MenuItemEventArgs e)
        {
            // call the question box option selected event handler with option B selected
            OptionSelected(this, new QuestionBoxEventArgs(SelectedOption.optionB));

            // close the question box
            CloseState();
        }

        //------------------------------------------------------------------------------
        // Function: Draw
        // Author: agreen
        // Summary: darkens the game / underlying menus before drawing the popup 
        //          menu as normal
        //------------------------------------------------------------------------------
        public override void Draw()
        {
            // call the base draw functionality
            base.Draw();
        }
    }
}
