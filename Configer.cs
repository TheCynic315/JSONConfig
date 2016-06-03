using System;
using System.Text;
using Crestron.SimplSharp;                         				// For Basic SIMPL# Classes
using Newtonsoft.Json;                                          //Thanks to Neil Colvin. Full Library @ http://www.nivloc.com/downloads/crestron/SSharp/
using Crestron.SimplSharp.CrestronIO;
using System.Collections.Generic;



namespace Config
{

    public class MyConfig
    {
        public string RmName;           //Name of the Room
        public ushort RoomID;           //Control ID or other ID
        public ushort SubAudio;         //Has Audio
        public ushort SubVideo;         //Has Video
        public ushort SubLights;        //Has Lights
        public ushort SubShades;        //Has Shades
        public ushort SubHVAC;          //Has HVAC
        public ushort[] HazSource;      //Which Sources are avaiable?
        public string[] HazSourceName;  //What are the Source's names?
        public ushort[] HazSourceType;  //What is the Source so we can display correct subpage/controls in SIMPL. Rout to an equ
        public int Count;               //Total number of sources found. I'm passing this back to SIMPL+ to make the loop dynamic
      
        

    
/*Pass the FilePath from SIMPL+ then read in the file.
Create the JSON Object and use the library to deserialize it into 
our classes.
*/ 
        public void Reader(string FilePath)
        {

            string DaString;

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

            Configuration Obj = JsonConvert.DeserializeObject<Configuration>(DaString); //All the heavy lifting

            HazSource = new ushort[25];
            HazSourceName = new string[25];
            HazSourceType = new ushort[25];
           

            this.RmName = Obj.RoomName;
            this.RoomID = Obj.ID;
            this.SubAudio = Obj.Audio;
            this.SubVideo = Obj.Video;
            this.SubLights = Obj.Lights;
            this.SubShades = Obj.Shades;
            this.SubHVAC = Obj.HVAC;

            Count = Obj.Sources.Count;

            for (int i = 0; i < Count; i++) //fill in the arrays
            {
                HazSourceName[i] = Obj.Sources[i].Name;
                HazSource[i] = Obj.Sources[i].isUsing;
                HazSourceType[i] = Obj.Sources[i].SourceType;
            }
        }
        
        

        /*Classes built from http://jsonutils.com/
         
         Sources is an array of 24 entries. Can add properties here
         example:
"Sources": [
    {
		"Name": "Cable 1",
		"isUsing": 1,
		"Type": 1
	},{
		"Name": "Cable 2",
		"isUsing": 1,
		"Type": 1
	},{
		"Name": "Cable 3",
		"isUsing": 0,
		"Type": 1
	},{
		"Name": "Cable 4",
		"isUsing": 0,
		"Type": 1
	},{
		"Name": "Apple TV 1",
		"isUsing": 1,
		"Type": 3
         */

        public class Source  
        {
            [JsonProperty("Name")]
            public string Name { get; set; }
            [JsonProperty("isUsing")]
            public ushort isUsing { get; set; }
            [JsonProperty("Type")]
            public ushort SourceType { get; set; }
        }


        /* This is the main object
        This tells the JsonConvert where to put everything  
        */
        public class Configuration
        {
            [JsonProperty("RoomName")]
            public string RoomName { get; set; }
            [JsonProperty("ID")]
            public ushort ID { get; set; }
            [JsonProperty("Audio")]
            public ushort Audio { get; set; }
            [JsonProperty("Video")]
            public ushort Video { get; set; }
            [JsonProperty("Lights")]
            public ushort Lights { get; set; }
            [JsonProperty("Shades")]
            public ushort Shades { get; set; }
            [JsonProperty("HVAC")]
            public ushort HVAC { get; set; }
            [JsonProperty("Sources")]
            public IList<Source> Sources { get; set; }
        }

    }
}   