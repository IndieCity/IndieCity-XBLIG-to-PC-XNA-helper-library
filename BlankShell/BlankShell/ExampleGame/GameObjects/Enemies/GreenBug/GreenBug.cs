//------------------------------------------------------------------------------
// File: GreenBug.cs
// Author: Neil Holmes & Andrew Green
// Summary: functionality for creating and controlling the green bug AI character
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
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using IndieCityXna.Common;
using IndieCityXna.GameObject;

namespace BlankShell.ExampleGame
{
    //------------------------------------------------------------------------------
    // Class: Player
    // Author: Neil Holmes & Andrew Green
    // Summary: handles player processing rendering etc etc
    //------------------------------------------------------------------------------
    public class GreenBug : Enemy
    {
        // list of the player's available states
        public Enemy_WalkLeft GreenBugState_WalkLeft;
        public GreenBug_WalkUp GreenBugState_WalkUp;
        public GreenBug_WalkRight GreenBugState_WalkRight;
        public Enemy_WalkDown GreenBugState_WalkDown;
 
        //------------------------------------------------------------------------------
        // Constructor: GreenBug 
        // Author: Neil Holmes & Andrew Green
        // Summary: Constructor - prepares the player for use
        //------------------------------------------------------------------------------
        public GreenBug(Game game, GameObject2DManager gameObjectManager, DisplayManager displayManager, Vector2 position, ShadowSystem shadowSystem, Random random)
            : base(game, gameObjectManager, displayManager, position, shadowSystem, random)
        {
            // nothing special to do here :)
        }

        //------------------------------------------------------------------------------
        // Method: LoadContent
        // Author: Neil Holmes & Andrew Green
        // Summary: load all the content that the player requires and prepare the player
        //          to be drawn and processed for the first time
        //------------------------------------------------------------------------------
        public override void LoadContent(ContentManager content)
        {
            // create all of the players states
            GreenBugState_WalkLeft = new Enemy_WalkLeft(this);
            GreenBugState_WalkUp = new GreenBug_WalkUp(this);
            GreenBugState_WalkRight = new GreenBug_WalkRight(this);
            GreenBugState_WalkDown = new Enemy_WalkDown(this);

            // load all of the green bug graphics
            LoadFrames(content, (int)EnemyAnimation.walkLeft, @"Game\GreenBug\WalkLeft\", 1);
            LoadFrames(content, (int)EnemyAnimation.walkUp, @"Game\GreenBug\WalkUp\", 1);
            LoadFrames(content, (int)EnemyAnimation.walkRight, @"Game\GreenBug\WalkRight\", 1);
            LoadFrames(content, (int)EnemyAnimation.walkDown, @"Game\GreenBug\WalkDown\", 1);
            
            // set the default animation speed (num frames per second)
            AnimationSpeed = DefaultAnimationSpeed = 6;

            // set the green bug to its starting state
            // NOTE: this MUST be done after content is loaded so that animation frames are available
            stateManager.SetState(GreenBugState_WalkLeft);
        }

        //------------------------------------------------------------------------------
        // Method: ProcessAI
        // Author: Neil Holmes & Andrew Green
        // Summary: performs all the AI processing for the green bug
        //------------------------------------------------------------------------------
        public override bool ProcessAI()
        {
            // calculte predicted next position
            Vector2 predictedPos = Position + (Velocity * TimerSystem.TimeStep);
                        
            // check for collision with the edges of the play area
            if (predictedPos.X < 128) 
                return stateManager.SetState(GreenBugState_WalkRight);
            if (predictedPos.X > (1280 - 128)) 
                return stateManager.SetState(GreenBugState_WalkLeft);
            if (predictedPos.Y < 96) 
                return stateManager.SetState(GreenBugState_WalkDown);
            if (predictedPos.Y > (720 - 32)) 
                return stateManager.SetState(GreenBugState_WalkUp);
            
            // perform complex AI simulation!
            if (random.Next(1000) > 990)
            {
                switch (random.Next(4))
                {
                    case 0:

                        // lets walk left for a bit
                        return stateManager.SetState(GreenBugState_WalkLeft);

                    case 1:

                        // lets walk up for a bit
                        return stateManager.SetState(GreenBugState_WalkUp);

                    case 2:
    
                        // lets walk right for a bit
                        return stateManager.SetState(GreenBugState_WalkRight);

                    case 3:

                        // lets walk down for a bit
                        return stateManager.SetState(GreenBugState_WalkDown);
                 }
            }

            // no change!
            return false;
        }
    }
}
