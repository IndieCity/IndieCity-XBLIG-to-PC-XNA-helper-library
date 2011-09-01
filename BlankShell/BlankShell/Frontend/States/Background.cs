//------------------------------------------------------------------------------
// Filename: Background.cs
// Author: nholmes
// Summary: the background image that sits behind all the menus
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
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using IndieCityXna.Common;
using IndieCityXna.GameState;

namespace BlankShell.Frontend
{
    //------------------------------------------------------------------------------
    // Class: Background
    // Author: nholmes
    // Summary: sits behind all the other menus and draws a background image that
    //          remains fixed in place at all times
    //------------------------------------------------------------------------------
    class Background : GameState
    {
        // content manager for the background
        ContentManager content;
        
        // the background texture
        Texture2D backgroundTexture;

        //------------------------------------------------------------------------------
        // Function: Background
        // Author: nholmes
        // Summary: constructor
        //------------------------------------------------------------------------------
        public Background(Game game, PlayerIndex? controllingPlayer)
            : base(game, controllingPlayer)
        {
        }

        //------------------------------------------------------------------------------
        // Function: LoadContent
        // Author: nholmes
        // Summary: uses its own content manager to load the background texture
        //------------------------------------------------------------------------------
        public override void LoadContent()
        {
            // check if we already created a content manager
            if (content == null)
            {
                // nope, create one!
                content = new ContentManager(game.Services, "Content");
            }

            // load the backgroun texure
            backgroundTexture = content.Load<Texture2D>("Frontend\\Background");
        }

        //------------------------------------------------------------------------------
        // Function: UnloadContent
        // Author: nholmes
        // Summary: unloads the background texture when we are finished with it
        //------------------------------------------------------------------------------
        public override void UnloadContent()
        {
            content.Unload();
        }

        //------------------------------------------------------------------------------
        // Function: Update
        // Author: nholmes
        // Summary: custom update for the background that forces coveredByOtherScreen to
        //          always be false to ensure that the background gets drawn 
        //------------------------------------------------------------------------------
        public override void Update(bool otherStateHasFocus, bool coveredByOtherScreen)
        {
            // call base update functionality with forced coveredByOtherScreen
            base.Update(otherStateHasFocus, false);
        }

        //------------------------------------------------------------------------------
        // Function: Draw
        // Author: nholmes
        // Summary: draws the background
        //------------------------------------------------------------------------------
        public override void Draw()
        {
            // get a reference to the sprite batch from game state manager
            SpriteBatch spriteBatch = displayManager.GlobalSpriteBatch;

            // set the background to render to the full screen
            Viewport viewport = game.GraphicsDevice.Viewport;
            Rectangle fullscreen = new Rectangle(0, 0, (int)displayManager.GameResolutionX, (int)displayManager.GameResolutionY);
            
            // set the brightness of the background according to transition alpha so that it fades in and out
            byte fade = TransitionAlpha;

            // draw the background
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Opaque, SamplerState.LinearClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, displayManager.TransformMatrix);
            spriteBatch.Draw(backgroundTexture, fullscreen, new Color(fade, fade, fade));
            spriteBatch.End();
        }
    }
}
