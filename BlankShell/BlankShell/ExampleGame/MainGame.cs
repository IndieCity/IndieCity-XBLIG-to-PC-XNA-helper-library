//------------------------------------------------------------------------------
// Filename: MainGame.cs
// Author: nholmes
// Summary: main game update
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
using Microsoft.Xna.Framework.Input;
using IndieCityXna.Common;
using IndieCityXna.GameState;
using IndieCityXna.GameObject;
using IndieCityXna.TileMap;

namespace BlankShell.ExampleGame
{
    //------------------------------------------------------------------------------
    // Class: MainGame
    // Author: nholmes
    // Summary: handles everything to do with the playable part of the game
    //------------------------------------------------------------------------------
    class MainGame : GameState
    {
        // handles updates of each game object
        private GameObject2DManager gameObjectManager;

        // content manager for the space flight mode
        private ContentManager content;
 
        // background images for the game levels
        private Texture2D backgroundTexture;
        private Texture2D foregroundTexture;

        // world position
        private Vector2 worldPosition;

        // player reference pointers
        private Player playerOne;
        private Player playerTwo;

        // shadow system
        private ShadowSystem shadowSystem;

        //------------------------------------------------------------------------------
        // Constructor: MainGame
        // Author: nholmes
        // Summary: Default constructor
        //------------------------------------------------------------------------------
        public MainGame(Game game, PlayerIndex? controllingPlayer)
            : base(game, controllingPlayer)
        {
            // store reference to global ContentManager
            content = new ContentManager(game.Services, @"Content\");

            // Create gameObjectManager
            gameObjectManager = new GameObject2DManager(game, content);

            // set the world position to 0,0 - this game doesnt scroll so this will never change :)
            worldPosition.X = 0;
            worldPosition.Y = 0;
        }

        //------------------------------------------------------------------------------
        // Method: LoadContent
        // Author: nholmes
        // Summary: load everything needed for the main game
        //------------------------------------------------------------------------------
        public override void LoadContent()
        {
            // load the background texture
            backgroundTexture = content.Load<Texture2D>(@"game\background\background");
            foregroundTexture = content.Load<Texture2D>(@"game\background\foreground");

            // create the shadow system and cause it to load
            shadowSystem = new ShadowSystem(); 
            shadowSystem.LoadContent(content);

            // initialise the players - note that while the players are standard game objects we do NOT
            // add them to the game object managers list of managed objects but instead update them manually
            // this is to ensure that the player can be processed before any other objects etc
            playerOne = new Player(game, gameObjectManager, displayManager, new Vector2(128, 128), PlayerIndex.One, shadowSystem, 1);
            playerTwo = new Player(game, gameObjectManager, displayManager, new Vector2(256, 128), PlayerIndex.Two, shadowSystem, 2);

            // load the players content (this is normally handled by the game object manager - but see above)
            playerOne.LoadContent(content);
            playerTwo.LoadContent(content);

            // create a random number generator for the bugs
            Random random = new Random();

            // create some bugs!
            for (int i = 0; i < 50; i++)
            {
                gameObjectManager.AddObject(new GreenBug(game, gameObjectManager, displayManager, new Vector2(-128 - (i * 64) - random.Next(16), 345 + random.Next(64)), shadowSystem, random));
            }
                        
            // once the load has finished, we must tell the timer system that we have 
            // finished a very long frame, so it does not try to catch up
            timerSystem.ResetElapsedTime();
        }

        //------------------------------------------------------------------------------
        // Method: UnloadContent
        // Author: nholmes
        // Summary: unload all the content that was used by the main game
        //------------------------------------------------------------------------------
        public override void UnloadContent()
        {
            content.Unload();
        }   

        //------------------------------------------------------------------------------
        // Method: Update
        // Author: nholmes
        // Summary: Updates the game! This method checks the GameState.IsActive property
        //          so the game will stop updating when the pause menu is active, or if
        //          you alt+tab away to a different application.
        //------------------------------------------------------------------------------
        public override void Update(bool otherStateHasFocus, bool coveredByOtherScreen)
        {
            // call base update functionality with forced coveredByOtherScreen so we always draw the game 
            base.Update(otherStateHasFocus, false);
 
            // is the game active?
            if (IsActive)
            {
                // update the players
                playerOne.Update(worldPosition);
                playerTwo.Update(worldPosition);

                // Update all of the game objects
                gameObjectManager.Update(worldPosition);
            }
        }

        //------------------------------------------------------------------------------
        // Method: HandleInput
        // Author: nholmes
        // Summary: Captures game-wide user input and processes it
        //------------------------------------------------------------------------------
        public override void HandleInput()
        {
            if (inputManager.IsPauseGame(controllingPlayer))
            {
                // player wants to pause the game - add the pause state
                gameStateManager.AddGameState(new PauseMenu(game, controllingPlayer));
            }
        }

        //------------------------------------------------------------------------------
        // Method: Draw
        // Author: nholmes
        // Summary: Draws the game objects and text
        //------------------------------------------------------------------------------
        public override void Draw()
        {
            // get a reference to the global sprite batch
            SpriteBatch spriteBatch = displayManager.GlobalSpriteBatch;
            
            // draw the background
            spriteBatch.Begin();
            spriteBatch.Draw(backgroundTexture, new Rectangle(0, 0, displayManager.DisplaySize.Width, displayManager.DisplaySize.Height), Color.White);
            spriteBatch.End();

            // start drawing sprites that are sorted based on their height on the screen
            spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, displayManager.TransformMatrix);

            // draw the players
            playerOne.Draw(worldPosition, displayManager.GlobalSpriteBatch, true);
            playerTwo.Draw(worldPosition, displayManager.GlobalSpriteBatch, true);
            
            // draw all of the game objects
            gameObjectManager.DrawObjects(worldPosition, displayManager.GlobalSpriteBatch);

            // finished drawing all the sorted sprites
            spriteBatch.End();

            // draw the forekground
            spriteBatch.Begin();
            spriteBatch.Draw(foregroundTexture, new Rectangle(0, 0, displayManager.DisplaySize.Width, displayManager.DisplaySize.Height), Color.White);
            spriteBatch.End();

            // draw the debug information
            DrawDebugInfo();
            
            // If the game is transitioning on or off, fade it out to black.
            if (transitionAmount > 0) gameStateManager.DarkenBackground(255 - TransitionAlpha);
        }
       
        //------------------------------------------------------------------------------
        // Method: DrawDebugInfo
        // Author: nholmes
        // Summary: Draws the game objects and text
        //------------------------------------------------------------------------------
        private void DrawDebugInfo()
        {
            int row = 0;
            Vector2 textPosition;
            Vector2 textOffset;

            // Initialise text position
            textPosition = new Vector2(8, 8);
            textOffset = new Vector2(0, 12);

            // create a sprite batch to draw with
            SpriteBatch spriteBatch = displayManager.GlobalSpriteBatch;

            // start the sprite batch
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, displayManager.TransformMatrix);

            // display all the debug text
            spriteBatch.DrawString(displayManager.DebugFont, "Framerate: " + timerSystem.FrameRate.ToString(), textPosition + (textOffset * row++), Color.LightGreen);

            // finish the sprite batch
            spriteBatch.End();
        }
    }
}
