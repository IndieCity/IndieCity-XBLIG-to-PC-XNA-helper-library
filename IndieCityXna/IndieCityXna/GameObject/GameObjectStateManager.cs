//------------------------------------------------------------------------------
// Filename: GameObjectStateManager.cs
// Author: Neil Holmes & Andrew Greeen
// Summary: this is a simplified version of the game state manager that only
//          allows a single state to be active at a time. It has been designed
//          to be used by in-game objects such as the player and AI characters
//          when they state-based processing
//------------------------------------------------------------------------------

using System;
using Microsoft.Xna.Framework;

namespace IndieCityXna.GameObject
{
    //------------------------------------------------------------------------------
    // Class: GameObjectStateManager.cs
    // Author: Neil Holmes & Andrew Greeen
    // Summary: manages a single object state. when new objects are added the exit
    //          method of the previous state and the enter method of the new state
    //          are both called
    //------------------------------------------------------------------------------
    public class GameObjectStateManager
    {
        // parent game
        private Game game;

        // the object's current state
        private GameObjectState currentState = null;

        //------------------------------------------------------------------------------
        // Constructor: GameObjectStateManager
        // Author: nholmes
        // Summary: constructor for the game object state manager
        //-----------------------------------------------------------------------------
        public GameObjectStateManager(Game game)
        {
            // store a reference to the parent game
            this.game = game;
        }

        //------------------------------------------------------------------------------
        // Method: Reset
        // Author: Neil Holmes & Andrew Greeen
        // Summary: resets the game object state manager - clears the current state
        //-----------------------------------------------------------------------------
        public void Reset()
        {
            // clear the current state
            currentState = null;
        }

        //------------------------------------------------------------------------------
        // Method: SetState
        // Author: Neil Holmes & Andrew Greeen
        // Summary: changes the current state - calls exit on the previous state and
        //          enter on the new state. Will not allow the current state to be reset
        //          returns true if the new state is set
        //-----------------------------------------------------------------------------
        public bool SetState(GameObjectState newState)
        {
            // check that we aren't trying to enter the state we are already in
            if (newState == currentState) return false;
            
            // if we have a current state, call it's exit method
            if (currentState != null) currentState.Exit();
            
            // setup the new state
            newState.Enter();

            // set the new state as the current state
            currentState = newState;

            // state set succesfully - return true
            return true;
        }

        //------------------------------------------------------------------------------
        // Method: GetState
        // Author: Neil Holmes & Andrew Greeen
        // Summary: returns the current state
        //-----------------------------------------------------------------------------
        public GameObjectState GetState()
        {
            return currentState;
        }
 
        //------------------------------------------------------------------------------
        // Method: Update
        // Author: Neil Holmes & Andrew Greeen
        // Summary: updates the current state
        //-----------------------------------------------------------------------------
        public void Update()
        {
            // update the current game sate
            currentState.Update();
        }
    }
}
