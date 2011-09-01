//------------------------------------------------------------------------------
// Filename: ICLeaderboards.cs
// Author: Neil Holmes
// Summary: IndieCity leaderboard display exmaple
//------------------------------------------------------------------------------

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace ICLeaderboards
{
    // the different colour modes of the leaderboard display
    public enum ICLeaderboardColorMode
    {
        normal = 0,
        inverted,
    };

    // the different sizes of the leaderboard display
    public enum ICLeaderboardScale
    {
        normal = 0,
        small,
    };

    public class ICLeaderboardsBrowser
    {
        private float displayScale;

        // the graphics device
        private GraphicsDevice graphicsDevice;

        // the sprite batch to use for drawing
        private SpriteBatch spriteBatch;

        // index of the first leaderboard entry to be displayed in the list
        private uint entryDisplayIndex;

        // used to track how many leaderboard entries were rendered when the leaderboard was drawn
        private uint entriesDisplayed;

        // the leaderboard graphics
        private Texture2D bannerTexture;

        // the leaderboard fonts
        private SpriteFont nameFont;
        private SpriteFont valueFont;

        // variables used for drawing
        private Rectangle bannerRect;
        private Vector2 namePosition;
        private Vector2 valuePosition;
        private string nameText;
        private string valueText;

        // constant values for drawing the leaderboard
        private static float safeZoneScaleValue = 0.07f;
        private static int minDisplayWidth = 300;
        private static int maxDisplayWidth = 800;
        private static int bannerHeight = 32;
        private static int listSpacingY = 44;
        private static int textOffsetX = 12;
        private static int textOffsetY = 4;

        // constant colours for normal display
        private static Color normalBannerColor = new Color(255, 255, 255, 255);
        private static Color normalNameTextColor = new Color(0, 0, 0, 200);
        private static Color normalValueTextColor = new Color(0, 0, 0, 200);

        // constant colours for inverted display
        private static Color invertedBannerColor = new Color(25, 25, 30, 255);
        private static Color invertedNameTextColor = new Color(225, 225, 225, 200);
        private static Color invertedValueTextColor = new Color(255, 255, 255, 200);


        public ICLeaderboardsBrowser(Game game, GraphicsDeviceManager graphicsDeviceManager, ICLeaderboardScale scale)
        {
            // create a content manager to handle loading the textures we need
            ContentManager content = new ContentManager(game.Services);

            // store the graphics device for future reference
            this.graphicsDevice = game.GraphicsDevice;

            // store the scale we are using
            if (scale == ICLeaderboardScale.normal)
                displayScale = 1.0f;
            else
                displayScale = 0.5f;

            // create a sprite batch for rendering
            spriteBatch = new SpriteBatch(graphicsDevice);

            // load the leaderboard display textures
            bannerTexture = content.Load<Texture2D>(@"Content\ICLeaderboards\Shadow");

            // load the leaderboard fonts
            if (scale == ICLeaderboardScale.normal)
            {
                nameFont = content.Load<SpriteFont>(@"Content\ICLeaderboards\NameFont");
                valueFont = content.Load<SpriteFont>(@"Content\ICLeaderboards\ValueFont");
            }
            else
            {
                nameFont = content.Load<SpriteFont>(@"Content\ICLeaderboards\NameFontSmall");
                valueFont = content.Load<SpriteFont>(@"Content\ICLeaderboards\ValueFontSmall");
            }

            // by default we want to display the leaderboard from the first entry onwards
            entryDisplayIndex = 0;

            // the number of leaderboard entries that were previously displayed
            entriesDisplayed = 0;
        }
                
        public void Draw(Rectangle displayArea, int displayWidth, ICLeaderboardColorMode colorMode, bool useSafeZone = false)
        {
            Color bannerColor;
            Color nameTextColor;
            Color valueTextColor;

            // set the first achievement to be displayed
            uint entryIndex = entryDisplayIndex;

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

            // preset size and intial display position of the first leaderboard entry to be drawn
            bannerRect.X = displayArea.X + (displayArea.Width - displayWidth) / 2;
            if (useSafeZone)
                bannerRect.Y = displayArea.Y + (int)(displayArea.Height * safeZoneScaleValue);
            else
                bannerRect.Y = displayArea.Y;
            bannerRect.Height = (int)(bannerHeight * displayScale);
            bannerRect.Width = displayWidth;

            // setup colours based on requested colour mode
            if (colorMode == ICLeaderboardColorMode.normal)
            {
                bannerColor = normalBannerColor;
                nameTextColor = normalNameTextColor;
                valueTextColor = normalValueTextColor;
            }
            else
            {
                bannerColor = invertedBannerColor;
                nameTextColor = invertedNameTextColor;
                valueTextColor = invertedValueTextColor;
            }

            // loop through and display as many leaderboard entries as possible on the screen...   
            for (; entryIndex < 100; entryIndex++)
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

                // setup the information for this leaderboard entry
                nameText = (entryIndex + 1).ToString() + ".    " + "JunkFoot";
                valueText = "10,000";

                // set the text position
                namePosition.X = bannerRect.X + (textOffsetX * displayScale);
                namePosition.Y = bannerRect.Y + (textOffsetY * displayScale);

                // set the value position
                valuePosition.X = bannerRect.X + bannerRect.Width - ((textOffsetX + valueFont.MeasureString(valueText).X) * displayScale);
                valuePosition.Y = bannerRect.Y + (textOffsetY * displayScale);
               
                // draw this leaderboard entry

                // start drawing the banner
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied);

                // draw the banner
                spriteBatch.Draw(bannerTexture, bannerRect, bannerColor);

                // finished drawing
                spriteBatch.End();

                // store the current scissor rectangle settings so we can restore them after we finish drawing
                Rectangle storedScissorRect = graphicsDevice.ScissorRectangle;

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

                // start dawring the text with the new scissor rectangle set
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.LinearClamp, DepthStencilState.None, new RasterizerState { ScissorTestEnable = true });

                // draw the achievement name
                spriteBatch.DrawString(nameFont, nameText, namePosition, nameTextColor);

                // draw the value
                spriteBatch.DrawString(valueFont, valueText, valuePosition, valueTextColor);

                // finished drawing the text
                spriteBatch.End();

                // restore the previous scissor rectangle settings
                graphicsDevice.ScissorRectangle = storedScissorRect;
 
                // move on to the next leaderboard entry in the list
                bannerRect.Y += (int)(listSpacingY * displayScale);
            }

            // store the number of leaderboard entries that were displayed
            if (entryIndex - entryDisplayIndex > entriesDisplayed)
                entriesDisplayed = entryIndex - entryDisplayIndex;
        }

        public void PageUp()
        {
        }

        public void PageDown()
        {
        }

        public void NextLeaderboard()
        {
        }

        public void PrevLeaderboard()
        {
        }

    }
}