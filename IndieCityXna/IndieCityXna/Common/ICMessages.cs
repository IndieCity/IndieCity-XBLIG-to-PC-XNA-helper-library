//------------------------------------------------------------------------------
// Filename: ICMessages.cs
// Author: Neil Holmes
// Summary: IndieCity message display exmaple
//------------------------------------------------------------------------------

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using System.Collections.Generic;

namespace ICMessages
{
    // the available rendering positions of the achievements
    public enum ICMessagePosition
    {
        topLeft = 0,
        topMiddle,
        topRight,
        bottomLeft,
        bottomMiddle,
        bottomRight,
        listMode
    };

    // the different colour modes of the message display
    public enum ICMessageColorMode
    {
        normal = 0,
        inverted,
    };

    // the different sizes of the message display
    public enum ICMessageScale
    {
        normal = 0,
        small,
    };

    // message priority level for the message display system - urgent interupts any 
    // currently displayed message, others are ordered based on priority and time 
    public enum ICMessagePriority
    {
        low = 0,
        medium,
        high,
        urgent,
    };

    //------------------------------------------------------------------------------
    // Class: ICMessageData
    // Author: Neil Holmes
    // Summary: defines the content of a message
    //------------------------------------------------------------------------------
    public class ICMessageData
    {
        public string title;
        public string message;
        public ICMessagePriority priority;

        public ICMessageData(string title, string message, ICMessagePriority priority)
        {
            this.title = title;
            this.message = message;
            this.priority = priority;
        }
    };

    //------------------------------------------------------------------------------
    // Class: ICMessagePopUp
    // Author: Neil Holmes
    // Summary: class for displaying pop-up messages / warnings etc
    //------------------------------------------------------------------------------
    public class ICMessagePopUp
    {
        // the graphics device
        private GraphicsDevice graphicsDevice;

        // this position mode that the game has requested
        private ICMessagePosition positionMode;

        // the sprite batch to use for drawing
        private bool display;
        private SpriteBatch spriteBatch;

        // position of the message
        private Vector2 displayPosition;

        // position and size of the drawn elements of the achievement
        private int badgeSize;
        private Rectangle badgeRect;
        private Rectangle messageRect;
        private Vector2 titlePosition;
        private Vector2 messagePosition;
        private string titleText;
        private string messageText;
        private int messageWidth;
        private int displayWidth;
        private int messageHeight;
        private float displayScale;

        // update-related values
        private float counter;

        // the message graphics
        private Texture2D badgeTexture;
        private Texture2D bannerTexture;

        // the message fonts
        private SpriteFont titleFont;
        private SpriteFont messageFont;

        // constant values for the animation
        private static int badgeBorderSize = 2;
        private static int messageBorderSize = 8;
        private static int minDisplayWidth = 300;
        private static int maxDisplayWidth = 800;
        private static float safeZoneScaleValue = 0.07f;
        private static float messageAppearDuration = 0.25f;
        private static float messageDisplayDuration = 3.0f;
        private static float messageRemoveDuration = 0.25f;

        // constant colours for normal display
        private static Color normalBannerColor = new Color(255, 255, 255, 255);
        private static Color normalTitleColor = new Color(90, 0, 0, 200);
        private static Color normalMessageColor = new Color(0, 0, 0, 150);

        // constant colours for inverted display
        private static Color invertedBannerColor = new Color(25, 25, 30, 255);
        private static Color invertedTitleColor = new Color(225, 90, 90, 200);
        private static Color invertedMessageColor = new Color(150, 150, 150, 200);

        // the message queue
        List<ICMessageData> messageQueue = new List<ICMessageData>();

        // the different stages of the pop-up message display
        private enum MessageStage
        {
            idle = 0,
            begin,
            messageAppear,
            messageDisplay,
            messageRemove,
        };

        // current stage of the pop-up message display
        private MessageStage stage;
        
        //------------------------------------------------------------------------------
        // Constructor: ICMessagePopUp 
        // Author: Neil Holmes
        // Summary: Constructor - prepares the message pop up system
        //------------------------------------------------------------------------------
        public ICMessagePopUp(Game game, GraphicsDeviceManager graphicsDeviceManager, ICMessagePosition positionMode, ICMessageScale scale)
        {
            // create a content manager to handle loading the textures we need
            ContentManager content = new ContentManager(game.Services);

            // store the graphics device for future reference
            this.graphicsDevice = game.GraphicsDevice;

            // store the position mode that we are using
            this.positionMode = positionMode;

            // store the scale we are using
            if (scale == ICMessageScale.normal)
                displayScale = 1.0f;
            else
                displayScale = 0.5f;

            // create a sprite batch for rendering
            spriteBatch = new SpriteBatch(graphicsDevice);

            // load the achievement display textures
            badgeTexture = content.Load<Texture2D>(@"Content\ICMessages\Badge"); 
            bannerTexture = content.Load<Texture2D>(@"Content\ICMessages\Shadow");

            // store the size of the badge graphic
            badgeSize = badgeTexture.Height;

            // load the message fonts
            if (scale == ICMessageScale.normal)
            {
                titleFont = content.Load<SpriteFont>(@"Content\ICMessages\titleFont");
                messageFont = content.Load<SpriteFont>(@"Content\ICMessages\MessageFont");
            }
            else
            {
                titleFont = content.Load<SpriteFont>(@"Content\ICMessages\titleFontSmall");
                messageFont = content.Load<SpriteFont>(@"Content\ICMessages\MessageFontSmall");
            }

            // nothing to display yet!
            display = false;
            
            // set the message display stage to 'idle' so nothing is displayed
            this.stage = MessageStage.idle;
        }

        //------------------------------------------------------------------------------
        // Method: CheckForPendingMessages
        // Author: Neil Holmes
        // Summary: checks to see if there are messages queued and returns true if there
        //          are and false if not
        //------------------------------------------------------------------------------
        public bool CheckForPendingMessages()
        {
            if (messageQueue.Count > 0 || stage != MessageStage.idle)
            {
                // still messages in the queue - return true
                return true;
            }
            else
            {
                // no messages left to display - return false
                return false;
            }
        }
        
        //------------------------------------------------------------------------------
        // Method: Update
        // Author: Neil Holmes
        // Summary: updates the achievement display
        //------------------------------------------------------------------------------
        public void Update(Rectangle displayArea, float timeStep)
        {
            float displayAmount;

            // increase the counter if an achievement is actively being displayed
            if (stage > MessageStage.begin)
                counter += timeStep;
            else
            {
                // check the message list for a new message
                if (messageQueue.Count > 0)
                {
                    // store the text to be used
                    titleText = messageQueue[0].title;
                    messageText = messageQueue[0].message;

                    // remove the message from the list
                    messageQueue.Remove(messageQueue[0]);

                    // calcuate the number of lines in the messageText
                    int index = 0;
                    int numLines = 1;
                    while ((index = messageText.IndexOf('\n', index)) != -1)
                    {
                        index++;
                        numLines++;
                    }

                    // set the message height
                    messageHeight = (int)(((messageBorderSize * 2) * displayScale) + titleFont.LineSpacing + (messageFont.LineSpacing * numLines));

                    // calculate the width of the text
                    messageWidth = (int)Math.Max(messageFont.MeasureString(messageText).X, messageFont.MeasureString(titleText).X);
                    
                    // calculate the display width (and enforce minimum and maximum widths)
                    displayWidth = messageWidth + (int)((badgeBorderSize + (messageBorderSize * 2) + badgeSize) * displayScale);
                    if (displayWidth < minDisplayWidth * displayScale) displayWidth = (int)(minDisplayWidth * displayScale);
                    if (displayWidth > maxDisplayWidth * displayScale)
                    {
                        displayWidth = (int)(maxDisplayWidth * displayScale);
                        messageWidth = (int)(maxDisplayWidth * displayScale);
                    }

                    // get screen width and height
                    int screenWidth = displayArea.Width;
                    int screenHeight = displayArea.Height;
                    int halfScreenWidth = screenWidth / 2;

                    // calculate the size of the screen safe zone 
                    int screenBorderSizeX = (int)(screenWidth * safeZoneScaleValue);
                    int screenBorderSizeY = (int)(screenHeight * safeZoneScaleValue);

                    // calculate the top middle of the achievement based on the requested positioning
                    switch (positionMode)
                    {
                        case ICMessagePosition.topLeft:
                            displayPosition.X = screenBorderSizeX;
                            displayPosition.Y = screenBorderSizeY;
                            break;

                        case ICMessagePosition.topMiddle:
                            displayPosition.X = halfScreenWidth - (displayWidth / 2);
                            displayPosition.Y = screenBorderSizeY;
                            break;

                        case ICMessagePosition.topRight:
                            displayPosition.X = screenWidth - screenBorderSizeX - displayWidth;
                            displayPosition.Y = screenBorderSizeY;
                            break;

                        case ICMessagePosition.bottomLeft:
                            displayPosition.X = screenBorderSizeX;
                            displayPosition.Y = screenHeight - (screenBorderSizeY + messageHeight);
                            break;

                        case ICMessagePosition.bottomMiddle:
                            displayPosition.X = halfScreenWidth - (displayWidth / 2);
                            displayPosition.Y = screenHeight - (screenBorderSizeY + messageHeight);
                            break;

                        case ICMessagePosition.bottomRight:
                            displayPosition.X = screenWidth - screenBorderSizeX - displayWidth;
                            displayPosition.Y = screenHeight - (screenBorderSizeY + messageHeight);
                            break;
                    }

                    // set up the badge display rectangle
                    badgeRect.X = (int)(displayPosition.X + (badgeBorderSize * displayScale));
                    badgeRect.Y = (int)(displayPosition.Y + (badgeBorderSize * displayScale));
                    badgeRect.Width = (int)(badgeSize * displayScale);
                    badgeRect.Height = (int)(badgeSize * displayScale);
                    
                    // setup the title and message text positions
                    titlePosition.X = displayPosition.X + ((badgeBorderSize + messageBorderSize + badgeSize) * displayScale);
                    titlePosition.Y = displayPosition.Y + (messageBorderSize * displayScale);
                    messagePosition.X = displayPosition.X + ((badgeBorderSize + messageBorderSize + badgeSize) * displayScale);
                    messagePosition.Y = displayPosition.Y + (messageBorderSize * displayScale) + titleFont.LineSpacing;

                    // tell the message to start displaying
                    stage = MessageStage.begin;
                }
            }

            // process according to the current stage of achievement display
            switch (stage)
            {
                // do nothing
                case MessageStage.idle:
                    break;

                // initialise the achievement unlocked display ready to draw
                case MessageStage.begin:

                    // set the counter to zero and display flag to true
                    counter = 0;

                    // set the initial size of the message box
                    messageRect.X = (int)(displayPosition.X);
                    messageRect.Y = (int)(displayPosition.Y + messageHeight / 2);
                    messageRect.Width = displayWidth;
                    messageRect.Height = 0;

                    // tell the message system to draw stuff
                    display = true;

                    // move on to the next step in the process
                    stage++;
                    break;

                // makes the message appear on screen
                case MessageStage.messageAppear:

                    // clamp counter to the appearance duration
                    if (counter > messageAppearDuration) counter = messageAppearDuration;

                    // calculate how much of the message should be displayed
                    displayAmount = counter / messageAppearDuration;
                    messageRect.Y = (int)((displayPosition.Y + messageHeight / 2) - ((messageHeight / 2) * displayAmount));
                    messageRect.Height = (int)(messageHeight * displayAmount);

                    // check if we have reached the end of this stage
                    if (counter == messageAppearDuration)
                    {
                        // we have reached the end of the message appearance stage, reset counter and move on to the next stage
                        counter = 0;
                        stage++;
                    }
                    break;

                // pause for a short time before removing the message
                case MessageStage.messageDisplay:

                    // check if we have reached the end of this stage
                    if (counter >= messageDisplayDuration)
                    {
                        // we have reached the end of this pause - reset counter and move on to the next stage
                        counter = 0.0f;
                        stage++;
                    }
                    break;

                // remove the message
                case MessageStage.messageRemove:

                    // clamp counter to the message removal duration
                    if (counter > messageRemoveDuration) counter = messageRemoveDuration;

                    // calculate how much of the message should be displayed
                    displayAmount = 1 - (counter / messageAppearDuration);
                    messageRect.Y = (int)((displayPosition.Y + messageHeight / 2) - ((messageHeight / 2) * displayAmount));
                    messageRect.Height = (int)(messageHeight * displayAmount);

                    // check if we have reached the end of this stage
                    if (counter == messageRemoveDuration)
                    {
                        // display finished - ensure everything is reset to the defaults
                        counter = 0;
                        display = false;
                        stage = MessageStage.idle;
                    }
                    break;
            }
        }

        //------------------------------------------------------------------------------
        // Method: SetPositionMode
        // Author: Neil Holmes
        // Summary: allows user to set a different position mode after the initial setup
        //------------------------------------------------------------------------------
        public void SetPositionMode(ICMessagePosition positionMode)
        {
            this.positionMode = positionMode;
        }

        //------------------------------------------------------------------------------
        // Method: Draw
        // Author: Neil Holmes
        // Summary: draws a message
        //------------------------------------------------------------------------------
        public void Draw(Rectangle displayArea, ICMessageColorMode colorMode)
        {
            Color bannerColor;
            Color titleTextColor;
            Color messageTextColor;

            // bail out if we have nothing to display
            if (display == false) return;

            // store the current scissor rectangle settings so we can restore them after we finish drawing
            Rectangle storedScissorRect = graphicsDevice.ScissorRectangle;

            //---

            // setup colours based on requested colour mode
            if (colorMode == ICMessageColorMode.normal)
            {
                bannerColor = normalBannerColor;
                titleTextColor = normalTitleColor;
                messageTextColor = normalMessageColor;
            }
            else
            {
                bannerColor = invertedBannerColor;
                titleTextColor = invertedTitleColor;
                messageTextColor = invertedMessageColor;
            }

            //----

            // start drawing the banner
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied);

            // draw the banner
            spriteBatch.Draw(bannerTexture, messageRect, bannerColor);

            // finished drawing
            spriteBatch.End();

            //----

            // calculate the position and width of the scissor rectangle we will use & clip it to the current scissor region to 
            // ensure that we don't try to draw outside of the screen area etc
            Rectangle scissorRect = messageRect;
            if (scissorRect.X < 0)
            {
                scissorRect.Width += scissorRect.X;
                scissorRect.X = 0;
            }
            if (scissorRect.X > graphicsDevice.ScissorRectangle.Width)
            {
                scissorRect.X = graphicsDevice.ScissorRectangle.Width;
                scissorRect.Width = 0;
            }
            if (scissorRect.X + scissorRect.Width > graphicsDevice.ScissorRectangle.Width)
            {
                scissorRect.Width = graphicsDevice.ScissorRectangle.Width - scissorRect.X;
            }
            if (scissorRect.Y + scissorRect.Height > graphicsDevice.ScissorRectangle.Height)
            {
                scissorRect.Height = graphicsDevice.ScissorRectangle.Height - scissorRect.Y;
            }

            // set the scissor region to the banner area
            graphicsDevice.ScissorRectangle = scissorRect;

            // start dawring the text with the new scissor rectangle set
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.LinearClamp, DepthStencilState.None, new RasterizerState { ScissorTestEnable = true });

            // draw the title text
            spriteBatch.DrawString(titleFont, titleText, titlePosition, titleTextColor);

            // draw the value
            spriteBatch.DrawString(messageFont, messageText, messagePosition, messageTextColor);

            // draw the badge
            spriteBatch.Draw(badgeTexture, badgeRect, null, Color.White);

            // finished drawing the text
            spriteBatch.End();

            // restore the previous scissor rectangle settings
            graphicsDevice.ScissorRectangle = storedScissorRect;
        }

        //------------------------------------------------------------------------------
        // Method: AddMessage
        // Author: Neil Holmes
        // Summary: add a new message to the message queue for display
        //------------------------------------------------------------------------------
        public void AddMessage(string title, string message, ICMessagePriority priority)
        {
            int insertionIndex = 0;
            ICMessageData newMessage = new ICMessageData(title, message, priority);

            // is this an urgent message?
            if (newMessage.priority == ICMessagePriority.urgent)
            {
                // tell the message system to stop displaying any message it might be in the middle of
                stage = MessageStage.idle;

                // urgent messages always go at the top of the queue, in time order
                foreach (ICMessageData item in messageQueue)
                {
                    if (item.priority != ICMessagePriority.urgent)
                        break;
                    else
                        insertionIndex ++;
                }
            }
            else            
            {
                // find the first lower-priorty message in the queue and push this one ahead of it
                foreach (ICMessageData item in messageQueue)
                {
                    if (item.priority < newMessage.priority) 
                        break;
                    else
                        insertionIndex ++;
                }
            }

            // insert here!
            messageQueue.Insert(insertionIndex, newMessage);
        }
    }
}
