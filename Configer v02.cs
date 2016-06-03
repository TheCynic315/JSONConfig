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
        public string[] HazHvacLightZone;    //Hvac or Light Zone Name
        public int Count;               //Total number of sources found. I'm passing this back to SIMPL+ to make the loop dynamic
        private string DaString;
        private Configuration Obj;
        private SourceList MysList;
        private HVACandLIGHTList myHVACandLIGHTList;
        
      
        

    
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
            HazSourceName = new string[50];

            Obj = JsonConvert.DeserializeObject<Configuration>(DaString); //All the heavy lifting
            
            this.RmName = Obj.room[ConnectTo].RoomName;
            this.RoomID = Obj.room[ConnectTo].ControlID;
            this.AudioEquipID = Obj.room[ConnectTo].AudioEquipID;
            this.VideoEquipID = Obj.room[ConnectTo].VideoEquipID;
            this.SubAudio = Obj.room[ConnectTo].Audio;
            this.SubVideo = Obj.room[ConnectTo].Video;
            this.SubLights = Obj.room[ConnectTo].Lights;
            this.SubShades = Obj.room[ConnectTo].Shades;
            this.SubHVAC = Obj.room[ConnectTo].HVAC;

            Count = Obj.room[ConnectTo].Sources.Count;

            for (ushort i = 0; i < Count; i++) //fill in the arrays
            {
                HazSourceName[i] = Obj.room[ConnectTo].Sources[i].Name;
                HazSource[i] = Obj.room[ConnectTo].Sources[i].isUsing;

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

        public class ListOfSource : myList
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

        #region HVACandLIGHT
        public void BuildHVACandLIGHT()
        {
            myHVACandLIGHTList = JsonConvert.DeserializeObject<HVACandLIGHTList>(DaString);
        }

        public string HVACandLIGHTZoneName(ushort Zone)
        {
            return myHVACandLIGHTList.HVACandLIGHTZones[Zone].Name;
        }

        public ushort HVACandLIGHTEquipID(ushort zone)
        {
            return myHVACandLIGHTList.HVACandLIGHTZones[zone].EquipID;
        }

        public class HVACandLIGHTList
        {
            public IList<myHVACLightList> HVACandLIGHTZones { get; set; }
        }

        public class myHVACLightList : myList
        {
            public ushort EquipID;
        }
        #endregion
        #region Main
        //Classes built from http://jsonutils.com/
       

        public class myList
        {
            public string Name { get; set; }
            public ushort isUsing { get; set; }
                      
        }

          
        /* This is the main object
        This tells the JsonConvert where to put everything  
        */
        public class Room
        {
            public string RoomName { get; set; }
            public ushort ControlID { get; set; }
            public ushort AudioEquipID { get; set; }
            public ushort VideoEquipID { get; set; }
            public ushort Audio { get; set; }
            public ushort Video { get; set; }
            public ushort Lights { get; set; }
            public ushort Shades { set; get; }
            public ushort HVAC { get; set; }
            public IList<myList> Sources { get; set; }
        }

        public class Configuration
        {
            public IList<Room> room{ get; set; }
        }
        #endregion
    }
        
}   