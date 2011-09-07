//------------------------------------------------------------------------------
// Filename: ItemSlider.cs
// Author: nholmes
// Summary: a menu item that allows selection from a large range of values
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
    // Summary: a menu item that displays a slider that allows the user to select
    //          from a wide range of values. Can be selectable or not, aligned or 
    //          specifically positioned within the menu
    //------------------------------------------------------------------------------
    class ItemSlider : MenuItem
    {
        // the minimum selectable value
        private int minValue;

        // the maximum selectable value
        private int maxValue;

        // the currently selected value
        private int currentValue;

        // which of the pre-loaded menu fonts to use to display the item's text
        private SpriteFont font;

        // colour to use when drawing the font
        Color fontColor;

        // the width of the button
        int width;

        //------------------------------------------------------------------------------
        // Function: ItemSlider
        // Author: nholmes
        // Summary: item constructor, allows user to specify the width of the slider,
        //          the values that can be selected between, the vertical position, the
        //          alignment and the font  
        //------------------------------------------------------------------------------
        public ItemSlider(Menu menu, Game game, int minValue, int maxValue, int initialValue, int y, int width, Alignment alignment, SpriteFont font, Color fontColor, int textOffset)
            : base(menu, game, textOffset)
        {
            // store the width of the button, options text, selected option and the font to use
            this.width = width;
            this.minValue = minValue;
            this.maxValue = maxValue;
            this.currentValue = initialValue;
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
                    position.X = width;
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
        // Function: ItemSlider
        // Author: nholmes
        // Summary: item constructor, allows user to specify the width of the button, 
        //          the text to use, the position and the font  
        //------------------------------------------------------------------------------
        public ItemSlider(Menu menu, Game game, int minValue, int maxValue, int initialValue, int x, int y, int width, SpriteFont font, Color fontColor, int textOffset)
            : base(menu, game, textOffset)
        {
            // store the width of the button and the text, font and font color to use
            this.width = width;
            this.minValue = minValue;
            this.maxValue = maxValue;
            this.currentValue = initialValue;
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
        // Function: IncreaseValue
        // Author: nholmes
        // Summary: increase the current value
        //------------------------------------------------------------------------------
        public int IncreaseValue()
        {
            // increase the current value
            currentValue ++;

            // if we've gone past the maximum value, clamp it
            if (currentValue > maxValue) currentValue = maxValue;

            // return the current value
            return currentValue;
        }

        //------------------------------------------------------------------------------
        // Function: DecreaseValue
        // Author: nholmes
        // Summary: decrease the current value
        //------------------------------------------------------------------------------
        public int DecreaseValue()
        {
            // decrease the current value
            currentValue --;

            // if we've gone past the minimum value, clamp it
            if (currentValue < minValue) currentValue = minValue;

            // return the current value
            return currentValue;
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
                return MenuItemSelectionType.decreaseValue;
            }

            // is the supplied point within the shape defined by the right arrow?
            if ((point.X > (menu.ItemArea.X + position.X + width)) && (point.X < (menu.ItemArea.X + position.X + width + menu.SelectorRightTexture.Width)) &&
                (point.Y > (menu.ItemArea.Y + position.Y)) && (point.Y < (menu.ItemArea.Y + position.Y + menu.SelectorRightTexture.Height)))
            {
                return MenuItemSelectionType.increaseValue;
            }

            // not selected - return failure
            return MenuItemSelectionType.none;
        }

        //------------------------------------------------------------------------------
        // Function: Draw
        // Author: nholmes
        // Summary: draws the button item
        //------------------------------------------------------------------------------
        public override void Draw(SpriteBatch spriteBatch, bool isSelected)
        {
            Rectangle shape = new Rectangle();
            
            // draw selected items in yellow, otherwise white
            Color color = menu.GetItemColor(isSelected);
            
            // modify the alpha to fade button out during transitions
            color = new Color(color.R, color.G, color.B, menu.TransitionAlpha);

            // draw the slider bar
            shape.X = (int)(menu.ItemArea.X + position.X);
            shape.Y = (int)(menu.ItemArea.Y + position.Y);
            shape.Width = width;
            shape.Height = menu.SliderBarTexture.Height;
            spriteBatch.Draw(menu.SliderBarTexture, shape, color);

            // draw the slider button
            shape.X += (int)((((float)width / (float)(maxValue - minValue)) * currentValue) - (menu.SliderButtonTexture.Width / 2));
            shape.Width = menu.SliderButtonTexture.Width;
            shape.Height = menu.SliderButtonTexture.Height;
            spriteBatch.Draw(menu.SliderButtonTexture, shape, color);
            
            // draw the min and max text
            fontColor.A = menu.TransitionAlpha;
            Vector2 fontPosition = new Vector2(menu.ItemArea.X + position.X + width + 4, menu.ItemArea.Y + position.Y - 2);
            spriteBatch.DrawString(font, currentValue.ToString(), fontPosition, fontColor);
            fontPosition = new Vector2(menu.ItemArea.X + position.X - (int)font.MeasureString(minValue.ToString()).X - 4, menu.ItemArea.Y + position.Y + textOffset);
            spriteBatch.DrawString(font, minValue.ToString(), fontPosition, fontColor);
        }
    }
}
