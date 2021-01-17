using Conexus.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Conexus.Core
{
    public class DataProcesses
    {
        //Download source HTML from a given Steam collection URL
        public void DownloadHTML(string url, string fileDir)
        {
            //If the _DD_TextFiles folder does not exist, create it
            if (!Directory.Exists(fileDir))
                Directory.CreateDirectory(fileDir);

            //Overwrite whatever may be in ModInfo.txt, if it exists
            if (File.Exists(UserSettings.Default.ModsDir + "\\_DD_TextFiles\\ModInfo.txt"))
                File.WriteAllText(UserSettings.Default.ModsDir + "\\_DD_TextFiles\\ModInfo.txt", String.Empty);

            //Create a new WebClient
            WebClient webClient = new WebClient();
            //Download the desired collection and save the file
            webClient.DownloadFile(url, fileDir + "\\HTML.txt");
            //Added v1.2.0
            //Free up resources, cleanup
            webClient.Dispose();
            //Move on to parsing through the raw source
            IterateThroughHTML(fileDir);
        }

        //Go through the source line by line
        public void IterateThroughHTML(string fileDir)
        {
            //Temp variable to store an individual line
            string line;
            //List of strings to store a line that houses all neccesary info for each mod
            List<string> mods = new List<string>();
            //Create a file reader and load the previously saved source file
            StreamReader file = new StreamReader(@fileDir + "\\HTML.txt");

            //Iterate through the file one line at a time
            while ((line = file.ReadLine()) != null)
            {
                //If a line contains "a href" and "workshopItemTitle," then this line contains mod information
                if (line.Contains("a href") & line.Contains("workshopItemTitle"))
                {
                    //Add this line to the mods list
                    mods.Add(line.Substring(line.IndexOf("<")));
                }
            }

            //Added v1.2.0
            //Close file, cleanup
            file.Close();

            //Write this information to a file
            Variables.fileOps.WriteToFile(mods.ToArray(), fileDir + "\\Mods.txt");
            //Move on to parsing out the relevant info
            SeparateInfo(fileDir);
        }

        //Parses out all relevant info from the source's lines
        public void SeparateInfo(string fileDir)
        {
            //Temp variable to store an individual line
            string line;
            //Stores the initial folder index
            int folderIndex = 0;
            //Stores the final folder index (with leading zeroes)
            string folderIndex_S = "";
            //Load the previously stored file for further refinement
            StreamReader file = new StreamReader(@fileDir + "\\Mods.txt");

            //Iterate through each line the file
            while ((line = file.ReadLine()) != null)
            {
                //First pass, remove everything up to ?id=
                string firstPass = line.Substring(line.IndexOf("?id="));
                //Second pass, remove everything after </div>
                string secondPass = firstPass.Substring(0, firstPass.IndexOf("</div>"));
                //Strip the app id from the string and store that in its own variables
                string id = secondPass.Substring(0, secondPass.IndexOf("><div") - 1);
                //Remove remaining fluff from the id string
                id = id.Substring(4);
                //Strip the mod name from the string and store that in its own variable
                string name = secondPass.Substring(secondPass.IndexOf("\"workshopItemTitle\">") + ("\"workshopItemTitle\">").Length);
                //Remove any invalid characters in the mod name
                name = Regex.Replace(name, @"['<''>'':''/''\''|''?''*']", "", RegexOptions.None);

                //Add leading zeroes to the folder index, two if the index is less than 10
                if (folderIndex < 10)
                    folderIndex_S = "00" + folderIndex.ToString();

                //Add leading zeroes to the folder index, one if the index is more than 9 and less than 100
                if (folderIndex > 9 & folderIndex < 100)
                    folderIndex_S = "0" + folderIndex.ToString();

                //If the index is greater than 100, no leading zeroes should be added
                if (folderIndex > 100)
                    folderIndex_S = folderIndex.ToString();

                //Create the final name that will be used to identify the folder/mod
                string final = folderIndex_S + "_" + id + "_" + name;

                //Add the final name to the modInfo list
                Variables.modInfo.Add(final);

                //Add the app id to the appIDs list
                Variables.appIDs.Add(id);

                //Increment folderIndex
                folderIndex++;
            }

            //Added v1.2.0
            //Close file, cleanup
            file.Close();

            //Write the modInfo to a text file
            Variables.fileOps.WriteToFile(Variables.modInfo.ToArray(), @fileDir + "\\ModInfo.txt");
        }

        public void ParseFromList(string fileDir)
        {
            //Examples:
            // > Format: https://steamcommunity.com/sharedfiles/filedetails/?id=1282438975
            // > Ignore: * 50% Stealth Chance in Veteran Quests

            //Overwrite whatever may be in ModInfo.txt, if it exists
            if (File.Exists(UserSettings.Default.ModsDir + "\\_DD_TextFiles\\ModInfo.txt"))
                File.WriteAllText(UserSettings.Default.ModsDir + "\\_DD_TextFiles\\ModInfo.txt", String.Empty);

            //Temp variable to store an individual line
            string line;
            //Stores the initial folder index
            int folderIndex = 0;
            //Stores the final folder index (with leading zeroes)
            string folderIndex_S = "";
            //Load the previously stored file for further refinement
            StreamReader file = new StreamReader(@fileDir + "\\Links.txt");

            //Iterate through each line the file
            while ((line = file.ReadLine()) != null)
            {
                //If the line being looked at is a comment, marked by *, then skip this line
                //Otherwise, we need to get the ID from this line
                if (!line.Contains("*"))
                {
                    //Remove everything up to ?id=, plus 4 to remove ?id= in the link
                    string id = line.Substring(line.IndexOf("?id=") + 4);

                    //Add leading zeroes to the folder index, two if the index is less than 10
                    if (folderIndex < 10)
                        folderIndex_S = "00" + folderIndex.ToString();

                    //Add leading zeroes to the folder index, one if the index is more than 9 and less than 100
                    if (folderIndex > 9 & folderIndex < 100)
                        folderIndex_S = "0" + folderIndex.ToString();

                    //If the index is greater than 100, no leading zeroes should be added
                    if (folderIndex > 100)
                        folderIndex_S = folderIndex.ToString();

                    //Add the final name to the modInfo list
                    Variables.modInfo.Add(folderIndex_S + "_" + id);

                    //Add this ID to the appIDs list
                    Variables.appIDs.Add(id);

                    //Increment folderIndex
                    folderIndex++;
                }
            }

            //Write the modInfo to a text file if the file doesn't exist
            if (!File.Exists(UserSettings.Default.ModsDir + "\\_DD_TextFiles\\ModInfo.txt"))
                Variables.fileOps.WriteToFile(Variables.modInfo.ToArray(), @fileDir + "\\_DD_TextFiles\\ModInfo.txt");

            //Added v1.2.0
            //Close file, cleanup
            file.Close();
        }
    }
}
