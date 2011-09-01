//------------------------------------------------------------------------------
// File: Player.cs
// Author: Neil Holmes & Andrew Green
// Summary: functionality for creating and controlling the main player character
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
    // public player animations enums
    public enum PlayerAnimation
    {
        spawn = 0,
        walkLeft,
        walkUpLeft,
        walkUp,
        walkUpRight,
        walkRight,
        walkDownRight,
        walkDown,
        walkDownLeft,
        death1,
        death2,
        death3,
        death4,
        death5,
        maxAnimations
    }

    // public player weapon type enums
    public enum PlayerWeaponType
    {
        normal = 0,
        threeWay,
        eightWay,
        laser,
        threeWayLaser,
        eightWayLaser,
        flameThrower,
        grenadeLauncher,
        heatSeakers
    }

    // public player shield types
    public enum PlayerShieldType
    {
        none = 0,
        bubble,
        rotating
    }

    // public class to hold the key settings for each player
    public class PlayerKeys
    {
        // public key settings
        public Keys left;
        public Keys right;
        public Keys up;
        public Keys down;

        // constructor that sets the keys to use
        public PlayerKeys(Keys left, Keys right, Keys up, Keys down)
        {
            // store all of the keys that are to be used for this player
            this.left = left;
            this.right = right;
            this.up = up;
            this.down = down;
        }
    }

    //------------------------------------------------------------------------------
    // class: PlayerData
    // Author: Neil Holmes & Andrew Green
    // Summary: structue containing all the informaton about the player that we will
    //          be saving and loading
    //------------------------------------------------------------------------------
    public class PlayerData
    {
        // players health and lives
        public int health;
        public int lives;
        public int score;

        // players inventory 
        public int crispsHeld;
        public PlayerWeaponType weaponType;
        public PlayerShieldType shieldType;
 
        //------------------------------------------------------------------------------
        // Constructor: PlayerData
        // Author: Neil Holmes & Andrew Green
        // Summary: sets player score and number of keys to 0 and call reset to default 
        //          other variables
        //------------------------------------------------------------------------------
        public PlayerData()
        {
            FullReset();
        }
        
        //------------------------------------------------------------------------------
        // Method: FullReset 
        // Author: Neil Holmes & Andrew Green
        // Summary: sets all player data to default values. used when the game restarts
        //------------------------------------------------------------------------------
        public void FullReset()
        {
            // reset player's lives and score
            lives = 3;
            score = 0;

            // reset everything else
            Reset();
        }

        //------------------------------------------------------------------------------
        // Method: Reset 
        // Author: Neil Holmes & Andrew Green
        // Summary: resets relevant player data when the player dies
        //------------------------------------------------------------------------------
        public void Reset()
        {
            // reset player's health and weapon type
            health = 100;
            weaponType = PlayerWeaponType.normal;
            shieldType = PlayerShieldType.none;
        }
    }

    //------------------------------------------------------------------------------
    // Class: Player
    // Author: Neil Holmes & Andrew Green
    // Summary: handles player processing rendering etc etc
    //------------------------------------------------------------------------------
    public class Player : GameObject2D
    {
        // index of the controlling player
        private PlayerIndex controllingPlayer;

        // which player is this - one or two
        private int playerIndex;

        // the physical properties of the player's 2D game object
        private static GameObject2DProperties properties = new GameObject2DProperties
        (
            "Player",                   // type
            true,                       // always update the player
            0,                          // process range is not used - player always updates        
            new Vector2(-32, -64),      // display offset
            new Vector2(64, 64),        // display size
            new Vector2(-16, -16),      // collision offset
            new Vector2(32, 16)         // collision size
        );

        // array to hold the keys for each of the players
        private PlayerKeys playerKeys;
        
        // Player Data - Health, number of lives etc..
        public PlayerData playerData;

        // random number generator
        public Random random;

        // player's state manager
        public GameObjectStateManager stateManager;

        // list of the player's available states
        public PlayerState_Spawn playerState_Spawn;
        public PlayerState_Idle playerState_Idle;
        public PlayerState_WalkLeft playerState_WalkLeft;
        public PlayerState_WalkUpLeft playerState_WalkUpLeft;
        public PlayerState_WalkUp playerState_WalkUp;
        public PlayerState_WalkUpRight playerState_WalkUpRight;
        public PlayerState_WalkRight playerState_WalkRight;
        public PlayerState_WalkDownRight playerState_WalkDownRight;
        public PlayerState_WalkDown playerState_WalkDown;
        public PlayerState_WalkDownLeft playerState_WalkDownLeft;
        public PlayerState_Die playerState_Die;

        // reference to the shadow system used to draw the shadow under the player
        ShadowSystem shadowSystem;

        //------------------------------------------------------------------------------
        // Constructor: Player 
        // Author: Neil Holmes & Andrew Green
        // Summary: Constructor - prepares the player for use
        //------------------------------------------------------------------------------
        public Player(Game game, GameObject2DManager gameObjectManager, DisplayManager displayManager, Vector2 position, PlayerIndex controllingPlayer, ShadowSystem shadowSystem, int playerIndex)
            : base(game, gameObjectManager, displayManager, position, properties)
        {
            // store the controlling player index
            this.controllingPlayer = controllingPlayer;

            // store the player index
            this.playerIndex = playerIndex;

            // store a reference to the shadow system
            this.shadowSystem = shadowSystem;

            // create a random number generator
            random = new Random();

            // create all of the players states
            playerState_Spawn = new PlayerState_Spawn(this);
            playerState_Idle = new PlayerState_Idle(this);
            playerState_WalkLeft = new PlayerState_WalkLeft(this);
            playerState_WalkUpLeft = new PlayerState_WalkUpLeft(this);
            playerState_WalkUp = new PlayerState_WalkUp(this);
            playerState_WalkUpRight = new PlayerState_WalkUpRight(this);
            playerState_WalkRight = new PlayerState_WalkRight(this);
            playerState_WalkDownRight = new PlayerState_WalkDownRight(this);
            playerState_WalkDown = new PlayerState_WalkDown(this);
            playerState_WalkDownLeft = new PlayerState_WalkDownLeft(this);
            playerState_Die = new PlayerState_Die(this);
            
            // create the player's state manager
            stateManager = new GameObjectStateManager(game);

            // set direction to right
            Velocity = new Vector2(4, 0);

            // initialise player data
            playerData = new PlayerData();

            // reset player data to defaults
            playerData.Reset();
        }

        //------------------------------------------------------------------------------
        // Method: LoadContent
        // Author: Neil Holmes & Andrew Green
        // Summary: load all the content that the player requires and prepare the player
        //          to be drawn and processed for the first time
        //------------------------------------------------------------------------------
        public override void LoadContent(ContentManager content)
        {
            // load all of the players graphics
            LoadFrames(content, (int)PlayerAnimation.spawn, @"Game\Player" + playerIndex.ToString() + @"\Spawn\", 1);
            LoadFrames(content, (int)PlayerAnimation.walkLeft, @"Game\Player" + playerIndex.ToString() + @"\WalkLeft\", 1);
            LoadFrames(content, (int)PlayerAnimation.walkUpLeft, @"Game\Player" + playerIndex.ToString() + @"\WalkUpLeft\", 1);
            LoadFrames(content, (int)PlayerAnimation.walkUp, @"Game\Player" + playerIndex.ToString() + @"\WalkUp\", 1);
            LoadFrames(content, (int)PlayerAnimation.walkUpRight, @"Game\Player" + playerIndex.ToString() + @"\WalkUpRight\", 1);
            LoadFrames(content, (int)PlayerAnimation.walkRight, @"Game\Player" + playerIndex.ToString() + @"\WalkRight\", 1);
            LoadFrames(content, (int)PlayerAnimation.walkDownRight, @"Game\Player" + playerIndex.ToString() + @"\WalkDownRight\", 1);
            LoadFrames(content, (int)PlayerAnimation.walkDown, @"Game\Player" + playerIndex.ToString() + @"\WalkDown\", 1);
            LoadFrames(content, (int)PlayerAnimation.walkDownLeft, @"Game\Player" + playerIndex.ToString() + @"\WalkDownLeft\", 1);
            LoadFrames(content, (int)PlayerAnimation.death1, @"Game\Player" + playerIndex.ToString() + @"\Death1\", 1);
            LoadFrames(content, (int)PlayerAnimation.death2, @"Game\Player" + playerIndex.ToString() + @"\Death2\", 1);
            LoadFrames(content, (int)PlayerAnimation.death3, @"Game\Player" + playerIndex.ToString() + @"\Death3\", 1);
            LoadFrames(content, (int)PlayerAnimation.death4, @"Game\Player" + playerIndex.ToString() + @"\Death4\", 1);
            LoadFrames(content, (int)PlayerAnimation.death5, @"Game\Player" + playerIndex.ToString() + @"\Death5\", 1);
            
            // set the default animation speed (num frames per second)
            DefaultAnimationSpeed = 6;

            // set the player to their idle state
            // NOTE: this MUST be done after content is loaded so that animation frames are available
            stateManager.SetState(playerState_Spawn);

            // initialise the key inputs for this player
            if (playerIndex == 1)
                playerKeys = new PlayerKeys(Keys.A, Keys.D, Keys.W, Keys.S);
            else
                playerKeys = new PlayerKeys(Keys.J, Keys.L, Keys.I, Keys.K);
        }

        //------------------------------------------------------------------------------
        // Method: Update
        // Author: Neil Holmes & Andrew Green
        // Summary: updates the player's current state
        //------------------------------------------------------------------------------
        public override void Update(Vector2 worldPosition)
        {
            // update the current state
            stateManager.Update();
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

        //------------------------------------------------------------------------------
        // Method: ProcessStandardInputs
        // Author: Neil Holmes & Andrew Green
        // Summary: process the standard set of player inputs and respond accordingly
        //------------------------------------------------------------------------------
        public bool ProcessStandardInputs()
        {
            // check for walking left
            if (InputManager.IsKeyPressed(playerKeys.left, null))
            {
                // check for walking up and left
                if (InputManager.IsKeyPressed(playerKeys.up, null))
                {
                    return stateManager.SetState(playerState_WalkUpLeft);
                }

                // check for walking down and left
                if (InputManager.IsKeyPressed(playerKeys.down, null))
                {
                    return stateManager.SetState(playerState_WalkDownLeft);
                }

                // just walking left
                return stateManager.SetState(playerState_WalkLeft);
            }

            // check for walking right
            if (InputManager.IsKeyPressed(playerKeys.right, null))
            {
                // check for walking up and right
                if (InputManager.IsKeyPressed(playerKeys.up, null))
                {
                    return stateManager.SetState(playerState_WalkUpRight);
                }

                // check for walking down and right
                if (InputManager.IsKeyPressed(playerKeys.down, null))
                {
                    return stateManager.SetState(playerState_WalkDownRight);
                }

                // just walking right
                return stateManager.SetState(playerState_WalkRight);
            }

            // check for walking up
            if (InputManager.IsKeyPressed(playerKeys.up, null))
            {
                // just walking up
                return stateManager.SetState(playerState_WalkUp);
            }

            // check for walking down
            if (InputManager.IsKeyPressed(playerKeys.down, null))
            {
                // just walking down
                return stateManager.SetState(playerState_WalkDown);
            }

            // debug functionality to kill the player
            if (InputManager.WasKeyPressed(Keys.X, null))
            {
                return stateManager.SetState(playerState_Die);
            }
            
            // if we get here the player is not providing any input - ensure we are in idle state
            return stateManager.SetState(playerState_Idle);
        }
    }
}
