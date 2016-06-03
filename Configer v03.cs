using System;
using System.Text;
using Crestron.SimplSharp;                         				// For Basic SIMPL# Classes
using Newtonsoft.Json;                                          //Thanks to Neil Colvin. Full Library @ http://www.nivloc.com/downloads/crestron/SSharp/
using Crestron.SimplSharp.CrestronIO;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using System.Linq;



namespace Config
{

    public class MyConfig
    {
        public string RmName;           //Name of the Room
        public ushort RoomID;           //Control ID or other ID
        public ushort AudioEquipID;
        public ushort VideoEquipID;
        public ushort SubAudio;         //Has Audio
        public ushort SubVideo;         //Has Video
        public ushort SubLights;        //Has Lights
        public ushort SubShades;        //Has Shades
        public ushort SubHVAC;          //Has HVAC
        public ushort[] HazSource;      //Which Sources are avaiable?
        public string[] HazSourceName;  //What are the Source's names?
        public ushort[] HazHVACID;
        public string[] HazHVACName;
        public ushort[] HazLightID;
        public string[] HazLightName;
        public ushort[] HazShadeID;
        public string[] HazShadeName;    //Hvac or Light or shade Zone Name & ID
        private string DaString;
        private Configuration Obj;
        private SourceList MysList;
        
        
      
        

    
/*Pass the FilePath from SIMPL+ then read in the file.
Create the JSON Object and use the library to deserialize it ushorto 
our classes.
*/ 
        public void Reader(string FilePath)
        {
            if (File.Exists(FilePath))       //Ok make sure the file is there
            {
                StreamReader daFile = new StreamReader(FilePath);
                DaString = daFile.ReadToEnd();
                daFile.Close();
                //debug
                //CrestronConsole.PrintLine(DaString);
            }
            else
            {
                CrestronConsole.PrintLine("File Not found\n\r");    //Generate error
                DaString = "";
            }
        }

        
        public void Builder(ushort ConnectTo)
        {

            HazSource = new ushort[25];
            HazSourceName = new string[25];
            HazLightID = new ushort[31];
            HazLightName = new string[31];
            HazShadeID = new ushort[31];
            HazShadeName = new string[31];
            HazHVACID = new ushort[31];
            HazHVACName = new string[31];

            Obj = JsonConvert.DeserializeObject<Configuration>(DaString);
            
            //debug
            //CrestronConsole.PrintLine("Deserialized");

            RmName = Obj.Rooms[ConnectTo].RoomName;
            RoomID = Obj.Rooms[ConnectTo].ControlID;
            AudioEquipID = Obj.Rooms[ConnectTo].Audio.AudioEquipID;
            VideoEquipID = Obj.Rooms[ConnectTo].Video.VideoEquipID;
            SubAudio = Obj.Rooms[ConnectTo].Audio.isUsing;
            SubVideo = Obj.Rooms[ConnectTo].Video.isUsing;
            SubLights = Obj.Rooms[ConnectTo].Lights.isUsing;
            SubShades = Obj.Rooms[ConnectTo].Shades.isUsing;
            SubHVAC = Obj.Rooms[ConnectTo].HVAC.isUsing;
            //debug
            //CrestronConsole.PrintLine("Assigned to all Variables");
            try
            {
                for (int i = 0; i < 25; i++) //fill in the arrays
                {
                    HazSourceName[i] = Obj.Rooms[ConnectTo].Sources[i].Name;
                    HazSource[i] = Obj.Rooms[ConnectTo].Sources[i].isUsing;
                }
            }
            catch
            {
                CrestronConsole.PrintLine("Failed to fill in Source Names and HazSource");
            }
            
            try
            {
                for (int i = 0; i < Obj.Rooms.Count; i++)
                {
                    HazLightName[i] = Obj.Rooms[i].RoomName;
                    HazLightID[i] = Obj.Rooms[i].Lights.LightEquipmentID;
                    HazShadeName[i] = Obj.Rooms[i].RoomName;
                    HazShadeID[i] = Obj.Rooms[i].Shades.ShadeEquipmentID;
                    HazHVACName[i] = Obj.Rooms[i].RoomName;
                    HazHVACID[i] = Obj.Rooms[i].HVAC.HVACEquipmentID;
                }
            }
            catch
            {
                CrestronConsole.PrintLine("Failed to set up Light/HVAC/Shades");
            }
        }

        #region BuildSource
        //v2 Get info about Source X then send data 
        public void SourceInfo()
        {
            MysList = JsonConvert.DeserializeObject<SourceList>(DaString);
        }

        public string SourceName(ushort ConnectTo)
        {
            return MysList.ListOfSources[ConnectTo].Name;
        }

        public ushort SourceEquipID(ushort ConnectTo)
        {
            return MysList.ListOfSources[ConnectTo].EquipID;
        }

        public ushort SourceSubPageType(ushort ConnectTo)
        {
            return MysList.ListOfSources[ConnectTo].Type;
        }

        public ushort SourceAudioInput(ushort ConnectTo)
        {
            return MysList.ListOfSources[ConnectTo].Ainput;
        }

        public ushort SourceVideoInput(ushort ConnectTo)
        {
            return MysList.ListOfSources[ConnectTo].Vinput;
        }

        public class ListOfSource : Source
        {
            public ushort Type { get; set; }
            public ushort EquipID { get; set; }
            public ushort Ainput { get; set; }
            public ushort Vinput { get; set; }
        }

        public class SourceList
        {
            public IList<ListOfSource> ListOfSources { get; set; }
        }
        
        #endregion

        #region Main
        //Classes built from http://jsonutils.com/
        public class Audio
        {
            [JsonProperty("isUsing")]
            public ushort isUsing { get; set; }
            [JsonProperty("AudioEquipID")]
            public ushort AudioEquipID { get; set; }
        }

        public class Video
        {
            [JsonProperty("isUsing")]
            public ushort isUsing { get; set; }
            [JsonProperty("VideoEquipID")]
            public ushort VideoEquipID { get; set; }
        }

        public class Lights
        {
            [JsonProperty("isUsing")]
            public ushort isUsing { get; set; }
            [JsonProperty("LightEquipmentID")]
            public ushort LightEquipmentID { get; set; }
        }

        public class Shades
        {
            [JsonProperty("isUsing")]
            public ushort isUsing { get; set; }
            [JsonProperty("ShadeEquipmentID")]
            public ushort ShadeEquipmentID { get; set; }
        }

        public class HVAC
        {
            [JsonProperty("isUsing")]
            public ushort isUsing { get; set; }
            [JsonProperty("HVACEquipmentID")]
            public ushort HVACEquipmentID { get; set; }
        }

        public class Source
        {
            [JsonProperty("Name")]
            public string Name { get; set; }
            [JsonProperty("isUsing")]
            public ushort isUsing { get; set; }
        }

        public class Room
        {
            [JsonProperty("RoomName")]
            public string RoomName { get; set; }
            [JsonProperty("ControlID")]
            public ushort ControlID { get; set; }
            [JsonProperty("Audio")]
            public Audio Audio { get; set; }
            [JsonProperty("Video")]
            public Video Video { get; set; }
            [JsonProperty("Lights")]
            public Lights Lights { get; set; }
            [JsonProperty("Shades")]
            public Shades Shades { get; set; }
            [JsonProperty("HVAC")]
            public HVAC HVAC { get; set; }
            [JsonProperty("Sources")]
            public IList<Source> Sources { get; set; }
        }

        public class Configuration
        {
            [JsonProperty("Rooms")]
            public IList<Room> Rooms { get; set; }
        }
        #endregion
    }
        
}   