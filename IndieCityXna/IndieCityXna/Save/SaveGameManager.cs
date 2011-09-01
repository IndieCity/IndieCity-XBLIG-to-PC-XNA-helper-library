//------------------------------------------------------------------------------
// Filename: SaveGameManager.cs
// Author: nholmes
// Summary: provides saving and loading functionality. relies on the game providing
//          a class called SaveData (accessed through game.saveData) in 
//          order to know what to save / load
//------------------------------------------------------------------------------

//**********************************************************************************
//**********************************************************************************
// NOTE this code is OLD and very WIP - we will update it to work properly ASAP soon
//**********************************************************************************
//**********************************************************************************

using System;
using System.IO;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.GamerServices;

//TODO: desperately needs to inform the player about failed load/save attempts and deal with them properly!

namespace IndieCityXna.SaveGame
{
    //------------------------------------------------------------------------------
    // Class: SaveGameManager
    // Author: nholmes
    // Summary: provides all functionality for loading and saving the game class
    //          saveData. the game code must supply a valid saveData class in order
    //          for this class to compile successfully
    //------------------------------------------------------------------------------
    public class SaveGameManager
    {
        // parent game
        private Game game;

        // name to be used for the save data file
        private const string saveName = "SaveData.dat";

        // storage Objects
        private IAsyncResult deviceSelectionResult = null;
        private IAsyncResult openContainerResult = null;
        private StorageDevice selectedDevice = null;

        // the current version number
        private byte versionIDa = (byte)'v';
        private byte versionIDb = (byte)'1';
        private byte versionIDc = (byte)'.';
        private byte versionIDd = (byte)'0';

        //------------------------------------------------------------------------------
        // Function: SaveGameManager
        // Author: nholmes
        // Summary: constructs a new save game manager
        //------------------------------------------------------------------------------
        public SaveGameManager(Game game)
        {
            // store the game system that owns this game state manager
            this.game = game;
        }

        //------------------------------------------------------------------------------
        // Function: Save
        // Author: nholmes
        // Summary: saves whatever game.saveData currently contains into a folder
        //          called gameName. if the save fails for any reason the contents of
        //          saveData will remain unchanged
        //------------------------------------------------------------------------------
        public void Save(string gameName, XmlSerializer saveData)
        {
            // has the user already selected a storage device?
            if (deviceSelectionResult == null)
            {
                // ask the user to select the storage device
                deviceSelectionResult = StorageDevice.BeginShowSelector(PlayerIndex.One, null, null);
            }
            
            // has the device selection process completed yet?
            if ((deviceSelectionResult == null) || (!deviceSelectionResult.IsCompleted))
            {
                // device selection failed - bail out
                return;
            }

            // have we already got a selected device?
            if (selectedDevice == null)
            {
                // get the device that was selected and close the blade
                selectedDevice = StorageDevice.EndShowSelector(deviceSelectionResult);
            }

            // check if a device was not selcted or if the device is not connected
            if ((selectedDevice == null) || !selectedDevice.IsConnected)
            {
                // selected device is invalid - bail out
                return;
            }

            // open a storage container
            openContainerResult = selectedDevice.BeginOpenContainer(gameName, null, null);
            
            // Wait for the WaitHandle to become signalled
            openContainerResult.AsyncWaitHandle.WaitOne();

            StorageContainer container = selectedDevice.EndOpenContainer(openContainerResult);

            // Close the WaitHandle
            openContainerResult.AsyncWaitHandle.Close();

            // Open the file, creating it if necessary
            Stream stream = container.OpenFile(saveName, FileMode.OpenOrCreate);

            // write the version ID bytes to the file seperately to the stream
            stream.WriteByte(versionIDa);
            stream.WriteByte(versionIDb);
            stream.WriteByte(versionIDc);
            stream.WriteByte(versionIDd);

            // convert the save data class to XML data and put it in the stream
            saveData.Serialize(stream, saveData);

            // close the file
            stream.Close();

            // Dispose the container to commit the changes
            container.Dispose();
        }


        //------------------------------------------------------------------------------
        // Function: Load
        // Author: nholmes
        // Summary: searches for valid saved data under the name gameName and loads it
        //          into game.saveData. If the load fails for any reason an error is 
        //          reported so that saveDatacan be reset to default settings to ensure
        //          that it is at least usable
        //------------------------------------------------------------------------------
        public bool Load(string gameName, XmlSerializer saveData)
        {
            StorageContainer container = null;

            // has the user already selected a storage device?
            if (deviceSelectionResult == null)
            {
                // ask the user to select the storage device
                deviceSelectionResult = selectedDevice.BeginOpenContainer(gameName, null, null);

                // Wait for the WaitHandle to be signalled
                deviceSelectionResult.AsyncWaitHandle.WaitOne();

                // open a storage container
                container = selectedDevice.EndOpenContainer(deviceSelectionResult);

                // Close the WaitHandle
                deviceSelectionResult.AsyncWaitHandle.Close();
            }
            
            
            // check if a device was not selcted or if the device is not connected
            if (container == null)
            {
                // load failed - exit with error reported
                return false;
            }

            // check if the save file actually exists
            if (!container.FileExists(saveName))
            {
                // save file does not exit - ext with error reported
                return false;
            }

            // open the saved file
            using (Stream stream = container.OpenFile(saveName, FileMode.OpenOrCreate))
            {
                // write the version ID bytes to the file seperately to the stream
                byte fileIDa = (byte)stream.ReadByte();
                byte fileIDb = (byte)stream.ReadByte();
                byte fileIDc = (byte)stream.ReadByte();
                byte fileIDd = (byte)stream.ReadByte();

                // check that the version ID from the file matches the current version ID
                if (fileIDa != versionIDa ||
                    fileIDb != versionIDb ||
                    fileIDc != versionIDc ||
                    fileIDd != versionIDd)
                {
                    // version miss-match. exit with error reported
                    stream.Close();
                    return false;
                }

                // read the data from the file
                saveData.Deserialize(stream);

                // close the file
                stream.Close();

                // Dispose of the container
                container.Dispose();

                // success!
                return true;
            }
        }
    }
}