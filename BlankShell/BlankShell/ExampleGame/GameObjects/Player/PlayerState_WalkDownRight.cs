//------------------------------------------------------------------------------
// File: PlayerState_WalkDownRight.cs
// Author: Neil Holmes & Andrew Green
// Summary: player's walk down right state
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
    // Class: PlayerState_WalkDownRight
    // Author: Neil Holmes & Andrew Green
    // Summary: player's walk down right functionality
    //------------------------------------------------------------------------------
    public class PlayerState_WalkDownRight : GameObjectState
    {
        // reference to the player object that owns this state
        private Player player;

        //------------------------------------------------------------------------------
        // Constructor: PlayerState_WalkLeft
        // Author: Neil Holmes & Andrew Green
        // Summary: state constructor
        //------------------------------------------------------------------------------
        public PlayerState_WalkDownRight(GameObject2D parent)
            : base(parent)
        {
            // store a reference to the player who owns this state
            this.player = (Player)parent;
        }

        //------------------------------------------------------------------------------
        // Method: Enter
        // Author: Neil Holmes & Andrew Green
        // Summary: called whenever the state is activated
        //------------------------------------------------------------------------------
        public override void Enter()
        {
            // set the initial display frame
            player.SetAnimation((int)PlayerAnimation.walkDownRight);
        }

        //------------------------------------------------------------------------------
        // Method: Update
        // Author: Neil Holmes & Andrew Green
        // Summary: called every update that the state is active
        //------------------------------------------------------------------------------
        public override void Update()
        {
            // process standard player inputs - bail out if state has been changed
            if (player.ProcessStandardInputs() == true) return;

            // increase the animation frame
            player.UpdateAnimation(true);

            // set player direction
            player.Velocity = new Vector2(90, 90);

            // apply player's movement
            player.Position += player.Velocity * player.TimerSystem.TimeStep;
        }

        //------------------------------------------------------------------------------
        // Method: Exit
        // Author: Neil Holmes & Andrew Green
        // Summary: called whenever the state is being replaced
        //------------------------------------------------------------------------------
        public override void Exit() { }
    }
}
