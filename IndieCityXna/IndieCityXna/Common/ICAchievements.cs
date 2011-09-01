//------------------------------------------------------------------------------
// Filename: ICAchievements.cs
// Author: Neil Holmes
// Summary: indie cigty achievement display exmaple
//------------------------------------------------------------------------------

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using System.Collections.Generic;
using ICELandaLib;

namespace ICAchievements
{
    // the available rendering positions of the achievements
    public enum ICAchievementPosition
    {
        topLeft = 0,
        topMiddle,
        topRight,
        bottomLeft,
        bottomMiddle,
        bottomRight,
        listMode
    };

    // the different stages of the achievement display
    public enum ICAchievementColorMode
    {
        normal = 0,
        inverted,
    };

    // the different sizes of the achievement display
    public enum ICAchievementScale
    {
        normal = 0,
        small,
    };

    // class to hold information about a specific achievement
    public class achievement
    {
        public string name;
        public string description;
        public int value;
        public Texture2D icon;
        public bool unlocked;

        //------------------------------------------------------------------------------
        // Constructor: achievement 
        // Author: Neil Holmes
        // Summary: Constructor - prepares the achievement data
        //------------------------------------------------------------------------------
        public achievement(achievement rhs)
        {
            this.name = string.Copy(rhs.name);
            this.description = string.Copy(rhs.description);
            this.value = rhs.value;
            this.icon = rhs.icon;
            this.unlocked = rhs.unlocked;
        }

        //------------------------------------------------------------------------------
        // Constructor: achievement 
        // Author: Neil Holmes
        // Summary: Constructor - prepares a blank achievement
        //------------------------------------------------------------------------------
        public achievement()
        {
            name = null;
            description = null;
            value = 0;
            icon = null;
            unlocked = false;
        }
    };

    //------------------------------------------------------------------------------
    // Class: ICAchievementPopUp
    // Author: Neil Holmes
    // Summary: class for displaying a pop-up message when an achievement is unlocked
    //------------------------------------------------------------------------------
    public class ICAchievementPopUp : ICAchievementSystem
    {
        // the different stages of the pop-up achievement display
        private enum AchievementStage
        {
            idle = 0,
            begin,
            badgeAppear,
            badgeAppearPause,
            bannerReveal,
            bannerRevealPause,
            bannerRemove,
            badgeRemove
        };

        // current stage of the pop-up achievemnt display
        private AchievementStage stage;

        //------------------------------------------------------------------------------
        // Constructor: AchievementPopUp 
        // Author: Neil Holmes
        // Summary: Constructor - prepares the achievement pop up system
        //------------------------------------------------------------------------------
        public ICAchievementPopUp(Game game, GraphicsDeviceManager graphicsDeviceManager, ICAchievementPosition positionMode, ICAchievementScale scale) 
            : base(game, graphicsDeviceManager, null, positionMode, scale)
        {
            // set the achievement display stage to 'idle' so nothing is displayed
            this.stage = AchievementStage.idle;
        
            // achievement pop-ups always have full brightness icons
            iconColor = Color.White;
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
            if (stage > AchievementStage.begin)
                counter += timeStep;
            else
            {
                // check the achievement list and look for any that need to be displayed
                if (unlockedAchievements.Count > 0)
                {
                    // get the next achievement from the unlocked list
                    achievement achievement = unlockedAchievements.Dequeue();
                   
                    // store the achievement text
                    this.achievementText = achievement.name;

                    // set the icon texture to use
                    iconTexture = achievement.icon;

                    // build the value text
                    valueText = "+" + achievement.value.ToString() + " Points";
                    
                    // calculate the width of the text
                    messageWidth = (int)messageFont.MeasureString(achievementText).X;

                    // calculate the width of the value text
                    valueWidth = (int)valueFont.MeasureString(valueText).X;

                    // calculate the display width (and enforce minimum and maximum widths)
                    displayWidth = messageWidth + (int)(messageWidthPad * displayScale);
                    if (displayWidth < (minDisplayWidth * displayScale)) displayWidth = (int)(minDisplayWidth * displayScale);
                    if (displayWidth > (maxDisplayWidth * displayScale))
                    {
                        displayWidth = (int)(maxDisplayWidth * displayScale);
                        messageWidth = (int)((maxDisplayWidth - messageWidthPad) * displayScale);
                    }
                    halfDisplayWidth = displayWidth / 2;

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
                        case ICAchievementPosition.topLeft:
                            displayPosition.X = screenBorderSizeX + halfDisplayWidth;
                            displayPosition.Y = screenBorderSizeY - (int)(bannerYOffset * displayScale);
                            break;

                        case ICAchievementPosition.topMiddle:
                            displayPosition.X = halfScreenWidth;
                            displayPosition.Y = screenBorderSizeY - (int)(bannerYOffset * displayScale);
                            break;

                        case ICAchievementPosition.topRight:
                            displayPosition.X = screenWidth - (screenBorderSizeX + halfDisplayWidth);
                            displayPosition.Y = screenBorderSizeY - (int)(bannerYOffset * displayScale);
                            break;

                        case ICAchievementPosition.bottomLeft:
                            displayPosition.X = screenBorderSizeX + halfDisplayWidth;
                            displayPosition.Y = screenHeight - (screenBorderSizeY + bannerHeight + (int)(bannerYOffset * displayScale));
                            break;

                        case ICAchievementPosition.bottomMiddle:
                            displayPosition.X = halfScreenWidth;
                            displayPosition.Y = screenHeight - (screenBorderSizeY + bannerHeight + (int)(bannerYOffset * displayScale));
                            break;

                        case ICAchievementPosition.bottomRight:
                            displayPosition.X = screenWidth - (screenBorderSizeX + halfDisplayWidth);
                            displayPosition.Y = screenHeight - (screenBorderSizeY + bannerHeight + (int)(bannerYOffset * displayScale));
                            break;
                    }

                    // tell the achievement to start displaying
                    stage = AchievementStage.begin;
                }
            }

            // process according to the current stage of achievement display
            switch (stage)
            {
                // do nothing
                case AchievementStage.idle:
                    break;

                // initialise the achievement unlocked display ready to draw
                case AchievementStage.begin:

                    // set the counter to zero and display flag to true
                    counter = 0;
                    display = true;

                    // set the initial badge position
                    badgeRect.X = (int)displayPosition.X - halfBadgeSize + halfDisplayWidth;
                    badgeRect.Y = (int)displayPosition.Y;

                    // set banner size to the right height, but zero width
                    bannerRect.Y = badgeRect.Y + (int)(bannerYOffset * displayScale);
                    bannerRect.Width = 0;
                    bannerRect.Height = (int)(bannerHeight * displayScale);

                    // set the text position
                    textPosition.X = displayPosition.X - (messageWidth / 2) - (int)(textOffsetX * displayScale);
                    textPosition.Y = bannerRect.Y + (int)(textOffsetY * displayScale);

                    // set the value position
                    if (valueWidth > messageWidth)
                        valuePosition.X = displayPosition.X - (valueWidth / 2) - (int)(textOffsetX * displayScale);
                    else
                        valuePosition.X = textPosition.X;
                    valuePosition.Y = bannerRect.Y + (int)(valueOffsetY * displayScale);
                    
                    // set the icon position
                    iconRect.Y = bannerRect.Y + (int)(iconBorderSize * displayScale);
                    iconRect.Height = bannerRect.Height - (int)(iconBorderSize * 2 * displayScale);
                    iconRect.Width = iconRect.Height;
                    iconRect.X = (int)displayPosition.X + (displayWidth / 2) - (int)(iconBorderSize * displayScale) - iconRect.Width;

                    // set the alpha to it's initial value
                    alpha = 0;

                    // move on to the next step in the process
                    stage ++;
                    break;

                // makes the indie city badge appear on screen with zoom & rotation
                case AchievementStage.badgeAppear:

                    // clamp counter to the appearance duration
                    if (counter > badgeAppearDuration) counter = badgeAppearDuration;

                    // calculate the fade-in for the badge
                    alpha = (int)(255 * (counter / badgeAppearDuration));
 
                    // check if we have reached the end of this stage
                    if (counter == badgeAppearDuration)
                    {
                        // we have reached the end of the badge appearance stage, reset counter and move on to the next stage
                        counter = 0;
                        stage ++;
                    }
                    break;

                // pause for a short time before revealing the banner
                case AchievementStage.badgeAppearPause:

                    // check if we have reached the end of this stage
                    if (counter >= badgeAppearPauseDuration)
                    {
                        // we have reached the end of this pause - reset counter and move on to the next stage
                        counter = 0.0f;
                        stage ++;
                    }
                    break;

                // reveal the banner along with the achievement text
                case AchievementStage.bannerReveal:

                    // clamp counter to the banner reveal duration
                    if (counter > bannerRevealDuration) counter = bannerRevealDuration;

                    // calculate how much of the banner should be displayed
                    displayAmount = counter / bannerRevealDuration;

                    // set the banner width and where the badge should be positioned
                    bannerRect.Width = (int)(displayWidth * displayAmount);
                    bannerRect.X = (int)displayPosition.X + halfDisplayWidth - bannerRect.Width;
                    badgeRect.X = bannerRect.X - halfBadgeSize;

                    // check if we have reached the end of this stage
                    if (counter == bannerRevealDuration)
                    {
                        // we have reached the end of the banner reveal stage- reset counter and move on to the next stage
                        counter = 0.0f;
                        stage ++;
                    }
                    break;

                // pause for a short time before removing the banner
                case AchievementStage.bannerRevealPause:

                    // check if we have reached the end of this stage
                    if (counter >= bannerRevealPauseDuration)
                    {
                        // we have reached the end of this pause - reset counter and move on to the next stage
                        counter = 0.0f;
                        stage++;
                    }
                    break;

                // remove the banner
                case AchievementStage.bannerRemove:

                    // clamp counter to the banner removal duration
                    if (counter > bannerRemoveDuration) counter = bannerRemoveDuration;

                    // calculate how much of the banner should be displayed
                    displayAmount = 1 - (counter / bannerRemoveDuration);

                    // calculate how much of the banner should be displayed and where the badge should be positioned
                    bannerRect.Width = (int)(displayWidth * displayAmount);
                    bannerRect.X = (int)displayPosition.X + halfDisplayWidth - bannerRect.Width;
                    badgeRect.X = bannerRect.X - halfBadgeSize;

                    // check if we have reached the end of this stage
                    if (counter == bannerRemoveDuration)
                    {
                        // we have reached the end of the banner removal stage- reset counter and move on to the next stage
                        counter = 0.0f;
                        stage ++;
                    }
                    break;

                // fade off the indiecity badge
                case AchievementStage.badgeRemove:

                    if (counter < badgeRemoveDuration)
                    {
                        // calculate the fade-out for the badge
                        alpha = (int)(255 * (1 - (counter / badgeRemoveDuration)));
                    }
                    else
                    {
                        // display finished - ensure everything is reset to the defaults
                        alpha = 0;
                        counter = 0;
                        display = false;
                        stage = AchievementStage.idle;
                    }
                    break;
            }
        }

        //------------------------------------------------------------------------------
        // Method: Draw
        // Author: Neil Holmes
        // Summary: draws the achievement unlocked display
        //------------------------------------------------------------------------------
        public void Draw(Rectangle displayArea, ICAchievementColorMode colorMode)
        {
            DrawAchievement(false, displayArea, colorMode);
        }
    }

    //------------------------------------------------------------------------------
    // Class: ICAchievementList
    // Author: Neil Holmes
    // Summary: class for displaying a browsable list of achievements in the game
    //------------------------------------------------------------------------------
    public class ICAchievementList : ICAchievementSystem
    {
        // index of the first achievement to be displayed in the list
        private uint achievementDisplayIndex;
        
        // used to track how many achievements were rendered when the list was drawn
        private uint achievementsDisplayed;
        
        //------------------------------------------------------------------------------
        // Constructor: ICAchievementList 
        // Author: Neil Holmes
        // Summary: Constructor - prepares the achievement list system
        //------------------------------------------------------------------------------
        public ICAchievementList(Game game, GraphicsDeviceManager graphicsDeviceManager, ICELandaLib.CoAchievementGroup achievementGroup, ICAchievementScale scale)
            : base(game, graphicsDeviceManager, achievementGroup, ICAchievementPosition.listMode, scale)
        {
            // by default we want to display the list from the first achievement onwards
            achievementDisplayIndex = 0;

            // the number of achievements that were previously displayed
            achievementsDisplayed = 0;
        }

        //------------------------------------------------------------------------------
        // Method: Update
        // Author: Neil Holmes
        // Summary: updates the achievement list display so it knows which achievements
        //          have been unlocked by the user
        //------------------------------------------------------------------------------
        public void Update(CoUserAchievementList userList)
        {
            // store a copy of the updated user list
            userAchievementList = userList;
        }

        //------------------------------------------------------------------------------
        // Method: Draw
        // Author: Neil Holmes
        // Summary: draws the achievement list display
        //------------------------------------------------------------------------------
        public void Draw(Rectangle displayArea, int displayWidth, ICAchievementColorMode colorMode, bool useSafeZone = false)
        {
            // set the first achievement to be displayed
            uint achievementIndex = achievementDisplayIndex;
            
            // ensure that the supplied display area is inside the screen
            if (displayArea.Left < 0)
            {
                displayArea.Width += displayArea.Left;
                displayArea.X = 0;
            }
            if (displayArea.Right > graphicsDevice.Viewport.Width)
            {
                displayArea.Width -= (displayArea.Right - graphicsDevice.Viewport.Width);
            }
            if (displayArea.Top < 0)
            {
                displayArea.Height += displayArea.Top;
                displayArea.Y = 0;
            }
            if (displayArea.Bottom > graphicsDevice.Viewport.Height)
            {
                displayArea.Height -= (displayArea.Bottom - graphicsDevice.Viewport.Height);
            }

            // clamp display width to min and max
            if (displayWidth < (minDisplayWidth * displayScale)) displayWidth = (int)(minDisplayWidth * displayScale);
            if (displayWidth > (maxDisplayWidth * displayScale)) displayWidth = (int)(maxDisplayWidth * displayScale);

            // preset size and intial display position of the first achievement in the list
            bannerRect.X = displayArea.X + (displayArea.Width - displayWidth) / 2;
            if (useSafeZone)
                bannerRect.Y = displayArea.Y + (int)(displayArea.Height * safeZoneScaleValue);
            else
                bannerRect.Y = displayArea.Y;
            bannerRect.Height = (int)(bannerHeight * displayScale);
            bannerRect.Width = displayWidth;

            // set the text position
            textPosition.X = bannerRect.X + (textOffsetX * displayScale);
            textPosition.Y = bannerRect.Y + (textOffsetY * displayScale);

            // set the value position
            valuePosition.X = textPosition.X;
            valuePosition.Y = bannerRect.Y + (valueOffsetY * displayScale);

            // set the icon position
            iconRect.Y = bannerRect.Y + (int)(iconBorderSize * displayScale);
            iconRect.Height = bannerRect.Height - (int)((iconBorderSize * 2) * displayScale);
            iconRect.Width = iconRect.Height;
            iconRect.X = bannerRect.X + bannerRect.Width - (int)(iconBorderSize * displayScale) - iconRect.Width;

            // loop through and display as many achievemnts as possible on the screen...   
            for (; achievementIndex < achievementGroup.AchievementCount; achievementIndex++)
            {
                // bail out if the bottom of the banner rectangle is outside the safe zone
                if (useSafeZone)
                {
                    if (bannerRect.Y + bannerRect.Height > (displayArea.Y + displayArea.Height - (int)(displayArea.Height * safeZoneScaleValue))) break;
                }
                else
                {
                    if (bannerRect.Y + bannerRect.Height > displayArea.Y + displayArea.Height) break;
                }
                
                // setup the information for this achievement
                CoAchievement achievementData = achievementGroup.GetAchievementFromIndex(achievementIndex);
                int width;
                int heightPixels;
                achievementData.Image.GetDimensions(out width, out heightPixels);
                Byte[] pixelData = (Byte[])achievementData.Image.PixelData;
                iconTexture = new Texture2D(graphicsDevice, width, heightPixels, false, SurfaceFormat.Color);
                iconTexture.SetData<Byte>(pixelData);
                achievementText = (int)achievementData.TrueValue + " - " + String.Copy(achievementData.Title);
                valueText = String.Copy(achievementData.Description);


                // set the color of the achievement icon
                if (userAchievementList != null && userAchievementList.IsAchievementUnlocked(achievementData.AchievementId))
                {
                    iconColor = Color.White;
                }
                else
                {
                    iconColor = Color.Gray;
                }

                // draw this achievement
                DrawAchievement(true, displayArea, colorMode);

                // move on to the next achievement in the list
                bannerRect.Y += (int)(listSpacingY * displayScale);
                iconRect.Y += (int)(listSpacingY * displayScale);
                valuePosition.Y += (int)(listSpacingY * displayScale);
                textPosition.Y += (int)(listSpacingY * displayScale);
            }

            // store the number of achievements that were displayed
            if (achievementIndex - achievementDisplayIndex > achievementsDisplayed)
                achievementsDisplayed = achievementIndex - achievementDisplayIndex;
        }

        public void PageUp()
        {
            if (achievementDisplayIndex < achievementsDisplayed) 
                achievementDisplayIndex = 0;
            else
                achievementDisplayIndex -= achievementsDisplayed;
        }

        public void PageDown()
        {
            if (achievementDisplayIndex + achievementsDisplayed < achievementGroup.AchievementCount)
                achievementDisplayIndex += achievementsDisplayed;
        }
    }

    //------------------------------------------------------------------------------
    // Class: ICAchievementSystem
    // Author: Neil Holmes
    // Summary: base class containing common functionality for displaying achievement
    //          pop-ups and lists
    //------------------------------------------------------------------------------
    public class ICAchievementSystem : IAchievementEventHandler
    {
        // the graphics device
        protected GraphicsDevice graphicsDevice;

        // this position mode that the game has requested
        protected ICAchievementPosition positionMode;

        // the sprite batch to use for drawing
        protected SpriteBatch spriteBatch;

        // position of the achievement
        protected Vector2 displayPosition;

        // position and size of the drawn elements of the achievement
        protected Rectangle badgeRect;
        protected Rectangle bannerRect;
        protected Rectangle iconRect;
        protected Vector2 textPosition;
        protected Vector2 valuePosition;
        protected string achievementText;
        protected string valueText;
        protected int halfBadgeSize;
        protected int messageWidth;
        protected int valueWidth;
        protected int displayWidth;
        protected int halfDisplayWidth;
        protected Color iconColor;
        protected float displayScale;

        // update-related values
        protected float counter;
        protected bool display;
        protected int alpha;

        // the achievement graphics
        protected Texture2D badgeTexture;
        protected Texture2D bannerTexture;
        protected Texture2D padlockTexture;
        protected Texture2D iconTexture;

        // the achievement font
        protected SpriteFont messageFont;
        protected SpriteFont valueFont;
        
        // constant values for the animation
        protected static int bannerYOffset = 36;
        protected static int bannerHeight = 60;
        protected static int textOffsetX = 12;
        protected static int textOffsetY = 6;
        protected static int valueOffsetY = 30;
        protected static int iconBorderSize = 4;
        protected static int messageWidthPad = 128;
        protected static int minDisplayWidth = 300;
        protected static int maxDisplayWidth = 800;
        protected static int listSpacingY = 76;
        protected static float safeZoneScaleValue = 0.07f;
        protected static float badgeAppearDuration = 0.2f;
        protected static float badgeAppearPauseDuration = 0.1f;
        protected static float bannerRevealDuration = 0.3f;
        protected static float bannerRevealPauseDuration = 3.0f;
        protected static float bannerRemoveDuration = 0.3f;
        protected static float badgeRemoveDuration = 0.07f;

        // constant colours for normal banner display
        private static Color normalBannerColor = new Color(255, 255, 255, 255);
        private static Color normalMessageTextColor = new Color(0, 0, 0, 200);
        private static Color normalValueTextColor = new Color(0, 90, 0, 150);

        // constant colours for inverted banner display
        private static Color invertedBannerColor = new Color(25, 25, 30, 255);
        private static Color invertedMessageTextColor = new Color(225, 225, 225, 200);
        private static Color invertedValueTextColor = new Color(150, 150, 150, 200);

        // queue of achievments that have been unlocked and are ready for display
        protected Queue<achievement> unlockedAchievements = new Queue<achievement>();
        
        // list of achievements
        protected ICELandaLib.CoAchievementGroup achievementGroup = null;

        // list of user's unlocked achievements
        protected CoUserAchievementList userAchievementList;

        //------------------------------------------------------------------------------
        // Constructor: ICAchievementSystem
        // Author: Neil Holmes
        // Summary: main constructor for the achievement display system
        //------------------------------------------------------------------------------
        public ICAchievementSystem(Game game, GraphicsDeviceManager graphicsDeviceManager, ICELandaLib.CoAchievementGroup achievementGroup, ICAchievementPosition positionMode, ICAchievementScale scale)
        {
            // create a content manager to handle loading the textures we need
            ContentManager content = new ContentManager(game.Services);
            
            // store the graphics device for future reference
            this.graphicsDevice = game.GraphicsDevice;
 
            // store the achievement group
            this.achievementGroup = achievementGroup;

            // store the position mode that we are using
            this.positionMode = positionMode;

            // store the scale we are using
            if (scale == ICAchievementScale.normal)
                displayScale = 1.0f;
            else
                displayScale = 0.5f;

            // create a sprite batch for rendering
            spriteBatch = new SpriteBatch(graphicsDevice);
            
            // defulat display to false
            display = false;
 
            // load the achievement display textures
            badgeTexture = content.Load<Texture2D>(@"Content\ICAchievements\Badge");
            bannerTexture = content.Load<Texture2D>(@"Content\ICAchievements\Shadow");
            padlockTexture = content.Load<Texture2D>(@"Content\ICAchievements\Padlock");

            // load the achievement font
            if (scale == ICAchievementScale.normal)
            {
                messageFont = content.Load<SpriteFont>(@"Content\ICAchievements\MessageFont");
                valueFont = content.Load<SpriteFont>(@"Content\ICAchievements\ValueFont");
            }
            else
            {
                messageFont = content.Load<SpriteFont>(@"Content\ICAchievements\MessageFontSmall");
                valueFont = content.Load<SpriteFont>(@"Content\ICAchievements\ValueFontSmall");
            }

            // calculate some constants for the display system
            badgeRect.Width = badgeRect.Height = (int)(badgeTexture.Width * displayScale);
            halfBadgeSize = badgeRect.Width / 2;
        }

        //------------------------------------------------------------------------------
        // Method: SetPositionMode
        // Author: Neil Holmes
        // Summary: allows user to set a different position mode after the initial setup
        //------------------------------------------------------------------------------
        public void SetPositionMode(ICAchievementPosition positionMode)
        {
            this.positionMode = positionMode;
        }

        //------------------------------------------------------------------------------
        // Method: DrawAchievement
        // Author: Neil Holmes
        // Summary: draws an achievement
        //------------------------------------------------------------------------------
        public void DrawAchievement(bool listMode, Rectangle displayArea, ICAchievementColorMode colorMode)
        {
            Color bannerColor;
            Color messageTextColor;
            Color valueTextColor; 
            
            // bail out if we have nothing to display
            if (display == false && listMode == false) return;

            // store the current scissor rectangle settings so we can restore them after we finish drawing
            Rectangle storedScissorRect = graphicsDevice.ScissorRectangle;
            
            //---

            // setup colours based on requested colour mode
            if (colorMode == ICAchievementColorMode.normal)
            {
                bannerColor = normalBannerColor;
                messageTextColor = normalMessageTextColor;
                valueTextColor = normalValueTextColor;
            }
            else
            {
                bannerColor = invertedBannerColor;
                messageTextColor = invertedMessageTextColor;
                valueTextColor = invertedValueTextColor;
            }

            //----

            // start drawing the banner
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied);

            // draw the banner
            spriteBatch.Draw(bannerTexture, bannerRect, bannerColor);

            // finished drawing
            spriteBatch.End();

            //----

            // calculate the position and width of the scissor rectangle we will use & clip it to the current scissor region to 
            // ensure that we don't try to draw outside of the screen area etc
            Rectangle scissorRect = bannerRect;
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

            // start dawring
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.LinearClamp, DepthStencilState.None, new RasterizerState { ScissorTestEnable = true });

            // draw the achievement icon
            spriteBatch.Draw(iconTexture, iconRect, null, iconColor);

            // if we are in list mode check the colour of the icon and draw a padlock if required
            if (listMode && iconColor == Color.Gray) spriteBatch.Draw(padlockTexture, iconRect, null, Color.White);

            // finished drawing the icon
            spriteBatch.End();

            // adjust the scissor region to the text area
            scissorRect.Width -= (iconRect.Width + (iconBorderSize * 2));
            if (scissorRect.Width < 0) scissorRect.Width = 0;
            graphicsDevice.ScissorRectangle = scissorRect;

            // start dawring the text with the new scissor rectangle set
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.LinearClamp, DepthStencilState.None, new RasterizerState { ScissorTestEnable = true });

            // draw the text
            if (listMode == false)
            {
                // draw the achievement name
                spriteBatch.DrawString(messageFont, achievementText, textPosition, messageTextColor);

                // draw the value
                spriteBatch.DrawString(valueFont, valueText, valuePosition, valueTextColor);
            }
            else
            {
                // draw the achievement name
                spriteBatch.DrawString(valueFont, achievementText, textPosition, messageTextColor);

                // draw the description
                spriteBatch.DrawString(messageFont, valueText, valuePosition, valueTextColor);
            }

            // finished drawing the text
            spriteBatch.End();

            // restore the previous scissor rectangle settings
            graphicsDevice.ScissorRectangle = storedScissorRect;
 
            //----

            if (listMode == false)
            {
                // start dawring the badge
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied);

                // draw the badge
                spriteBatch.Draw(badgeTexture, badgeRect, null, new Color(255, 255, 255, alpha));

                // finished drawing the badge
                spriteBatch.End();
            }
        }
        
        //------------------------------------------------------------------------------
        // Method: Activate
        // Author: Neil Holmes
        // Summary: test function to make the achievement display
        //------------------------------------------------------------------------------
        public void OnAchievementUnlocked(int userId, ICELandaLib.CoAchievement achievement)
        {
            int width;
            int heightPixels;
            achievement.Image.GetDimensions(out width, out heightPixels);

            Byte[] pixelData = (Byte[])achievement.Image.PixelData;
            Texture2D icon = new Texture2D(graphicsDevice, width, heightPixels, false, SurfaceFormat.Color);
            icon.SetData<Byte>(pixelData);
            
            achievement unlockedAchievement = new achievement();

            unlockedAchievement.icon = icon; // testIcon; // testAchievements[0].icon;

            unlockedAchievement.name = String.Copy(achievement.Title);
            unlockedAchievement.description = String.Copy(achievement.Description);
            unlockedAchievement.value = (int)achievement.TrueValue;

            unlockedAchievement.unlocked = false;

            unlockedAchievements.Enqueue(unlockedAchievement);
        }

       
        public void OnAchievementUpdated(long achievementId, uint trueValue, uint awardCount)
        {
        }

        public void OnUserAchievementsFetched(int userId, System.Array achievements)
        {
        }

        public void OnAchievementGroupInitialised(CoAchievementGroup group, Boolean modsDetected)
        {
        }

        public void OnAllAchievementsUpdated()
        {

        }
    }
}
