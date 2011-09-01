//------------------------------------------------------------------------------
// File: Shadow.cs
// Author: Neil Holmes & Andrew Green
// Summary: functionality for drawing a shadow under a game object
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
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using IndieCityXna.Common;
using IndieCityXna.GameObject;

namespace BlankShell.ExampleGame
{
    //------------------------------------------------------------------------------
    // Class: ShadowSystem
    // Author: Neil Holmes & Andrew Green
    // Summary: handles drawing the shadows under the game objects
    //------------------------------------------------------------------------------
    public class ShadowSystem
    {
        // the shadow system texture
        Texture2D shadowTexture;
        
        //------------------------------------------------------------------------------
        // Constructor: Player 
        // Author: Neil Holmes & Andrew Green
        // Summary: Constructor - prepares the shadow system for use
        //------------------------------------------------------------------------------
        public ShadowSystem()
        {
        }

        //------------------------------------------------------------------------------
        // Method: LoadContent
        // Author: Neil Holmes & Andrew Green
        // Summary: load all the content that the shadow system needs
        //------------------------------------------------------------------------------
        public void LoadContent(ContentManager content)
        {
            // load the shadow texture
            shadowTexture = content.Load<Texture2D>(@"Game\Shadow\Shadow");
        }

        //------------------------------------------------------------------------------
        // Method: Draw
        // Author: Neil Holmes & Andrew Green
        // Summary: draws a shadow
        //------------------------------------------------------------------------------
        public void Draw(SpriteBatch spriteBatch, Vector2 position, float scale)
        {
            Rectangle drawPosition;
            
            // offset to the correct position for drawing
            drawPosition.X = (int)position.X - (int)(32 * scale);
            drawPosition.Y = (int)position.Y - (int)(36 * scale);
            drawPosition.Width = (int)(64 * scale);
            drawPosition.Height = (int)(64 * scale);

            // draw the shadow
            spriteBatch.Draw(shadowTexture, drawPosition, null, Color.White, 0, new Vector2(0,0), SpriteEffects.None, 0);
        }
    }
}
