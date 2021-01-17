/*
> Darkest Dungeon - Organize Mods
    Created by MatthiosArcanus(Discord)/Hypocrita_2013(Steam)/Hypocrita20XX(GitHub) 
    A GUI-based program designed to streamline the process of organizing mods according to an existing Steam collection or list of links
    Handles downloading and updating mods through the use of SteamCMD (https://developer.valvesoftware.com/wiki/SteamCMD)

> APIs used:
    Ookii.Dialogs
    Source: http://www.ookii.org/software/dialogs/

    Extended WPF Toolkit
    Source: https://github.com/xceedsoftware/wpftoolkit

> Code used/adapated:
    Function: CopyFolders
    Author: Timm
    Source: http://www.csharp411.com/c-copy-folder-recursively/

    Function: password reveal functionality
    Author: DaisyTian-MSFT
    Source: https://docs.microsoft.com/en-us/answers/questions/99602/wpf-passwordbox-passwordrevealmode-was-not-found-i.html

> License: MIT
    Copyright (c) 2019, 2020, 2021 MatthiosArcanus/Hypocrita_2013/Hypocrita20XX

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

using Conexus.Core;
using Ookii.Dialogs.Wpf;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Path = System.IO.Path;

#endregion

namespace Conexus
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            
            this.DataContext = this;
        }

        #region ComboBox Functionality

        private void Mode_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            //Get the index of the selected item
            //0 = download
            //1 = update
            int i = cmbMode.SelectedIndex;

            //If the user wishes to download mods
            if (i == 0)
            {
                //Change local variables accordingly
                Variables.downloadMods = true;
                Variables.updateMods = false;
            }

            //If the user wishes to update their existing mods
            if (i == 1)
            {
                //Change local variables accordingly
                Variables.downloadMods = false;
                Variables.updateMods = true;
            }
        }

        private void Platform_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            //Get the index of the selected item
            //0 = steam
            //1 = list
            int i = cmbPlatform.SelectedIndex;

            //If the user is using Steam
            if (i == 0)
                //Change local variables accordingly
                Variables.steam = true;

            //If the user is using a list
            if (i == 1)
                //Change local variables accordingly
                Variables.steam = false;
        }

        #endregion

        #region Button Functionality

        private void ModDir_Click(object sender, RoutedEventArgs e)
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

            //Added v1.2.0
            //Verify that the provided directory is valid, not empty, and contains Darkest.exe
            if (Verification.VerifyModDir(folderBrowser.SelectedPath))
            {
                //Set the settings variable to the one selected
                UserSettings.Default.ModsDir = folderBrowser.SelectedPath;
                //Save this setting
                UserSettings.Default.Save();
            }
            //If the given path is invalid, let the user know why
            else
            {
                //If the given path contains any info, provide that with the error message
                if (folderBrowser.SelectedPath.Length > 0)
                    ModDir.Content = "Invalid mods location: " + folderBrowser.SelectedPath;
                //If the given path is blank, provide that information
                else
                    ModDir.Content = "Invalid mods Location: no path given";
            }
        }

        private void SteamCMDDir_Click(object sender, RoutedEventArgs e)
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

            //Added v1.2.0
            //Verify that the provided directory is valid, not empty, and contains steamcmd.exe
            if (Verification.VerifySteamCMDDir(folderBrowser.SelectedPath))
            {
                //Set the settings variable to the one selected
                UserSettings.Default.SteamCMDDir = folderBrowser.SelectedPath;
                //Save this setting
                UserSettings.Default.Save();
            }
            //If the given path is invalid, let the user know why
            else
            {
                //If the given path contains any info, provide that with the error message
                if (folderBrowser.SelectedPath.Length > 0)
                    SteamCMDDir.Content = "Invalid SteamCMD location: " + folderBrowser.SelectedPath;
                //If the given path is blank, provide that information
                else
                    SteamCMDDir.Content = "Invalid SteamCMD location: no path given";
            }
        }

        #endregion

        #region Checkbox Functionality

        //Added v1.2.0
        //Reveals the username when the checkbox is checked
        private void UsernameReveal_Checked(object sender, RoutedEventArgs e)
        {
            //Assign the text in the textbox to the given password
            SteamUsername_TextBox.Text = SteamUsername.Password;
            //Hide the password box
            SteamUsername.Visibility = Visibility.Collapsed;
            //Show the text box
            SteamUsername_TextBox.Visibility = Visibility.Visible;
        }

        //Added v1.2.0
        //Hides the username when the checkbox is unchecked
        private void UsernameReveal_Unchecked(object sender, RoutedEventArgs e)
        {
            //Assign the text in the password to what's in the text box
            SteamUsername.Password = SteamUsername_TextBox.Text;
            //Hide the text box
            SteamUsername_TextBox.Visibility = Visibility.Collapsed;
            //Show the password box
            SteamUsername.Visibility = Visibility.Visible;
        }

        //Added v1.2.0
        //Reveals the password when the checkbox is unchecked
        private void PasswordReveal_Checked(object sender, RoutedEventArgs e)
        {
            //Assign the text in the textbox to the given password
            SteamPassword_TextBox.Text = SteamPassword.Password;
            //Hide the password box
            SteamPassword.Visibility = Visibility.Collapsed;
            //Show the text box
            SteamPassword_TextBox.Visibility = Visibility.Visible;
        }

        //Added v1.2.0
        //Hides the password when the checkbox is unchecked
        private void PasswordReveal_Unchecked(object sender, RoutedEventArgs e)
        {
            //Assign the text in the password to what's in the text box
            SteamPassword.Password = SteamPassword_TextBox.Text;
            //Hide the text box
            SteamPassword_TextBox.Visibility = Visibility.Collapsed;
            //Show the password box
            SteamPassword.Visibility = Visibility.Visible;
        }

        #endregion

        //Main workhorse function
        private void OrganizeMods_Click(object sender, RoutedEventArgs e)
        {
            //If this directory is deleted or otherwise not found, it needs to be created, otherwise stuff will break
            if (!Directory.Exists(UserSettings.Default.ModsDir + "\\_DD_TextFiles"))
                Directory.CreateDirectory(UserSettings.Default.ModsDir + "\\_DD_TextFiles");

            //If the user wants to use a Steam collection, ensure all functionality relates to that
            if (Variables.steam)
            {
                //Check the provided URL to make sure it's valid
                if (Verification.VerifyCollectionURL(URLLink.Text, UserSettings.Default.ModsDir + "\\_DD_TextFiles"))
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

                    //If the user wants to download mods, send them through that chain
                    if (Variables.downloadMods)
                    {
                        //Create all necessary text files
                        DataProcesses.DownloadHTML(UserSettings.Default.CollectionURL, UserSettings.Default.ModsDir + "\\_DD_TextFiles");
                        //Start downloading mods
                       SteamProcesses.DownloadModsFromSteam();
                    }

                    //If the user wants to update mods, send them through that chain so long as they've run through the download chain once
                    if (Variables.updateMods && File.Exists(UserSettings.Default.ModsDir + "\\_DD_TextFiles\\ModInfo.txt"))
                        SteamProcesses.UpdateModsFromSteam();
                    //Otherwise the user needs to download and create all relevant text files
                    else if (Variables.updateMods && !File.Exists(UserSettings.Default.ModsDir + "\\_DD_TextFiles\\ModInfo.txt"))
                    {
                        DataProcesses.DownloadHTML(UserSettings.Default.CollectionURL, UserSettings.Default.ModsDir + "\\_DD_TextFiles");
                        SteamProcesses.UpdateModsFromSteam();
                    }
                }
                //URL is not valid, don't do anything
                else
                    return;
            }
            //Otherwise, the user wants to use a list of URLs
            else
            {
                //If the user wants to download mods, send them through that chain
                if (Variables.downloadMods)
                {
                    //Parse IDs from the user-populated list
                    DataProcesses.ParseFromList(UserSettings.Default.ModsDir);

                    SteamProcesses.DownloadModsFromSteam();
                }

                //If the user wants to update mods, send them through that chain
                if (Variables.updateMods && File.Exists(UserSettings.Default.ModsDir + "\\_DD_TextFiles\\ModInfo.txt"))
                    SteamProcesses.UpdateModsFromSteam();
                //Otherwise the user needs to download and create all relevant text files
                else if (Variables.updateMods && !File.Exists(UserSettings.Default.ModsDir + "\\_DD_TextFiles\\ModInfo.txt"))
                {
                    DataProcesses.ParseFromList(UserSettings.Default.ModsDir);
                    SteamProcesses.UpdateModsFromSteam();
                }
            }
        }

        #region Data Saving And Loading

        //Called when the UI window has loaded, used to set proper info in the UI from the settings file
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
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

            //Added v1.2.0
            //Ensure Steam username exists before checking its data
            if (UserSettings.Default.SteamUsername == null)
                UserSettings.Default.SteamUsername = "";

            //Added v1.2.0
            //Check the length of the username variable in the settings file, if so, set it to the UI variable
            if (UserSettings.Default.SteamUsername.Length > 0)
                SteamUsername.Password = UserSettings.Default.SteamUsername;

            //Added v1.2.0
            //Ensure Steam password exists before checking its data
            if (UserSettings.Default.SteamPassword == null)
                UserSettings.Default.SteamPassword = "";

            //Added v1.2.0
            //Check the length of the password variable in the settings file, if so, set it to the UI variable
            if (UserSettings.Default.SteamPassword.Length > 0)
                SteamPassword.Password = UserSettings.Default.SteamPassword;

            //Check the platform variable and set the platform combobox accordingly
            if (UserSettings.Default.Platform == "steam")
            {
                cmbPlatform.SelectedIndex = 0;
                Variables.steam = true;
            }
            else if (UserSettings.Default.Platform == "other")
            {
                cmbPlatform.SelectedIndex = 1;
                Variables.steam = false;
            }

            //Make sure that Links.txt exists
            if (!File.Exists(UserSettings.Default.ModsDir + "\\Links.txt"))
                File.Create(UserSettings.Default.ModsDir + "\\Links.txt").Dispose();

            //Initialize modInfo and appIDs lists based on the existence of the appropriate text files
            if (File.Exists(UserSettings.Default.ModsDir + "\\_DD_TextFiles\\ModInfo.txt"))
            {
                //Instantiate the lists
                Variables.modInfo = new List<string>();
                Variables.appIDs = new List<string>();

                //Temp variable to store an individual line
                string line;

                //Create a file reader and load the previously saved ModInfo file
                StreamReader file = new StreamReader(@UserSettings.Default.ModsDir + "\\_DD_TextFiles\\ModInfo.txt");

                if (Variables.steam)
                {
                    //Iterate through the file one line at a time
                    while ((line = file.ReadLine()) != null)
                    {
                        //Information in the file can be added as-is to the modInfo list
                        Variables.modInfo.Add(line);
                        //On the other hand, info specific to the app ID needs extracted
                        //Strip off the index of the folder name, store it
                        line = line.Substring(4);
                        //Strip off the ID, store it
                        line = line.Substring(0, line.IndexOf("_"));
                        //Now store that ID in the appIDs list
                        Variables.appIDs.Add(line);
                    }
                }
                else
                {
                    //Iterate through the file one line at a time
                    while ((line = file.ReadLine()) != null)
                    {
                        //Information in the file can be added as-is to the modInfo list
                        Variables.modInfo.Add(line);
                        //On the other hand, info specific to the app ID needs extracted
                        //Strip off the index of the folder name, store it
                        line = line.Substring(4);
                        //Strip off the ID, store it
                        //line = line.Substring(0, line.IndexOf("_"));
                        //Now store that ID in the appIDs list
                        Variables.appIDs.Add(line);
                    }
                }
            }
            else
            {
                //if the modInfo file does not exist, instantiate the lists with no data
                Variables.modInfo = new List<string>();
                Variables.appIDs = new List<string>();
            }

            if (!Directory.Exists(UserSettings.Default.ModsDir + "\\_DD_TextFiles"))
                Directory.CreateDirectory(UserSettings.Default.ModsDir + "\\_DD_TextFiles");
        }

        //Called right after the user indicates they want to close the program (through the use of the "X" button)
        //Used to ensure all proper data is set to their corrosponding variables in the settings file
        private void Window_Closing(object sender, EventArgs e)
        {
            //Check to ensure the URLLink content is indeed provided (length greater than 0 indicates data in the field)
            //Make sure the variable in the settings file is correct
            if (URLLink.Text.Length > 0)
                UserSettings.Default.CollectionURL = Variables.main.URLLink.Text;

            //Check to ensure the SteamCMDDir content is indeed provided (length greater than 0 indicates data in the field)
            //Make sure the variable in the settings file is correct
            if (SteamCMDDir.Content != null)
                UserSettings.Default.SteamCMDDir = Variables.main.SteamCMDDir.Content.ToString();

            //Check to ensure the ModDir content is indeed provided (length greater than 0 indicates data in the field)
            //Make sure the variable in the settings file is correct
            if (ModDir.Content != null)
                UserSettings.Default.ModsDir = Variables.main.ModDir.Content.ToString();

            //Added v1.2.0
            //Check to ensure the Steam username content is indeed provided (length greater than 0 indicates data in the field)
            //Make sure the variable in the settings file is correct
            if (SteamUsername.Password.Length > 0)
                UserSettings.Default.SteamUsername = Variables.main.SteamUsername.Password;

            //Added v1.2.0
            //Check to ensure the Steam password content is indeed provided (length greater than 0 indicates data in the field)
            //Make sure the variable in the settings file is correct
            if (SteamPassword.Password.Length > 0)
                UserSettings.Default.SteamPassword = Variables.main.SteamPassword.Password;

            //Save which platform the user has chosen
            if (Variables.steam)
                UserSettings.Default.Platform = "steam";
            else
                UserSettings.Default.Platform = "other";
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
