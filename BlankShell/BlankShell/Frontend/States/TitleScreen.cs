//------------------------------------------------------------------------------
// Filename: TitleScreen.cs
// Author: nholmes
// Summary: main title screen for the game
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

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using IndieCityXna.Common;

namespace BlankShell.Frontend
{
    //------------------------------------------------------------------------------
    // Class: TitleScreen
    // Author: nholmes
    // Summary: main title. presents the player with the game logo and "press start"
    //------------------------------------------------------------------------------
    class TitleScreen : Menu
    {
        // content manager for the title screen
        ContentManager content;

        // texture for the logo
        private Texture2D logoTexture;
       
        //------------------------------------------------------------------------------
        // Function: TitleScreen
        // Author: nholmes
        // Summary: constructor, creates the title screen and adds the press start button
        //------------------------------------------------------------------------------
        public TitleScreen(Game game, PlayerIndex? controllingPlayer)
            : base(game, controllingPlayer, Color.Black)
        {
            // tell the menu that we dont want to display anything other than items
            backgroundVisible = false;

            // set the menu size and position
            SetArbitary((displayManager.GameResolutionX / 2) - 120, displayManager.GameResolutionY - 200, 200, 100);

            // create the press start menu item
            ItemButton pressStartMenuItem = new ItemButton(this, game, "Press Start", 0, 250, Alignment.centre, textFont, Color.White, Alignment.centre, textYOffset);

            // hook up the menu item event handler
            pressStartMenuItem.Selected += PressStartMenuItemSelected;

            // add the item to the menu
            menuItems.Add(pressStartMenuItem);
        }

        //------------------------------------------------------------------------------
        // Function: LoadContent
        // Author: nholmes
        // Summary: uses its own content manager to load the logo texture
        //------------------------------------------------------------------------------
        public override void LoadContent()
        {
            // check if we already created a content manager
            if (content == null)
            {
                // nope, create one!
                content = new ContentManager(game.Services, @"Content\Frontend");
            }

            // load the backgroun texure
            logoTexture = content.Load<Texture2D>(@"logo");
        }

        //------------------------------------------------------------------------------
        // Function: UnloadContent
        // Author: nholmes
        // Summary: unloads the logo texture when we are finished with it
        //------------------------------------------------------------------------------
        public override void UnloadContent()
        {
            content.Unload();
        }

        //------------------------------------------------------------------------------
        // Function: PressStartMenuItemSelected
        // Author: nholmes
        // Summary: event handler for when press start is selected
        //------------------------------------------------------------------------------
        void PressStartMenuItemSelected(object sender, MenuItemEventArgs e)
        {
            // add the main menu state
            gameStateManager.AddGameState(new MainMenu(game, e.PlayerIndex));

            // set isExiting so that the player can't return to the title screen
            isExiting = true;
        }

        //------------------------------------------------------------------------------
        // Function: OnCancel
        // Author: nholmes
        // Summary: if the user cancels the main menu, exit the game
        //------------------------------------------------------------------------------
        protected override void OnCancel(PlayerIndex playerIndex)
        {
            game.Exit();
        }

        //------------------------------------------------------------------------------
        // Function: Draw
        // Author: nholmes
        // Summary: draws the logo
        //------------------------------------------------------------------------------
        public override void Draw()
        {
            Rectangle destRect;
            
            // get a reference to the sprite batch from game state manager
            SpriteBatch spriteBatch = displayManager.GlobalSpriteBatch;

            // set the logo's position
            destRect.X = (int)((displayManager.GameResolutionX - logoTexture.Width) / 2);
            destRect.Y = 360;
            destRect.Width = logoTexture.Width;
            destRect.Height = logoTexture.Height;

            // set the brightness of the logo according to transition alpha so that it fades in and out
            byte fade = (byte)(TransitionAlpha * 0.75f);

            // draw the logo
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, SamplerState.LinearClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, displayManager.TransformMatrix);
            spriteBatch.Draw(logoTexture, destRect, new Color(fade, fade, fade));
            spriteBatch.End();

            // call the base drawing functionality
            base.Draw();
        }
    }
}