//------------------------------------------------------------------------------
// Filename: ItemText.cs
// Author: nholmes
// Summary: a menu item that displays a piece of text
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
    // Class: ItemText
    // Author: nholmes
    // Summary: a menu item that displays text. Can be selectable or not, aligned or
    //          specifically positioned within the menu
    //------------------------------------------------------------------------------
    class ItemText : MenuItem
    {
        // the text associated with this item  
        private string text;

        // which of the pre-loaded menu fonts to use to display the item's text
        private SpriteFont font;

        // colour to use when drawing the font
        private Color fontColor;

        //------------------------------------------------------------------------------
        // Function: ItemText
        // Author: nholmes
        // Summary: item constructor, allows user to specify text, vertical position, 
        //          alignment and font as well as whether the text is selectable  
        //------------------------------------------------------------------------------
        public ItemText(Menu menu, Game game, string text, int y, Alignment alignment, SpriteFont font, Color fontColor, int textOffset, bool selectable)
            : base(menu, game, textOffset)
        {
            // store the text font and font color to use
            this.text = text;
            this.font = font;
            this.fontColor = fontColor;

            // store whether this item is selectable
            this.selectable = selectable;

            // set the vy position of this text item
            position.Y = y;

            // calculate position based on alignment provided
            switch (alignment)
            {
                case Alignment.left:
                    position.X = 0;
                    break;
                case Alignment.centre:
                    position.X = (int)((menu.ItemArea.Width - font.MeasureString(text).X) / 2);
                    break;
                case Alignment.right:
                    position.X = (int)(menu.ItemArea.Width - font.MeasureString(text).X);
                    break;
                default:
                    position.X = 0;
                    break;
            }

            // store the size of this text item
            size = font.MeasureString(text);
        }

        //------------------------------------------------------------------------------
        // Function: ItemText
        // Author: nholmes
        // Summary: item constructor, allows user to specify text, position and font  
        //------------------------------------------------------------------------------
        public ItemText(Menu menu, Game game, string text, int x, int y, SpriteFont font, int textOffset, bool selectable)
            : base(menu, game, textOffset)
        {
            // store the text and font to use
            this.text = text;
            this.font = font;

            // store whether this text item is selectable
            this.selectable = selectable;

            // set the position of this text item
            position.X = x;
            position.Y = y;

            // store the size of this text item
            size = font.MeasureString(text);
        }
 
        //------------------------------------------------------------------------------
        // Function: Draw
        // Author: nholmes
        // Summary: draws the text item
        //------------------------------------------------------------------------------
        public override void Draw(SpriteBatch spriteBatch, bool isSelected)
        {
            Vector2 displayPosition = new Vector2();

            // calculate the display position of the text item
            displayPosition.X = (int)(menu.ItemArea.X + position.X);
            displayPosition.Y = (int)(menu.ItemArea.Y + position.Y + textOffset);
            
            // draw the text
            fontColor.A = menu.TransitionAlpha;
            spriteBatch.DrawString(font, text, displayPosition, fontColor);
        }
    }
}
