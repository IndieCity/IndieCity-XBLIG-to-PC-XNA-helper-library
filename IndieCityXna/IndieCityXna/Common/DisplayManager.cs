//------------------------------------------------------------------------------
// Filename: DisplayManager.cs
// Author: Neil Holmes & Andrew Green
// Summary: everything to do with displaying the game and setting screen sizes
//------------------------------------------------------------------------------

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace IndieCityXna.Common
{
    // public enum of available screen modes - windowed or full screen
    public enum ScreenMode
    {
        Windowed = 0,
        FullScreen = 1,
    };

    //------------------------------------------------------------------------------
    // Class: DisplayManager
    // Author: Neil Holmes & Andrew Green
    // Summary: handles set up and maintenance of the display. supports full screen 
    //          and windowed modes and provides functionality for correct display 
    //          of the game at any resolution or window size
    //------------------------------------------------------------------------------
    public class DisplayManager
    {
        // parent game
        private Game game;

        // parent window
        private GameWindow window;

        // game's graphics device manager
        private GraphicsDeviceManager graphicsDeviceManager;

        // the aspect ratio we should be using for full screen display modes
        private float aspectRatio;

        // main render target - all graphics are rendered here and then scaled to the back buffer once per frame
        private RenderTarget2D mainRenderTarget;

        // the actual pixel resolution the game will render at
        private Vector2 gameResolution;

        // the screen resolution the game will try to display at when in full screen mode
        private Vector2 fullScreenDisplaySize;

        // the size of the window that the game will display in when in windowed mode
        private Vector2 windowedDisplaySize;

        // rectangle size and offset used for copying the back buffer to the display buffer, calculated by CalculateDisplaySize()
        private Rectangle displaySize;

        // the current screen mode - windowed or full screen
        private ScreenMode currentScreenMode;

        // the render scale - allows a game to think it is rendering at normal resolution but actually be drawing at a smaller one
        private float renderScale;
        
        // the transform matrix - allows a game to think it is rendering at normal resolution but actually be drawing at a smaller one
        private Matrix transformMatrix;

        // global sprite batch instance to be used by the game
        private SpriteBatch globalSpriteBatch;

        // a blank texture for use when drawing the black bars in letter boxed modes
        private Texture2D blankTexture;

        // font for displaying debug messages
        private SpriteFont debugFont;

        //------------------------------------------------------------------------------
        // Constructor: DisplayManager
        // Author: Neil Holmes & Andrew Green
        // Summary: creates the display manager and sets-up the back buffer and
        //          main render targets then sets the requested screen mode
        //------------------------------------------------------------------------------
        public DisplayManager(Game game, GameWindow window, GraphicsDeviceManager graphicsDeviceManager, int width, int height, ScreenMode screenMode, float renderScale)
        {
            int bestFullScreenWidth, bestFullScreenHeight;

            // store a reference to the parent game
            this.game = game;

            // store a reference to the parent window
            this.window = window;

            // store a reference to the parent game's graphics device manager
            this.graphicsDeviceManager = graphicsDeviceManager;

            // store a copy of the requested render scale
            this.renderScale = renderScale;

            // create the transform matrix using the requested render scale
            transformMatrix = Matrix.CreateScale(renderScale, renderScale, 1.0f);

            // tell the window that we want to allow resizing (remove if you don't want this!)
            window.AllowUserResizing = true;

            // stop the user from being able to make the window so small that we lose the graphics device ;)
            // this happens if the window height becomes zero so you can set the minimum size as low as you like as
            // long as you dont allow it to reach zero. if you turn off AllowUserResizing you dont need this
            System.Windows.Forms.Form.FromHandle(window.Handle).MinimumSize = new System.Drawing.Size(320, 240);

            // subscribe to the game window's ClientSizeChanged event - again not needed if you turn off user resizing
            window.ClientSizeChanged += new EventHandler<System.EventArgs>(WindowClientSizeChanged);

            // force vsync, because not doing so is UGLY and LAZY and BAD and developers that don't use it should be SHOT :P
            graphicsDeviceManager.SynchronizeWithVerticalRetrace = true;

            // run at max framerate allowed. you can set a fixed time step if you like but your game should really be set
            // up to run at any render speed or on slow machines it will all slow down, not just drop frames
            game.IsFixedTimeStep = false;

            // grab the pixel aspect ratio from the current desktop display mode - we'll assume that the user has this set 
            // correctly for their monitor and use it to filter our full screen modes accordingly
            aspectRatio = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.AspectRatio;

            // store the requested game resolution
            gameResolution.X = width;
            gameResolution.Y = height;

            // set the prefered windowed size for the game to be the game resolution (take rendering scale into account)
            windowedDisplaySize.X = width * renderScale;
            windowedDisplaySize.Y = height * renderScale;

            // check that the prefered window size is not larger than the desktop - if it is make it 10% smaller than the desktop
            // size so that the user can actually see it all and mvoe it around/minimize/maximize it etc
            if (windowedDisplaySize.X > GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width) windowedDisplaySize.X = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width * 0.9f;
            if (windowedDisplaySize.Y > GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height) windowedDisplaySize.Y = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height * 0.9f;

            // find the most suitable full screen resolution (take rendering scale into account)
            FindBestFullScreenMode((int)(width * renderScale), (int)(height * renderScale), aspectRatio, out bestFullScreenWidth, out bestFullScreenHeight);
            fullScreenDisplaySize.X = bestFullScreenWidth;
            fullScreenDisplaySize.Y = bestFullScreenHeight;

            // set the requested screen mode (full screen or windowed)
            SetScreenMode(screenMode);

            // initialise the main render target and depth buffer that will be used for all game rendering
            mainRenderTarget = CreateMainRenderTarget(renderScale);

            // create the global sprite batch that we will use for doing all 2d rendering in the game
            globalSpriteBatch = new SpriteBatch(graphicsDeviceManager.GraphicsDevice);

            // load a blank texture to use for drawing black bars for letter boxed screen modes
            blankTexture = game.Content.Load<Texture2D>(@"System\Blank");
            
            // load a font we can use for displaying debug messages
            debugFont = game.Content.Load<SpriteFont>(@"System\DebugFont");
        }

        //------------------------------------------------------------------------------
        // Method: FindBestFullScreenMode
        // Author: Neil Holmes & Andrew Green
        // Summary: scans through all the available display modes and finds one that best
        //          fits the game's requested resoltuion. displayManager will automatically
        //          scale the game res to fit whatever resolution is closest if an exact
        //          match is not found
        //------------------------------------------------------------------------------
        private void FindBestFullScreenMode(int desiredWidth, int desiredHeight, float aspectRatio, out int bestWidth, out int bestHeight)
        {
            // start off with some impossible numbers for the best width and height
            bestWidth = bestHeight = Int32.MaxValue;

            // run through all the available modes on the default adapter and find the one that's closes to the game's
            // width, height and format
            foreach (DisplayMode displayMode in GraphicsAdapter.DefaultAdapter.SupportedDisplayModes)
            {
                // check if this mode is closer to the requested size than any we already tested
                // NOTE: this test will always pick a resolution larger than the required size rather than smaller if no 
                // exact match for the requested resolution is not available
                if ((displayMode.Width - desiredWidth) >= 0 && displayMode.Width < bestWidth && (displayMode.Height - desiredHeight) >= 0 && displayMode.Height < bestHeight && displayMode.AspectRatio == aspectRatio)
                {
                    // found a better resolution match than any previous mode tested - store it's size
                    bestWidth = displayMode.Width;
                    bestHeight = displayMode.Height;
                }
            }

            // check if we found a good match and drop out if we have!
            if (bestWidth != Int32.MaxValue) return;

            // ok, if we get here then no available resolution was large enough to display the requested game resolution, or,
            // none that were large enough matched the aspect ration we were hoping for. scan through the list of modes again
            // and pick the largest available that matches the aspect ratio we are after
            bestWidth = bestHeight = 0;

            foreach (DisplayMode displayMode in GraphicsAdapter.DefaultAdapter.SupportedDisplayModes)
            {
                // check if this mode is larger than any previous mode tested - we can ignore the fact that this will
                // tend to find the largest size available because we've already proven that no available screen size
                // was large enough for the desired game resolution... just get the biggest mode we can!
                if (displayMode.Width >= bestWidth && displayMode.Height >= bestHeight && displayMode.AspectRatio == aspectRatio)
                {
                    // found a larger resolution than any previous mode tested - store it's size
                    bestWidth = displayMode.Width;
                    bestHeight = displayMode.Height;
                }
            }

            // check if we found a match and drop out if we have!
            if (bestWidth != Int32.MaxValue) return;

            // ok, if we get here then trying to match the aspect ratio is almost certainly making it so that we can't find any
            // suitable screen resolution - let's try again, but this time we'll ignore the aspect ratio and just go for the best
            // size we can find...
            bestWidth = bestHeight = Int32.MaxValue;

            foreach (DisplayMode displayMode in GraphicsAdapter.DefaultAdapter.SupportedDisplayModes)
            {
                // check if this mode is closer to the requested size than any we already tested
                // NOTE: this test will always pick a resolution larger than the required size rather than smaller if an 
                // exact match for the requested resolution is not available
                if ((displayMode.Width - desiredWidth) >= 0 && displayMode.Width < bestWidth && (displayMode.Height - desiredHeight) >= 0 && displayMode.Height < bestHeight)
                {
                    // found a better resolution match than any previous mode tested - store it's size
                    bestWidth = displayMode.Width;
                    bestHeight = displayMode.Height;
                }
            }

            // check if we found a match and drop out if we have!
            if (bestWidth != Int32.MaxValue) return;

            // ok, we're almost out of options! just try and find *something* that we can use! We really don't care about the
            // size or aspect ratio at this point! this should not happen, but you never know! ;-)
            bestWidth = bestHeight = 0;

            foreach (DisplayMode displayMode in GraphicsAdapter.DefaultAdapter.SupportedDisplayModes)
            {
                // check if this mode is larger than any previous mode tested - we can ignore the fact that this will
                // tend to find the largest size available because we've already proven that no available screen size
                // was large enough for the desired game resolution... just get the biggest mode we can!
                if (displayMode.Width >= bestWidth && displayMode.Height >= bestHeight)
                {
                    // found a larger resolution than any previous mode tested - store it's size
                    bestWidth = displayMode.Width;
                    bestHeight = displayMode.Height;
                }
            }
        }

        //------------------------------------------------------------------------------
        // Method: WindowClientSizeChanged
        // Author: Neil Holmes & Andrew Green
        // Summary: event handler called when the window size is changed. calculates the
        //          new window size and updates the display manager's settings accordingly
        //------------------------------------------------------------------------------
        void WindowClientSizeChanged(object sender, EventArgs e)
        {
            // we only care if we are in windowed mode
            if (currentScreenMode != ScreenMode.Windowed) return;

            // get the updated width and height of the window
            windowedDisplaySize.X = window.ClientBounds.Width;
            windowedDisplaySize.Y = window.ClientBounds.Height;

            // if the window size has been set to zero in x or y don't set the new size
            // NOTE: this should only occur if the window has been minimised!
            if (windowedDisplaySize.X == 0 || windowedDisplaySize.Y == 0) 
                return;

            // update the display settings to handle the new window size.
            SetScreenMode(currentScreenMode);
        }

        //------------------------------------------------------------------------------
        // Method: CreateMainRenderTarget
        // Author: Neil Holmes & Andrew Green
        // Summary: helper function for creating the main render target
        //------------------------------------------------------------------------------
        public RenderTarget2D CreateMainRenderTarget(float renderScale)
        {
            // create the main render target
            return new RenderTarget2D(graphicsDeviceManager.GraphicsDevice, (int)(gameResolution.X * renderScale), (int)(gameResolution.Y * renderScale), false, SurfaceFormat.Color, DepthFormat.Depth24Stencil8);
        }
 
        //------------------------------------------------------------------------------
        // Method: SetScreenMode
        // Author: Neil Holmes & Andrew Green
        // Summary: helper function for setting full screen or windowed display mode
        //------------------------------------------------------------------------------
        public void SetScreenMode(ScreenMode screenMode)
        {
            // store the new screen mode
            currentScreenMode = screenMode;

            // process according to the new screen mode
            switch (currentScreenMode)
            {
                case ScreenMode.FullScreen:

                    // create a backbuffer that matches the resolution of the current display mode
                    graphicsDeviceManager.PreferredBackBufferWidth = (int)fullScreenDisplaySize.X;
                    graphicsDeviceManager.PreferredBackBufferHeight = (int)fullScreenDisplaySize.Y;
                    graphicsDeviceManager.IsFullScreen = true;
                    graphicsDeviceManager.ApplyChanges();
                    break;

                case ScreenMode.Windowed:

                    // create a backbuffer that matches the resolution of the current display mode
                    graphicsDeviceManager.PreferredBackBufferWidth = (int)windowedDisplaySize.X;
                    graphicsDeviceManager.PreferredBackBufferHeight = (int)windowedDisplaySize.Y;
                    graphicsDeviceManager.IsFullScreen = false;
                    graphicsDeviceManager.ApplyChanges();
                    break;
            }

            // calculate the display rectangle needed to fit the game to the requested display size
            CalculateDisplaySize();
        }

        //------------------------------------------------------------------------------
        // Method: CalculateDisplaySize
        // Author: Neil Holmes & Andrew Green
        // Summary: calculates the display rectangle needed to copy the game from the 
        //          main render target to a back buffer of the requested display size
        //------------------------------------------------------------------------------
        private void CalculateDisplaySize()
        {
            // are we in windowed mode, or full screen mode?
            if (currentScreenMode == ScreenMode.Windowed)
            {
                // check if x size matches the window width
                if (gameResolution.X != windowedDisplaySize.X)
                {
                    // x size does not match, set the maximum we can have and scale the y size to match it
                    float scale = windowedDisplaySize.X / gameResolution.X;
                    displaySize.Width = (int)windowedDisplaySize.X;
                    displaySize.Height = (int)(gameResolution.Y * scale);
                }
                else
                {
                    // x size matches, store it and the y size
                    displaySize.Width = (int)gameResolution.X;
                    displaySize.Height = (int)gameResolution.Y;
                }

                // check y size fits window height
                if (displaySize.Height > windowedDisplaySize.Y)
                {
                    // y size does not fit, set the maximum we can have and scale the x size down to match it
                    float scale = windowedDisplaySize.Y / displaySize.Height;
                    displaySize.Width = (int)(displaySize.Width * scale);
                    displaySize.Height = (int)windowedDisplaySize.Y;
                }

                // calculate the x and y offsets for the display rectangle
                displaySize.X = (int)((windowedDisplaySize.X - displaySize.Width) * 0.5f);
                displaySize.Y = (int)((windowedDisplaySize.Y - displaySize.Height) * 0.5f);
            }
            else
            {
                // check x size matches the screen resolution
                if (gameResolution.X != fullScreenDisplaySize.X)
                {
                    // x size does not match, set the maximum we can have and scale the y size to match it
                    float scale = fullScreenDisplaySize.X / gameResolution.X;
                    displaySize.Width = (int)fullScreenDisplaySize.X;
                    displaySize.Height = (int)(gameResolution.Y * scale);
                }
                else
                {
                    // x size matches, store it and the y size
                    displaySize.Width = (int)gameResolution.X;
                    displaySize.Height = (int)gameResolution.Y;
                }

                // check y size fits the screen resolution
                if (displaySize.Height > fullScreenDisplaySize.Y)
                {
                    // y size does not fit, set the maximum we can have and scale the x size down to match it
                    float scale = fullScreenDisplaySize.Y / displaySize.Height;
                    displaySize.Width = (int)(displaySize.Width * scale);
                    displaySize.Height = (int)fullScreenDisplaySize.Y;
                }

                // calculate the x and y offsets for the display rectangle
                displaySize.X = (int)((fullScreenDisplaySize.X - displaySize.Width) * 0.5f);
                displaySize.Y = (int)((fullScreenDisplaySize.Y - displaySize.Height) * 0.5f);
            }
        }

        //------------------------------------------------------------------------------
        // Method: StartDraw
        // Author: Neil Holmes & Andrew Green
        // Summary: prepares everything for drawing a new frame
        //------------------------------------------------------------------------------
        public void StartDraw()
        {
            // set the main render target as the destination for all draw calls
            graphicsDeviceManager.GraphicsDevice.SetRenderTarget(mainRenderTarget);
        }

        //------------------------------------------------------------------------------
        // Method: EndDraw
        // Author: Neil Holmes & Andrew Green
        // Summary: copies the finished frame to the back buffer for display
        //------------------------------------------------------------------------------
        public void EndDraw()
        {
            Rectangle shape;
            
            // set the render target to point at the back buffer
            graphicsDeviceManager.GraphicsDevice.SetRenderTarget(null);

            // start the sprite batch (we don't use the matrix for this as we dont want this final copy to be scaled!)
            globalSpriteBatch.Begin();

            // copy the main render target into the back buffer 
            globalSpriteBatch.Draw(mainRenderTarget, displaySize, Color.White);

            // draw black bars if the main render target doesn't exactly fit the screen
            if (displaySize.Y > 0)
            {
                // draw the bar at the top of the screen
                shape.X = shape.Y = 0;
                shape.Width = displaySize.Width;
                shape.Height = displaySize.Y;
                globalSpriteBatch.Draw(blankTexture, shape, Color.Black);

                // draw the bar at the bottom of the screen
                shape.Y = displaySize.Y + displaySize.Height;
                globalSpriteBatch.Draw(blankTexture, shape, Color.Black);
            }
            else if (displaySize.X > 0)
            {
                // draw the bar on the left side of the screen
                shape.X = shape.Y = 0;
                shape.Width = displaySize.X;
                shape.Height = displaySize.Height;
                globalSpriteBatch.Draw(blankTexture, shape, Color.Black);

                // draw the bar on the right side of the screen
                shape.X = displaySize.X + displaySize.Width;
                globalSpriteBatch.Draw(blankTexture, shape, Color.Black);
            }

            // draw safe zone borders
            //DrawSafeArea(spriteBatch);

            // all done - end sprite batch
            globalSpriteBatch.End();
        }

        //------------------------------------------------------------------------------
        // Method: DrawSafeArea
        // Author: Neil Holmes & Andrew Green
        // Summary: returns the amount of scaling that is currently being applied to the 
        //          game's render resolution in order to display it on screen
        //------------------------------------------------------------------------------
        public void DrawSafeArea(SpriteBatch spriteBatch)
        {
            Rectangle rectangle = new Rectangle();
            Color color = new Color(127, 0, 0, 127);

            int screenWidth = (int)gameResolution.X;
            int screenHeight = (int)gameResolution.Y;
            int borderWidth = (int)(screenWidth * 0.1f);
            int borderHeight = (int)(screenHeight * 0.1f);

            // draw the top unsafe area
            rectangle.X = 0;
            rectangle.Y = 0;
            rectangle.Width = screenWidth;
            rectangle.Height = borderHeight;
            spriteBatch.Draw(blankTexture, rectangle, color);

            // draw the left side unsafe area
            rectangle.X = 0;
            rectangle.Y = screenHeight - borderHeight;
            rectangle.Width = screenWidth;
            rectangle.Height = borderHeight;
            spriteBatch.Draw(blankTexture, rectangle, color);

            // draw the bottom unsafe area
            rectangle.X = 0;
            rectangle.Y = borderHeight;
            rectangle.Width = borderWidth;
            rectangle.Height = screenHeight - (borderHeight * 2);
            spriteBatch.Draw(blankTexture, rectangle, color);

            // draw the right side unsafe area
            rectangle.X = screenWidth - borderWidth;
            rectangle.Y = borderHeight;
            rectangle.Width = borderWidth;
            rectangle.Height = screenHeight - (borderHeight * 2);
            spriteBatch.Draw(blankTexture, rectangle, color);
        }

        //------------------------------------------------------------------------------
        // Property: CurrentScreenMode
        // Author: Neil Holmes & Andrew Green
        // Summary: returns the screen mode that the game is currently running in
        //------------------------------------------------------------------------------
       public ScreenMode CurrentScreenMode
        {
            get { return currentScreenMode; }
        }

        //------------------------------------------------------------------------------
        // Property: DisplayScale
        // Author: Neil Holmes & Andrew Green
        // Summary: returns the amount of scaling that is currently being applied to the 
        //          game's render resolution in order to display it on screen
        //------------------------------------------------------------------------------
        public float DisplayScale
        {
            get 
            {
                if (currentScreenMode == ScreenMode.FullScreen)
                {
                    if (gameResolution.X != fullScreenDisplaySize.X)
                        return gameResolution.X / fullScreenDisplaySize.X;
                    else if (gameResolution.Y != fullScreenDisplaySize.Y)
                        return gameResolution.Y / fullScreenDisplaySize.Y;
                    else
                        return 1.0f;
                }
                else
                {
                    if (displaySize.Y > 0)
                        return gameResolution.X / windowedDisplaySize.X;
                    else if (displaySize.X > 0)
                        return gameResolution.Y / windowedDisplaySize.Y;
                    else
                        return 1.0f;
                }
            }
        }

        //------------------------------------------------------------------------------
        // Property: GraphicsDeviceManager
        // Author: Neil Holmes & Andrew Green
        // Summary: returns the graphics device manager
        //------------------------------------------------------------------------------
        public GraphicsDeviceManager GraphicsDeviceManager
        {
            get { return graphicsDeviceManager; }
        }

        //------------------------------------------------------------------------------
        // Property: DisplaySize
        // Author: Neil Holmes & Andrew Green
        // Summary: returns the rectangle being using to define the display size and
        //          offset of the main render target within the back buffer
        //------------------------------------------------------------------------------
        public Rectangle DisplaySize
        {
            get { return displaySize; }
        }

        //------------------------------------------------------------------------------
        // Property: GameResolutionX
        // Author: Neil Holmes & Andrew Green
        // Summary: gets the game's X resolution
        //------------------------------------------------------------------------------
        public int GameResolutionX
        {
            get { return (int)gameResolution.X; }
        }

        //------------------------------------------------------------------------------
        // Property: GameResolutionY
        // Author: Neil Holmes & Andrew Green
        // Summary: gets the display manager's Y resolution
        //------------------------------------------------------------------------------
        public int GameResolutionY
        {
            get { return (int)gameResolution.Y; }
        }

        //------------------------------------------------------------------------------
        // Property: GameResolution
        // Author: Neil Holmes & Andrew Green
        // Summary: gets the display manager's X and Y resolution
        //------------------------------------------------------------------------------
        public Vector2 GameResolution
        {
            get { return gameResolution; }
        }
         
        //------------------------------------------------------------------------------
        // Property: GlobalSpriteBatch
        // Author: Neil Holmes & Andrew Green
        // Summary: gets the global sprite batch instance
        //------------------------------------------------------------------------------
        public SpriteBatch GlobalSpriteBatch
        {
            get { return globalSpriteBatch; }
        }

        //------------------------------------------------------------------------------
        // Property: TransformMatrix
        // Author: Neil Holmes & Andrew Green
        // Summary: gets the global transformation matrix that should be used when drawing
        //------------------------------------------------------------------------------
        public Matrix TransformMatrix
        {
            get { return transformMatrix; }
        }

        //------------------------------------------------------------------------------
        // Property: RenderScale
        // Author: Neil Holmes & Andrew Green
        // Summary: gets the global render scale that we are using when drawing
        //------------------------------------------------------------------------------
        public float RenderScale
        {
            get { return renderScale; }
        }

        //------------------------------------------------------------------------------
        // Property: DebugFont
        // Author: Neil Holmes & Andrew Green
        // Summary: gets the global debug font - useful for displaying on-screen debug!
        //------------------------------------------------------------------------------
        public SpriteFont DebugFont
        {
            get { return debugFont; }
        }
    }
}
