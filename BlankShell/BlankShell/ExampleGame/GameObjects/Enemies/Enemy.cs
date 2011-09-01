//------------------------------------------------------------------------------
// File: Enemy.cs
// Author: Neil Holmes & Andrew Green
// Summary: base functionality for creating and controlling the enemy AI 
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
    // public enemy animations enums
    public enum EnemyAnimation
    {
        walkLeft,
        walkUp,
        walkRight,
        walkDown,
        maxAnimations
    }

    //------------------------------------------------------------------------------
    // Class: Enemy
    // Author: Neil Holmes & Andrew Green
    // Summary: handles base processing rendering etc for enemy AI characters
    //------------------------------------------------------------------------------
    public class Enemy : GameObject2D
    {
        // the physical properties of the enemy's 2D game object
        private static GameObject2DProperties properties = new GameObject2DProperties
        (
            "Enemy",                    // type
            true,                       // always update the player
            0,                          // process range is not used - player always updates        
            new Vector2(-32, -64),      // display offset
            new Vector2(64, 64),        // display size
            new Vector2(-16, -16),      // collision offset
            new Vector2(32, 16)         // collision size
        );

        // random number generator
        public Random random;

        // player's state manager
        public GameObjectStateManager stateManager;

        // reference to the shadow system used to draw the shadow under the player
        ShadowSystem shadowSystem;

        //------------------------------------------------------------------------------
        // Constructor: Enemy 
        // Author: Neil Holmes & Andrew Green
        // Summary: Constructor - prepares the player for use
        //------------------------------------------------------------------------------
        public Enemy(Game game, GameObject2DManager gameObjectManager, DisplayManager displayManager, Vector2 position, ShadowSystem shadowSystem, Random random)
            : base(game, gameObjectManager, displayManager, position, properties)
        {
            // store a reference to the shadow system
            this.shadowSystem = shadowSystem;

            // create a random number generator
            this.random = random;

            // create the player's state manager
            stateManager = new GameObjectStateManager(game);

            // set direction to right
            Velocity = new Vector2(4, 0);
        }

        //------------------------------------------------------------------------------
        // Method: Update
        // Author: Neil Holmes & Andrew Green
        // Summary: updates the enemy's current state
        //------------------------------------------------------------------------------
        public override void Update(Vector2 worldPosition)
        {
            // update the current state
            stateManager.Update();
        }

        //------------------------------------------------------------------------------
        // Method: ProcessAI
        // Author: Neil Holmes & Andrew Green
        // Summary: updates the enemy's AI
        //------------------------------------------------------------------------------
        public virtual bool ProcessAI()
        {
            return true;
        }

        //------------------------------------------------------------------------------
        // Method: Draw
        // Author: Neil Holmes & Andrew Green
        // Summary: updates the player's current state
        //------------------------------------------------------------------------------
        public override void Draw(Vector2 worldPosition, SpriteBatch spriteBatch, bool sortByYPos = false)
        {
            // draw a shadow under the player
            shadowSystem.Draw(spriteBatch, Position - worldPosition, 1.0f);

            // call the base draw functionality
            base.Draw(worldPosition, spriteBatch, sortByYPos);
        }
    }
}
