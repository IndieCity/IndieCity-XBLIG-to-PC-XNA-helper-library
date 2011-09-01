//------------------------------------------------------------------------------
// Filename: Menu.cs
// Author: nholmes
// Summary: base class for states that contain a menu of selectable items
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

// TODO: add the following menu item types:
//      image
//
// TODO: add the following overloads:
//      positional image
//      selectable image

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using IndieCityXna.Common;
using IndieCityXna.GameState;

namespace BlankShell.Frontend
{
    // alignment enum used to specify alignment of the title and menu items within a menu
    public enum Alignment
    {
        centre,
        left,
        right,
    };

    // enum list of different fonts that are available for the items to use
    public enum MenuFont
    {
        title,
        text,
        button,
        warning,
    };

    // enum list of different shadow textures that are available for menus to use
    public enum ShadowType
    {
        none,
        normal,
        question,
        warning,
    };
    
    //------------------------------------------------------------------------------
    // Class: Menu
    // Author: nholmes
    // Summary: Base class for states that contain menus of options. The user can
    //          move up/down to choose an item, left/right to change an item, press 
    //          select to activate an item or cancel to back out of the screen
    //------------------------------------------------------------------------------
    public class Menu : GameState
    {
        // content manger for the menus
        private static ContentManager content;

        // the position and size of this menu
        private Rectangle shape;

        // the position and size of the area items occupy
        private Rectangle itemArea;

        // handle to the mouse pointer display manager
        private Pointer mousePointer;

        // textures used to create the menu
        private static Texture2D topLeftTexture;
        private static Texture2D bottomLeftTexture; 
        private static Texture2D topRightTexture;
        private static Texture2D bottomRightTexture;
        private static Texture2D topEdgeTexture;
        private static Texture2D bottomEdgeTexture;
        private static Texture2D leftEdgeTexture;
        private static Texture2D rightEdgeTexture;
        private static Texture2D centerTexture;

        // textures used by the menu items
        protected static Texture2D buttonTexture;
        protected static Texture2D buttonLeftTexture;
        protected static Texture2D buttonRightTexture;
        protected static Texture2D selectorLeftTexture;
        protected static Texture2D selectorRightTexture;
        protected static Texture2D sliderButtonTexture;
        protected static Texture2D sliderBarTexture;

        // texture to be used by the loading screen
        private static Texture2D loadingTexture;

        // the size of the border around the menu
        private const int exteriorBorderSize = 8;

        // the size of the border around the menu items
        private const int interiorBorderSize = 6;

        // offset used to move the title font into the correct Y alignment
        private const int titleOffset = 14;

        // size of the horizontal gap between the title and the top edge of the menu
        private const int titleSpacing = 2;

        // y offset for aligning text with menu items
        protected const int textYOffset = 1;

        // the currently selected item
        private int selectedItem = 0;

        // the color selected items should use when drawing
        private Color selectionColor = Color.White;

        // controls whether the selection color is fading up or down
        private bool selectionColorDirection = false;

        // the name and alignment of this page's title
        protected string title = "missing title";
        protected Alignment alignment;
        
        // list of fonts that the menu and it's items can use
        protected static SpriteFont textFont;
        protected static SpriteFont titleFont;

        // the colour of the menu font
        protected Color fontColor;

        // list of all of the menu items on the page
        protected List<MenuItem> menuItems = new List<MenuItem>();

        // controls rendering of the title and menu border
        protected bool backgroundVisible = true;
 
        //------------------------------------------------------------------------------
        // Function: Menu
        // Author: nholmes
        // Summary: default Constructor, allows user to specify the tile of the page  
        //------------------------------------------------------------------------------
        public Menu(Game game, PlayerIndex? controllingPlayer, Color fontColor)
             : base(game, controllingPlayer)
        {
            // get a handle to the mouse pointer service
            mousePointer = (Pointer)game.Services.GetService(typeof(Pointer));
            
            // check if we have previously created the static content manager
            if (content == null)
            {
                // create the static content manager
                content = new ContentManager(game.Services, @"Content\Frontend");

                // load all the static fonts
                titleFont = content.Load<SpriteFont>(@"Fonts\titleFont");
                textFont = content.Load<SpriteFont>(@"Fonts\textFont");

                // load all the static textures
                topLeftTexture = content.Load<Texture2D>(@"Dialog\TopLeft");
                topRightTexture = content.Load<Texture2D>(@"Dialog\TopRight");
                bottomLeftTexture = content.Load<Texture2D>(@"Dialog\BottomLeft");
                bottomRightTexture = content.Load<Texture2D>(@"Dialog\BottomRight");
                topEdgeTexture = content.Load<Texture2D>(@"Dialog\TopEdge");
                bottomEdgeTexture = content.Load<Texture2D>(@"Dialog\BottomEdge");
                leftEdgeTexture = content.Load<Texture2D>(@"Dialog\LeftEdge");
                rightEdgeTexture = content.Load<Texture2D>(@"Dialog\RightEdge");
                centerTexture = content.Load<Texture2D>(@"Dialog\center");
                buttonTexture = content.Load<Texture2D>(@"Button\Button");
                buttonLeftTexture = content.Load<Texture2D>(@"Button\ButtonLeft");
                buttonRightTexture = content.Load<Texture2D>(@"Button\ButtonRight");
                selectorLeftTexture = content.Load<Texture2D>(@"Selector\SelectorLeft");
                selectorRightTexture = content.Load<Texture2D>(@"Selector\SelectorRight");
                sliderButtonTexture = content.Load<Texture2D>(@"Slider\SliderButton");
                sliderBarTexture = content.Load<Texture2D>(@"Slider\SliderBar");

                // load the loading image
                loadingTexture = content.Load<Texture2D>(@"loading");
            }

            // store the font colour
            this.fontColor = fontColor;
        }

        //------------------------------------------------------------------------------
        // Function: SetFullScreen
        // Author: nholmes
        // Summary: helper function used to set the menu to occupy the full safe area of
        //          the screen
        //------------------------------------------------------------------------------
        protected void SetFullScreen()
        {
            // calculate the safe border based on the viewport settings
            int safeBorderWidth = (int)(displayManager.GameResolutionX * 0.1f);
            int safeBorderHeight = (int)(displayManager.GameResolutionY * 0.1f);

            // set the position and size of the menu using the safe border sizes and viewport settings
            shape.X = safeBorderWidth;
            shape.Y = safeBorderHeight;
            shape.Width = displayManager.GameResolutionX - (safeBorderWidth * 2);
            shape.Height = displayManager.GameResolutionY - (safeBorderHeight * 2);

            // set the position and size of the item area
            SetItemArea(0, 0);
        }

        //------------------------------------------------------------------------------
        // Function: SetCentered
        // Author: nholmes
        // Summary: helper function used to set the menu to a fixed size in the centre of
        //          the viewport
        //------------------------------------------------------------------------------
        protected void SetCentered(int width, int height)
        {
            // calculate the centre based on the viewport settings
            int centreX = (int)(displayManager.GameResolutionX * 0.5f);
            int centreY = (int)(displayManager.GameResolutionY * 0.5f);

            // set the position and size of the menu using the safe border sizes and viewport settings
            shape.X = centreX - (width / 2);
            shape.Y = centreY - (height / 2);
            shape.Width = width;
            shape.Height = height;

            // set the position and size of the item area
            SetItemArea(0, 0);
        }

        //------------------------------------------------------------------------------
        // Function: SetArbitary
        // Author: nholmes
        // Summary: helper function used to set the menu to a fixed size and position
        //------------------------------------------------------------------------------
        protected void SetArbitary(int x, int y, int width, int height)
        {
            // set the position and size of the menu using the safe border sizes and viewport settings
            shape.X = x;
            shape.Y = y;
            shape.Width = width;
            shape.Height = height;

            // set the position and size of the item area
            SetItemArea(0, 0);
        }

        //------------------------------------------------------------------------------
        // Function: SetItemArea
        // Author: nholmes
        // Summary: sets the position and size of the area that items can occupy
        //------------------------------------------------------------------------------
        private void SetItemArea(int offsetX, int offsetY)
        {
            // set the itemArea rectangle based on menu position and edge and interior border sizes
            itemArea.X = shape.X + leftEdgeTexture.Width + interiorBorderSize + offsetX;
            itemArea.Y = shape.Y + topEdgeTexture.Height + interiorBorderSize + offsetY;
            itemArea.Width = shape.Width - (leftEdgeTexture.Width + rightEdgeTexture.Width + (interiorBorderSize * 2));
            itemArea.Height = shape.Height - (topEdgeTexture.Height + bottomEdgeTexture.Height + (interiorBorderSize * 2));
        }

        //------------------------------------------------------------------------------
        // Function: ItemArea
        // Author: nholmes
        // Summary: accessor function to get the item area of the menu
        //------------------------------------------------------------------------------
        public Rectangle ItemArea
        {
            get { return itemArea; }
        }

        //------------------------------------------------------------------------------
        // Function: ButtonTexture
        // Author: nholmes
        // Summary: accessor function to get the texture used for button items
        //------------------------------------------------------------------------------
        public Texture2D ButtonTexture
        {
            get { return buttonTexture; }
        }

        //------------------------------------------------------------------------------
        // Function: ButtonLeftTexture
        // Author: nholmes
        // Summary: accessor function to get the texture used for button items
        //------------------------------------------------------------------------------
        public Texture2D ButtonLeftTexture
        {
            get { return buttonLeftTexture; }
        }

        //------------------------------------------------------------------------------
        // Function: ButtonRghtTexture
        // Author: nholmes
        // Summary: accessor function to get the texture used for button items
        //------------------------------------------------------------------------------
        public Texture2D ButtonRightTexture
        {
            get { return buttonRightTexture; }
        }

        //------------------------------------------------------------------------------
        // Function: SelectorLeftTexture
        // Author: nholmes
        // Summary: accessor function to get the left arrow texture used for selector items
        //------------------------------------------------------------------------------
        public Texture2D SelectorLeftTexture
        {
            get { return selectorLeftTexture; }
        }

        //------------------------------------------------------------------------------
        // Function: SelectorRghtTexture
        // Author: nholmes
        // Summary: accessor function to get the right arrow texture used for selector items
        //------------------------------------------------------------------------------
        public Texture2D SelectorRightTexture
        {
            get { return selectorRightTexture; }
        }

        //------------------------------------------------------------------------------
        // Function: SliderButtonTexture
        // Author: nholmes
        // Summary: accessor function to get the slider button used for slider items
        //------------------------------------------------------------------------------
        public Texture2D SliderButtonTexture
        {
            get { return sliderButtonTexture; }
        }

        //------------------------------------------------------------------------------
        // Function: SliderBarTexture
        // Author: nholmes
        // Summary: accessor function to get the slider bar used for slider items
        //------------------------------------------------------------------------------
        public Texture2D SliderBarTexture
        {
            get { return sliderBarTexture; }
        }

        //------------------------------------------------------------------------------
        // Function: LoadingTexture
        // Author: nholmes
        // Summary: accessor function to get the loading texture
        //------------------------------------------------------------------------------
        public Texture2D LoadingTexture
        {
            get { return loadingTexture; }
        }

        //------------------------------------------------------------------------------
        // Function: GetItemColor
        // Author: nholmes
        // Summary: helper function to get the color items should use when drawing
        //------------------------------------------------------------------------------
        public Color GetItemColor(bool selected)
        {
            // is the item selected?
            if (selected)
            {
                // yup, return the pulsing selection color
                return selectionColor;
            }
            else
            {
                // nope, return the unselected color
                return Color.White;
            }
        }

        //------------------------------------------------------------------------------
        // Function: HandleInput
        // Author: nholmes
        // Summary: overrides the base handle input to provide menu specific controlls  
        //------------------------------------------------------------------------------
        public override void HandleInput()
        {
            // check if we should move to the previous menu item
            if (IsMenuUp(controllingPlayer))
            {
                // yup decrease the selected item index
                selectedItem --;

                // if we reached the start of the list, wrap around
                if (selectedItem < 0)
                {
                    selectedItem = menuItems.Count - 1;
                }

                // check that the new item is selectable and if not keep going until we find one that is
                FindPreviousSelectableItem();
            }

            // check if we should move on to the next menu item
            if (IsMenuDown(controllingPlayer))
            {
                // yup increase the selected item index
                selectedItem ++;

                // if we reached the end of the list, wrap around
                if (selectedItem >= menuItems.Count)
                {
                    selectedItem = 0;
                }

                // check that the new item is selectable and if not keep going until we find one that is
                FindNextSelectableItem();
            }

            // check if the user (or users) have selected the current item, changed the seleted option or cancelled the menu
            if (IsMenuSelect(controllingPlayer))
            {
                // item selected - call the on item selected action for the item
                OnItemSelected(selectedItem, inputManager.GetPlayerResponsible());
            }
            else if (IsMenuNextOption(controllingPlayer))
            {
                // next option required - call the on item next option action for the item
                OnItemNextOption(selectedItem, inputManager.GetPlayerResponsible());
            }
            else if (IsMenuPreviousOption(controllingPlayer))
            {
                // previous option required - call the on item previous option action for the item
                OnItemPreviousOption(selectedItem, inputManager.GetPlayerResponsible());
            }
            else if (IsMenuIncreaseValue(controllingPlayer))
            {
                // selected option increased - call the on item increased action for the item
                OnItemIncreaseValue(selectedItem, inputManager.GetPlayerResponsible());
            }
            else if (IsMenuDecreaseValue(controllingPlayer))
            {
                // selected option decreased - call the on item decreased action for the item
                OnItemDecreaseValue(selectedItem, inputManager.GetPlayerResponsible());
            }
            else if (IsMenuCancel(controllingPlayer))
            {
                // menu cancelled - call the on cancel action
                OnCancel(inputManager.GetPlayerResponsible());
            }

            // handle mouse selection
            if (inputManager.IsNewLeftMouseButtonPressed())
            {
                // spin through all the items and ask them to check if they've been clicked
                for(int item = 0; item < menuItems.Count; item ++)
                {
                    switch (menuItems[item].OnPointerSelection(mousePointer.Position))
                    {
                        case MenuItemSelectionType.none:

                            // nothing to do :)
                            break;

                        case MenuItemSelectionType.selected:

                            // tell this item that it has been selected
                            OnItemSelected(item, inputManager.GetPlayerResponsible());
                            break;

                        case MenuItemSelectionType.increaseValue:
 
                            // tell this item that it should increase in value
                            OnItemIncreaseValue(item, inputManager.GetPlayerResponsible());
                            break;

                        case MenuItemSelectionType.decreaseValue:

                            // tell this item that it has been selected
                            OnItemDecreaseValue(item, inputManager.GetPlayerResponsible());
                            break;

                        case MenuItemSelectionType.nextOption:

                            // tell this item that it should increase in value
                            OnItemNextOption(item, inputManager.GetPlayerResponsible());
                            break;

                        case MenuItemSelectionType.previousOption:

                            // tell this item that it has been selected
                            OnItemPreviousOption(item, inputManager.GetPlayerResponsible());
                            break;
                    }
                }
            }
        }

        //------------------------------------------------------------------------------
        // Function: FindPreviousSelectableItem
        // Author: nholmes
        // Summary: helper function, checks if current item is selectable and if not
        //          marches backwards through the item list until it finds one that is
        //------------------------------------------------------------------------------
        private void FindPreviousSelectableItem()
        {
            // check if the item we are on is selectable and if not move onto the next
            while (!menuItems[selectedItem].Selectable)
            {
                // not selectable, decrease the selected item index
                selectedItem--;

                // if we reached the start of the list, wrap around
                if (selectedItem < 0)
                {
                    selectedItem = menuItems.Count - 1;
                }
            }
        }

        //------------------------------------------------------------------------------
        // Function: FindNextSelectableItem
        // Author: nholmes
        // Summary: helper function, checks if current item is selectable and if not
        //          marches forward through the item list until it finds one that is
        //------------------------------------------------------------------------------
        private void FindNextSelectableItem()
        {
            // store the initial index we are searching from
            int initialSelectedItem = selectedItem;
            
            // check if the item we are on is selectable and if not move onto the next
            while (!menuItems[selectedItem].Selectable)
            {
                // not selectable, increase the selected item index
                selectedItem++;

                // if we reached the end of the list, wrap around
                if (selectedItem >= menuItems.Count)
                {
                    selectedItem = 0;
                }

                // have we searched the whole list and not found a selectable item? 
                if (selectedItem == initialSelectedItem)
                {
                    // no selectable items in the list! bail out!
                    return;
                }
            }
        }

        //------------------------------------------------------------------------------
        // Function: OnItemSelected
        // Author: nholmes
        // Summary: handler for when the user has selected a menu item
        //------------------------------------------------------------------------------
        protected virtual void OnItemSelected(int entryIndex, PlayerIndex playerIndex)
        {
            menuItems[entryIndex].OnSelectEntry(playerIndex);
        }

        //------------------------------------------------------------------------------
        // Function: OnItemNextOption
        // Author: nholmes
        // Summary: handler for when the user wants to select the item's next option
        //------------------------------------------------------------------------------
        protected virtual void OnItemNextOption(int entryIndex, PlayerIndex playerIndex)
        {
            menuItems[entryIndex].OnNextOptionEntry(playerIndex);
        }

        //------------------------------------------------------------------------------
        // Function: OnItemPreviousOption
        // Author: nholmes
        // Summary: handler for when the user wants to select the item's previous option
        //------------------------------------------------------------------------------
        protected virtual void OnItemPreviousOption(int entryIndex, PlayerIndex playerIndex)
        {
            menuItems[entryIndex].OnPreviousOptionEntry(playerIndex);
        }

        //------------------------------------------------------------------------------
        // Function: OnItemIncreased
        // Author: nholmes
        // Summary: handler for when the user wants to increase the item's value
        //------------------------------------------------------------------------------
        protected virtual void OnItemIncreaseValue(int entryIndex, PlayerIndex playerIndex)
        {
            menuItems[entryIndex].OnItemIncreaseValueEntry(playerIndex);
        }

        //------------------------------------------------------------------------------
        // Function: OnItemDecreased
        // Author: nholmes
        // Summary: handler for when the user wants to decrease the item's value
        //------------------------------------------------------------------------------
        protected virtual void OnItemDecreaseValue(int entryIndex, PlayerIndex playerIndex)
        {
            menuItems[entryIndex].OnItemDecreaseValueEntry(playerIndex);
        }

        //------------------------------------------------------------------------------
        // Function: OnCancel
        // Author: nholmes
        // Summary: handler for when the user has cancelled the menu
        //------------------------------------------------------------------------------
        protected virtual void OnCancel(PlayerIndex playerIndex)
        {
            CloseState();
        }

        //------------------------------------------------------------------------------
        // Function: OnCancel
        // Author: nholmes
        // Summary: helper overload makes it easy to use OnCancel as a MenuItem event handler
        //------------------------------------------------------------------------------
        protected void OnCancel(object sender, MenuItemEventArgs e)
        {
            OnCancel(e.PlayerIndex);
        }

        //------------------------------------------------------------------------------
        // Function: Update
        // Author: nholmes
        // Summary: updates the menu
        //------------------------------------------------------------------------------
        public override void Update(bool otherStateHasFocus, bool coveredByOtherState)
        {
            // update the game state base class
            base.Update(otherStateHasFocus, coveredByOtherState);

            // store another state has focus so we can effect draw brightness
            this.otherStateHasFocus = otherStateHasFocus;

            // update each nested MenuItem object
            for (int item = 0; item < menuItems.Count; item ++)
            {
                // figure out if this item is selected or nto
                bool isSelected = IsActive && (item == selectedItem);

                // update the item
                menuItems[item].Update(isSelected);
            }

            // ensure that we are on a selectable item (this should only really be untrue
            // if the first item the user adds to a menu is an unselectable one)
            FindNextSelectableItem();

            // update the item selection color based on current dirction
            if (selectionColorDirection)
            {
                // fading up, check for reaching top and then reverse
                if (selectionColor.G < 252)
                {
                    selectionColor.R += 4;
                    selectionColor.G += 4;
                    selectionColor.B += 4;
                }
                else
                {
                    // reached the top, set to max and reverse
                    selectionColor.R = 255;
                    selectionColor.G = 255;
                    selectionColor.B = 255;
                    selectionColorDirection = false;
                }
            }
            else
            {
                // fading down, check for reaching bottom and then reverse
                if (selectionColor.G > 180)
                {
                    selectionColor.R -= 4;
                    selectionColor.G -= 4;
                    selectionColor.B -= 4;
                }
                else
                {
                    // reached the bottom, set to min and reverse
                    selectionColor.R = 180;
                    selectionColor.G = 180;
                    selectionColor.B = 180;
                    selectionColorDirection = true;
                }
            }
        }

        //------------------------------------------------------------------------------
        // Function: IsMenuSelect
        // Author: nholmes
        // Summary: checks if one of the menu select keys or button was just pressed. if 
        //          controllingPlayer is null it will accept input from any player. 
        //------------------------------------------------------------------------------
        public bool IsMenuSelect(PlayerIndex? controllingPlayer)
        {
            return inputManager.WasKeyPressed(Keys.Space, controllingPlayer) ||
                   inputManager.WasKeyPressed(Keys.Enter, controllingPlayer) ||
                   inputManager.WasButtonPressed(Buttons.A, controllingPlayer) ||
                   inputManager.WasButtonPressed(Buttons.Start, controllingPlayer);
        }

        //------------------------------------------------------------------------------
        // Function: IsMenuNextOption
        // Author: nholmes
        // Summary: checks if one of the menu next option keys or buttons was just pressed 
        //          or if the user moved the left stick to the right. if controllingPlayer
        //          is null it will accept input from any player. 
        //------------------------------------------------------------------------------
        public bool IsMenuNextOption(PlayerIndex? controllingPlayer)
        {
            float X, Y;
            
            inputManager.GetLeftStickPosition(controllingPlayer, out X, out Y);

            return inputManager.WasKeyPressed(Keys.Right, controllingPlayer) ||
                   inputManager.WasButtonPressed(Buttons.DPadRight, controllingPlayer) ||
                   (X > 0.5f);
        }

        //------------------------------------------------------------------------------
        // Function: IsMenuPreviousOption
        // Author: nholmes
        // Summary: checks if one of the menu previos option keys or buttons was just pressed 
        //          or if the user moved the left stick to the left. if controllingPlayer
        //          is null it will accept input from any player. 
        //------------------------------------------------------------------------------
        public bool IsMenuPreviousOption(PlayerIndex? controllingPlayer)
        {
            float X, Y;

            inputManager.GetLeftStickPosition(controllingPlayer, out X, out Y);

            return inputManager.WasKeyPressed(Keys.Left, controllingPlayer) ||
                   inputManager.WasButtonPressed(Buttons.DPadLeft, controllingPlayer) ||
                   (X < -0.5f);
        }

        //------------------------------------------------------------------------------
        // Function: IsMenuIncreaseValue
        // Author: nholmes
        // Summary: checks if one of the menu increase value keys or buttons is pressed 
        //          or if the user moved the left stick to the right. if controllingPlayer
        //          is null it will accept input from any player. 
        //------------------------------------------------------------------------------
        public bool IsMenuIncreaseValue(PlayerIndex? controllingPlayer)
        {
            float X, Y;

            inputManager.GetLeftStickPosition(controllingPlayer, out X, out Y);

            return inputManager.IsKeyPressed(Keys.Right, controllingPlayer) ||
                   inputManager.IsButtonPressed(Buttons.DPadRight, controllingPlayer) ||
                   (X > 0.5f);
        }

        //------------------------------------------------------------------------------
        // Function: IsMenuDecreaseValue
        // Author: nholmes
        // Summary: checks if one of the menu decrease value keys or buttons is pressed 
        //          or if the user moved the left stick to the left. if controllingPlayer
        //          is null it will accept input from any player. 
        //------------------------------------------------------------------------------
        public bool IsMenuDecreaseValue(PlayerIndex? controllingPlayer)
        {
            float X, Y;

            inputManager.GetLeftStickPosition(controllingPlayer, out X, out Y);

            return inputManager.IsKeyPressed(Keys.Left, controllingPlayer) ||
                   inputManager.IsButtonPressed(Buttons.DPadLeft, controllingPlayer) ||
                   (X < -0.5f);
        }

        //------------------------------------------------------------------------------
        // Function: IsMenuCancel
        // Author: nholmes
        // Summary: checks if one of the menu cancel keys or button was just pressed. if 
        //          controllingPlayer is null it will accept input from any player. 
        //------------------------------------------------------------------------------
        public bool IsMenuCancel(PlayerIndex? controllingPlayer)
        {
            return inputManager.WasKeyPressed(Keys.Back, controllingPlayer) ||
                   inputManager.WasButtonPressed(Buttons.B, controllingPlayer) ||
                   inputManager.WasButtonPressed(Buttons.Back, controllingPlayer);
        }

        //------------------------------------------------------------------------------
        // Function: IsMenuUp
        // Author: nholmes
        // Summary: checks if one of the menu up keys or button was just pressed. if 
        //          controllingPlayer is null it will accept input from any player. 
        //------------------------------------------------------------------------------
        public bool IsMenuUp(PlayerIndex? controllingPlayer)
        {
            return inputManager.WasKeyPressed(Keys.Up, controllingPlayer) ||
                   inputManager.WasButtonPressed(Buttons.DPadUp, controllingPlayer) ||
                   inputManager.WasButtonPressed(Buttons.LeftThumbstickUp, controllingPlayer);
        }

        //------------------------------------------------------------------------------
        // Function: IsMenuDown
        // Author: nholmes
        // Summary: checks if one of the menu down keys or button was just pressed. if 
        //          controllingPlayer is null it will accept input from any player. 
        //------------------------------------------------------------------------------
        public bool IsMenuDown(PlayerIndex? controllingPlayer)
        {
            return inputManager.WasKeyPressed(Keys.Down, controllingPlayer) ||
                   inputManager.WasButtonPressed(Buttons.DPadDown, controllingPlayer) ||
                   inputManager.WasButtonPressed(Buttons.LeftThumbstickDown, controllingPlayer);
        }

        //------------------------------------------------------------------------------
        // Function: Draw
        // Author: nholmes
        // Summary: draws the menu
        //------------------------------------------------------------------------------
        public override void Draw()
        {
            SpriteBatch spriteBatch = displayManager.GlobalSpriteBatch;
            int transitionOffset;
            float fadeAmount = 1 - transitionAmount;

            // prepare to draw the menu - are we transitioning off?
            if (subState == SubState.TransitionOff)
            {
                // menu should slide upwards when transitioning off
                transitionOffset = (int)((float)Math.Pow(transitionAmount, 2) * 384.0f);
            }
            else
            {
                // menu should slide downwards when transitioning on
                transitionOffset = (int)((float)Math.Pow(transitionAmount, 2) * 256.0f);
            }

            // only render the background and title if they are required
            if (backgroundVisible)
            {
                Rectangle rectangle = new Rectangle();
                Color color;

                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, displayManager.TransformMatrix);

                // setup ready to draw the menu background
                color = new Color(255, 255, 255, fadeAmount);
                rectangle.X = (int)(shape.X - exteriorBorderSize);
                rectangle.Y = (int)(shape.Y - exteriorBorderSize + transitionOffset);
                rectangle.Width = (int)(shape.Width + (exteriorBorderSize * 2));
                rectangle.Height = (int)(shape.Height + (exteriorBorderSize * 2));

                // draw top left corner
                rectangle.X = (int)shape.X;
                rectangle.Y = (int)(shape.Y + transitionOffset);
                rectangle.Width = (int)topLeftTexture.Width;
                rectangle.Height = (int)topLeftTexture.Height;
                spriteBatch.Draw(topLeftTexture, rectangle, color);

                // draw top right corner
                rectangle.X = (int)(shape.X + shape.Width - topRightTexture.Width);
                rectangle.Width = (int)topRightTexture.Width;
                spriteBatch.Draw(topRightTexture, rectangle, color);

                // draw bottom right corner
                rectangle.Y = (int)(shape.Y + shape.Height - bottomRightTexture.Height + transitionOffset);
                rectangle.Height = (int)bottomRightTexture.Height;
                spriteBatch.Draw(bottomRightTexture, rectangle, color);

                // draw bottom left corner
                rectangle.X = (int)shape.X;
                rectangle.Width = (int)bottomLeftTexture.Width;
                spriteBatch.Draw(bottomLeftTexture, rectangle, color);

                // draw left side
                rectangle.Y = (int)(shape.Y + topLeftTexture.Height + transitionOffset);
                rectangle.Width = (int)leftEdgeTexture.Width;
                rectangle.Height = (int)(shape.Height - (topLeftTexture.Height + bottomLeftTexture.Height));
                spriteBatch.Draw(leftEdgeTexture, rectangle, color);

                // draw right side
                rectangle.X = (int)(shape.X + shape.Width - rightEdgeTexture.Width) ;
                rectangle.Width = (int)rightEdgeTexture.Width;
                spriteBatch.Draw(rightEdgeTexture, rectangle, color);

                // draw top edge
                rectangle.X = (int)(shape.X + topLeftTexture.Width);
                rectangle.Y = (int)(shape.Y + transitionOffset);
                rectangle.Width = (int)(shape.Width - (topLeftTexture.Width + topRightTexture.Width));
                rectangle.Height = (int)topEdgeTexture.Height;
                spriteBatch.Draw(topEdgeTexture, rectangle, color);

                // draw top center
                rectangle.X = (int)(shape.X + leftEdgeTexture.Width);
                rectangle.Y = (int)(shape.Y + topEdgeTexture.Width + transitionOffset);
                rectangle.Width = (int)(shape.Width - (leftEdgeTexture.Width + rightEdgeTexture.Width));
                rectangle.Height = (int)(shape.Height - (topEdgeTexture.Height + bottomEdgeTexture.Height));
                spriteBatch.Draw(centerTexture, rectangle, color);
                
                // draw bottom edge
                rectangle.X = (int)(shape.X + bottomLeftTexture.Width) ;
                rectangle.Y = (int)(shape.Y + shape.Height - bottomEdgeTexture.Height + transitionOffset);
                rectangle.Width = (int)(shape.Width - (bottomLeftTexture.Width + bottomRightTexture.Width));
                rectangle.Height = (int)bottomEdgeTexture.Height;
                spriteBatch.Draw(bottomEdgeTexture, rectangle, color);

                // draw the menu title
                Vector2 titlePosition = new Vector2();
                Vector2 titleSize = titleFont.MeasureString(title);

                switch (alignment)
                {
                    case Alignment.left:
                        titlePosition.X = (int)((shape.X + topLeftTexture.Width) + 4);
                        break;
                    case Alignment.centre:
                        titlePosition.X = (int)(shape.X + ((shape.Width - titleSize.X) / 2));
                        break;
                    case Alignment.right:
                        titlePosition.X = (int)(shape.X + shape.Width - titleSize.X - topRightTexture.Width);
                        break;
                    default:
                        titlePosition.X = 0;
                        break;
                }

                fontColor.A = (byte)(fadeAmount * 255);
                titlePosition.Y = (int)(shape.Y + titleOffset + transitionOffset);
                spriteBatch.DrawString(titleFont, title, titlePosition, fontColor);

                // finsished drawing the menu background and title
                spriteBatch.End();
            }

            // set the position and size of the item area
            SetItemArea(0, transitionOffset);

            // start drawing the menu items
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, displayManager.TransformMatrix);

            // draw each menu item in turn
            for (int item = 0; item < menuItems.Count; item ++)
            {
                // figure out if this item is selected
                bool isSelected = IsActive && (item == selectedItem);

                // draw the item
                menuItems[item].Draw(spriteBatch, isSelected);
            }
           
            // finished drawing the menu items
            spriteBatch.End();
        }
    }
}
