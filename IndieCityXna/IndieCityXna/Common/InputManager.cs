//------------------------------------------------------------------------------
// Filename: InputManager.cs
// Author: Neil Holmes & Andrew Green
// Summary: input manager - handles 1 to 4 players and supports keyboard, mouse
//          and joypads on PC and Xbox360
//------------------------------------------------------------------------------

//TODO: add support to check for unplugged pads and display replace pad screen etc 

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace IndieCityXna.Common
{
    // public enumeration for the number of players to be supported by the game
    public enum NumPlayers
    {
        one = 1,
        two,
        three,
        four,
    };
    
    //------------------------------------------------------------------------------
    // Class: InputManager
    // Author: Neil Holmes & Andrew Green
    // Summary: tracks current and previous states of all input devices and provides
    //          simple query methods for high level input actions
    //------------------------------------------------------------------------------
    public class InputManager
    {
        // maximum number of player inputs to process
        private int numPlayers;

        // current and previous states of each input
        private readonly KeyboardState[] currentKeyboardStates;
        private readonly GamePadState[] currentGamePadStates;
        private readonly KeyboardState[] previousKeyboardStates;
        private readonly GamePadState[] previousGamePadStates;
        private MouseState currentMouseState;
        private MouseState previousMouseState;

        // stores which player was responsible for the last input checked so user can retrieve it if required
        PlayerIndex playerIndex = PlayerIndex.One;

        // tracks if each pad was ever connected so we know when to check for disconnection
        private readonly bool[] gamePadWasConnected;

        //------------------------------------------------------------------------------
        // Constructor: InputManager
        // Author: nholmes
        // Summary: constructs a new input manager - specify the number of players you 
        //          wish to support
        //------------------------------------------------------------------------------
        public InputManager(NumPlayers numPlayers)
        {
            // store the requested number of inputs
            this.numPlayers = (int)numPlayers;

            // create arrays to hold current input states for each player
            currentKeyboardStates = new KeyboardState[this.numPlayers];
            currentGamePadStates = new GamePadState[this.numPlayers];

            // create arrays to hold previos input states for each player
            previousKeyboardStates = new KeyboardState[this.numPlayers];
            previousGamePadStates = new GamePadState[this.numPlayers];

            // crete an array to hold pad connection history
            gamePadWasConnected = new bool[this.numPlayers];
        }

        //------------------------------------------------------------------------------
        // Method: Update
        // Author: Neil Holmes & Andrew Green
        // Summary: updates the status of the input devices for all requested players
        //          should be called at the start of each frame of processing
        //------------------------------------------------------------------------------
        public void Update()
        {
            // process the update for all players we care about
            for (int player = 0; player < numPlayers; player++)
            {
                // copy current input statse into previous states ready for updating
                previousKeyboardStates[player] = currentKeyboardStates[player];
                previousGamePadStates[player] = currentGamePadStates[player];
                previousMouseState = currentMouseState;

                // update the all input states
                currentKeyboardStates[player] = Keyboard.GetState((PlayerIndex)player);
                currentGamePadStates[player] = GamePad.GetState((PlayerIndex)player);
                currentMouseState = Mouse.GetState();

                // keep track of whether a gamepad has ever been connected for this player, so we can detect if it is unplugged
                if (currentGamePadStates[player].IsConnected)
                {
                    gamePadWasConnected[player] = true;
                }
            }
        }

        //------------------------------------------------------------------------------
        // Method: WasKeyPressed
        // Author: Neil Holmes & Andrew Green
        // Summary: checks if a key was newly pressed during this update. if controllingPlayer
        //          is null it will accept input from any player. 
        //------------------------------------------------------------------------------
        public bool WasKeyPressed(Keys key, PlayerIndex? controllingPlayer)
        {
            // check if the user specified a controlling player
            if (controllingPlayer.HasValue)
            {
                // return input state read from the specified controlling player
                return (currentKeyboardStates[(int)controllingPlayer].IsKeyDown(key) && previousKeyboardStates[(int)controllingPlayer].IsKeyUp(key));
            }
            else
            {
                // no controlling player specified, accept input from any valid player
                bool retVal = WasKeyPressed(key, PlayerIndex.One);
                if (numPlayers > 1) retVal |= WasKeyPressed(key, PlayerIndex.One);
                if (numPlayers > 2) retVal |= WasKeyPressed(key, PlayerIndex.One);
                if (numPlayers > 3) retVal |= WasKeyPressed(key, PlayerIndex.One);
                return retVal;
            }
        }

        //------------------------------------------------------------------------------
        // Method: IsKeyPressed
        // Author: Neil Holmes & Andrew Green
        // Summary: checks if a key is currently pressed. if controllingPlayer is null it
        //          will accept input from any player.
        //------------------------------------------------------------------------------
        public bool IsKeyPressed(Keys key, PlayerIndex? controllingPlayer)
        {
            // check if the user specified a controlling player
            if (controllingPlayer.HasValue)
            {
                // yup, set the player index to match the specified controlling player
                playerIndex = controllingPlayer.Value;

                // return input state read from the specified controlling player
                return (currentKeyboardStates[(int)playerIndex].IsKeyDown(key));
            }
            else
            {
                // no controlling player specified, accept input from any valid player
                bool retVal = IsKeyPressed(key, PlayerIndex.One);
                if (numPlayers > 1) retVal |= IsKeyPressed(key, PlayerIndex.One);
                if (numPlayers > 2) retVal |= IsKeyPressed(key, PlayerIndex.One);
                if (numPlayers > 3) retVal |= IsKeyPressed(key, PlayerIndex.One);
                return retVal;
            }
        }

        //------------------------------------------------------------------------------
        // Method: IsKeyHeld
        // Author: Neil Holmes & Andrew Green
        // Summary: checks if a key is being held down. if controllingPlayer is null it
        //          will accept input from any player. 
        //------------------------------------------------------------------------------
        public bool IsKeyHeld(Keys key, PlayerIndex? controllingPlayer)
        {
            // check if the user specified a controlling player
            if (controllingPlayer.HasValue)
            {
                // yup, set the player index to match the specified controlling player
                playerIndex = controllingPlayer.Value;

                // return input state read from the specified controlling player
                return (currentKeyboardStates[(int)playerIndex].IsKeyDown(key) && previousKeyboardStates[(int)playerIndex].IsKeyDown(key));
            }
            else
            {
                // no controlling player specified, accept input from any valid player
                bool retVal = IsKeyHeld(key, PlayerIndex.One);
                if (numPlayers > 1) retVal |= IsKeyHeld(key, PlayerIndex.One);
                if (numPlayers > 2) retVal |= IsKeyHeld(key, PlayerIndex.One);
                if (numPlayers > 3) retVal |= IsKeyHeld(key, PlayerIndex.One);
                return retVal;
            }
        }

        //------------------------------------------------------------------------------
        // Method: WasKeyReleased
        // Author: Neil Holmes & Andrew Green
        // Summary: checks if a key has just been released. if controllingPlayer is null it will
        //          accept input from any player.
        //------------------------------------------------------------------------------
        public bool WasKeyReleased(Keys key, PlayerIndex? controllingPlayer)
        {
            // check if the user specified a controlling player
            if (controllingPlayer.HasValue)
            {
                // yup, set the player inedex to match the specified controlling player
                playerIndex = controllingPlayer.Value;

                // return input state read from the specified controlling player
                return (currentKeyboardStates[(int)playerIndex].IsKeyUp(key) && previousKeyboardStates[(int)playerIndex].IsKeyDown(key));
            }
            else
            {
                // no controlling player specified, accept input from any valid player
                bool retVal = WasKeyReleased(key, PlayerIndex.One);
                if (numPlayers > 1) retVal |= WasKeyReleased(key, PlayerIndex.One);
                if (numPlayers > 2) retVal |= WasKeyReleased(key, PlayerIndex.One);
                if (numPlayers > 3) retVal |= WasKeyReleased(key, PlayerIndex.One);
                return retVal;
            }
        }

        //------------------------------------------------------------------------------
        // Method: WasButtonPressed
        // Author: Neil Holmes & Andrew Green
        // Summary: checks if a pad button was newly pressed during this update. if 
        //          controllingPlayer is null it will accept input from any player.
        //------------------------------------------------------------------------------
        public bool WasButtonPressed(Buttons button, PlayerIndex? controllingPlayer)
        {
            // check if the user specified a controlling player
            if (controllingPlayer.HasValue)
            {
                // yup, set the player index to match the specified controlling player
                playerIndex = controllingPlayer.Value;

                // return input state read from the specified controlling player
                return (currentGamePadStates[(int)playerIndex].IsButtonDown(button) && previousGamePadStates[(int)playerIndex].IsButtonUp(button));
            }
            else
            {
                // no controlling player specified, accept input from any valid player
                bool retVal = WasButtonPressed(button, PlayerIndex.One);
                if (numPlayers > 1) retVal |= WasButtonPressed(button, PlayerIndex.One);
                if (numPlayers > 2) retVal |= WasButtonPressed(button, PlayerIndex.One);
                if (numPlayers > 3) retVal |= WasButtonPressed(button, PlayerIndex.One);
                return retVal;
            }
        }

        //------------------------------------------------------------------------------
        // Method: IsButtonPressed
        // Author: Neil Holmes & Andrew Green
        // Summary: checks if a pad button is currently pressed. if controllingPlayer is 
        //          null it will accept input from any player.
        //------------------------------------------------------------------------------
        public bool IsButtonPressed(Buttons button, PlayerIndex? controllingPlayer)
        {
            // check if the user specified a controlling player
            if (controllingPlayer.HasValue)
            {
                // yup, set the player index to match the specified controlling player
                playerIndex = controllingPlayer.Value;

                // return input state read from the specified controlling player
                return (currentGamePadStates[(int)playerIndex].IsButtonDown(button));
            }
            else
            {
                // no controlling player specified, accept input from any valid player
                bool retVal = IsButtonPressed(button, PlayerIndex.One);
                if (numPlayers > 1) retVal |= IsButtonPressed(button, PlayerIndex.One);
                if (numPlayers > 2) retVal |= IsButtonPressed(button, PlayerIndex.One);
                if (numPlayers > 3) retVal |= IsButtonPressed(button, PlayerIndex.One);
                return retVal;
            }
        }

        //------------------------------------------------------------------------------
        // Method: IsButtonHeld
        // Author: Neil Holmes & Andrew Green
        // Summary: checks if a pad button is being held down. if controllingPlayer is 
        //          null it will accept input from any player.
        //------------------------------------------------------------------------------
        public bool IsButtonHeld(Buttons button, PlayerIndex? controllingPlayer)
        {
            // check if the user specified a controlling player
            if (controllingPlayer.HasValue)
            {
                // yup, set the player inedex to match the specified controlling player
                playerIndex = controllingPlayer.Value;

                // return input state read from the specified controlling player
                return (currentGamePadStates[(int)playerIndex].IsButtonDown(button) && previousGamePadStates[(int)playerIndex].IsButtonDown(button));
            }
            else
            {
                // no controlling player specified, accept input from any valid player
                bool retVal = IsButtonHeld(button, PlayerIndex.One);
                if (numPlayers > 1) retVal |= IsButtonHeld(button, PlayerIndex.One);
                if (numPlayers > 2) retVal |= IsButtonHeld(button, PlayerIndex.One);
                if (numPlayers > 3) retVal |= IsButtonHeld(button, PlayerIndex.One);
                return retVal;
            }
        }
 
        //------------------------------------------------------------------------------
        // Method: WasButtonReleased
        // Author: Neil Holmes & Andrew Green
        // Summary: checks if a button was just released. if controllingPlayer is null it will
        //          accept input from any player.
        //------------------------------------------------------------------------------
        public bool WasButtonReleased(Buttons button, PlayerIndex? controllingPlayer)
        {
            // check if the user specified a controlling player
            if (controllingPlayer.HasValue)
            {
                // yup, set the out player inedex to match the specified controlling player
                playerIndex = controllingPlayer.Value;

                // return input state read from the specified controlling player
                return (currentGamePadStates[(int)playerIndex].IsButtonUp(button) && previousGamePadStates[(int)playerIndex].IsButtonDown(button));
            }
            else
            {
                // no controlling player specified, accept input from any valid player
                bool retVal = WasButtonReleased(button, PlayerIndex.One);
                if (numPlayers > 1) retVal |= WasButtonReleased(button, PlayerIndex.One);
                if (numPlayers > 2) retVal |= WasButtonReleased(button, PlayerIndex.One);
                if (numPlayers > 3) retVal |= WasButtonReleased(button, PlayerIndex.One);
                return retVal;
            }
        }
        
        //------------------------------------------------------------------------------
        // Method: GetLeftStickPosition
        // Author: Neil Holmes & Andrew Green
        // Summary: outputs the X and Y position of the left stick. if controllingPlayer
        //          is null it will return 0 for both axis.
        //------------------------------------------------------------------------------
        public void GetLeftStickPosition(PlayerIndex? controllingPlayer, out float X, out float Y)
        {
            // check if the user specified a controlling player
            if (controllingPlayer.HasValue)
            {
                // yup, set the out player inedex to match the specified controlling player
                playerIndex = controllingPlayer.Value;

                // report the left stick axis positions from the specified controlling player
                X = currentGamePadStates[(int)playerIndex].ThumbSticks.Left.X;
                Y = currentGamePadStates[(int)playerIndex].ThumbSticks.Left.Y;
            }
            else
            {
                // no controlling player specified, report 0 for boh axis
                X = 0.0f;
                Y = 0.0f;
            }
        }

        //------------------------------------------------------------------------------
        // Method: GetRightStickPosition
        // Author: Neil Holmes & Andrew Green
        // Summary: outputs the X and Y position of the right stick. if controllingPlayer
        //          is null it will return 0 for both axis.
        //------------------------------------------------------------------------------
        public void GetRightStickPosition(PlayerIndex? controllingPlayer, out float X, out float Y)
        {
            // check if the user specified a controlling player
            if (controllingPlayer.HasValue)
            {
                // yup, set the out player inedex to match the specified controlling player
                playerIndex = controllingPlayer.Value;

                // report the right stick axis positions from the specified controlling player
                X = currentGamePadStates[(int)playerIndex].ThumbSticks.Right.X;
                Y = currentGamePadStates[(int)playerIndex].ThumbSticks.Right.Y;
            }
            else
            {
                // no controlling player specified, report 0 for boh axis
                X = 0.0f;
                Y = 0.0f;
            }
        }

        //------------------------------------------------------------------------------
        // Method: GetLeftTriggerPosition
        // Author: Neil Holmes & Andrew Green
        // Summary: outputs the position of the left trigger. if controllingPlayer
        //          is null it will return 0.
        //------------------------------------------------------------------------------
        public void GetLeftTriggerPosition(PlayerIndex? controllingPlayer, out float position)
        {
            // check if the user specified a controlling player
            if (controllingPlayer.HasValue)
            {
                // yup, set the out player inedex to match the specified controlling player
                playerIndex = controllingPlayer.Value;

                // report the left trigger position from the specified controlling player
                position = currentGamePadStates[(int)playerIndex].Triggers.Left;
            }
            else
            {
                // no controlling player specified, report 0 for the position
                position = 0.0f;
            }
        }

        //------------------------------------------------------------------------------
        // Method: GetRightTriggerPosition
        // Author: Neil Holmes & Andrew Green
        // Summary: outputs the position of the right trigger. if controllingPlayer
        //          is null it will return 0.
        //------------------------------------------------------------------------------
        public void GetRightTriggerPosition(PlayerIndex? controllingPlayer, out float position)
        {
            // check if the user specified a controlling player
            if (controllingPlayer.HasValue)
            {
                // yup, set the out player inedex to match the specified controlling player
                playerIndex = controllingPlayer.Value;

                // report the right trigger position from the specified controlling player
                position = currentGamePadStates[(int)playerIndex].Triggers.Right;
            }
            else
            {
                // no controlling player specified, report 0 for the position
                position = 0.0f;
            }
        }

        //------------------------------------------------------------------------------
        // Method: IsNewLeftMouseButtonPressed
        // Author: Neil Holmes & Andrew Green
        // Summary: checks if the left mouse button was newly pressed during this update
        //------------------------------------------------------------------------------
        public bool IsNewLeftMouseButtonPressed()
        {
            // there is only ever one mouse, so the controlling player is always player one
            playerIndex = PlayerIndex.One;

            // check current and previous state and act accordingly
            if ((currentMouseState.LeftButton == ButtonState.Pressed) &&
               (previousMouseState.LeftButton == ButtonState.Released))
                return true;
            else
                return false;
        }

        //------------------------------------------------------------------------------
        // Method: IsLeftMouseButtonPressed
        // Author: Neil Holmes & Andrew Green
        // Summary: checks if the left mouse button is currently pressed
        //------------------------------------------------------------------------------
        public bool IsLeftMouseButtonPressed()
        {
            // there is only ever one mouse, so the controlling player is always player one
            playerIndex = PlayerIndex.One;

            // true if the button is currently pressed
            if (currentMouseState.LeftButton == ButtonState.Pressed)
                return true;
            else
                return false;
        }

        //------------------------------------------------------------------------------
        // Method: WasLeftMouseButtonReleased
        // Author: Neil Holmes & Andrew Green
        // Summary: checks if the left mouse button is currently pressed
        //------------------------------------------------------------------------------
        public bool WasLeftMouseButtonReleased()
        {
            // there is only ever one mouse, so the controlling player is always player one
            playerIndex = PlayerIndex.One;

            // check current and previous state and act accordingly
            if ((currentMouseState.LeftButton == ButtonState.Released) &&
                (previousMouseState.LeftButton == ButtonState.Pressed))
                return true;
            else
                return false;
        }

        //------------------------------------------------------------------------------
        // Method: IsNewRightMouseButtonPressed
        // Author: Neil Holmes & Andrew Green
        // Summary: checks if the right mouse button was newly pressed during this update
        //------------------------------------------------------------------------------
        public bool IsNewRightMouseButtonPressed()
        {
            // there is only ever one mouse, so the controlling player is always player one
            playerIndex = PlayerIndex.One;

            // check current and previous states and act accordingly
            if ((currentMouseState.RightButton == ButtonState.Pressed) &&
               (previousMouseState.RightButton == ButtonState.Released))
                return true;
            else
                return false;
        }

        //------------------------------------------------------------------------------
        // Method: IsRightMouseButtonPressed
        // Author: Neil Holmes & Andrew Green
        // Summary: checks if the right mouse button is currently pressed
        //------------------------------------------------------------------------------
        public bool IsRightMouseButtonPressed()
        {
            // there is only ever one mouse, so the controlling player is always player one
            playerIndex = PlayerIndex.One;

            // true if the button is currently pressed
            if (currentMouseState.RightButton == ButtonState.Pressed)
                return true;
            else
                return false;
        }

        //------------------------------------------------------------------------------
        // Method: WasRightMouseButtonReleased
        // Author: Neil Holmes & Andrew Green
        // Summary: checks if the right mouse button was just released
        //------------------------------------------------------------------------------
        public bool WasRightMouseButtonReleased()
        {
            // there is only ever one mouse, so the controlling player is always player one
            playerIndex = PlayerIndex.One;

            // check current and previous states and act accordingly
            if ((currentMouseState.RightButton == ButtonState.Released) &&
                (previousMouseState.RightButton == ButtonState.Pressed))
                return true;
            else
                return false;
        }

        //------------------------------------------------------------------------------
        // Method: WasMiddleMouseButtonPressed
        // Author: Neil Holmes & Andrew Green
        // Summary: checks if the middle mouse button was newly pressed during this update
        //------------------------------------------------------------------------------
        public bool WasMiddleMouseButtonPressed()
        {
            // there is only ever one mouse, so the controlling player is always player one
            playerIndex = PlayerIndex.One;

            // check current and previous states and act accordingly
            if ((currentMouseState.MiddleButton == ButtonState.Pressed) &&
               (previousMouseState.MiddleButton == ButtonState.Released))
                return true;
            else
                return false;
        }

        //------------------------------------------------------------------------------
        // Method: IsMiddleMouseButtonPressed
        // Author: Neil Holmes & Andrew Green
        // Summary: checks if the middle mouse button is currently pressed
        //------------------------------------------------------------------------------
        public bool IsMiddleMouseButtonPressed()
        {
            // there is only ever one mouse, so the controlling player is always player one
            playerIndex = PlayerIndex.One;

            // true if the button is currently pressed
            if (currentMouseState.MiddleButton == ButtonState.Pressed)
                return true;
            else
                return false;
        }

        //------------------------------------------------------------------------------
        // Method: WasMiddleMouseButtonReleased
        // Author: Neil Holmes & Andrew Green
        // Summary: checks if the middle mouse button was just released
        //------------------------------------------------------------------------------
        public bool WasMiddleMouseButtonReleased()
        {
            // there is only ever one mouse, so the controlling player is always player one
            playerIndex = PlayerIndex.One;

            // check current and previous states and act accordingly
            if ((currentMouseState.MiddleButton == ButtonState.Released) &&
                (previousMouseState.MiddleButton == ButtonState.Pressed))
                return true;
            else
                return false;
        }

        //------------------------------------------------------------------------------
        // Method: HasMouseMoved
        // Author: Neil Holmes & Andrew Green
        // Summary: returns true if the mouse has moved 
        //------------------------------------------------------------------------------
        public bool HasMouseMoved()
        {
            // check if mouse x or y is different
            if ((currentMouseState.X != previousMouseState.X) || (currentMouseState.Y != previousMouseState.Y))
            {
                // mouse has moved
                return true;
            }

            // mouse hasn't moved
            return false;
        }


        //------------------------------------------------------------------------------
        // Method: GetMousePosition
        // Author: Neil Holmes & Andrew Green
        // Summary: copies the current mouse X and Y into the supplied variables
        //------------------------------------------------------------------------------
        public void GetMousePosition(out float X, out float Y)
        {
            // get the current mouse position and set X and Y
            X = currentMouseState.X;
            Y = currentMouseState.Y;
        }
        
        //------------------------------------------------------------------------------
        // Method: GetMouseWheelPosition
        // Author: Neil Holmes & Andrew Green
        // Summary: copies the current mouse wheel position in the supplied variable
        //------------------------------------------------------------------------------
        public void GetMouseWheelPosition(out float Z)
        {
            // gets the cumulative mouse scroll wheel value since the app started
            Z = currentMouseState.ScrollWheelValue;
        }

        //------------------------------------------------------------------------------
        // Method: GetPlayerResponsible
        // Author: Neil Holmes & Andrew Green
        // Summary: returns the index of the player responsible for the result of the 
        //          last input function that was called.
        //------------------------------------------------------------------------------
        public PlayerIndex GetPlayerResponsible()
        {
            // return the stored player index
            return playerIndex;
        }

        //------------------------------------------------------------------------------
        // Method: IsPauseGame
        // Author: Neil Holmes & Andrew Green
        // Summary: checks if one of the game pause keys or buttons was just pressed. if 
        //          controllingPlayer is null it will accept input from any player. 
        //------------------------------------------------------------------------------
        public bool IsPauseGame(PlayerIndex? controllingPlayer)
        {
            return WasKeyPressed(Keys.Escape, controllingPlayer) || WasButtonPressed(Buttons.Start, controllingPlayer);
        }
    }
}