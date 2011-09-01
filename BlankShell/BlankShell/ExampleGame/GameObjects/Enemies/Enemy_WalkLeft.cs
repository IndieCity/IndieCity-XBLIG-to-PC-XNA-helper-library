//------------------------------------------------------------------------------
// File: Enemy_WalkLeft.cs
// Author: Neil Holmes & Andrew Green
// Summary: Enemy walk left state
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
using IndieCityXna.GameObject;

namespace BlankShell.ExampleGame
{
    //------------------------------------------------------------------------------
    // Class: Enemy_WalkLeft
    // Author: Neil Holmes & Andrew Green
    // Summary: enemy walk left functionality
    //------------------------------------------------------------------------------
    public class Enemy_WalkLeft : GameObjectState
    {
        // reference to the enemyg that owns this state
        private Enemy parent;

        //------------------------------------------------------------------------------
        // Constructor: Enemy_WalkLeft
        // Author: Neil Holmes & Andrew Green
        // Summary: state constructor
        //------------------------------------------------------------------------------
        public Enemy_WalkLeft(GameObject2D parent)
            : base(parent)
        {
            // store a reference to the enemy that owns this state
            this.parent = (Enemy)parent;
        }

        //------------------------------------------------------------------------------
        // Method: Enter
        // Author: Neil Holmes & Andrew Green
        // Summary: called whenever the state is activated
        //------------------------------------------------------------------------------
        public override void Enter()
        {
            // set the initial display frame
            parent.SetAnimation((int)EnemyAnimation.walkLeft);
        }

        //------------------------------------------------------------------------------
        // Method: Update
        // Author: Neil Holmes & Andrew Green
        // Summary: called every update that the state is active
        //------------------------------------------------------------------------------
        public override void Update()
        {
            // process parent AI
            if (parent.ProcessAI() == true) return;

            // increase the animation frame
            parent.UpdateAnimation(true);

            // set enemy direction
            parent.Velocity = new Vector2(-100, 0);

            // apply enemy movement
            parent.Position += parent.Velocity * parent.TimerSystem.TimeStep;
        }

        //------------------------------------------------------------------------------
        // Method: Exit
        // Author: Neil Holmes & Andrew Green
        // Summary: called whenever the state is being replaced
        //------------------------------------------------------------------------------
        public override void Exit() { }
    }
}
