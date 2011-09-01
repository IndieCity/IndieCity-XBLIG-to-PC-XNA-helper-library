//------------------------------------------------------------------------------
// Filename: Pointer.cs
// Author: Neil Holmes & Andrew Green
// Summary: Control and display of AN in game cursor
//------------------------------------------------------------------------------
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace IndieCityXna.Common
{
    // three types of pointer are available
    public enum PointerType
    {
        mouse = 0,
        rightStick,
        leftStick
    };

    //------------------------------------------------------------------------------
    // Class: Pointer
    // Author: nholmes
    // Summary: an in game pointer than can be controlled by the mouse or joypad
    //------------------------------------------------------------------------------
    public class Pointer
    {
        // handle to the parent game
        private Game game;

        // handle to the display manager service
        DisplayManager displayManager;

        // handle to the timer System service
        TimerSystem timerSystem;

        // handle to the input manager service
        InputManager inputManager;

        // stores the current pointer type
        private PointerType pointerType;

        // is the pointer currently visible?
        private bool visible;

        // position of the pointer
        private Vector2 position;

        // the render position of the mouse pointer
        private Vector2 renderPosition;
 
        // the pointer graphic
        private Texture2D pointerTexture;

        // movement speed of the pointer when controlled by the analog sticks - measured in pixels/second
        private float pointerSpeed;

        //------------------------------------------------------------------------------
        // Constructor: Pointer
        // Author: Neil Holmes & Andrew Green
        // Summary: creates a software pointer that can be turned on and off and 
        //          controlled in three different ways
        //------------------------------------------------------------------------------
        public Pointer(Game game, PointerType pointerType, bool visible)
        {
            // store the game that owns this pointer
            this.game = game;

            // get a handle to the display manager service
            displayManager = (DisplayManager)game.Services.GetService(typeof(DisplayManager));

            // get a handle to the timer system service
            timerSystem = (TimerSystem)game.Services.GetService(typeof(TimerSystem));

            // get a handle to the input manager service
            inputManager = (InputManager)game.Services.GetService(typeof(InputManager));
            
            // store the pointer type and visibility setting
            this.pointerType = pointerType;
            this.visible = visible;

            // set the initial position of the pointer according to the pointer type
            switch (pointerType)
            {
                case PointerType.mouse:

                    // pointer is mouse driven - get the position from the hardware mouse
                    inputManager.GetMousePosition(out position.X, out position.Y);
                    break;

                case PointerType.rightStick:
                case PointerType.leftStick:

                    // pointer is joypad driven - set the intial position to be the centre of the screen
                    position = new Vector2(displayManager.GameResolutionX / 2, displayManager.GameResolutionY / 2);
                    break;
            }

            // load the pointer graphic
            pointerTexture = game.Content.Load<Texture2D>(@"System\Pointer");

            // set the pointer speed for analog stick controlled pointer to 600 pixels/second;
            pointerSpeed = 600;
        }

        //------------------------------------------------------------------------------
        // Method: Update
        // Author: Neil Holmes & Andrew Green
        // Summary: updates the pointer
        //------------------------------------------------------------------------------
        public void Update()
        {
            float stickPosX, stickPosY;
            
            // update the  pointer according to the pointer type
            switch (pointerType)
            {
                case PointerType.mouse:

                    // pointer is mouse driven - get the position from the hardware mouse
                    inputManager.GetMousePosition(out position.X, out position.Y);
                    break;

                case PointerType.rightStick:

                    // use right stick movement to move the pointer
                    inputManager.GetRightStickPosition(PlayerIndex.One, out stickPosX, out stickPosY);
                    position.X += stickPosX * (pointerSpeed * timerSystem.TimeStep);
                    position.Y -= stickPosY * (pointerSpeed * timerSystem.TimeStep);
                    break;

                case PointerType.leftStick:

                    // use left stick movement to move the pointer
                    inputManager.GetLeftStickPosition(PlayerIndex.One, out stickPosX, out stickPosY);
                    position.X += stickPosX * (pointerSpeed * timerSystem.TimeStep);
                    position.Y -= stickPosY * (pointerSpeed * timerSystem.TimeStep);
                   break;
            }

            // ok, we now need to convert the actual position into a position that will make sense for rendering the pointer
            // and as a position that we can test for interacting with on-screen elements in a sensible way :)
            
            // are we in full screen or windowed mode?
            if (displayManager.CurrentScreenMode == ScreenMode.Windowed)
            {
                // scale the desktop mouse position into a render space position
                renderPosition = position * displayManager.DisplayScale;

                // apply the scaled display offset to render space position
                renderPosition.X -= displayManager.DisplaySize.X * displayManager.DisplayScale;
                renderPosition.Y -= displayManager.DisplaySize.Y * displayManager.DisplayScale;

            }
            else
            {
                // just use the provided position
                renderPosition = position;
            }

            // apply the render scale to the position
            renderPosition /= displayManager.RenderScale;
        }
        
        //------------------------------------------------------------------------------
        // Method: Draw
        // Author: Neil Holmes & Andrew Green
        // Summary: draws the pointer
        //------------------------------------------------------------------------------
        public void Draw()
        {
            // get a reference to the sprite batch from game state manager
            SpriteBatch spriteBatch = displayManager.GlobalSpriteBatch;

            // draw the mouse pointer
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, displayManager.TransformMatrix);
            spriteBatch.Draw(pointerTexture, renderPosition, Color.White);
            spriteBatch.End();
        }

        //------------------------------------------------------------------------------
        // Property: Position
        // Author: Neil Holmes & Andrew Green
        // Summary: gets the screen-space position of the pointer
        //------------------------------------------------------------------------------
        public Vector2 Position
        {
            get { return renderPosition; }
        }

        //------------------------------------------------------------------------------
        // Property: Visible
        // Author: Neil Holmes & Andrew Green
        // Summary: gets or sets the visible status of the pointer
        //------------------------------------------------------------------------------
        public bool Visible
        {
            get { return visible; }
            set { visible = value; }
        }
    }
}
