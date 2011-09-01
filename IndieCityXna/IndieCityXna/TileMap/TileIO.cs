
// Filename: TileIO.cs
// Author: Neil Holmes & Andrew Green
// Summary: everthing needed to save and load tile layers, maps and sets created
//          in the IndieCity xna map editor
//------------------------------------------------------------------------------

//**********************************************************************************
//**********************************************************************************
// NOTE this code is WIP - it is designed to be used with my map editor which is not
// currently ready for public release. as i get it finished the source code for it
// will also be released as part of this package :)
//**********************************************************************************
//**********************************************************************************

using System;
using System.Xml;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace IndieCityXna.TileMap
{
    //------------------------------------------------------------------------------
    // Class: TileIO
    // Author: Neil Holmes & Andrew Green
    // Summary: class for saving and loading tile layers, maps and sets that were
    //          created in the IndieCity xna map editor
    //------------------------------------------------------------------------------
    public class TileIO
    {
        // xml dom obhect for holding the xml data before writing
        XmlDocument writeLevelDOM;
        XmlNode writeRoot = null;

        // xml dom object for holding the xml data during reading
        XmlDocument readLevelDOM;
        XmlNode readRoot = null;

        // the file path and name being used during the write process
        // used for writing actions performed after the xml document has been created
        string fileName;
        string filePath;

        //------------------------------------------------------------------------------
        // Function: WriteLevelStart
        // Author: Neil Holmes & Andrew Green
        // Summary: opens the specified output file and prepares it for writing
        //          NOTE: always call this first when writing data :)
        //------------------------------------------------------------------------------
        public bool WriteLevelStart(string filePath, string fileName)
        {
            // Store file details
            this.fileName = fileName;
            this.filePath = filePath;

            // create the xml DOM
            writeLevelDOM = new XmlDocument();

            // add a nice comment at the top of the tree
            writeLevelDOM.AppendChild(writeLevelDOM.CreateComment("Created with IndieCity XNA Map Editor"));

            // add the version number comment
            writeLevelDOM.AppendChild(writeLevelDOM.CreateComment("Version 1.0"));

            // create the top level node and store a reference to it for future write processing
            writeRoot = writeLevelDOM.CreateElement("ICXNAMapEditorProjectFile");
            writeLevelDOM.AppendChild(writeRoot);

            // return success!
            return true;
        }

        //------------------------------------------------------------------------------
        // Function: WriteTileSets
        // Author: nholmes
        // Summary: writes all the tile sets to the xml document
        //------------------------------------------------------------------------------
        public bool WriteTileSets(TileSet[] tileSets)
        {
            XmlNode listNode;
            XmlNode childNode;
            XmlAttribute newAttribute;

            // if we haven't got a valid root node, return failure
            if (writeRoot == null) return false;

            // create a node to hold the list of tile sets
            listNode = writeLevelDOM.CreateElement("TileSets");
            
            // add an attribute to the list node to store the number of tile sets we are going to store
            newAttribute = writeLevelDOM.CreateAttribute("Count");
            newAttribute.Value = tileSets.Length.ToString();
            listNode.Attributes.Append(newAttribute);

            // create a node for every tile set that contains it's data
            for (int tileSet = 0; tileSet < tileSets.Length; tileSet++)
            {
                // create a node for this tile set
                childNode = writeLevelDOM.CreateElement("TileSet");

                // write the name of the tile map as an attribute of the node
                newAttribute = writeLevelDOM.CreateAttribute("Name");
                newAttribute.Value = tileSets[tileSet].Name;
                childNode.Attributes.Append(newAttribute);

                // write the index of this tile set in an attribute of the node
                newAttribute = writeLevelDOM.CreateAttribute("Number");
                newAttribute.Value = tileSet.ToString();
                childNode.Attributes.Append(newAttribute);

                // write the size of this tile set in an attribute of the node
                newAttribute = writeLevelDOM.CreateAttribute("TileSize");
                newAttribute.Value = tileSets[tileSet].TileSize.ToString();
                childNode.Attributes.Append(newAttribute);

                // append the tile set node to the list of tile sets
                listNode.AppendChild(childNode);
            }

            // append the list of tile sets to the document
            writeRoot.AppendChild(listNode);

            // report success!
            return true;
        }

        //------------------------------------------------------------------------------
        // Function: WriteTileMaps
        // Author: nholmes
        // Summary: writes all the tile map data to the xml document
        //------------------------------------------------------------------------------
        public bool WriteTileMaps(TileMap[] tileMaps)
        {
            XmlNode listNode;
            XmlNode childNode;
            XmlNode dataNode;
            XmlAttribute newAttribute;

            // if we haven't got a valid root node, return failure
            if (writeRoot == null) return false;

            // create the a node to hold the list of tile maps
            listNode = writeLevelDOM.CreateElement("TileMaps");

            // add an attribute to the list node to store the number of tile maps we are going to store
            newAttribute = writeLevelDOM.CreateAttribute("Count");
            newAttribute.Value = tileMaps.Length.ToString();
            listNode.Attributes.Append(newAttribute);

            // create a node for every tile map that contains it's data
            for (int tileMap = 0; tileMap < tileMaps.Length; tileMap++)
            {
                // create a node for this tile map
                childNode = writeLevelDOM.CreateElement("TileMap");

                // write the name of the tile map as an attribute of the node
                newAttribute = writeLevelDOM.CreateAttribute("Name");
                newAttribute.Value = tileMaps[tileMap].Name;
                childNode.Attributes.Append(newAttribute);

                // write the index of this tile layer as an attribute of the node
                newAttribute = writeLevelDOM.CreateAttribute("Number");
                newAttribute.Value = tileMap.ToString();
                childNode.Attributes.Append(newAttribute);

                // write the width of the tile map as an attribute of the node
                newAttribute = writeLevelDOM.CreateAttribute("Width");
                newAttribute.Value = tileMaps[tileMap].Width.ToString();
                childNode.Attributes.Append(newAttribute);

                // write the height of the tile map as an attribute of the node
                newAttribute = writeLevelDOM.CreateAttribute("Height");
                newAttribute.Value = tileMaps[tileMap].Height.ToString();
                childNode.Attributes.Append(newAttribute);

                // create a data node to hold all of the map data
                dataNode = writeLevelDOM.CreateElement("Data");

                // transfer the map data into the data node
                for (int y = 0; y < tileMaps[tileMap].Height; y++)
                {
                    for (int x = 0; x < tileMaps[tileMap].Width; x++)
                    {
                        dataNode.InnerText += tileMaps[tileMap].GetTileIndex(x, y).ToString() + ",";
                    }
                }

                // append the data node to the tile map node
                childNode.AppendChild(dataNode);

                // append the tile map node to the list of tile maps
                listNode.AppendChild(childNode);
            }

            // append the list of tile maps to the document
            writeRoot.AppendChild(listNode);

            // report success!
            return true;
        }

        //------------------------------------------------------------------------------
        // Function: WriteTileLayers
        // Author: nholmes
        // Summary: writes all the tile layer data
        //------------------------------------------------------------------------------
        public bool WriteTileLayers(TileLayer[] tileLayers, int masterTileLayer)
        {
            XmlNode listNode;
            XmlNode childNode;
            XmlAttribute newAttribute;

            // if we haven't got a valid root node, return failure
            if (writeRoot == null) return false;

            // create a node to hold the list of tile layers
            listNode = writeLevelDOM.CreateElement("TileLayers");

            // write the index of the master tile layer as an attribute of the node
            newAttribute = writeLevelDOM.CreateAttribute("MasterTileLayer");
            newAttribute.Value = masterTileLayer.ToString(); ;
            listNode.Attributes.Append(newAttribute);
            
            // add an attribute to the list node to store the number of tile layers we are going to store
            newAttribute = writeLevelDOM.CreateAttribute("Count");
            newAttribute.Value = tileLayers.Length.ToString();
            listNode.Attributes.Append(newAttribute);

            // create a node for each tile layer to hold it's data
            for (int tileLayer = 0; tileLayer < tileLayers.Length; tileLayer++)
            {
                // create a node for this tile layer
                childNode = writeLevelDOM.CreateElement("TileLayer");

                // write the name of the tile layer as an attribute of the node
                newAttribute = writeLevelDOM.CreateAttribute("Name");
                newAttribute.Value = tileLayers[tileLayer].Name;
                childNode.Attributes.Append(newAttribute);
                
                // write the index of this tile layer as an attribute of the node
                newAttribute = writeLevelDOM.CreateAttribute("Number");
                newAttribute.Value = tileLayer.ToString();
                childNode.Attributes.Append(newAttribute);

                // write the mode of the tile layer as an attribute of the node
                newAttribute = writeLevelDOM.CreateAttribute("Mode");
                newAttribute.Value = tileLayers[tileLayer].LayerMode.ToString();
                childNode.Attributes.Append(newAttribute);

                // write the target layer of this tile layer as an attribte of the node
                newAttribute = writeLevelDOM.CreateAttribute("Target");
                if (tileLayers[tileLayer].Target == null)
                    newAttribute.Value = "None";
                else
                    newAttribute.Value = tileLayers[tileLayer].Target.Name;
                childNode.Attributes.Append(newAttribute);

                // write the tile set this tile layer uses as an attribute of the node
                newAttribute = writeLevelDOM.CreateAttribute("TileSet");
                newAttribute.Value = tileLayers[tileLayer].TileSet.Name;
                childNode.Attributes.Append(newAttribute);

                // write the tile map this tile layer uses as an attribte of the node
                newAttribute = writeLevelDOM.CreateAttribute("TileMap");
                newAttribute.Value = tileLayers[tileLayer].TileMap.Name;
                childNode.Attributes.Append(newAttribute);

                // write the tint color this tile layer uses as an attribte of the node
                newAttribute = writeLevelDOM.CreateAttribute("TintColor");
                newAttribute.Value = tileLayers[tileLayer].TintColor.A + "," + tileLayers[tileLayer].TintColor.R + "," + tileLayers[tileLayer].TintColor.G + "," + tileLayers[tileLayer].TintColor.B;
                childNode.Attributes.Append(newAttribute);

                // write the position scale this tile layer uses as an attribte of the node
                newAttribute = writeLevelDOM.CreateAttribute("PositionScale");
                newAttribute.Value = tileLayers[tileLayer].PositionScaleX + "," + tileLayers[tileLayer].PositionScaleY;
                childNode.Attributes.Append(newAttribute);

                // write the position offset this tile layer uses as an attribte of the node
                newAttribute = writeLevelDOM.CreateAttribute("PositionOffset");
                newAttribute.Value = tileLayers[tileLayer].PositionOffsetX + "," + tileLayers[tileLayer].PositionOffsetY;
                childNode.Attributes.Append(newAttribute);

                // append the node to the list of tile layer nodes
                listNode.AppendChild(childNode);
            }

            // add the list of tile layers to the docuemnt
            writeRoot.AppendChild(listNode);

            // report success!
            return true;
        }

        //------------------------------------------------------------------------------
        // Function: WriteEditorInfo
        // Author: nholmes
        // Summary: writes editor related info (background colour, view scales etc)
        //------------------------------------------------------------------------------
        public bool WriteEditorInfo(Color backgroundColor, float tileSetViewScale, float tileLayerViewScale)
        {
            XmlNode listNode;
            XmlAttribute newAttribute;

            // if we haven't got a valid root node, return failure
            if (writeRoot == null) return false;

            // create a node to hold the editor info
            listNode = writeLevelDOM.CreateElement("EditorInfo");

            // write the background color
            newAttribute = writeLevelDOM.CreateAttribute("BackgroundColor");
            newAttribute.Value = backgroundColor.A + "," + backgroundColor.R + "," + backgroundColor.G + "," + backgroundColor.B;
            listNode.Attributes.Append(newAttribute);

            // write the tile set view scale
            newAttribute = writeLevelDOM.CreateAttribute("TileSetViewScale");
            newAttribute.Value = tileSetViewScale.ToString();
            listNode.Attributes.Append(newAttribute);

            // write the tile layer view scale
            newAttribute = writeLevelDOM.CreateAttribute("TileLayerViewScale");
            newAttribute.Value = tileLayerViewScale.ToString();
            listNode.Attributes.Append(newAttribute);

            // add the list of tile layers to the docuemnt
            writeRoot.AppendChild(listNode);

            // return success!
            return true;
        }

        //------------------------------------------------------------------------------
        // Function: WriteLevelEnd
        // Author: nholmes
        // Summary: finalises the data and closes the file when writing is finised
        //          NOTE: only call this when you have completely finished writing data
        //------------------------------------------------------------------------------
        public bool WriteLevelEnd()
        {
            XmlTextWriter levelWriter;
            
            // if we haven't got a valid root node, return failure
            if (writeRoot == null) return false;

            // create the file and prepare it for data writing
            levelWriter = new XmlTextWriter(filePath + fileName, null);

            // make the output look pretty ;)
            levelWriter.Formatting = Formatting.Indented;

            // write the xml header
            levelWriter.WriteStartDocument();

            // write the xml content
            writeLevelDOM.WriteContentTo(levelWriter);

            // close the file
            levelWriter.Close();

            // destroy the level DOM and write root
            writeLevelDOM = null;
            writeRoot = null;

            // return success!
            return true;
        }

        //------------------------------------------------------------------------------
        // Function: ReadLevelStart
        // Author: nholmes
        // Summary: opens and reads the level file and stores a reference to the root
        //          used for starting to read
        //------------------------------------------------------------------------------
        public bool ReadLevelStart(string filePath, string fileName)
        {
            // open the file and prepare it for reading
            readLevelDOM = new XmlDocument();
            readLevelDOM.Load(filePath + fileName);

            // check that the file was read successfully and return failure if not
            if (readLevelDOM == null) return false;

            // get the root reference to the xml DOM
            readRoot = readLevelDOM.DocumentElement;

            // return success
            return true;
        }

        //------------------------------------------------------------------------------
        // Function: ReadTileSets
        // Author: nholmes
        // Summary: finds all the tile sets in the loaded level and re-creates them
        //------------------------------------------------------------------------------
        public bool ReadTileSets(ContentManager contentManager, string filePath, out TileSet[] tileSets)
        {
            // initialy set tileSets to null, in case there are any errors and we need to bail out early!
            tileSets = null;
            
            // if we haven't got a valid root node, return failure
            if (readRoot == null) return false;

            // retrieve number of TileSets
            int numTileSets = readRoot.SelectNodes("//TileSets/TileSet").Count;

            // create the tile set array
            tileSets = new TileSet[numTileSets];

            // loop through all of the tile sets and re-create them
            for (int tileSet = 0; tileSet < numTileSets; tileSet++)
            {
                // get the tile set that corresponds to the current index
                XmlNode tileSetData = readRoot.SelectSingleNode("//TileSets/TileSet[@Number=" + tileSet + "]");                
                
                // check that the tile set was found and return failure if not
                if (tileSetData == null) return false;

                // read the tile set name
                XmlNode tileSetNameNode = tileSetData.Attributes.GetNamedItem("Name");

                // was the name data found ok? return failure if not!
                if (tileSetNameNode == null) return false;

                // read the tile set size
                XmlNode tileSetSizeNode = tileSetData.Attributes.GetNamedItem("TileSize");

                // was the size data found ok? return failure if not!
                if (tileSetSizeNode == null) return false;

                // create the tile set
                tileSets[tileSet] = new TileSet(contentManager, filePath, tileSetNameNode.InnerText, Convert.ToInt32(tileSetSizeNode.InnerText));
            }

            // return success!
            return true;
        }

        //------------------------------------------------------------------------------
        // Function: ReadTileMaps
        // Author: nholmes
        // Summary: finds all the tile maps in the loaded level and re-creates them
        //------------------------------------------------------------------------------
        public bool ReadTileMaps(out TileMap[] tileMaps)
        {
            // initialy set tileMaps to null, in case there are any errors and we need to bail out early!
            tileMaps = null;

            // if we haven't got a valid root node, return failure
            if (readRoot == null) return false;

            // retrieve number of tile maaps
            int numTileMaps = readRoot.SelectNodes("//TileMaps/TileMap").Count;

            // create the tile map array
            tileMaps = new TileMap[numTileMaps];

            // loop through all of the tile maps and re-create them
            for (int tileMap = 0; tileMap < numTileMaps; tileMap++)
            {
                // get the tile map that corresponds to the current index
                XmlNode tileMapData = readRoot.SelectSingleNode("//TileMaps/TileMap[@Number=" + tileMap + "]");

                // check that the tile map was found and return failure if not
                if (tileMapData == null) return false;

                // read the tile map name
                XmlNode tileMapNameNode = tileMapData.Attributes.GetNamedItem("Name");

                // was the name data found ok? return failure if not!
                if (tileMapNameNode == null) return false;

                // find the width data
                XmlNode widthData = tileMapData.Attributes.GetNamedItem("Width");
                
                // was the width data found ok? return failure if not!
                if (widthData == null) return false;

                // find the height data
                XmlNode heightData = tileMapData.Attributes.GetNamedItem("Height");

                // was the height data found ok? return failure if not!
                if (heightData == null) return false;

                // create the new tile map
                tileMaps[tileMap] = new TileMap(tileMapNameNode.InnerText, Convert.ToInt32(widthData.InnerText), Convert.ToInt32(heightData.InnerText));

                // find the map data
                XmlNode mapData = tileMapData.SelectSingleNode("Data");

                // was the map data found ok? return failure if not!
                if (mapData == null) return false;

                // get a reference to the start of the data string
                string dataRef = mapData.InnerText;

                // parse through the map data and set all the tile indexes
                for (int y = 0; y < tileMaps[tileMap].Height; y++)
                {
                    for (int x = 0; x < tileMaps[tileMap].Width; x++)
                    {
                        // find the next comma in the string
                        int nextComma = dataRef.IndexOf(',');
                      
                        // convert the value up to the next comma into a tile index and write it to the map data
                        tileMaps[tileMap].SetTileIndex(x, y, Convert.ToInt32(dataRef.Remove(nextComma)));

                        // remove the string up to the next comma ready to read the next value
                        dataRef = dataRef.Remove(0, nextComma + 1);
                    }
                }
            }

            // return success!
            return true;
        }

        //------------------------------------------------------------------------------
        // Function: ReadTileLayers
        // Author: nholmes
        // Summary: finds all the tile layers in the loaded level and re-creates them
        //------------------------------------------------------------------------------
        public bool ReadTileLayers(Game game, out TileLayer[] tileLayers, out int masterTileLayer, TileSet[] tileSets, TileMap[] tileMaps)
        {
            // initialy set tileLayers to null, in case there are any errors and we need to bail out early!
            tileLayers = null;

            // set master tile layer to -1, in case there are any errors and we need to bail out early!
            masterTileLayer = -1;

            // if we haven't got a valid root node, return failure
            if (readRoot == null) return false;

            // find the tile layer node
            XmlNode tileLayerNode = readRoot.SelectSingleNode("//TileLayers");

            // find the master tile layer attribute
            XmlNode masterTileLayerNode = tileLayerNode.Attributes.GetNamedItem("MasterTileLayer");

            // was the master tile layer attribute found ok? return failure if not!
            if (masterTileLayerNode == null) return false;

            // store the master tile layer index
            masterTileLayer = Convert.ToInt32(masterTileLayerNode.InnerText);

            // find the number of tile layers attribute
            XmlNode numberOfTileLayersNode = tileLayerNode.Attributes.GetNamedItem("Count");

            // was the number of tile layers attribute found ok? return failure if not!
            if (numberOfTileLayersNode == null) return false;

            // store the number of tile layers
            int numTileLayers = Convert.ToInt32(numberOfTileLayersNode.InnerText);

            // create the tile layer array
            tileLayers = new TileLayer[numTileLayers];

            // create an array to hold the tile layer target names so we can process them once all layers are loaded
            string[] layerTargetNames = new string[numTileLayers];

            // loop through all of the tile maps and re-create them
            for (int tileLayer = 0; tileLayer < numTileLayers; tileLayer++)
            {
                // get the tile layer that corresponds to the current index
                XmlNode tileLayerData = readRoot.SelectSingleNode("//TileLayers/TileLayer[@Number=" + tileLayer + "]");

                // check that the tile layer was found and return failure if not
                if (tileLayerData == null) return false;

                // read the tile layer name
                XmlNode tileLayerNameNode = tileLayerData.Attributes.GetNamedItem("Name");

                // was the name data found ok? return failure if not!
                if (tileLayerNameNode == null) return false;

                // create the tile layer
                tileLayers[tileLayer] = new TileLayer(game, tileLayerNameNode.InnerText);

                // null the tile layer's tile set and tile map pointers in case there is an error and we have to bail out
                tileLayers[tileLayer].TileSet = null;
                tileLayers[tileLayer].TileMap = null;

                // find the tile set data
                XmlNode tileSetData = tileLayerData.Attributes.GetNamedItem("TileSet");

                // was the tile set data found ok? return failure if not!
                if (tileSetData == null) return false;

                // Find the correct tile set in the supplied list of tile sets
                for(int tileSet = 0; tileSet < tileSets.Length; tileSet++)
                {
                    if(tileSets[tileSet].Name == tileSetData.InnerText)
                    {
                        // found the correct tile set - store a reference to the tile set in the layer
                        tileLayers[tileLayer].TileSet = tileSets[tileSet];
                        break;
                    }
                }

                // check that we found a valid tile set - return failure if we didn't!
                if (tileLayers[tileLayer].TileSet == null) return false;

                // find the tile map data
                XmlNode tileMapData = tileLayerData.Attributes.GetNamedItem("TileMap");

                // was the tile map data found ok? return failure if not!
                if (tileMapData == null) return false;

                // Find the correct tile map in the supplied list of tile maps
                for (int tileMap = 0; tileMap < tileMaps.Length; tileMap++)
                {
                    if (tileMaps[tileMap].Name == tileMapData.InnerText)
                    {
                        // found the correct tile map - store a reference to the tile map in the layer
                        tileLayers[tileLayer].TileMap = tileMaps[tileMap];
                        break;
                    }
                }

                // check that we found a valid tile map - return failure if we didn't!
                if (tileLayers[tileLayer].TileMap == null) return false;

                // find the layer mode data
                XmlNode layerModeData = tileLayerData.Attributes.GetNamedItem("Mode");

                // was the layer mode data found ok? return failure if not!
                if (layerModeData == null) return false;

                // set the layer mode according to the data we extracted
                switch (layerModeData.InnerText)
                {
                    case "Static":
                        tileLayers[tileLayer].LayerMode = TileLayerMode.Static;
                        break;

                    case "Forced":
                        tileLayers[tileLayer].LayerMode = TileLayerMode.Forced;
                        break;
                    
                    case "Follow":                  
                        tileLayers[tileLayer].LayerMode = TileLayerMode.Follow;
                        break;

                    case "Relative":
                        tileLayers[tileLayer].LayerMode = TileLayerMode.Relative;
                        break;
                        
                    case "Scaled":
                        tileLayers[tileLayer].LayerMode = TileLayerMode.Scaled;
                        break;

                    default:

                        // something has gone wrong - return failure!
                        return false;
                }

                // find the layer target data
                XmlNode layerTargetData = tileLayerData.Attributes.GetNamedItem("Target");

                // was the layer target data found ok? return failure if not!
                if (layerTargetData == null) return false;

                // read and store the layer target name so we can fix these up once all layers are loaded
                layerTargetNames[tileLayer] = layerTargetData.InnerText;
 
                // find the tint color data
                XmlNode tintColorData = tileLayerData.Attributes.GetNamedItem("TintColor");

                // was the tint color data found ok? return failure if not!
                if (tintColorData == null) return false;

                // read and store the tint color
                string dataRef = tintColorData.InnerText;
                Color tintColor = new Color();

                // get the alpha value
                int nextComma = dataRef.IndexOf(',');
                tintColor.A = (byte)Convert.ToInt32(dataRef.Remove(nextComma));
                dataRef = dataRef.Remove(0, nextComma + 1);
            
                // get the red value
                nextComma = dataRef.IndexOf(',');
                tintColor.R = (byte)Convert.ToInt32(dataRef.Remove(nextComma));
                dataRef = dataRef.Remove(0, nextComma + 1);

                // get the green value
                nextComma = dataRef.IndexOf(',');
                tintColor.G = (byte)Convert.ToInt32(dataRef.Remove(nextComma));
                dataRef = dataRef.Remove(0, nextComma + 1);

                // get the blue value
                tintColor.B = (byte)Convert.ToInt32(dataRef);

                // store the tint color
                tileLayers[tileLayer].TintColor = tintColor;

                // find the position scale data
                XmlNode positionScaleData = tileLayerData.Attributes.GetNamedItem("PositionScale");

                // was the position scale data found ok? return failure if not!
                if (positionScaleData == null) return false;

                // read and store the position scale
                dataRef = positionScaleData.InnerText;

                // get and store the X value
                nextComma = dataRef.IndexOf(',');
                tileLayers[tileLayer].PositionScaleX = Convert.ToSingle(dataRef.Remove(nextComma));
                dataRef = dataRef.Remove(0, nextComma + 1);

                // get and store the Y value
                tileLayers[tileLayer].PositionScaleY = Convert.ToSingle(dataRef);

                // find the position scale data
                XmlNode positionOffsetData = tileLayerData.Attributes.GetNamedItem("PositionOffset");

                // was the position offset data found ok? return failure if not!
                if (positionOffsetData == null) return false;

                // read and store the position offset
                dataRef = positionOffsetData.InnerText;

                // get and store the X value
                nextComma = dataRef.IndexOf(',');
                tileLayers[tileLayer].PositionOffsetX = Convert.ToSingle(dataRef.Remove(nextComma));
                dataRef = dataRef.Remove(0, nextComma + 1);

                // get and store the Y value
                tileLayers[tileLayer].PositionOffsetY = Convert.ToSingle(dataRef);
            }

            // now all the layers are loaded and created we need to fix up the layer targets
            for (int tileLayer = 0; tileLayer < numTileLayers; tileLayer++)
            {
                if (layerTargetNames[tileLayer] == "None")
                    tileLayers[tileLayer].Target = null;
                else
                {
                    int searchLayer;

                    // search all of the tile layers for the layer this layer wishes to target
                    for (searchLayer = 0; searchLayer < numTileLayers; searchLayer++)
                    {
                        // check if this layer matches the name we are looking for
                        if (tileLayers[searchLayer].Name == layerTargetNames[tileLayer])
                        {
                            // found the match - store it and bail out
                            tileLayers[tileLayer].Target = tileLayers[searchLayer];
                            break;
                        }
                    }

                    // check to see if we failed to find a batch and bail out with failure if so
                    if (searchLayer == numTileLayers) return false;
                }
            }
    
            // return success
            return true;
        }

        //------------------------------------------------------------------------------
        // Function: ReadEditorInfo
        // Author: nholmes
        // Summary: reads the editor related info (background colour, view scales etc)
        //------------------------------------------------------------------------------
        public bool ReadEditorInfo(out Color backgroundColor, out float tileSetViewScale, out float tileLayerViewScale, TileLayer[] tileSetLayers)
        {
            // set some defaults just in case we have a problem and need to bail out...
            backgroundColor = Color.Blue;
            tileSetViewScale = 1.0f;
            tileLayerViewScale = 1.0f;

            // if we haven't got a valid root node, return failure
            if (readRoot == null) return false;

            // find the editor info node
            XmlNode tileLayerNode = readRoot.SelectSingleNode("//EditorInfo");

            // find the background colour attribute
            XmlNode attributeNode = tileLayerNode.Attributes.GetNamedItem("BackgroundColor");

            // was the background colour attribute found ok? return failure if not!
            if (attributeNode == null) return false;

            // read and store the background color
            string dataRef = attributeNode.InnerText;
            backgroundColor = new Color();

            // get the alpha value
            int nextComma = dataRef.IndexOf(',');
            backgroundColor.A = (byte)Convert.ToInt32(dataRef.Remove(nextComma));
            dataRef = dataRef.Remove(0, nextComma + 1);

            // get the red value
            nextComma = dataRef.IndexOf(',');
            backgroundColor.R = (byte)Convert.ToInt32(dataRef.Remove(nextComma));
            dataRef = dataRef.Remove(0, nextComma + 1);

            // get the green value
            nextComma = dataRef.IndexOf(',');
            backgroundColor.G = (byte)Convert.ToInt32(dataRef.Remove(nextComma));
            dataRef = dataRef.Remove(0, nextComma + 1);

            // get the blue value
            backgroundColor.B = (byte)Convert.ToInt32(dataRef);

            // find the tile list view scale attribute
            attributeNode = tileLayerNode.Attributes.GetNamedItem("TileSetViewScale");

            // was the tile list view scale attribute found ok? return failure if not!
            if (attributeNode == null) return false;

            // retrieve and store the tile list view scale
            tileSetViewScale = Convert.ToSingle(attributeNode.Value);

            // find the tile layer view scale attribute
            attributeNode = tileLayerNode.Attributes.GetNamedItem("TileLayerViewScale");

            // was the tile layer view scale attribute found ok? return failure if not!
            if (attributeNode == null) return false;

            // retrieve and store the tile layer view scale
            tileLayerViewScale = Convert.ToSingle(attributeNode.Value);
            
            // return success!
            return true;
        }

        //------------------------------------------------------------------------------
        // Function: ReadLevelEnd
        // Author: nholmes
        // Summary: finalises the read process - should always be called when reading is done
        //------------------------------------------------------------------------------
        public bool ReadLevelEnd()
        {
            // get rid of the read root and xml DOM
            readLevelDOM = null;
            readRoot = null;

            // return success!
            return true;
        }
    }
}
