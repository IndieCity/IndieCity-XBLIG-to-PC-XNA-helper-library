//------------------------------------------------------------------------------
// Filename: ICLeaderboards.cs
// Author: Neil Holmes
// Summary: IndieCity leaderboard display exmaple
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
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

    // small class to hold a leaderboard entry once it has been retrieved from the server
    class leaderboardEntry
    {
        public string name;
        public string value;

        public leaderboardEntry(string name, string value)
        {
            this.name = name;
            this.value = value;
        }
    }

    public class ICLeaderboardsBrowser
    {
        private float displayScale;

        // the graphics device
        private GraphicsDevice graphicsDevice;

        // the sprite batch to use for drawing
        private SpriteBatch spriteBatch;

        // handle to the leaderboard manager
        private ICELandaLib.CoLeaderboardManager leaderboardManager;

        // the leaderboard currently being displayed
        private int currentLeaderboardIndex;

        // index of the first leaderboard entry to be displayed in the list
        private int entryDisplayIndex;

        // used to track how many leaderboard entries were rendered when the leaderboard was drawn
        private int entriesDisplayed;

        // the leaderboard graphics
        private Texture2D bannerTexture;

        // the leaderboard fonts
        private SpriteFont nameFont;
        private SpriteFont valueFont;

        // variables used for drawing
        private Rectangle bannerRect;
        private Vector2 namePosition;
        private Vector2 valuePosition;

        // constant values for drawing the leaderboard
        private static float safeZoneScaleValue = 0.07f;
        private static int minDisplayWidth = 300;
        private static int maxDisplayWidth = 800;
        private static int bannerHeight = 32;
        private static int listSpacingY = 44;
        private static int textOffsetX = 12;
        private static int textOffsetY = 2;

        // constant colours for normal display
        private static Color normalBannerColor = new Color(255, 255, 255, 255);
        private static Color normalNameTextColor = new Color(0, 0, 0, 200);
        private static Color normalValueTextColor = new Color(0, 0, 0, 200);

        // constant colours for inverted display
        private static Color invertedBannerColor = new Color(25, 25, 30, 255);
        private static Color invertedNameTextColor = new Color(225, 225, 225, 200);
        private static Color invertedValueTextColor = new Color(255, 255, 255, 200);

        // ordered list of UIDs to use for the leaderboard browser
        private int[] leaderboardUIDlist;

        // variables for controlling the requesting and storing of leaderboard pages from indiecity.com
        private int numEntriesReceived;
        private bool noMoreEntries;
        private ICELandaLib.CoLeaderboard currentLeaderboard; 
        private ICELandaLib.CoLeaderboardPage requestedPage;
        private ICELandaLib.CoLeaderboardPage lastRequestedPage;
        private List<leaderboardEntry> entries; 
        private static uint entriesPerBatch = 25;
        
        public ICLeaderboardsBrowser(Game game, GraphicsDeviceManager graphicsDeviceManager, ICELandaLib.CoLeaderboardManager leaderboardManager, ICLeaderboardScale scale, int[] leaderboardUIDlist)
        {
            // create a content manager to handle loading the textures we need
            ContentManager content = new ContentManager(game.Services);

            // store the graphics device for future reference
            this.graphicsDevice = game.GraphicsDevice;

            // store a handle to the leaderboard manager
            this.leaderboardManager = leaderboardManager;

            // store the scale we are using
            if (scale == ICLeaderboardScale.normal)
                displayScale = 1.0f;
            else
                displayScale = 0.5f;

            // store the ordered list of leaderboard UID values
            this.leaderboardUIDlist = leaderboardUIDlist;

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

            // by default we want to display the first leaderbaord in the list
            currentLeaderboardIndex = 0;

            // by default we want to display the leaderboard from the first entry onwards
            entryDisplayIndex = 0;

            // the number of leaderboard entries that were previously displayed
            entriesDisplayed = 0;

            // ensure leaderboard entries recieved, currentLeaderboard and requestedPage are all reset
            numEntriesReceived = 0;
            noMoreEntries = false;
            currentLeaderboard = null; 
            requestedPage = null;
            lastRequestedPage = null;
            entries = new List<leaderboardEntry>();
        }

        public void Update()
        {
            // do we have a current leaderboard?
            if (currentLeaderboard != null)
            {
                // are we currently waiting for a page request to finish?
                if (requestedPage != null)
                {
                    // check if the page request has completed and process accoringly
                    switch (requestedPage.PopulationState)
                    {
                        case ICELandaLib.LeaderboardPopulationState.LPS_PENDING:

                            // nothing to do yet...
                            break;

                        case ICELandaLib.LeaderboardPopulationState.LPS_INVALID:

                            // data request failed - bail out and release requested page so we can try again later!
                            requestedPage = null;
                            break;

                        case ICELandaLib.LeaderboardPopulationState.LPS_POPULATED:

                            // check to see if we recieved less entries than expected
                            if (requestedPage.Size < 20)
                            {
                                // if we recieved less than 20 entries then we must have reached the end of the leaderboard
                                // set no more entries to true so we dont constantly spam the website asking for more entries
                                noMoreEntries = true;
                            }
                        
                            // got the data - copy it to our local list
                            for (uint i = 0; i < requestedPage.Size; i ++)
                            {
                                ICELandaLib.CoLeaderboardRow row = requestedPage.GetRow(i);
                                entries.Add(new leaderboardEntry(row.Rank.ToString() + " - " + row.UserName, row.Score.ToString()));
                            }                            
                           
                            // increased the number of entries recieved
                            numEntriesReceived += (int)requestedPage.Size;
                            
                            // store the last requested page and release the requested page ready for next time
                            lastRequestedPage = requestedPage;
                            requestedPage = null;
                            break;
                    }
                }
            }
        }
                
        public void Draw(Rectangle displayArea, int displayWidth, ICLeaderboardColorMode colorMode, bool useSafeZone = false)
        {
            Color bannerColor;
            Color nameTextColor;
            Color valueTextColor;
            Color leaderboardBannerColor;
            Color leaderboardTextColor;

            // have we recieved any data for this leaderboard?
            if (numEntriesReceived == 0 && noMoreEntries == false)
            {
                // if we havn't recieved anything before then we need to start getting the first set of data to display!
                RequestFirstBlockOfEntries();
            }
            
            // set the first achievement to be displayed
            int entryIndex = entryDisplayIndex;

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
                leaderboardBannerColor = invertedBannerColor;
                leaderboardTextColor = invertedNameTextColor;
            }
            else
            {
                bannerColor = invertedBannerColor;
                nameTextColor = invertedNameTextColor;
                valueTextColor = invertedValueTextColor;
                leaderboardBannerColor = normalBannerColor;
                leaderboardTextColor = normalNameTextColor;
            }

            // display the name of this leaderbaord
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied);
            spriteBatch.Draw(bannerTexture, bannerRect, leaderboardBannerColor);
            namePosition.X = displayArea.X + (int)(displayArea.Width / 2) - (int)(valueFont.MeasureString(currentLeaderboard.Name).X / 2);
            namePosition.Y = bannerRect.Y + (textOffsetY * displayScale);
            spriteBatch.DrawString(valueFont, currentLeaderboard.Name, namePosition, leaderboardTextColor);
            spriteBatch.End();

            // move past the area where we drew the leaderboard name
            bannerRect.Y += (int)(listSpacingY * displayScale);

            // are we currently waiting for data?
            if (requestedPage != null)
            {
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied);
                spriteBatch.Draw(bannerTexture, bannerRect, bannerColor);
                namePosition.X = displayArea.X + (int)(displayArea.Width / 2) - (int)(nameFont.MeasureString("Retrieving Data...").X / 2);
                namePosition.Y = bannerRect.Y + (textOffsetY * displayScale); 
                spriteBatch.DrawString(nameFont, "Retrieving Data...", namePosition, nameTextColor);
                spriteBatch.End();
                return;
            }
            
            // loop through and display as many leaderboard entries as possible on the screen...   
            do
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

                // check to see if we have enough entry data cached from indiecity.com to render this one
                if (entryIndex >= entries.Count)
                {
                    // have we previously recieved any entries at all? (ie are we still waiting for the first block?)
                    if (numEntriesReceived > 0)
                    {
                        // we have some entries but not enough! do we have more that we can request?
                        if (noMoreEntries == false)
                        {
                            RequestNextBlockOfEntries();
                            return;
                        }
                    }

                    // have we displayed some entries?
                    if (entryIndex > 0)
                    {
                        // if we get here there are no entries to display
                        spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied);
                        spriteBatch.Draw(bannerTexture, bannerRect, bannerColor);
                        namePosition.X = displayArea.X + (int)(displayArea.Width / 2) - (int)(nameFont.MeasureString("No more entries...").X / 2);
                        namePosition.Y = bannerRect.Y + (textOffsetY * displayScale);
                        spriteBatch.DrawString(nameFont, "No more entries...", namePosition, nameTextColor);
                        spriteBatch.End();
                    }
                    else
                    {
                        // if we get here there are no entries to display
                        spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied);
                        spriteBatch.Draw(bannerTexture, bannerRect, bannerColor);
                        namePosition.X = displayArea.X + (int)(displayArea.Width / 2) - (int)(nameFont.MeasureString("No entries found!").X / 2);
                        namePosition.Y = bannerRect.Y + (textOffsetY * displayScale);
                        spriteBatch.DrawString(nameFont, "No entries Found!", namePosition, nameTextColor);
                        spriteBatch.End();
                    }
                    return;
                }

                // set the text position
                namePosition.X = bannerRect.X + (textOffsetX * displayScale);
                namePosition.Y = bannerRect.Y + (textOffsetY * displayScale);

                // set the value position
                valuePosition.X = bannerRect.X + bannerRect.Width - ((int)(textOffsetX * displayScale) + (int)valueFont.MeasureString(entries[entryIndex].value).X);
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
                spriteBatch.DrawString(nameFont, entries[entryIndex].name, namePosition, nameTextColor);

                // draw the value
                spriteBatch.DrawString(valueFont, entries[entryIndex].value, valuePosition, valueTextColor);

                // finished drawing the text
                spriteBatch.End();

                // restore the previous scissor rectangle settings
                graphicsDevice.ScissorRectangle = storedScissorRect;

                // move on to the next leaderboard entry in the list
                bannerRect.Y += (int)(listSpacingY * displayScale);

                // move on to the next entry
                entryIndex++;

            } while (true);

            // store the number of leaderboard entries that were displayed
            if (entryIndex - entryDisplayIndex > entriesDisplayed)
                entriesDisplayed = entryIndex - entryDisplayIndex;
        }

        // request first block of entries
        private void RequestFirstBlockOfEntries()
        {
            // bail out if we are currently waiting for a page request to finish
            if (requestedPage != null) return;

            // reset the entry display index etc ready for drawing the new leaderboard
            entryDisplayIndex = 0;
            entriesDisplayed = 0;

            // ensure that entries recieved and entries are reset
            numEntriesReceived = 0;
            noMoreEntries = false;
            entries.Clear();

            // request the first 20 entries
            currentLeaderboard = leaderboardManager.GetLeaderboardFromId(leaderboardUIDlist[currentLeaderboardIndex]);
            requestedPage = currentLeaderboard.GetGlobalPage(entriesPerBatch);
        }

        // request next block of entries
        private void RequestNextBlockOfEntries()
        {
            // bail out if we are currently waiting for a page request to finish
            if (requestedPage != null) return;

            // request the next batch of entries
            lastRequestedPage.RequestNext();

            // set requested page so that we can monitor the status
            requestedPage = lastRequestedPage;
        }

        public void PageUp()
        {
            // bail out if we are currently waiting for a page request to finish
            if (requestedPage != null) return;

            // page up by the number of entries displayed
            entryDisplayIndex -= entriesDisplayed;
            if (entryDisplayIndex < 0) entryDisplayIndex = 0;
        }

        public void PageDown()
        {
            // bail out if we are currently waiting for a page request to finish
            if (requestedPage != null) return;

            // page down by the number of entries displayed
            entryDisplayIndex += entriesDisplayed;

            // have we reached the end of the list?
            if (noMoreEntries == true)
            {
                // have we gone past the end of the entries?
                if (entryDisplayIndex >= numEntriesReceived)
                {
                    // step back to a point where we can fill the screen
                    entryDisplayIndex = numEntriesReceived - entriesDisplayed;
                    if (entryDisplayIndex < 0) entryDisplayIndex = 0;
                }
            }
        }

        public void NextLeaderboard()
        {
            // bail out if we are currently waiting for a page request to finish
            if (requestedPage != null) return;
            
            // move on to the next leaderbaord in the list (wrap around to zero at list end)
            currentLeaderboardIndex++;
            if (currentLeaderboardIndex == leaderboardUIDlist.Length) currentLeaderboardIndex = 0;

            // request first block of entries
            RequestFirstBlockOfEntries();
        }

        public void PrevLeaderboard()
        {
            // bail out if we are currently waiting for a page request to finish
            if (requestedPage != null) return;
            
            // move to the previous leaderbaord in the list (wrap around to end at list start)
            currentLeaderboardIndex --;
            if (currentLeaderboardIndex < 0) currentLeaderboardIndex = leaderboardUIDlist.Length - 1;

            // request first block of entries
            RequestFirstBlockOfEntries();
        }
    }
}