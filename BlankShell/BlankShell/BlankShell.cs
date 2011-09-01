//------------------------------------------------------------------------------
// Filename: BlankShell.cs
// Author: Neil Holmes & Andrew Green
// Summary: example game setup file - contains game constructor and entry point
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
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using IndieCityXna.Common;
using IndieCityXna.GameState;
using IndieCityXna.SaveGame;
using BlankShell.Frontend;
using ICMessages;
using ICAchievements;
using ICLeaderboards;
using System.Windows.Forms;

namespace BlankShell
{
    //------------------------------------------------------------------------------
    // Class: Program
    // Author: Neil Holmes & Andrew Green
    // Summary: when the application first runs, this is the entry point :)
    //------------------------------------------------------------------------------
    static class Program
    {
        //------------------------------------------------------------------------------
        // Constructor: Program
        // Author: Neil Holmes & Andrew Green
        // Summary: main constructor and entry point for the game
        //------------------------------------------------------------------------------
        static void Main()
        {
            // create the game state manager and cause it to run
            using (BlankShell game = new BlankShell())
            {
                game.Run();
            }
        }
    }

    //------------------------------------------------------------------------------
    // Class: BlankShell
    // Author: Neil Holmes & Andrew Green
    // Summary: example game class, handles creating and updating an indie city xna 
    //          based game that uses all of the component classes we currently supply
    //------------------------------------------------------------------------------
    public class BlankShell : Microsoft.Xna.Framework.Game
    {
        // *** required components ***

        // status enum - used to control the game update
        private enum GameStatus
        {
            initialising = 0,
            running,
            exiting,
        };

        // main handle to the graphics device
        private GraphicsDeviceManager graphicsDeviceManager;

        // handle to the display manager
        private DisplayManager displayManager;

        // *** optional components ***

        // handle to the game state manager
        private GameStateManager gameStateManager;

        // handle to the timer system
        private TimerSystem timerSystem;

        // handle to the input manager
        private InputManager inputManager;

        // handle to the save game manager
        private SaveGameManager saveGameManager;
        
        // handle to the mouse pointer display manager
        private Pointer mousePointer;

        // the indiecity session
        private ICECoreLib.CoGameSession indieCitySession = null;
        private ICELandaLib.CoAchievementManager indieCityAchievementManager = null;
        private ICELandaLib.CoAchievementGroup indieCityAchievementGroup = null;
        private ICELandaLib.CoUserAchievementList indieCityUserList = null;
        private uint m_cookie = 0;

        // IC achievement example variables
        private ICAchievementPopUp achievementPopUp;
        private ICAchievementList achievementList;

        // IC leaderboard example variables
        private ICLeaderboardsBrowser leaderboardBrowser;

        // IC message example variables
        private ICMessagePopUp messagePopUp;
        
        // current status of the game - either initialising, running or exiting
        private GameStatus gameStatus = GameStatus.initialising;

        //------------------------------------------------------------------------------
        // Constructor: BlankShell
        // Author: Neil Holmes & Andrew Green
        // Summary: main game constructor
        //------------------------------------------------------------------------------
        public BlankShell()
        {
            // set the root content directory 
            Content.RootDirectory = @"Content\";

            // create the graphics device manager
            graphicsDeviceManager = new GraphicsDeviceManager(this);
        }

        //------------------------------------------------------------------------------
        // Method: StringToGameId
        // Author: Neil Holmes
        // Summary: helper function to convert game ID string to a real game id value
        //------------------------------------------------------------------------------
        static ICECoreLib.GameId StringToGameId(System.String gameIdString)
        {
            System.Guid gameGuid = new System.Guid(gameIdString);
            byte[] guidBytes = gameGuid.ToByteArray();

            ICECoreLib.GameId gameId = new ICECoreLib.GameId();

            // 4 bytes as u32
            gameId.Data1 = (UInt32)(((uint)(guidBytes[3]) << 24) | ((uint)(guidBytes[2]) << 16) | ((uint)(guidBytes[1]) << 8) | guidBytes[0]);

            // 2 bytes as ushort
            gameId.Data2 = (UInt16)(((uint)(guidBytes[5]) << 8) | ((uint)(guidBytes[4])));

            // 2 bytes as ushort
            gameId.Data3 = (UInt16)(((uint)(guidBytes[7]) << 8) | ((uint)(guidBytes[6])));

            // 8 bytes as byte array
            gameId.Data4 = new Byte[8];
            for (int i = 0; i < 8; ++i)
            {
                gameId.Data4[i] = guidBytes[8 + i];
            }

            return gameId;
        }
 
        //------------------------------------------------------------------------------
        // Method: Initialize
        // Author: Neil Holmes & Andrew Green
        // Summary: overrides base initialize so we can perform our own class 
        //          initialisations called after the XNA devices have been initialised
        //------------------------------------------------------------------------------
        protected override void Initialize()
        {
            // *** IndieCity Setup ***

            // initialise the message pop up display class and register it as a service
            messagePopUp = new ICMessagePopUp(this, graphicsDeviceManager, ICMessagePosition.bottomRight, ICMessageScale.normal);
            base.Services.AddService(typeof(ICMessagePopUp), messagePopUp);
 
            // create bridge
            ICEBridgeLib.CoBridge bridge = new ICEBridgeLib.CoBridge();
            if (bridge != null)
            {
                // setup the game id and game secret
                ICECoreLib.GameId myGameId = StringToGameId("540a5e40-669c-4782-8424-fd688e720573");
                string mySecret = "248f9aa3-624b-4e2d-b286-8d6de3781896";

                // initialise the bridge
                bridge.Initialise(myGameId, mySecret);

                //get some user info
                int userId = bridge.DefaultUserId;
                ICECoreLib.CoUserStore storeClass = bridge.UserStore;
                ICECoreLib.CoUserInfo userClass = storeClass.GetUserFromId(userId);
                string name = userClass.Name;

                //Create a game session
                try
                { 
                    indieCitySession = bridge.CreateDefaultGameSession();
                }
                catch (System.Runtime.InteropServices.COMException ex)
                {
                    // session creation failed - report the error and tell the game to bail out
                    messagePopUp.AddMessage("Session Creation Error", "Valid tokens for " + name +" could not be found.\nThis means a IndieCity session could not be created.", ICMessagePriority.urgent);
                    gameStatus = GameStatus.exiting;
                }

                // only do the following if the session was succesfully created
                if (indieCitySession != null)
                {
                    indieCitySession.RequestStartSession();
                    
                    // create the achievement manager and initialise it
                    indieCityAchievementManager = new ICELandaLib.CoAchievementManager();
                    indieCityAchievementManager.SetGameSession(indieCitySession);
                    indieCityAchievementManager.InitialiseAchievements(null);

                    // retrieve the achievement group from the achievement manager
                    indieCityAchievementGroup = indieCityAchievementManager.AchievementGroup;

                    // initialise the achievement list and pop up display classes and register them as services
                    achievementPopUp = new ICAchievementPopUp(this, graphicsDeviceManager, ICAchievementPosition.topRight, ICAchievementScale.normal);
                    achievementList = new ICAchievementList(this, graphicsDeviceManager, indieCityAchievementGroup, ICAchievementScale.normal);
                    base.Services.AddService(typeof(ICAchievementPopUp), achievementPopUp);
                    base.Services.AddService(typeof(ICAchievementList), achievementList);

                    // register the pop up achievement class with the achievement manager to handle events
                    ICELandaLib.IAchievementService iService = (ICELandaLib.IAchievementService)indieCityAchievementManager;
                    m_cookie = iService.RegisterAchievementEventHandler(achievementPopUp);

                    // initlaise the leaderboard browser class and register it as a service
                    leaderboardBrowser = new ICLeaderboardsBrowser(this, graphicsDeviceManager, ICLeaderboardScale.normal);
                    base.Services.AddService(typeof(ICLeaderboardsBrowser), leaderboardBrowser);

                    // get the current user's list of unlocked achievements
                    indieCityUserList = indieCityAchievementManager.GetUserAchievementList(indieCitySession.UserId);

                    // indiecity initialisation complete - set status to running
                   // gameStatus = GameStatus.running;
                }
            }

            // *** optional initialisations ***
           
            // create the display manager and register it as a service
            displayManager = new DisplayManager(this, Window, graphicsDeviceManager, 1280, 720, ScreenMode.Windowed, 1);
            base.Services.AddService(typeof(DisplayManager), displayManager);

            // create the timer system and add it as a service
            timerSystem = new TimerSystem(this);
            base.Services.AddService(typeof(TimerSystem), timerSystem);

            // create the input manager for a single player game and add it as a service
            inputManager = new InputManager(NumPlayers.one);
            base.Services.AddService(typeof(InputManager), inputManager);

            // create the save game manager and add it as a service
            saveGameManager = new SaveGameManager(this);
            base.Services.AddService(typeof(SaveGameManager), saveGameManager);

            // initialise the mouse pointer system and add it as a service
            mousePointer = new Pointer(this, PointerType.mouse, true);
            base.Services.AddService(typeof(Pointer), mousePointer);
 
            // create the game state manager and register it as a service
            gameStateManager = new GameStateManager(this);
            base.Services.AddService(typeof(GameStateManager), gameStateManager);

            // activate the initial example game states - in this example we are adding two :)
            gameStateManager.AddGameState(new Background(this, null));
            gameStateManager.AddGameState(new TitleScreen(this, null));

            // propogate initialise call to base class
            base.Initialize();
        }
                 
        //------------------------------------------------------------------------------
        // Method: Update
        // Author: Neil Holmes & Andrew Green
        // Summary: overrides base uUpdate so we can perform our own per-frame processing
        //------------------------------------------------------------------------------
        protected override void Update(GameTime gameTime)
        {
            // *** required updates for indiecity functionality ***
            switch (gameStatus)
            {
                case GameStatus.initialising:
                
                    // do nothing...
                    if (indieCitySession != null)
                    {
                        indieCitySession.UpdateSession();

                        // update the indiecity achievement manager
                        indieCityAchievementManager.Update();

                        // check that the session is still valid and bail out if not
                        if (indieCitySession.IsSessionStarted())
                        {
                            // session has ended - exit the game!
                            gameStatus = GameStatus.running;
                        }
                    }
                    break;

                case GameStatus.running:

                    // update the indiecity session
                    if (indieCitySession != null)
                    {
                        indieCitySession.UpdateSession();

                        // update the indiecity achievement manager
                        indieCityAchievementManager.Update();

                        // check that the session is still valid and bail out if not
                        if (!indieCitySession.IsSessionStarted())
                        {
                            // session has ended - exit the game!
                            gameStatus = GameStatus.exiting;
                        }
                    }
                    break;

                case GameStatus.exiting:

                    // check that there are messages left to display and then bail out!
                    if (messagePopUp.CheckForPendingMessages() == false) 
                        Exit();
                    break;
            }

            // update the timer system (this should always be called first if you are using this system)
            timerSystem.Update(gameTime);

            // update input
            inputManager.Update();

            // update all of the game states
            gameStateManager.Update();

            // update the the mouse pointer
            mousePointer.Update();

            // process indiecity pop up achivements
            if (achievementPopUp != null) achievementPopUp.Update(displayManager.DisplaySize, timerSystem.TimeStep);

            // process indiecity pop up achivements
            if (achievementList != null) achievementList.Update(indieCityUserList);

            // process indiecity messages
            messagePopUp.Update(displayManager.DisplaySize, timerSystem.TimeStep);
           
            /*// test code - allow user to unlock achievements in sequence when they press 'U'
            if (inputManager.WasKeyPressed(Microsoft.Xna.Framework.Input.Keys.U, null))
            {
                uint numAchievements = indieCityAchievementGroup.AchievementCount;
                for (uint i = 0; i < numAchievements; ++i)
                {
                    ICELandaLib.CoAchievement achievement = indieCityAchievementGroup.GetAchievementFromIndex(i);

                    uint unlockedCount = indieCityUserList.NumberAchievements;
                    if (!indieCityUserList.IsAchievementUnlocked(achievement.AchievementId))
                    {

                        ICELandaLib.CoUserAchievementList anotherUserList = indieCityAchievementManager.GetUserAchievementList(indieCitySession.UserId);
                        uint fred = anotherUserList.NumberAchievements;
                        achievement.Unlock();
                        break;
                    }
                }
            }*/
        }

        //------------------------------------------------------------------------------
        // Method: Draw
        // Author: Neil Holmes & Andrew Green
        // Summary: overrides base draw so we can perform our own custom drawing
        //------------------------------------------------------------------------------
        protected override void Draw(GameTime gameTime)
        {
            // tell the display manager we are ready to begin drawing
            displayManager.StartDraw();

            // tell all the game states to draw
            gameStateManager.Draw();

            // *** start of required indiecity draw calls ***

            // draw indiecity pop up achievements
            if (achievementPopUp != null) achievementPopUp.Draw(displayManager.DisplaySize, ICAchievementColorMode.inverted);

            // draw indiecity messages
            if (messagePopUp != null) messagePopUp.Draw(displayManager.DisplaySize, ICMessageColorMode.normal);

            // *** end of required indiecity draw calls ***
                        
            // draw the pointer (always done after everything else has drawn to ensure it is on top!)
            mousePointer.Draw();

            // tell the display manager we have finished drawing - will copy the result to the back buffer
            displayManager.EndDraw();
        }
    }
}