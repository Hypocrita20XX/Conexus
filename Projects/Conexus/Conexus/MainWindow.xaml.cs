/*
> Darkest Dungeon - Organize Mods
    Created by MatthiosArcanus(Discord)/Hypocrita_2013(Steam)/Hypocrita20XX(GitHub) 
    A GUI-based program designed to streamline the process of organizing mods according to an existing Steam collection

> APIs used:
    Ookii.Dialogs
    Source: http://www.ookii.org/software/dialogs/

> Code used/adapated:
    Function: CopyFolders
    Author: Timm
    Source: http://www.csharp411.com/c-copy-folder-recursively/

> License: MIT
    Copyright (c) 2019 MatthiosArcanus/Hypocrita_2013/Hypocrita20XX

    Permission is hereby granted, free of charge, to any person obtaining a copy
    of this software and associated documentation files (the "Software"), to deal
    in the Software without restriction, including without limitation the rights
    to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
    copies of the Software, and to permit persons to whom the Software is
    furnished to do so, subject to the following conditions:

    The above copyright notice and this permission notice shall be included in all
    copies or substantial portions of the Software.

    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
    SOFTWARE.
*/

#region Using Statements

using Ookii.Dialogs.Wpf;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Windows;
using Path = System.IO.Path;

#endregion

namespace Conexus
{
    public partial class MainWindow : Window
    {
        //Declarations
        //Lists to store info related to the mods that will/are downloaded
        List<string> modInfo;
        List<string> appIDs;
        //Bools to store the value of each checkbox
        bool downloadMods;
        bool updateMods;
        bool saveCredentials;

        public MainWindow()
        {
            InitializeComponent();
            
            //Initialize modInfo and appIDs lists based on the existence of the appropriate text files
            if (File.Exists(UserSettings.Default.ModsDir + "\\_DD_TextFiles\\ModInfo.txt"))
            {
                //Instantiate the lists
                modInfo = new List<string>();
                appIDs = new List<string>();

                //Temp variable to store an individual line
                string line;

                //Create a file reader and load the previously saved ModInfo file
                StreamReader file = new StreamReader(@UserSettings.Default.ModsDir + "\\_DD_TextFiles\\ModInfo.txt");

                //Iterate through the file one line at a time
                while ((line = file.ReadLine()) != null)
                {
                    //Information in the file can be added as-is to the modInfo list
                    modInfo.Add(line);
                    //On the other hand, info specific to the app ID needs extracted
                    //Strip off the index of the folder name, store it
                    line = line.Substring(4);
                    //Strip off the ID, store it
                    line = line.Substring(0, line.IndexOf("_"));
                    //Now store that ID in the appIDs list
                    appIDs.Add(line);
                }
            }
            else
            {
                //if the modInfo file does not exist, instantiate the lists with no data
                modInfo = new List<string>();
                appIDs = new List<string>();
            }

            this.DataContext = this;
        }

        #region Checkbox Functionality

        void DownloadMods_IsChecked(object sender, RoutedEventArgs e)
        {
            //Change local variables accordingly
            downloadMods = true;
            updateMods = false;
            
            //Ensure only the proper checkbox is checked
            DownloadMods.IsChecked = true;
            UpdateMods.IsChecked = false;
        }

        void DownloadMods_IsUnchecked(object sender, RoutedEventArgs e)
        {
            //Change local variables accordingly
            downloadMods = false;
            updateMods = true;

            //Ensure only the proper checkbox is checked
            DownloadMods.IsChecked = false;
            UpdateMods.IsChecked = true;
        }

        void UpdateMods_IsChecked(object sender, RoutedEventArgs e)
        {
            //Change local variables accordingly
            downloadMods = false;
            updateMods = true;

            //Ensure only the proper checkbox is checked
            DownloadMods.IsChecked = false;
            UpdateMods.IsChecked = true;
        }

        void UpdateMods_IsUnchecked(object sender, RoutedEventArgs e)
        {
            //Change local variables accordingly
            downloadMods = true;
            updateMods = false;

            //Ensure only the proper checkbox is checked
            DownloadMods.IsChecked = true;
            UpdateMods.IsChecked = false;
        }

        void SaveCredentials_IsChecked(object sender, RoutedEventArgs e)
        {
            //If the credentials prompt has not been shown/this checkbox was not previously active
            if (UserSettings.Default.CredentialsPrompt)
            {
                //Create a new TaskDialog
                using (TaskDialog prompt = new TaskDialog())
                {
                    //Set an appropriate prompt title
                    prompt.WindowTitle = "Please Verify Saving Your Credentials";
                    //Set the content of the prompt
                    prompt.Content = "Your Steam credentials will NOT be saved with encryption, but will only be saved locally." +
                                     "\n" + "If this is acceptable, press yes." +
                                     "\n" + "If you press no, you will need to re-enter your credentials " +
                                     "\n" + "every time you run this program.";
                    //Create two appropriately labeled buttons
                    TaskDialogButton yesButton = new TaskDialogButton("Yes");
                    TaskDialogButton noButton = new TaskDialogButton("No");
                    //Add them to the prompt
                    prompt.Buttons.Add(yesButton);
                    prompt.Buttons.Add(noButton);
                    //Show the prompt
                    TaskDialogButton button = prompt.ShowDialog(this);

                    //If the yes button is pressed
                    if (button == yesButton)
                    {
                        //Show a confirmation prompt
                        MessageBox.Show(this, "You can change this at any time by unchecking \"Save Steam Credentials\"");
                        //Indicate that the program should now save the user's credentials
                        saveCredentials = true;
                    }

                    //If the no button is pressed
                    if (button == noButton)
                    {
                        //Make sure the checkbox is unchecked
                        SaveCredentials.IsChecked = false;
                    }
                }

                //Ensure the prompt does not show up again (unless the user unchecks and then rechecks this option)
                UserSettings.Default.CredentialsPrompt = false;
                //Save this setting to the settings file
                UserSettings.Default.Save();
            }
            //Otherwise, the checkbox is most likely set at start and the prompt does not need to be shown
            else
                return;
        }

        void SaveCredentials_IsUnchecked(object sender, RoutedEventArgs e)
        {
            //Show relevant info
            MessageBox.Show(this, "Your Steam credentials won't be saved" + 
                                  "\n" + "Please delete the folder located at " + 
                                  "\n" + "C:\\Users\\<Username>\\AppData\\Local\\Conexus");

            //Ensure credentials aren't saved
            saveCredentials = false;
            //Ensure credential prompt is shown next time it's checked
            UserSettings.Default.CredentialsPrompt = true;
            UserSettings.Default.Save();
        }

        #endregion

        #region Button Functionality

        void ModDir_Click(object sender, RoutedEventArgs e)
        {
            //Create a new folder browser to allow easy navigation to the user's desired directory
            VistaFolderBrowserDialog folderBrowser = new VistaFolderBrowserDialog();
            //Show the folder browser
            folderBrowser.ShowDialog();
            //Set a correct description for the browser (seems to be non-functional, low priority to fix)
            folderBrowser.Description = "Mods Directory";
            //Ensure this description is used (seems to be non-functional, low priority to fix)
            folderBrowser.UseDescriptionForTitle = true;
            //Set the content of the button to what the user has selected
            ModDir.Content = folderBrowser.SelectedPath;
            //Set the settings variable to the one selected
            UserSettings.Default.ModsDir = folderBrowser.SelectedPath;
            //Save this setting
            UserSettings.Default.Save();
        }

        void SteamCMDDir_Click(object sender, RoutedEventArgs e)
        {
            //Create a new folder browser to allow easy navigation to the user's desired directory
            VistaFolderBrowserDialog folderBrowser = new VistaFolderBrowserDialog();
            //Show the folder browser
            folderBrowser.ShowDialog();
            //Set a correct description for the browser (seems to be non-functional, low priority to fix)
            folderBrowser.Description = "SteamCMD Directory";
            //Ensure this description is used (seems to be non-functional, low priority to fix)
            folderBrowser.UseDescriptionForTitle = true;
            //Set the content of the button to what the user has selected
            SteamCMDDir.Content = folderBrowser.SelectedPath;
            //Set the settings variable to the one selected
            UserSettings.Default.SteamCMDDir = folderBrowser.SelectedPath;
            //Save this setting
            UserSettings.Default.Save();
        }

        #endregion

        #region Main Functionality 

        //Main workhorse function
        void OrganizeMods_Click(object sender, RoutedEventArgs e)
        {
            //It is assumed that at this point, the user has entered a valid URL to the collection
            if (URLLink.Text.Length > 0)
            {
                //Set the setting's file variable to the correct URL
                UserSettings.Default.CollectionURL = URLLink.Text;
                //Save this setting
                UserSettings.Default.Save();
            }
            //Otherwise we need to quit and provide an error message
            else
            {
                //Provide a clear reason for aborting the process
                OrganizeMods.Content = "Invalid URL, process has now stopped.";
                //Exit out of this function
                return;
            }

            //If the user desires that their credentials are saved (it is assumed that the user has entered valid Steam credentials)
            if (saveCredentials)
            {
                //Check the length of the SteamUsername field
                if (SteamUsername.Text.Length > 0)
                {
                    //If the length is greater than 0 (has data) then set that to the settings file variable
                    UserSettings.Default.SteamUsername = SteamUsername.Text;
                    //Save that data to the file
                    UserSettings.Default.Save();
                }
                //Check the length of the SteamPassword field
                if (SteamPassword.Text.Length > 0)
                {
                    //If the length is greater than 0 (has data) then set that to the settings file variable
                    UserSettings.Default.SteamPassword = SteamPassword.Text;
                    //Save that data to the file
                    UserSettings.Default.Save();
                }
            }

            //Check if Steam can log in
            //We'll do this by downloading a single mod, then checking to make sure it was indeed downloaded, and deleting it afterwards
            //Create a process that will contain all relevant SteamCMD commands for all mods
            ProcessStartInfo processInfo = new ProcessStartInfo(UserSettings.Default.SteamCMDDir + "\\steamcmd.exe", "+login " + SteamUsername.Text + " " + SteamPassword.Text + " " + "+\"workshop_download_item 262060 " + "879079478" + "\" " + "validate " + "+quit");

            //Create a wrapper that will run all commands, wait for the process to finish, and then proceed to copying and renaming folders/files
            using (Process process = new Process())
            {
                //Set the commands for this process
                process.StartInfo = processInfo;
                //Start the commandline process
                process.Start();
                //Wait until SteamCMD finishes
                process.WaitForExit();

                //If the directory does not exist, it's safe to assume that Steam could not log in, return and print an appropriate message
                if (!Directory.Exists(UserSettings.Default.SteamCMDDir + "\\steamapps\\workshop\\content\\262060\\879079478"))
                {
                    OrganizeMods.Content = "Invalid Steam credentials, process has now stopped.";
                    return;
                }

                //Otherwise the credentials are valid, and the downloaded file needs deleted
                if (Directory.Exists(UserSettings.Default.SteamCMDDir + "\\steamapps\\workshop\\content\\262060\\879079478"))
                    Directory.Delete(UserSettings.Default.SteamCMDDir + "\\steamapps\\workshop\\content\\262060\\879079478", true);

                //If the user wants to download mods, send them through that chain
                if (downloadMods)
                {
                        //Create all necessary text files
                        DownloadHTML(UserSettings.Default.CollectionURL, UserSettings.Default.ModsDir + "\\_DD_TextFiles");
                        //Start downloading mods
                        DownloadModsFromSteam();
                }

                //If the user wants to update mods, send them through that chain so long as they've run through the download chain once
                if (updateMods && File.Exists(UserSettings.Default.ModsDir + "\\_DD_TextFiles\\ModInfo.txt"))
                    UpdateModsFromSteam();
                //Otherwise the user needs to download and create all relevant text files
                else if (updateMods && File.Exists(UserSettings.Default.ModsDir + "\\_DD_TextFiles\\ModInfo.txt"))
                    DownloadHTML(UserSettings.Default.CollectionURL, UserSettings.Default.ModsDir + "\\_DD_TextFiles");
            }
        }

        //Download source HTML from a given Steam collection URL
        void DownloadHTML(string url, string fileDir)
        {
            //If the _DD_TextFiles folder does not exist, create it
            if (!Directory.Exists(fileDir))
                Directory.CreateDirectory(fileDir);

            //Create a new WebClient
            WebClient webClient = new WebClient();
            //Download the desired collection and save the file
            webClient.DownloadFile(url, fileDir + "\\HTML.txt");
            //Move on to parsing through the raw source
            IterateThroughHTML(fileDir);
        }

        //Go through the source line by line
        void IterateThroughHTML(string fileDir)
        {
            //Temp variable to store an individual line
            string line;
            //List of strings to store a line that houses all neccesary info for each mod
            List<string> mods = new List<string>();
            //Create a file reader and load the previously saved source file
            StreamReader file = new StreamReader(@fileDir + "\\HTML.txt");

            //Iterate through the file one line at a time
            while((line = file.ReadLine()) != null)
            {
                //If a line contains "a href" and "workshopItemTitle," then this line contains mod information
                if (line.Contains("a href") & line.Contains("workshopItemTitle"))
                {
                    //Add this line to the mods list
                    mods.Add(line.Substring(line.IndexOf("<")));
                }
            }

            //Write this information to a file
            WriteToFile(mods.ToArray(), fileDir + "\\Mods.txt");
            //Move on to parsing out the relevant info
            SeparateInfo(fileDir);
        }

        //Parses out all relevant info from the source's lines
        void SeparateInfo(string fileDir)
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
                modInfo.Add(final);

                //Add the app id to the appIDs list
                appIDs.Add(id);

                //Increment folderIndex
                folderIndex++;
            }

            //Write the modInfo to a text file
            WriteToFile(modInfo.ToArray(), @fileDir + "\\ModInfo.txt");
        }

        void DownloadModsFromSteam()
        {
            //Stores the proper commands that will be passed to SteamCMD
            string cmdList = "";

            //Get a list of commamds for each mod stored in a single string
            for (int i =0; i < appIDs.Count; i++)
                cmdList += "+\"workshop_download_item 262060 " + appIDs[i] + "\" " + "validate ";

            string exe = UserSettings.Default.SteamCMDDir + "\\steamcmd.exe";
            string cmd = "+login " + UserSettings.Default.SteamUsername + " " + UserSettings.Default.SteamPassword + " " + cmdList + "+quit";

            //Create a process that will contain all relevant SteamCMD commands for all mods
            ProcessStartInfo processInfo = new ProcessStartInfo(UserSettings.Default.SteamCMDDir + "\\steamcmd.exe", "+login " + UserSettings.Default.SteamUsername + " " + UserSettings.Default.SteamPassword + " " + cmdList + "+quit");

            //Create a wrapper that will run all commands, wait for the process to finish, and then proceed to copying and renaming folders/files
            using (Process process = new Process())
            {
                //Set the commands for this process
                process.StartInfo = processInfo;
                //Start the process with the provided commands
                process.Start();
                //Wait until SteamCMD finishes
                process.WaitForExit();
                //Move on to copying and renaming the mods
                RenameAndMoveMods("DOWNLOAD");
            }
        }

        void UpdateModsFromSteam()
        {
            //Move all mods from the mods directory to the SteamCMD directory for updating.
            RenameAndMoveMods("UPDATE");

            //Stores the proper commands that will be passed to SteamCMD
            string cmdList = "";

            //Get a list of commamds for each mod stored in a single string
            for (int i = 0; i < appIDs.Count; i++)
                cmdList += "+\"workshop_download_item 262060 " + appIDs[i] + "\" " + "validate ";

            //Create a process that will contain all relevant SteamCMD commands for all mods
            ProcessStartInfo processInfo = new ProcessStartInfo(UserSettings.Default.SteamCMDDir + "\\steamcmd.exe", "+login " + UserSettings.Default.SteamUsername + " " + UserSettings.Default.SteamPassword + " " + cmdList + "+quit");

            //Create a wrapper that will run all commands, wait for the process to finish, and then proceed to copying and renaming folders/files
            using (Process process = new Process())
            {
                //Set the commands for this process
                process.StartInfo = processInfo;
                //Start the commandline process
                process.Start();
                //Wait until SteamCMD finishes
                process.WaitForExit();
                //Move on to copying and renaming the mods
                RenameAndMoveMods("DOWNLOAD");
            }
        }

        //Creates organized folders in the mods directory, then copies files from the SteaCMD directory to those folders
        //Requires that an operation be specified (DOWNLOAD or UPDATE)
        void RenameAndMoveMods(string DownloadOrUpdate)
        {
            //Create source/destination path list variables
            string[] source = new string[appIDs.Count];
            string[] destination = new string[modInfo.Count];

            //If the user has downloaded/Updated mods, copy all files/folders from the SteamCMD directory to the mod directory
            if (DownloadOrUpdate == "DOWNLOAD")
            {
                //Get the proper path to copy from
                for (int i = 0; i < appIDs.Count; i++)
                    source[i] = Path.Combine(UserSettings.Default.SteamCMDDir + "\\steamapps\\workshop\\content\\262060\\", appIDs[i]);

                //Get the proper path that will be copied to
                for (int i = 0; i < modInfo.Count; i++)
                    destination[i] = Path.Combine(UserSettings.Default.ModsDir, modInfo[i]);

                //Copy all folders/files from the SteamCMD directory to the mods directory
                for (int i = 0; i < destination.Length; i++)
                    CopyFolders(source[i], destination[i]);

                //Check to ensure the last mod is in the destination directory
                if (Directory.Exists(destination[modInfo.Count - 1]))
                {
                    //If so, delete all folders/files in the source destination
                    for (int i = 0; i < appIDs.Count; i++)
                    {
                        //Delete the directory
                        Directory.Delete(source[i], true);
                    }
                }
            }

            //If the userwants to update mods, copy all files/folders from the mod directory to the SteamCMD directory
            if (DownloadOrUpdate == "UPDATE")
            {
                //Get the proper path to copy from
                for (int i = 0; i < appIDs.Count; i++)
                    source[i] = Path.Combine(UserSettings.Default.ModsDir + "\\", modInfo[i]);

                //Get the proper path that will be copied to
                for (int i = 0; i < modInfo.Count; i++)
                    destination[i] = Path.Combine(UserSettings.Default.SteamCMDDir + "\\steamapps\\workshop\\content\\262060\\", appIDs[i]);

                //Copy all folders/files from the SteamCMD directory to the mods directory
                for (int i = 0; i < destination.Length; i++)
                    CopyFolders(source[i], destination[i]);

                //Check to ensure the last mod is in the destination directory
                if (Directory.Exists(destination[modInfo.Count - 1]))
                {
                    //If so, delete all folders/files in the source destination
                    for (int i = 0; i < appIDs.Count; i++)
                    {
                        //Delete the directory
                        Directory.Delete(source[i], true);
                    }
                }
            }

            //Indicate to the user that the desired process is finished
            OrganizeMods.Content = "Process has finished";
        }

        //A base function that will copy/rename any given folder(s)
        //Can be used recursively for multiple directories
        void CopyFolders(string source, string destination)
        {
            //Check if the directory exists, if not, create it
            if (!Directory.Exists(destination))
                Directory.CreateDirectory(destination);

            //Create an array of strings containing all files in the given source directory
            string[] files = Directory.GetFiles(source);

            //Iterate through these files and copy to the destination
            foreach (string file in files)
            {
                //Get the name of the file
                string name = Path.GetFileName(file);
                //Get the destination for this file
                string dest = Path.Combine(destination, name);
                //Copy this file to the destination
                File.Copy(file, dest, true);
            }

            //Create an array of strings containing any and all sub-directories
            string[] folders = Directory.GetDirectories(source);

            //Iterate through these sub-directories
            foreach (string folder in folders)
            {
                //Get the name of the folder
                string name = Path.GetFileName(folder);
                //Get the destination for this folder
                string dest = Path.Combine(destination, name);

                //Recursively copy any files in this directory, any sub-directories, and all files therein
                CopyFolders(folder, dest);
            }
        }

        //Utility function to write text to a file
        void WriteToFile(string[] text, string fileDir)
        {
            File.WriteAllLines(@fileDir, text);
        }

        #endregion

        #region Data Saving Functionality

        //Called when the UI window has loaded, used to set proper info in the UI from the settings file
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //Set the default process to be downloading mods
            DownloadMods.IsChecked = true;

            //Ensure user.settings exists before checking its data
            if (UserSettings.Default.CollectionURL == null)
                UserSettings.Default.CollectionURL = "";

            //Check the length of the URL variable in the settings file, if so, set it to the UI variable
            if (UserSettings.Default.CollectionURL.Length > 0)
                URLLink.Text = UserSettings.Default.CollectionURL;

            //Check the length of the SteamCMD variable in the settings file, if so, set it to the UI variable
            if (UserSettings.Default.SteamCMDDir.Length > 0)
                SteamCMDDir.Content = UserSettings.Default.SteamCMDDir;

            //Check the length of the ModsDir variable in the settings file, if so, set it to the UI variable
            if (UserSettings.Default.ModsDir.Length > 0)
                ModDir.Content = UserSettings.Default.ModsDir;

            //Check the length of the SteamUsername variable in the settings file
            if (UserSettings.Default.SteamUsername.Length > 0)
            {
                //If so, set it to the UI variable
                SteamUsername.Text = UserSettings.Default.SteamUsername;
                //Indicate to the user (through the checkbox) that their credentials are and will be saved
                //This check only needs to be done once, as technically the credentials are the same (they both are sensitive data and handled as a pair)
                SaveCredentials.IsChecked = true;
            }
            //Otherwise, make sure the user (through the checkbox state) knows that their username will not be saved
            else
                SaveCredentials.IsChecked = false;

            //Check the length of the SteamPassword variable in the settings file, if so, set it to the UI variable
            if (UserSettings.Default.SteamPassword.Length > 0)
                SteamPassword.Text = UserSettings.Default.SteamPassword;
        }

        //Called right after the user indicates they want to close the program (through the use of the "X" button)
        //Used to ensure all proper data is set to their corrosponding variables in the settings file
        private void Window_Closing(object sender, EventArgs e)
        {
            //Check to ensure the URLLink content is indeed provided (length greater than 0 indicates data in the field)
            //Make sure the variable in the settings file is correct
            if (URLLink.Text.Length > 0)
                UserSettings.Default.CollectionURL = URLLink.Text;

            //Check to ensure the SteamCMDDir content is indeed provided (length greater than 0 indicates data in the field)
            //Make sure the variable in the settings file is correct
            if (SteamCMDDir.Content != null)
                UserSettings.Default.SteamCMDDir = SteamCMDDir.Content.ToString();

            //Check to ensure the ModDir content is indeed provided (length greater than 0 indicates data in the field)
            //Make sure the variable in the settings file is correct
            if (ModDir.Content != null)
                UserSettings.Default.ModsDir = ModDir.Content.ToString();

            //If the user wants to save their credentials
            if (saveCredentials)
            {
                //Ensure that the field has data, then make sure the settings variable is correct 
                if (SteamUsername.Text.Length > 0)
                    UserSettings.Default.SteamUsername = SteamUsername.Text;

                //Ensure that the field has data, then make sure the settings variable is correct
                if (SteamPassword.Text.Length > 0)
                    UserSettings.Default.SteamPassword = SteamPassword.Text;
            }
        }

        //Final call that happens right after the window starts to close
        //Used to save all relevant data to the settings file
        private void Window_Closed(object sender, EventArgs e)
        {
            //Save all data to the settings file
            UserSettings.Default.Save();
        }

        #endregion
    }
}
