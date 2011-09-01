//------------------------------------------------------------------------------
// Filename: TimerSystem.cs
// Author: Neil Holmes & Andrew Green
// Summary: provides timer related functionality
//------------------------------------------------------------------------------

using System;
using Microsoft.Xna.Framework;
using IndieCityXna;

namespace IndieCityXna.Common
{
    //------------------------------------------------------------------------------
    // Class: TimerSystem
    // Author: Neil Holmes & Andrew Green
    // Summary: provides useful functionality for timing related calculations
    //------------------------------------------------------------------------------
    public class TimerSystem
    {
        // parent game
        private Game game;

        // framerate measurement
        private int frameCounter;
        private TimeSpan elapsedTime;

        // the last calculated framerate
        private int frameRate;
        
        // the last calculated time step measurement
        // this is the amount of time in milliseconds that the last frame took to process as a multiplier
        private float timeStep;

        //------------------------------------------------------------------------------
        // Constructor: TimerSystem
        // Author: Neil Holmes & Andrew Green
        // Summary: constructs a new timer system
        //------------------------------------------------------------------------------
        public TimerSystem(Game game)
        {
            // store a reference to the game system
            this.game = game;
       }

        //------------------------------------------------------------------------------
        // Method: Update
        // Author: Neil Holmes & Andrew Green
        // Summary: updates the timer system and all of it's properties. this should be
        //          called once per frame at the start of the main game loop
        //------------------------------------------------------------------------------
        public void Update(GameTime gameTime)
        {
            // calculate and store the amount of time (in milliseconds) that the last frame took to process
            timeStep = (float)gameTime.ElapsedGameTime.Milliseconds / 1000.0f;
            
            // update the total number of processed frames since the last frame rate calculation
            frameCounter ++;
            
            // accumulate our frame time
            elapsedTime += gameTime.ElapsedGameTime;

            // has elapsed time exceed 1 second?
            if (elapsedTime > TimeSpan.FromSeconds(1))
            {
                // reduce the elapsed time by 1 second
                elapsedTime -= TimeSpan.FromSeconds(1);

                // set frame rate to be the number of frames that were processed in the last 1 second perio
                frameRate = frameCounter;

                // reset the frame counter ready for the next second of testing
                frameCounter = 0;
            } 
        }

        //------------------------------------------------------------------------------
        // Method: ResetElapsedTime
        // Author: Neil Holmes & Andrew Green
        // Summary: force the elapsed time to be reset - useful when the game has stalled
        //          for a long time, such as during a load or when it has lost focus
        //------------------------------------------------------------------------------
        public void ResetElapsedTime()
        {
            // reset the amount of elapsed time
            game.ResetElapsedTime();
        }

        //------------------------------------------------------------------------------
        // Property: FrameRate
        // Author: Neil Holmes & Andrew Green
        // Summary: allows user to get the current frame rate
        //------------------------------------------------------------------------------
        public int FrameRate
        {
            get { return frameRate; }
        }

        //------------------------------------------------------------------------------
        // Property: TimeStep
        // Author: Neil Holmes & Andrew Green
        // Summary: allows user to get the current time step that they should be using to
        //          ensure things like movement and transitions are updated at the correct
        //          speed regardless of the actual framerate at which the game is running
        //------------------------------------------------------------------------------
        public float TimeStep
        {
            get { return timeStep; }
        }
    }
}