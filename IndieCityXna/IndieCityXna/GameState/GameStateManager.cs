//------------------------------------------------------------------------------
// Filename: GameStateManager.cs
// Author: Neil Holmes & Andrew Green
// Summary: functionality for managing the game state stack
//------------------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using IndieCityXna.Common;

namespace IndieCityXna.GameState
{
    //------------------------------------------------------------------------------
    // Class: GameStateManager.cs
    // Author: Neil Holmes & Andrew Green
    // Summary: manages one or more game state instances. maintains a stack of game
    //          states, calls their update and draw methodss, and automatically 
    //          routes input to the top-most active state
    //------------------------------------------------------------------------------
    public class GameStateManager
    {
        // parent game
        private Game game;

        // handle to the display manager service
        DisplayManager displayManager;

        // list of game states to process
        private List<GameState> gameStates;

        // list of game states to process
        private List<GameState> gameStatesToUpdate;
 
        // debug trace status flag - set to true to display list of game states being processed
        private bool traceEnabled = true;

        // texture used for displaying the safe zones and darkening the screen
        Texture2D blankTexture;
        
        //------------------------------------------------------------------------------
        // Function: GameStateManager
        // Author: Neil Holmes & Andrew Green
        // Summary: constructor for the game state manager
        //-----------------------------------------------------------------------------
        public GameStateManager(Game game)
        {
            // store the game system that owns this game state manager
            this.game = game;

            // get a handle to the display manager service
            displayManager = (DisplayManager)game.Services.GetService(typeof(DisplayManager));
 
            // create the list of game states and the list of game states to be updated
            gameStates = new List<GameState>();
            gameStatesToUpdate = new List<GameState>();

            // load the blank texture we use for dawing the safe zones and darkening the screen
            blankTexture = game.Content.Load<Texture2D>(@"System\Blank");
        }

        //------------------------------------------------------------------------------
        // Function: AddGameState
        // Author: Neil Holmes & Andrew Green
        // Summary: adds a new game state to the game state manager
        //-----------------------------------------------------------------------------
        public void AddGameState(GameState gameState)
        {
            // tell the game state to load it's content
            gameState.LoadContent();

            // add the game state to the manager
            gameStates.Add(gameState);
        }

        //------------------------------------------------------------------------------
        // Function: RemoveGameState
        // Author: Neil Holmes & Andrew Green
        // Summary: immediately removes a game state. the state will not transition off 
        //-----------------------------------------------------------------------------
        public void RemoveGameState(GameState gameState)
        {
            // tell the game state to unload it's content
            gameState.UnloadContent();

            // remove the game state from the manager
            gameStates.Remove(gameState);

            // remove the gane state from the current update list
            gameStatesToUpdate.Remove(gameState);
        }

        //------------------------------------------------------------------------------
        // Function: Update
        // Author: Neil Holmes & Andrew Green
        // Summary: updates all the game states on the stack
        //-----------------------------------------------------------------------------
        public void Update()
        {
            // set initial states of status flags
            bool otherStateHasFocus = false;
            bool coveredByOtherScreen = false;

            // esure that the list of game states to process is clear before we begin
            gameStatesToUpdate.Clear();

            // make a copy of the master game state list, to avoid confusion if updating one game state adds or removes others
            gameStatesToUpdate.AddRange(gameStates);

            // loop as long as there are game states to be updated
            while (gameStatesToUpdate.Count > 0)
            {
                // pop the top game state off the list
                GameState gameState = gameStatesToUpdate[gameStatesToUpdate.Count - 1];
                gameStatesToUpdate.RemoveAt(gameStatesToUpdate.Count - 1);

                // update the game state
                gameState.Update(otherStateHasFocus, coveredByOtherScreen);

                // check if this game state can accept input (ie - it's active or transitioning on and no other state already has focus)
                if (gameState.SubState == SubState.TransitionOn || gameState.SubState == SubState.Active && !otherStateHasFocus)
                {
                    // get this state to process the input
                    gameState.HandleInput();

                    // prevent any other game states from processing input
                    otherStateHasFocus = true;

                    // tell subsequent states that they are covered
                    coveredByOtherScreen = true;
                }
            }
        }

        //------------------------------------------------------------------------------
        // Function: Draw
        // Author: Neil Holmes & Andrew Green
        // Summary: tells each game state to draw itself
        //-----------------------------------------------------------------------------
        public void Draw()
        {
            // loop through all of the gsme states and draw them
            foreach (GameState gameState in gameStates)
            {
                // check if this particular state is hidden
                if (gameState.SubState != SubState.Hidden)
                {
                    // not hidden, tell it to draw
                    gameState.Draw();
                }
            }

            // if trace is enabled display the debug trace
            if (traceEnabled) TraceGameStates();
        }

        //------------------------------------------------------------------------------
        // Function: GetGameStates
        // Author: Neil Holmes & Andrew Green
        // Summary: exposes an array holding all of the game states. returns a copy because
        //          state should only ever be added or removed using the correct methods
        //-----------------------------------------------------------------------------
        public GameState[] GetGameStates()
        {
            return gameStates.ToArray();
        }

        //------------------------------------------------------------------------------
        // Function: DarkenBackground
        // Author: Neil Holmes & Andrew Green
        // Summary: draws a translucent black fullscreen sprite, used for fading screens
        //          in and out, and for darkening the background behind popups
        //------------------------------------------------------------------------------
        public void DarkenBackground(int alpha)
        {
            // create a full screen rectangle and calculate the color to use
            Rectangle rectangle = new Rectangle(0, 0, displayManager.GameResolutionX, displayManager.GameResolutionY);
            Color color = new Color(0, 0, 0, (byte)alpha);

            // get a reference to the global sprite batch
            SpriteBatch spriteBatch = displayManager.GlobalSpriteBatch;

            // draw the retangle to darken anything drawn before it
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, displayManager.TransformMatrix);
            spriteBatch.Draw(blankTexture, rectangle, color);
            spriteBatch.End();
        }

        //------------------------------------------------------------------------------
        // Function: TraceGameStates
        // Author: Neil Holmes & Andrew Green
        // Summary: displays a list of all current game states for debugging
        //          list will be displayed in the bottom left of the screen
        //------------------------------------------------------------------------------
        void TraceGameStates()
        {
            Color color;

            // start displaying state stack from the bottom left - we will work up the screen
            Vector2 position = new Vector2(8, displayManager.GameResolutionY - displayManager.DebugFont.LineSpacing - 8);
            
            // create a sprite batch to draw with
            SpriteBatch spriteBatch = displayManager.GlobalSpriteBatch;

            // start the sprite batch
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, displayManager.TransformMatrix);

            // loop throuh all of the game states and store their names into the list
            foreach (GameState gameState in gameStates)
            {
                // build string of information about this state
                string stateInfo = gameState.GetType().ToString();
                stateInfo += " :: ";
                stateInfo += gameState.SubState.ToString();
                stateInfo += " :: Alpha = ";
                stateInfo += gameState.TransitionAlpha.ToString();

                // if the state is active set the colour to green, else use red
                if (gameState.IsActive)
                    color = Color.Green;
                else
                    color = Color.Red;

                // display the information about this state
                spriteBatch.DrawString(displayManager.DebugFont, stateInfo, position, color);

                // move up the screen by 1 row
                position.Y -= displayManager.DebugFont.LineSpacing;
            }

            // finish the sprite batch
            spriteBatch.End();
        }

        //------------------------------------------------------------------------------
        // Function: LoadNewStates
        // Author: Neil Holmes & Andrew Green
        // Summary: helper function for transitioning from one section of the game to 
        //          another (such as the frontend to in game) where lots of loading is
        //          likely to be required. Ditches all the currently loaded states then
        //          creates the loading screen state and starts loading the list of new
        //          states
        //------------------------------------------------------------------------------
        public void LoadNewStates(Game game, Texture2D loadingImage, PlayerIndex? controllingPlayer, params GameState[] statesToLoad)
        {
            // loop through all currently active game states
            foreach (GameState screen in gameStates)
            {
                // tell this state to transition off and close itself
                screen.CloseState();
            }

            // create and activate the loading screen state
            LoadingScreen loadingScreen = new LoadingScreen(game, loadingImage, statesToLoad, controllingPlayer);

            // add the loading screen the game state manager so that it updates and displays
            AddGameState(loadingScreen);
        }
    }
}
