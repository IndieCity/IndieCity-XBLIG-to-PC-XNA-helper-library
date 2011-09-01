//------------------------------------------------------------------------------
// Filename: GameState.cs
// Author: Neil Holmes & Andrew Green
// Summary: base functionality for all game states
//------------------------------------------------------------------------------

using System;
using Microsoft.Xna.Framework;
using IndieCityXna.Common;

namespace IndieCityXna.GameState
{
    // public enim of available states for each game state
    public enum SubState
    {
        TransitionOn,
        Active,
        TransitionOff,
        Hidden,
    }

    // public enim of available transition directions
    public enum TransitionDirection
    {
        On,
        Off,
    }

    //------------------------------------------------------------------------------
    // Class: GameState
    // Author: Neil Holmes & Andrew Green
    // Summary: a game state defines a self contained component of the game. it has 
    //          its own update and draw logic, and can be combined or layored with 
    //          other states to build up a complex system
    //------------------------------------------------------------------------------
    public class GameState
    {
        // parent game
        protected Game game;

        // handle to the display manager service
        protected DisplayManager displayManager;

        // handle to the game state manager service
        protected GameStateManager gameStateManager;

        // handle to the timer system service
        protected TimerSystem timerSystem;

        // handle to the input manager service
        protected InputManager inputManager;

        // index of the player that has control of this state
        protected PlayerIndex? controllingPlayer;

        // the sub state of this state (active, hidden transitioning etc) 
        protected SubState subState;

        // current transition amount, from 0.0f (no transition) to 1.0f (fuly transitioned)
        protected float transitionAmount;

        // flag to say if the state is currently exiting rather than just transitioning of temporarily
        protected bool isExiting;

        // flag set when another state currently has focus
        protected bool otherStateHasFocus;

        //------------------------------------------------------------------------------
        // Function: GameState
        // Author: Neil Holmes & Andrew Green
        // Summary: main constructor
        //------------------------------------------------------------------------------
        public GameState(Game game, PlayerIndex? controllingPlayer)
        {
            // store a reference to the game that owns this game state
            this.game = game;

            // get the display manager service
            displayManager = (DisplayManager)game.Services.GetService(typeof(DisplayManager));

            // get the game state manager service
            gameStateManager = (GameStateManager)game.Services.GetService(typeof(GameStateManager));

            // get the game timer system service
            timerSystem = (TimerSystem)game.Services.GetService(typeof(TimerSystem));

            // get the game input manager service
            inputManager = (InputManager)game.Services.GetService(typeof(InputManager));

            // store the player that has control of this game state
            this.controllingPlayer = controllingPlayer;

            // set the default sub-state of the state to be transitioning on
            subState = SubState.TransitionOn;

            // by default, transition has not finished
            transitionAmount = 1.0f;

            // by default, we are not exiting ;)
            isExiting = false;
        }

        //------------------------------------------------------------------------------
        // Method: LoadContent
        // Author: Neil Holmes & Andrew Green
        // Summary: loads any graphical content required by this state
        //------------------------------------------------------------------------------
        public virtual void LoadContent() { }

        //------------------------------------------------------------------------------
        // Method: UnloadContent
        // Author: Neil Holmes & Andrew Green
        // Summary: unloads any graphical content that was loaded by this state
        //------------------------------------------------------------------------------
        public virtual void UnloadContent() { }

        //------------------------------------------------------------------------------
        // Method: Update
        // Author: Neil Holmes & Andrew Green
        // Summary: allows the state to run logic, always called regardless of sub state
        //          although default functionality is provided the user is free to override
        //          this if they wish
        //------------------------------------------------------------------------------
        public virtual void Update(bool otherStateHasFocus, bool coveredByOtherScreen)
        {
            // store whether we have focus or not, for future reference
            this.otherStateHasFocus = otherStateHasFocus;

            // are we currently exiting this state?
            if (isExiting)
            {
                // yup we are exiting, make sure that the state is transitioning off
                subState = SubState.TransitionOff;

                // update the transition and check if it has completed
                if (!UpdateTransition(TransitionDirection.Off))
                {
                    // transition has finished - remove the state!
                    gameStateManager.RemoveGameState(this);
                }
            }
            else if (coveredByOtherScreen)
            {
                // if the state is currently being 'covered' by another, it should transition off
                if (UpdateTransition(TransitionDirection.Off))
                {
                    // still busy transitioning, ensure it's sub state is set correctly
                    subState = SubState.TransitionOff;
                }
                else
                {
                    // transition finished, set the the states sub-state to 'hidden'
                    subState = SubState.Hidden;
                }
            }
            else
            {
                // if we get here the state should either be transitioning on or be 'active'
                if (UpdateTransition(TransitionDirection.On))
                {
                    // still transitioning, ensure sub-state is set correctly
                    subState = SubState.TransitionOn;
                }
                else
                {
                    // transition has finished - ensure sub state is set to 'active'
                    subState = SubState.Active;
                }
            }
        }

        //------------------------------------------------------------------------------
        // Method: UpdateTransition
        // Author: Neil Holmes & Andrew Green
        // Summary: updates transition amount based on direction and time supplied
        //------------------------------------------------------------------------------
        protected bool UpdateTransition(TransitionDirection direction)
        {
            // update transition amount according to direction
            if (direction == TransitionDirection.Off)
            {
                // update the transition amount and clamp it
                transitionAmount += (2.0f * timerSystem.TimeStep);
                transitionAmount = MathHelper.Clamp(transitionAmount, 0, 1);

                // did we reach the end of the transition?
                if (transitionAmount >= 1)
                {
                    // yup, end reached - return false 
                    return false;
                }
            }
            else
            {
                // update the transition amount and clamp it
                transitionAmount -= (2.0f * timerSystem.TimeStep);
                transitionAmount = MathHelper.Clamp(transitionAmount, 0, 1);

                // did we reach the end of the transition?
                if (transitionAmount <= 0)
                {
                    // yup, end reached - return false 
                    return false;
                }
            }

            // if we get here we are still busy transitioning - return true
            return true;
        }

        //------------------------------------------------------------------------------
        // Method: HandleInput
        // Author: Neil Holmes & Andrew Green
        // Summary: allows the state to handle user input. Called only when the state is
        //          active, and currently has focus
        //------------------------------------------------------------------------------
        public virtual void HandleInput() { }

        //------------------------------------------------------------------------------
        // Method: Draw
        // Author: Neil Holmes & Andrew Green
        // Summary: called when the state should draw itself
        //------------------------------------------------------------------------------
        public virtual void Draw() { }

        //------------------------------------------------------------------------------
        // Method: CloseState
        // Author: Neil Holmes & Andrew Green
        // Summary: called when the state should transition of then remove itself
        //------------------------------------------------------------------------------
        public void CloseState()
        {
            // set isExiting to true so that the state transitions off and then removes itself
            isExiting = true;
        }

        //------------------------------------------------------------------------------
        // Property: TransitionAlpha
        // Author: Neil Holmes & Andrew Green
        // Summary: helper to get transition alpha, 255 = fully active, 0 = fully off
        //------------------------------------------------------------------------------
        public byte TransitionAlpha
        {
            get { return (byte)(255 - (int)(transitionAmount * 255.0f)); }
        }

        //------------------------------------------------------------------------------
        // Property: SubState
        // Author: Neil Holmes & Andrew Green
        // Summary: accessor to get the sub state of the state 
        //------------------------------------------------------------------------------
        public SubState SubState
        {
            get { return subState; }
        }

        //------------------------------------------------------------------------------
        // Property: IsActive
        // Author: Neil Holmes & Andrew Green
        // Summary: gets whether the state is active or not
        //------------------------------------------------------------------------------
        public bool IsActive
        {
            get { return !otherStateHasFocus && (subState == SubState.TransitionOn || subState == SubState.Active); }
        }
    }
}
