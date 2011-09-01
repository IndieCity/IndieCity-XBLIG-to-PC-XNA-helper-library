//------------------------------------------------------------------------------
// Filename: MenuItem.cs
// Author: nholmes
// Summary: helper class for items that can be added to a menu
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
using Microsoft.Xna.Framework.Graphics;
using IndieCityXna.Common;

namespace BlankShell.Frontend
{
    // public enumeration used for specifying the type of mouse selection that has been mde
    public enum MenuItemSelectionType
    {
        none = 0,
        selected,
        increaseValue,
        decreaseValue,
        nextOption,
        previousOption,
    };

    //------------------------------------------------------------------------------
    // Class: MenuItem
    // Author: nholmes
    // Summary: helper class represents a blank item template with minimal functionality. 
    //          items provide an event that will be raised when the menu item is selected.
    //          can be overriden to provide many different types of menu items
    //------------------------------------------------------------------------------
    public class MenuItem
    {
        // the menu that owns this item
        protected Menu menu;

        // the game system that the menu owning this item is part of
        protected Game game;

        // handle to the display manager service
        protected DisplayManager displayManager;

        // handle to the timer system service
        protected TimerSystem timerSystem;

        // whether the menu item is selectable or not
        protected bool selectable = true;

        // tracks the fading selection effect on the item
        protected float selectionFade;

        // stores the position and size of the menu item
        protected Vector2 position;
        protected Vector2 size;

        // offset for the y position of the text within menu items
        protected int textOffset;

        // event raised when the item is selected
        public event EventHandler<MenuItemEventArgs> Selected;

        // event raised when the item's next option is wanted
        public event EventHandler<MenuItemEventArgs> NextOptionWanted;

        // event raised when the item's previous option is wanted
        public event EventHandler<MenuItemEventArgs> PreviousOptionWanted;

        // event raised when the item's value should be increased
        public event EventHandler<MenuItemEventArgs> ValueIncreased;

        // event raised when the item's value should be decreased
        public event EventHandler<MenuItemEventArgs> ValueDecreased;

        //------------------------------------------------------------------------------
        // Function: MenuItem
        // Author: nholmes
        // Summary: item constructor, stores the menu the item belongs to
        //------------------------------------------------------------------------------
        public MenuItem(Menu menu, Game game, int textOffset)
        {
            // store the menu that owns this item
            this.menu = menu;

            // store the game system reference
            this.game = game;

            // store the text offset
            this.textOffset = textOffset;

            // get the display manager service
            displayManager = (DisplayManager)game.Services.GetService(typeof(DisplayManager));

            // get the timer system service
            timerSystem = (TimerSystem)game.Services.GetService(typeof(TimerSystem));
        }

        //------------------------------------------------------------------------------
        // Function: OnSelectEntry
        // Author: nholmes
        // Summary: method for raising the selection event 
        //------------------------------------------------------------------------------
        protected internal virtual void OnSelectEntry(PlayerIndex playerIndex)
        {
            if (Selected != null)
            {
                Selected(this, new MenuItemEventArgs(playerIndex));
            }
        }

        //------------------------------------------------------------------------------
        // Function: OnNextOptionEntry
        // Author: nholmes
        // Summary: method for raising the next option event 
        //------------------------------------------------------------------------------
        protected internal virtual void OnNextOptionEntry(PlayerIndex playerIndex)
        {
            if (NextOptionWanted!= null)
            {
                NextOptionWanted(this, new MenuItemEventArgs(playerIndex));
            }
        }

        //------------------------------------------------------------------------------
        // Function: OnPreviousOptionEntry
        // Author: nholmes
        // Summary: method for raising the previous option event 
        //------------------------------------------------------------------------------
        protected internal virtual void OnPreviousOptionEntry(PlayerIndex playerIndex)
        {
            if (PreviousOptionWanted != null)
            {
                PreviousOptionWanted(this, new MenuItemEventArgs(playerIndex));
            }
        }

        //------------------------------------------------------------------------------
        // Function: OnItemIncreaseValueEntry
        // Author: nholmes
        // Summary: method for raising the increase value event 
        //------------------------------------------------------------------------------
        protected internal virtual void OnItemIncreaseValueEntry(PlayerIndex playerIndex)
        {
            if (ValueIncreased != null)
            {
                ValueIncreased(this, new MenuItemEventArgs(playerIndex));
            }
        }

        //------------------------------------------------------------------------------
        // Function: OnItemDecreaseValueEntry
        // Author: nholmes
        // Summary: method for raising the decrease value event 
        //------------------------------------------------------------------------------
        protected internal virtual void OnItemDecreaseValueEntry(PlayerIndex playerIndex)
        {
            if (ValueDecreased != null)
            {
                ValueDecreased(this, new MenuItemEventArgs(playerIndex));
            }
        }
        
        //------------------------------------------------------------------------------
        // Function: OnPointerSelection
        // Author: nholmes
        // Summary: provides basic checking to see if the pointer position supplied is 
        //          within the area that the menu item is being displayed and respond 
        //          with the action that should be taken
        //------------------------------------------------------------------------------
        public virtual MenuItemSelectionType OnPointerSelection(Vector2 point)
        {
            // is the supplied point within the shape defined by the items position and size?
            if ((point.X > (menu.ItemArea.X + position.X)) && (point.X < (menu.ItemArea.X + position.X + size.X)) &&
                (point.Y > (menu.ItemArea.Y + position.Y)) && (point.Y < (menu.ItemArea.Y + position.Y + size.Y)))
            {
                return MenuItemSelectionType.selected;
            }

            // not selected - return failure
            return MenuItemSelectionType.none;
        }

        //------------------------------------------------------------------------------
        // Function: Update
        // Author: nholmes
        // Summary: updates the menu item. when the menu selection changes, entries 
        //          gradually fade between their selected and deselected appearance
        //------------------------------------------------------------------------------
        public virtual void Update(bool isSelected)
        {
            // is this item selected?
            if (isSelected)
            {
                // yup, increase the selection fade
                selectionFade += timerSystem.TimeStep;

                // clamp to one
                if (selectionFade > 1.0f) selectionFade = 1.0f;
            }
            else
            {
                // nope, decrease the selection fade
                selectionFade -= timerSystem.TimeStep;

                // clamp to zero
                if (selectionFade < 0.0f) selectionFade = 0.0f;
            }
        }

                //------------------------------------------------------------------------------
        // Function: Draw
        // Author: nholmes
        // Summary: empty draw functionality, must be overriden by the inheriting class
        //------------------------------------------------------------------------------
        public virtual void Draw(SpriteBatch spriteBatch, bool isSelected)
        {
        }

        //------------------------------------------------------------------------------
        // Property: Height
        // Author: nholmes
        // Summary: accessor function - gets the height of the menu item
        //------------------------------------------------------------------------------
        public int Height
        {
            get { return (int)size.Y; }
        }

        //------------------------------------------------------------------------------
        // Property: Selectable
        // Author: nholmes
        // Summary: accessor function - returns whether the item is selectable or not
        //------------------------------------------------------------------------------
        public bool Selectable
        {
            get { return selectable; }
        }

    }
}
