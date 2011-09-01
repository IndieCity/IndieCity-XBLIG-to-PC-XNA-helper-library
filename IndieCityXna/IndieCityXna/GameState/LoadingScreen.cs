//------------------------------------------------------------------------------
// Filename: LoadingScreen.cs
// Author: Neil Holmes & Andrew Green
// Summary: loading screen - useful for handling a transition from one set of
//          states to another set where lots of loading is likely to be needed
//------------------------------------------------------------------------------

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using IndieCityXna.Common;
using IndieCityXna.GameState;

namespace IndieCityXna.GameState
{
    //------------------------------------------------------------------------------
    // Class: LoadingScreen
    // Author: Neil Holmes & Andrew Green
    // Summary: Normally one state wil another transitions off will transitions on.
    //          however, if we area changing from one area of the game to another 
    //          where lots of new data needs to be loaded we want to hide this nicely.
    //          this class allows an elegant way of doing that.
    //------------------------------------------------------------------------------
    class LoadingScreen : GameState
    {
        // full screen image to display during the load
        private Texture2D loadingImage;
        
        // flag set to true once the previous states have been unloaded
        private bool previousStatesUnloaded;

        // which new states should be loadedg while the loading screen is displayed
        private GameState[] statesToLoad;

        //------------------------------------------------------------------------------
        // Constructor: LoadingScreen
        // Author: Neil Holmes & Andrew Green
        // Summary: take an image (that will be displayed full screen) and a list of the 
        //          new states to be loaded. all currently loaded states (apart from the
        //          loading screen state) will be killed and unloaded before the new states
        //          are loaded
        //------------------------------------------------------------------------------
        public LoadingScreen(Game game, Texture2D loadingImage, GameState[] statesToLoad, PlayerIndex? controllingPlayer)
            : base(game, controllingPlayer)
        {
            // store a reference to the loading image that we will display during loads
            this.loadingImage = loadingImage;
            
            // store which section we are to load 
            this.statesToLoad = statesToLoad;
        }

        //------------------------------------------------------------------------------
        // Function: Update
        // Author: Neil Holmes & Andrew Green
        // Summary: updates the loading screen state. works just like any other game state
        //------------------------------------------------------------------------------
        public override void Update(bool otherStateHasFocus,  bool coveredByOtherScreen)
        {
            // base update
            base.Update(otherStateHasFocus, coveredByOtherScreen);

            // have all previosu states finished transitioning off?
            if (previousStatesUnloaded)
            {
                // previous states unloaded - remove this state
                gameStateManager.RemoveGameState(this);

                // create the new states that are to be loaded
                foreach (GameState newState in statesToLoad)
                {
                    // valid new state?
                    if (newState != null)
                    {
                        // add the new state to the game state manager (states were previously constructed when "Load" was called)
                        gameStateManager.AddGameState(newState);
                    }
                }

                // once the load has finished, we must tell the timer system that we have 
                // finished a very long frame, so it does not try to catch up
                timerSystem.ResetElapsedTime();
            }
        }

        //------------------------------------------------------------------------------
        // Function: Update
        // Author: Neil Holmes & Andrew Green
        // Summary: draws the loading screen
        //------------------------------------------------------------------------------
        public override void Draw()
        {
            // if we are the only active screen, that means all the previous screens
            // must have finished transitioning off. we check for this in the Draw
            // method, rather than in update, because it isn't enough just for the
            // screens to be gone: in order for the transition to look good we must
            // have actually drawn a frame without them before we perform the load
            if ((subState == SubState.Active) && (gameStateManager.GetGameStates().Length == 1))
            {
                // we're the only active state - set previous states unloaded to true
                previousStatesUnloaded = true;
            }

            SpriteBatch spriteBatch = displayManager.GlobalSpriteBatch;
                
            // draw the full screen loading image
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, displayManager.TransformMatrix);
            spriteBatch.Draw(loadingImage, new Rectangle(0, 0, displayManager.GameResolutionX, displayManager.GameResolutionY), new Color(255, 255, 255, TransitionAlpha));
            spriteBatch.End();
        }
    }
}
