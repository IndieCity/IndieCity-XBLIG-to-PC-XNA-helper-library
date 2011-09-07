//------------------------------------------------------------------------------
// Filename: ItemButton.cs
// Author: nholmes
// Summary: a menu item that displays a button with text on it
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
    // Class: ItemButton
    // Author: nholmes
    // Summary: a menu item that displays a button with text on it. Can be selectable
    //          or not, aligned or specifically positioned within the menu
    //------------------------------------------------------------------------------
    class ItemButton : MenuItem
    {
        // the text associated with this item  
        private string text;

        // which of the pre-loaded menu fonts to use to display the item's text
        private SpriteFont font;

        // colour to use when drawing the font
        Color fontColor;

        // font alignment type
        Alignment fontAlignment;

        // the width of the button
        int width;

        //------------------------------------------------------------------------------
        // Function: ItemButton
        // Author: nholmes
        // Summary: item constructor, allows user to specify the width of the button,
        //          the text to use, the vertical position, the alignment and the font  
        //------------------------------------------------------------------------------
        public ItemButton(Menu menu, Game game, string text, int y, int width, Alignment alignment, SpriteFont font, Color fontColor, Alignment fontAlignment, int textOffset)
            : base(menu, game, textOffset)
        {
            // store the width of the button, text and font to use
            this.width = width;
            this.text = text;
            this.font = font;
            this.fontColor = fontColor;
            this.fontAlignment = fontAlignment;

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
                    position.X = width - (int)font.MeasureString(text).X;
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
        // Function: ItemButton
        // Author: nholmes
        // Summary: item constructor, allows user to specify the width of the button, 
        //          the text to use, the position and the font  
        //------------------------------------------------------------------------------
        public ItemButton(Menu menu, Game game, string text, int x, int y, int width, SpriteFont font, Color fontColor, int textOffset)
            : base(menu, game, textOffset)
        {
            // store the width of the button and the text, font and font color to use
            this.width = width;
            this.text = text;
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
        // Function: Draw
        // Author: nholmes
        // Summary: draws the button item
        //------------------------------------------------------------------------------
        public override void Draw(SpriteBatch spriteBatch, bool isSelected)
        {
            Rectangle shape = new Rectangle();
            Vector2 fontPosition;

            // draw selected items in yellow, otherwise white
            Color color = menu.GetItemColor(isSelected);
            
            // modify the alpha to fade button out during transitions
            color = new Color(color.R, color.G, color.B, menu.TransitionAlpha);

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
            shape.Width = menu.ButtonRightTexture.Width;
            shape.Height = menu.ButtonRightTexture.Height;
            spriteBatch.Draw(menu.ButtonRightTexture, shape, color);

            // calculate text postiion
            switch (fontAlignment)
            {
                case Alignment.left:

                    fontPosition = new Vector2(menu.ItemArea.X + menu.ButtonLeftTexture.Width, menu.ItemArea.Y + position.Y + textOffset);
                    break;

                case Alignment.right:

                    fontPosition = new Vector2(menu.ItemArea.X + width - (int)font.MeasureString(text).X - menu.ButtonRightTexture.Width, menu.ItemArea.Y + position.Y + textOffset);
                    break;

                case Alignment.centre:

                    fontPosition = new Vector2(menu.ItemArea.X + position.X + (int)(width / 2) - (int)(font.MeasureString(text).X / 2), menu.ItemArea.Y + position.Y + textOffset);
                    break;

                default:

                    fontPosition = new Vector2(0, 0);
                    break;
            }

            // draw the text
            fontColor.A = menu.TransitionAlpha;
            spriteBatch.DrawString(font, text, fontPosition, fontColor);
        }
    }
}
