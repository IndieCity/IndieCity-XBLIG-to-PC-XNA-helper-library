//------------------------------------------------------------------------------
// File: PlayerState_Idle.cs
// Author: Neil Holmes & Andrew Green
// Summary: player's walk left state
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
    // Class: PlayerState_Spawn
    // Author: Neil Holmes & Andrew Green
    // Summary: player's idle functionality
    //------------------------------------------------------------------------------
    public class PlayerState_Spawn : GameObjectState
    {
        // reference to the player object that owns this state
        private Player player;

        //------------------------------------------------------------------------------
        // Constructor: PlayerState_Spawn
        // Author: Neil Holmes & Andrew Green
        // Summary: state constructor
        //------------------------------------------------------------------------------
        public PlayerState_Spawn(GameObject2D parent)
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
            // make the the player is reset ready to spawn
            player.playerData.Reset();
            
            // set the initial display frame
            player.SetAnimation((int)PlayerAnimation.spawn);

            // set the animation speed for the spawn animation
            player.AnimationSpeed = 20;
        }

        //------------------------------------------------------------------------------
        // Method: Update
        // Author: Neil Holmes & Andrew Green
        // Summary: called every update that the state is active
        //------------------------------------------------------------------------------
        public override void Update()
        {
            // increase the animation frame
            if (player.UpdateAnimation(false))
            {
                // reached the end of the spawn animation - set state to idle
                player.stateManager.SetState(player.playerState_WalkRight);
            }
        }

        //------------------------------------------------------------------------------
        // Method: Exit
        // Author: Neil Holmes & Andrew Green
        // Summary: called whenever the state is being replaced
        //------------------------------------------------------------------------------
        public override void Exit() { }
    }
}
