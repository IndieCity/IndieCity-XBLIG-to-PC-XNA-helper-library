//------------------------------------------------------------------------------
// Filename: GameObjectState.cs
// Author: Neil Holmes & Andrew Green
// Summary: a simple state class that can be used by 2D game objects
//------------------------------------------------------------------------------

using System;
using Microsoft.Xna.Framework;

namespace IndieCityXna.GameObject
{
    //------------------------------------------------------------------------------
    // Class: GameObjectState
    // Author: Neil Holmes & Andrew Green
    // Summary: states are managed by the game object state manager. user should create
    //          inherited versions of this class to construct the different states
    //          required by their 2D game objects.
    //------------------------------------------------------------------------------
    public class GameObjectState
    {
        // reference to the game object that owns this state
        private GameObject2D gameObject;

        //------------------------------------------------------------------------------
        // Function: GameObjectState
        // Author: Neil Holmes & Andrew Green
        // Summary: main constructor
        //------------------------------------------------------------------------------
        public GameObjectState(GameObject2D gameObject)
        {
            // store a reference to the game object that owns this state
            this.gameObject = gameObject;
        }

        //------------------------------------------------------------------------------
        // Function: Enter
        // Author: Neil Holmes & Andrew Green
        // Summary: called when the state is activated
        //------------------------------------------------------------------------------
        public virtual void Enter() {}
        
        //------------------------------------------------------------------------------
        // Function: Update
        // Author: Neil Holmes & Andrew Green
        // Summary: called when the state is processed
        //------------------------------------------------------------------------------
        public virtual void Update() { }

        //------------------------------------------------------------------------------
        // Function: Exit
        // Author: Neil Holmes & Andrew Green
        // Summary: called when the state is being replaced
        //------------------------------------------------------------------------------
        public virtual void Exit() { }
    }
}
