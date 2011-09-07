//------------------------------------------------------------------------------
// Filename: ItemSelector.cs
// Author: nholmes
// Summary: a menu item that allows selection between several different settings
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
using IndieCityXna.Common;

namespace BlankShell.Frontend
{
    //------------------------------------------------------------------------------
    // Class: ItemSelector
    // Author: nholmes
    // Summary: a menu item that displays a button with text on it and an arrow on
    //          either side that can be used to select between different options.
    //          Can be selectable or not, aligned or specifically positioned within
    //          the menu
    //------------------------------------------------------------------------------
    class ItemSelector : MenuItem
    {
        // the option text associated with this item  
        private string[] optionsText;

        // the index of the currently selected option
        public int selectedOption;

        // which of the pre-loaded menu fonts to use to display the item's text
        private SpriteFont font;

        // colour to use when drawing the font
        Color fontColor;

        // the width of the button
        int width;

        //------------------------------------------------------------------------------
        // Function: ItemSelector
        // Author: nholmes
        // Summary: item constructor, allows user to specify the width of the button,
        //          the options that can be selected from, the vertical position, the
        //          alignment and the font  
        //------------------------------------------------------------------------------
        public ItemSelector(Menu menu, Game game, string[] optionsText, int selectedOption, int y, int width, Alignment alignment, SpriteFont font, Color fontColor, int textOffset)
            : base(menu, game, textOffset)
        {
            // store the width of the button, options text, selected option and the font to use
            this.width = width;
            this.optionsText = optionsText;
            this.selectedOption = selectedOption;
            this.font = font;
            this.fontColor = fontColor;

            // set the vy position of this button item
            position.Y = y;

            // calculate position based on alignment provided
            switch (alignment)
            {
                case Alignment.left:
                    position.X = 0;
                    break;
                case Alignment.centre:
                    position.X = (menu.ItemArea.Width / 2) - (width / 2);
                    break;
                case Alignment.right:
                    position.X = width - (int)font.MeasureString(optionsText[selectedOption]).X;
                    break;
                default:
                    position.X = 0;
                    break;
            }

            // store the size of this button item
            size.X = width;
            size.Y = menu.ButtonTexture.Height;
        }

        //------------------------------------------------------------------------------
        // Function: ItemSelector
        // Author: nholmes
        // Summary: item constructor, allows user to specify the width of the button, 
        //          the text to use, the position and the font  
        //------------------------------------------------------------------------------
        public ItemSelector(Menu menu, Game game, string[] optionsText, int selectedOption, int x, int y, int width, SpriteFont font, Color fontColor, int textOffset)
            : base(menu, game, textOffset)
        {
            // store the width of the button and the text, font and font color to use
            this.width = width;
            this.optionsText = optionsText;
            this.selectedOption = selectedOption;
            this.font = font;
            this.fontColor = fontColor;

            // set the position of this button item
            position.X = x;
            position.Y = y;

            // store the size of this button item
            size.X = width;
            size.Y = menu.ButtonTexture.Height;
        }
 
        //------------------------------------------------------------------------------
        // Function: IncreaseSelection
        // Author: nholmes
        // Summary: increase the currently selected option
        //------------------------------------------------------------------------------
        public int IncreaseSelection()
        {
            // increase the index of the selected option
            selectedOption ++;

            // if we've reached the end of the list of options, wrap around to zero
            if (selectedOption == optionsText.Length) selectedOption =0;

            // return the index of the selected option
            return selectedOption;
        }

        //------------------------------------------------------------------------------
        // Function: DecreaseSelection
        // Author: nholmes
        // Summary: decrease the currently selected option
        //------------------------------------------------------------------------------
        public int DecreaseSelection()
        {
            // decrease the index of the selected option
            selectedOption --;

            // if we've gone past the start of the list of options, wrap around to the top
            if (selectedOption < 0 ) selectedOption = optionsText.Length - 1;

            // return the index of the selected optiom
            return selectedOption;
        }

        //------------------------------------------------------------------------------
        // Function: OnPointerSelection
        // Author: nholmes
        // Summary: check if the user has clicked in the left or right areas of the selector
        //------------------------------------------------------------------------------
        public override MenuItemSelectionType OnPointerSelection(Vector2 point)
        {
            // is the supplied point within the shape defined by the left arrow?
            if ((point.X > (menu.ItemArea.X + position.X - menu.SelectorLeftTexture.Width)) && (point.X < (menu.ItemArea.X + position.X)) &&
                (point.Y > (menu.ItemArea.Y + position.Y)) && (point.Y < (menu.ItemArea.Y + position.Y + menu.SelectorLeftTexture.Height)))
            {
                return MenuItemSelectionType.previousOption;
            }

            // is the supplied point within the shape defined by the right arrow?
            if ((point.X > (menu.ItemArea.X + position.X + width)) && (point.X < (menu.ItemArea.X + position.X + width + menu.SelectorRightTexture.Width)) &&
                (point.Y > (menu.ItemArea.Y + position.Y)) && (point.Y < (menu.ItemArea.Y + position.Y + menu.SelectorRightTexture.Height)))
            {
                return MenuItemSelectionType.nextOption;
            }

            // not selected - return failure
            return MenuItemSelectionType.none;
        }

        //------------------------------------------------------------------------------
        // Function: Draw
        // Author: nholmes
        // Summary: draws the selector item
        //------------------------------------------------------------------------------
        public override void Draw(SpriteBatch spriteBatch, bool isSelected)
        {
            Rectangle shape = new Rectangle();
            
            // draw selected items in yellow, otherwise white
            Color color = menu.GetItemColor(isSelected);
            
            // modify the alpha to fade button out during transitions
            color = new Color(color.R, color.G, color.B, menu.TransitionAlpha);

            // draw the left arrow
            shape.Width = (int)menu.SelectorLeftTexture.Width;
            shape.Height = (int)menu.SelectorLeftTexture.Height;
            shape.X = (int)(menu.ItemArea.X + position.X - shape.Width);
            shape.Y = (int)(menu.ItemArea.Y + position.Y);
            spriteBatch.Draw(menu.SelectorLeftTexture, shape, color);
            
            // draw the left end
            shape.X = (int)(menu.ItemArea.X + position.X);
            shape.Y = (int)(menu.ItemArea.Y + position.Y);
            shape.Width = (int)menu.ButtonLeftTexture.Width;
            shape.Height = (int)menu.ButtonLeftTexture.Height;
            spriteBatch.Draw(menu.ButtonLeftTexture, shape, color);

            // draw the middle
            shape.X += shape.Width;
            shape.Width = (int)(width - (menu.ButtonLeftTexture.Width + menu.ButtonRightTexture.Width));
            shape.Height = (int)menu.ButtonTexture.Height;
            spriteBatch.Draw(menu.ButtonTexture, shape, color);

            // draw the right end
            shape.X += shape.Width;
            shape.Width = (int)menu.ButtonRightTexture.Width;
            shape.Height = (int)menu.ButtonRightTexture.Height;
            spriteBatch.Draw(menu.ButtonRightTexture, shape, color);

            // draw the right arrow
            shape.X += shape.Width;
            shape.Width = (int)menu.SelectorRightTexture.Width;
            shape.Height = (int)menu.SelectorRightTexture.Height;
            spriteBatch.Draw(menu.SelectorRightTexture, shape, color);

            // draw the text
            Vector2 fontPosition = new Vector2(menu.ItemArea.X + position.X + (int)(width / 2) - (int)(font.MeasureString(optionsText[selectedOption]).X / 2), menu.ItemArea.Y + position.Y + textOffset);
            fontColor.A = menu.TransitionAlpha;
            spriteBatch.DrawString(font, optionsText[selectedOption], fontPosition, fontColor);
        }
    }
}
