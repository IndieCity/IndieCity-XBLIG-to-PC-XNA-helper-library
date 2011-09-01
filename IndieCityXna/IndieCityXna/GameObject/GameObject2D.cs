//------------------------------------------------------------------------------
// Filename: GameObject2D.cs
// Author: Neil Holmes & Andrew Green
// Summary: a simple 2D game object class
//------------------------------------------------------------------------------

using System.IO;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using IndieCityXna.Common;

namespace IndieCityXna.GameObject
{
    //------------------------------------------------------------------------------
    // Class: GameObject2DProperties
    // Author: Neil Holmes & Andrew Green
    // Summary: class that defines the physical properties of a 2D game object. used 
    //          when constructing a new 2D game object
    //------------------------------------------------------------------------------
    public class GameObject2DProperties
    {
        // the name of this type of 2D game object - set whatever you like!
        public string type;

        // whether the 2D game object should always be updated, regardless of distance from the current world position
        public bool alwaysUpdate;

        // if the 2D game object is not always updated, how far should it be from the current world position before it stops updating?
        public float updateRange;

        // the offset in pixels from the 2D game object's position to the top left corner of it's graphic
        public Vector2 displayOffset;

        // the width and height of 2D game object in pixels - used when drawing
        public Vector2 displaySize;

        // the offset in pixels from the 2D game object's position to the top left corner of it's collision box
        public Vector2 collisionOffset;

        // the width and height of 2D game object's collision box in pixels
        public Vector2 collisionSize;

        //------------------------------------------------------------------------------
        // Constructor: GameObject2DProperties
        // Author: Neil Holmes & Andrew Green
        // Summary: default constructor takes basic information required by the 2D game
        //          object and stores it ready for use
        //------------------------------------------------------------------------------
        public GameObject2DProperties(string type, bool alwaysUpdate, float updateRange, Vector2 displayOffset, Vector2 displaySize, Vector2 collisionOffset, Vector2 collisionSize)
        {
            // simply copy all of the data into the structure
            this.type = type;
            this.alwaysUpdate = alwaysUpdate;
            this.updateRange = updateRange;
            this.displayOffset = displayOffset;
            this.displaySize = displaySize;
            this.collisionOffset = collisionOffset;
            this.collisionSize = collisionSize;
        }
    };

    //------------------------------------------------------------------------------
    // Class: GameObject2D
    // Author: Neil Holmes & Andrew Green
    // Summary: provides common properties and methods for 2D sprite based game
    //          objects. should be a good base for building any sort of 2D game
    //------------------------------------------------------------------------------
    public class GameObject2D
    {
        // parent game
        private Game game;

        // refernce to display manager
        DisplayManager displayManager;

        // parent 2D game object manager
        private GameObject2DManager gameObjectManager;

        // handle to the input manager service
        private InputManager inputManager;

        // handle to the timer system service
        private TimerSystem timerSystem;

        // type of this object (simply a string - handy for debugging!)
        private string type;

        // whether the object should always be updated, regardless of distance from the current world position
        private bool alwaysUpdate;

        // how far should this be from the current world position before it stops updating
        private float updateRange;

        // position of this object
        private Vector2 position;

        // velocity of this object
        private Vector2 velocity;
                
        // number of frames per second that the object's animations should play at
        private float defaultAnimationSpeed;
        private float animationSpeed;

        // animation frames - array holding any number of animations, each with a single frame per texture
        private List<List<Texture2D>> animationFrames;

        // the index of the animation that is currently being displayed
        private int currentAnimation;

        // the current animation frame - used to calculate which texture frame to display
        private float currentAnimFrame;

        // flags for whether the current frame should be flipped in X or Y when drawn
        private bool textureFrameFlipX;
        private bool textureFrameFlipY;

        // the amount of rotation that should be applied to the curernt frame when drawn
        private float textureFrameRotation;

        // display offset and size for this object - specifies x/y offset from the objects origin and it's width and height
        private Vector2 displayOffset;
        private Vector2 displaySize;

        // rotation offset is calculated from the supplied display size - its always half
        private Vector2 rotationOffset;

        // collision offset and size (so that collision boxes can be different from the render size)
        private Vector2 collisionOffset;
        private Vector2 collisionSize;

        //------------------------------------------------------------------------------
        // Constructor: GameObject2D
        // Author: Neil Holmes & Andrew Green
        // Summary: default constructor - initialises a 2D game object ready for use
        //------------------------------------------------------------------------------
        public GameObject2D(Game game, GameObject2DManager gameObjectManager, DisplayManager displayManager, Vector2 position, GameObject2DProperties properties) 
        {
            // Store reference to parent game system object
            this.game = game;

            // Store reference to owning gameObjectManager
            this.gameObjectManager = gameObjectManager;

            // store reference to the display manager
            this.displayManager = displayManager;

            // get the game input manager service
            inputManager = (InputManager)game.Services.GetService(typeof(InputManager));

            // get the game input manager service
            timerSystem = (TimerSystem)game.Services.GetService(typeof(TimerSystem));
               
            // set the type of this game object
            type = properties.type;

            // store whether the object should always be updated
            alwaysUpdate = properties.alwaysUpdate;

            // store the update range for this object
            updateRange = properties.updateRange;

            // store the position
            this.position = position;

            // set initial velocity to zero
            velocity = new Vector2(0, 0);

            // create the master list for the animation frames
            animationFrames = new List<List<Texture2D>>();

            // set some default values for current animation frame and animation speed
            defaultAnimationSpeed = 10;
            animationSpeed = 0;

            // default to showing no animation at all
            currentAnimation = -1;

            // set the current animation frame index to zero
            currentAnimFrame = 0;

            // set the texture frame flip X and & to false by default
            textureFrameFlipX = false;
            textureFrameFlipY = false;
            
            // set the texture frame rotation to zero by defaul
            textureFrameRotation = 0;

            // store the display offset and size
            displayOffset = properties.displayOffset;
            displaySize = properties.displaySize;

            // set the rotation offset based on the display size
            rotationOffset = displaySize * 0.5f;

            // store the collision offset and size
            collisionOffset = properties.collisionOffset;
            collisionSize = properties.collisionSize;
        }

        //------------------------------------------------------------------------------
        // Method: LoadContent
        // Author: Neil Holmes & Andrew Green
        // Summary: virtual function to load any content associated with this object
        //------------------------------------------------------------------------------
        public virtual void LoadContent(ContentManager content) {}

        //------------------------------------------------------------------------------
        // Method: Update
        // Author: Neil Holmes & Andrew Green
        // Summary: virtual function to update the 2D game object
        //------------------------------------------------------------------------------
        public virtual void Update(Vector2 worldPosition) {}

        //------------------------------------------------------------------------------
        // Method: Draw
        // Author: Neil Holmes & Andrew Green
        // Summary: virtual function to draw the 2D game object - provides standard draw
        //          functionality but can be overridden if the user requires
        //------------------------------------------------------------------------------
        public virtual void Draw(Vector2 worldPosition, SpriteBatch spriteBatch, bool sortByYPos = false)
        {
            Rectangle drawPosition;

            // check that we have a valid animation to display
            if (currentAnimation == -1 || currentAnimation > animationFrames.Count) return;

            // check that we have a valid frame to display
            if (currentAnimFrame < 0 || currentAnimFrame > animationFrames[currentAnimation].Count) return;

            // set the top left of the 2D game object for the draw position
            drawPosition.X = (int)(Position.X + DisplayOffset.X - worldPosition.X + rotationOffset.X);
            drawPosition.Y = (int)(Position.Y + DisplayOffset.Y - worldPosition.Y + rotationOffset.Y);

            // set the size of this 2D game object
            drawPosition.Width = (int)DisplaySize.X;
            drawPosition.Height = (int)DisplaySize.Y;

            // set the flip mode to use
            SpriteEffects spriteEffects = SpriteEffects.None;
            if (textureFrameFlipX == true) spriteEffects |= SpriteEffects.FlipHorizontally;
            if (textureFrameFlipY == true) spriteEffects |= SpriteEffects.FlipVertically;

            // draw the 2D game object
            if (sortByYPos)
                spriteBatch.Draw(animationFrames[currentAnimation][(int)currentAnimFrame], drawPosition, null, Color.White, textureFrameRotation, rotationOffset, spriteEffects, position.Y / 720);
            else
                spriteBatch.Draw(animationFrames[currentAnimation][(int)currentAnimFrame], drawPosition, null, Color.White, textureFrameRotation, rotationOffset, spriteEffects, 0);

            //spriteBatch.DrawString(displayManager.DebugFont, (position.Y / 720).ToString(), position.Y, Color.White);        
        }

        //------------------------------------------------------------------------------
        // Method: LoadFrames
        // Author: Neil Holmes & Andrew Green
        // Summary: loads all the frames for an animation and adds the animation to the
        //          list of animations that the 2D game object can display
        //          NOTE: expects each frame to be a seperate texture file and the 
        //          filenames to be sequential and ending in the frame index in order to
        //          load them eg: fileName0, fileName1, fileName2 etc
        //------------------------------------------------------------------------------
        public void LoadFrames(ContentManager content, int animation, string fileName, int firstIndex)
        {
            int frame = firstIndex;
            
            // create a list to hold the frames of this animation
            List<Texture2D> frames =  new List<Texture2D>();

            // load texture frames until we can't find any more in the sequence
            while (true)
            {
                // attempt to find the next frame in the sequence, if it doesn't exist we must have reached the end, so bail out
                if (File.Exists(content.RootDirectory + fileName + frame.ToString() + ".xnb"))
                    frames.Add(content.Load<Texture2D>(fileName + frame.ToString()));
                else
                    break;
                
                // found and loaded the frame successfully - move on to the next frame!
                frame ++;
            }
 
            // add the new animation frames to the list of animations
            animationFrames.Add(frames);        
        }

        //------------------------------------------------------------------------------
        // Method: SetAnimation
        // Author: Neil Holmes & Andrew Green
        // Summary: helper function to set the the current animation sequence to be used
        //------------------------------------------------------------------------------
        public void SetAnimation(int animation)
        {
            // store the requested animation
            currentAnimation = animation;
            
            // reset current animation frame
            currentAnimFrame = 0;
        }

        //------------------------------------------------------------------------------
        // Method: ResetAnimation
        // Author: Neil Holmes & Andrew Green
        // Summary: helper function to reset current animation to the beginning
        //------------------------------------------------------------------------------
        public void ResetAnimation()
        {
            // reset current animation frame
            currentAnimFrame = 0;
        }

        //------------------------------------------------------------------------------
        // Method: UpdateAnimation
        // Author: Neil Holmes & Andrew Green
        // Summary: helper function to update the current animation frame. uses the
        //          animation speed and current animation to do the update
        //------------------------------------------------------------------------------
        public bool UpdateAnimation(bool looping)
        {
            // increase the anim frame counter based on the speed and elapsed time
            currentAnimFrame += animationSpeed * TimerSystem.TimeStep;

            // check if the animation has reached the end
            if (currentAnimFrame >= animationFrames[currentAnimation].Count)
            {
                // is this a looping animation?
                if (looping == true)
                {
                    // yup it's looping - wrap the animation back to the start
                    currentAnimFrame %= animationFrames[currentAnimation].Count;

                    // return true to signify that we reached the end of the animation during this update
                    return true;
                }
                else
                {
                    // not looping - clmap the animation frame to the number of frames so that we can tell if it has ended
                    currentAnimFrame = animationFrames[currentAnimation].Count - 1;
                    
                    // return true to signify that we reached the end of the animation during this update
                    return true;
                }
            }

            // return false to signify that we havn't reached the end of the animation during this update
            return false;
        }

        //------------------------------------------------------------------------------
        // Method: Kill
        // Author: Neil Holmes & Andrew Green
        // Summary: virtual function to kill the 2D game object
        //------------------------------------------------------------------------------
        public virtual void Kill()
        {
            // tell the 2D game object manager to remove this 2D game object
            gameObjectManager.RemoveObject(this);
        }

        //------------------------------------------------------------------------------
        // Property: Game
        // Author: Neil Holmes & Andrew Green
        // Summary: get the parent game that owns this 2D game object
        //------------------------------------------------------------------------------
        public Game Game
        {
            get { return game; }
        }

        //------------------------------------------------------------------------------
        // Property: GameObjectManager
        // Author: Neil Holmes & Andrew Green
        // Summary: get the game object manager from this 2D game object 
        //------------------------------------------------------------------------------
        public GameObject2DManager GameObjectManager
        {
            get { return gameObjectManager; }
        }

        //------------------------------------------------------------------------------
        // Property: InputManager
        // Author: Neil Holmes & Andrew Green
        // Summary: get the input manager service from this 2D game object  
        //------------------------------------------------------------------------------
        public InputManager InputManager
        {
            get { return inputManager; }
        }

        //------------------------------------------------------------------------------
        // Property: TimerSystem
        // Author: Neil Holmes & Andrew Green
        // Summary: get the timer system service from this 2D game object 
        //------------------------------------------------------------------------------
        public TimerSystem TimerSystem
        {
            get { return timerSystem; }
        }

        //------------------------------------------------------------------------------
        // Property: Type
        // Author: Neil Holmes & Andrew Green
        // Summary: get the type of this 2D game object
        //------------------------------------------------------------------------------
        public string Type
        {
            get { return type; }
        }

        //------------------------------------------------------------------------------
        // Property: AlwaysUpdate
        // Author: Neil Holmes & Andrew Green
        // Summary: get or set the status of the always update flag
        //------------------------------------------------------------------------------
        public bool AlwaysUpdate
        {
            get { return alwaysUpdate; }
            set { alwaysUpdate = value; }
        }

        //------------------------------------------------------------------------------
        // Property: UpdateRange
        // Author: Neil Holmes & Andrew Green
        // Summary: get or set the update range of this 2D game object
        //------------------------------------------------------------------------------
        public float UpdateRange
        {
            get { return updateRange; }
            set { updateRange = value; }
        }

        //------------------------------------------------------------------------------
        // Property: Position
        // Author: Neil Holmes & Andrew Green
        // Summary: get or set the position of the 2D game oject
        //------------------------------------------------------------------------------
        public Vector2 Position
        {
            get { return position; }
            set { position = value; }
        }

        //------------------------------------------------------------------------------
        // Property: Velocity
        // Author: Neil Holmes & Andrew Green
        // Summary: get or set the velocity of the 2D game oject
        //------------------------------------------------------------------------------
        public Vector2 Velocity
        {
            get { return velocity; }
            set { velocity = value; }
        }

        //------------------------------------------------------------------------------
        // Property: DefaultAnimationSpeed
        // Author: Neil Holmes & Andrew Green
        // Summary: get or set the default animation speed of the 2D game oject
        //------------------------------------------------------------------------------
        public float DefaultAnimationSpeed
        {
            get { return defaultAnimationSpeed; }
            set { defaultAnimationSpeed = value; }
        }

        //------------------------------------------------------------------------------
        // Property: AnimationSpeed
        // Author: Neil Holmes & Andrew Green
        // Summary: get or set the current animation speed of the 2D game oject
        //------------------------------------------------------------------------------
        public float AnimationSpeed
        {
            get { return animationSpeed; }
            set { animationSpeed = value; }
        }

        //------------------------------------------------------------------------------
        // Property: TextureFrameFlipX
        // Author: Neil Holmes & Andrew Green
        // Summary: get or set the X flip of the texture frame to be used when drawing it
        //------------------------------------------------------------------------------
        public bool TextureFrameFlipX
        {
            get { return textureFrameFlipX; }
            set { textureFrameFlipX = value; }
        }

        //------------------------------------------------------------------------------
        // Property: TextureFrameFlipY
        // Author: Neil Holmes & Andrew Green
        // Summary: get or set the Y flip of the texture frame to be used when drawing it
        //------------------------------------------------------------------------------
        public bool TextureFrameFlipY
        {
            get { return textureFrameFlipY; }
            set { textureFrameFlipY = value; }
        }

        //------------------------------------------------------------------------------
        // Property: TextureFrameRotation
        // Author: Neil Holmes & Andrew Green
        // Summary: get or set the X flip of the texture frame to be used when drawing it
        //------------------------------------------------------------------------------
        public float TextureFrameRotation
        {
            get { return textureFrameRotation; }
            set { textureFrameRotation = value; }
        }

        //------------------------------------------------------------------------------
        // Property: DisplayOffset
        // Author: Neil Holmes & Andrew Green
        // Summary: get or set the display offset of the 2D game oject
        //------------------------------------------------------------------------------
        public Vector2 DisplayOffset
        {
            get { return displayOffset; }
            set { displayOffset = value; }
        }

        //------------------------------------------------------------------------------
        // Property: DisplaySize
        // Author: Neil Holmes & Andrew Green
        // Summary: get or set the display size of the 2D game oject
        //------------------------------------------------------------------------------
        public Vector2 DisplaySize
        {
            get { return displaySize; }
            set 
            { 
                displaySize = value;
                rotationOffset = value * 0.5f;
            }
        }

        //------------------------------------------------------------------------------
        // Property: CollisionOffset
        // Author: Neil Holmes & Andrew Green
        // Summary: get or set the collision offset of the 2D game oject
        //------------------------------------------------------------------------------
        public Vector2 CollisionOffset
        {
            get { return collisionOffset; }
            set { collisionOffset = value; }
        }

        //------------------------------------------------------------------------------
        // Property: CollisionSize
        // Author: Neil Holmes & Andrew Green
        // Summary: get or set the collision size of the 2D game oject
        //------------------------------------------------------------------------------
        public Vector2 CollisionSize
        {
            get { return collisionSize; }
            set { collisionSize = value; }
        }
    }
}