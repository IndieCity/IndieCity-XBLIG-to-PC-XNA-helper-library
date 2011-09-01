//------------------------------------------------------------------------------
// Filename: GameObject2DManager.cs
// Author: Neil Holmes & Andrew Green
// Summary: Handles update and display of all 2D game objects within the specified
//          object update area
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using IndieCityXna.Common;

namespace IndieCityXna.GameObject
{
    //------------------------------------------------------------------------------
    // class: GameObject2DManager
    // Author: Neil Holmes & Andrew Green
    // Summary: manages all the 2D game objects, updates them and draws them
    //------------------------------------------------------------------------------
    public class GameObject2DManager
    {
        // parent game
        private Game game;

        // handle to the display manager service
        private DisplayManager displayManager;

        // linked list of 2D game objects
        private LinkedList<GameObject2D> gameObjects;
        
        // linked list of 2D game objects that are waiting to be removed
        private LinkedList<GameObject2D> objectsToBeRemoved;

        //------------------------------------------------------------------------------
        // Constructor: GameObject2DManager
        // Author: Neil Holmes & Andrew Green
        // Summary: main constructor for the 2D game object manager - creates the manager
        //          and prepares it for adding and processing 2D game objects
        //------------------------------------------------------------------------------
        public GameObject2DManager(Game game, ContentManager content)
        {
            // store a reference to the parent game
            this.game = game;

            // get a handle to the display manager service
            displayManager = (DisplayManager)game.Services.GetService(typeof(DisplayManager));
 
            // create LinkedList object to store the game objects
            gameObjects = new LinkedList<GameObject2D>();

            // create list of objects to be removed
            objectsToBeRemoved = new LinkedList<GameObject2D>();
        }

        //------------------------------------------------------------------------------
        // Method: AddObject
        // Author: Neil Holmes & Andrew Green
        // Summary: add a new 2D game object to the linked list of objects to be processed
        //------------------------------------------------------------------------------
        public void AddObject(GameObject2D gameObject)
        {
            // tell the object being added to load it's content
            gameObject.LoadContent(game.Content);

            // add the new object to the list of active game objects
            gameObjects.AddFirst(gameObject);
        }

        //------------------------------------------------------------------------------
        // Method: RemoveObject
        // Author: Neil Holmes & Andrew Green
        // Summary: add an object to the list of objects to be removed at the end of this frame
        //------------------------------------------------------------------------------
        public void RemoveObject(GameObject2D gameObject)
        {
            // add the object to the list of objects to be removed
            objectsToBeRemoved.AddFirst(gameObject);
        }

        //------------------------------------------------------------------------------
        // Method: ProcessRemoveList
        // Author: Neil Holmes & Andrew Green
        // Summary: process the list of objects to be removed and remove them :)
        //------------------------------------------------------------------------------
        void ProcessRemoveList()
        {
            // loop through each object to remove
            foreach (GameObject2D gameObject in objectsToBeRemoved)
            {
                // remove the object from the active list
                gameObjects.Remove(gameObject);
            }

            // reset the objects to be removed list
            objectsToBeRemoved.Clear();
        }

        //------------------------------------------------------------------------------
        // Method: Update
        // Author: Neil Holmes & Andrew Green
        // Summary: calls the update method of each GameObject in the linked list
        //------------------------------------------------------------------------------
        public void Update(Vector2 worldPosition)
        {
            // update objects
            foreach (GameObject2D gameObject in gameObjects)
            {
                // should this game object always be updated?
                if (gameObject.AlwaysUpdate)
                {
                    // always update the object
                    gameObject.Update(worldPosition);
                }                
                else 
                {
                    // check if object is in processing range and ignore it if not
                    if (Math.Abs(worldPosition.X - gameObject.Position.X) < gameObject.UpdateRange && Math.Abs(worldPosition.Y - gameObject.Position.Y) < gameObject.UpdateRange)
                    {
                        // object is in range - update the object
                        gameObject.Update(worldPosition);
                    }
                }
            }

            // remove any dead objects from the list
            ProcessRemoveList();
        }

        //------------------------------------------------------------------------------
        // Method: DrawObjects
        // Author: Neil Holmes & Andrew Green
        // Summary: loops through linked list and draws each object
        //------------------------------------------------------------------------------
        public void DrawObjects(Vector2 worldPosition, SpriteBatch spriteBatch)
        {
            // run through all of the game objects and draw any that need drawing
            foreach (GameObject2D gameObject in gameObjects)
            {
                Rectangle testRect;

                // calculate the top left and bottom right of the game object so we can check if it's on screen
                testRect.X = (int)(gameObject.Position.X - gameObject.DisplayOffset.X - worldPosition.X);
                testRect.Y = (int)(gameObject.Position.Y - gameObject.DisplayOffset.Y - worldPosition.Y);
                testRect.Width = (int)gameObject.DisplaySize.X;
                testRect.Height = (int)gameObject.DisplaySize.Y;

                // Check if this object is on screen
                if ((testRect.Left < displayManager.GameResolutionX) && (testRect.Right > 0) &&
                    (testRect.Top < displayManager.GameResolutionY) && (testRect.Bottom > 0))
                {
                    // it's on screen - draw it!
                    gameObject.Draw(worldPosition, spriteBatch, true);
                }
            }
        }

        //------------------------------------------------------------------------------
        // Method: ClearAllObjects
        // Author: Neil Holmes & Andrew Green
        // Summary: empties the linked list
        //------------------------------------------------------------------------------
        public void ClearAllObjects()
        {
            // clear all the game objects
            gameObjects.Clear();
        }
    
        //------------------------------------------------------------------------------
        // Property: Objects
        // Author: Neil Holmes & Andrew Green
        // Summary: returns the list of objects currently in the manager
        //------------------------------------------------------------------------------
        public LinkedList<GameObject2D> Objects
        {
            get { return gameObjects; }
        }
    }
}
