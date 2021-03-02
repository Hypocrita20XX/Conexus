/*
> Conexus (v1.4.0) for Darkest Dungeon
    Created by MatthiosArcanus(Discord)/Hypocrita(Steam)/Hypocrita20XX(GitHub) 
    A GUI-based program designed to streamline the process of organizing mods according to an existing Steam collection or list of links
    Handles downloading and updating mods through the use of SteamCMD (https://developer.valvesoftware.com/wiki/SteamCMD)

> APIs used:
    Ookii.Dialogs (v3.1.0)
    Authors: Sven Groot, Augusto Proiete
    Source: http://www.ookii.org/software/dialogs/

    Extended WPF Toolkit (v4.0.2)
    Author: Xceed Software
    Source: https://github.com/xceedsoftware/wpftoolkit

    Peanut Butter INI (v2.0.11)
    Author: Davys McColl
    Source: https://github.com/fluffynuts/PeanutButter

> Code used/adapted:
    Function: Copy Folders
    Author: Timm
    Source: http://www.csharp411.com/c-copy-folder-recursively/

    Function: Password Reveal Functionality
    Author: DaisyTian-MSFT
    Source: https://docs.microsoft.com/en-us/answers/questions/99602/wpf-passwordbox-passwordrevealmode-was-not-found-i.html

    Function: Internet Connectivity Test
    Author: ChaosPandion, T.Todua
    Source: https://stackoverflow.com/questions/2031824/what-is-the-best-way-to-check-for-internet-connectivity-using-net

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

using Ookii.Dialogs.Wpf;
using PeanutButter.INI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Path = System.IO.Path;

#endregion

namespace Conexus
{
    public partial class MainWindow : Window
    {
        #region Declarations
        //Lists to store info related to the mods that will/are downloaded
        List<string> modInfo = new List<string>();
        List<string> appIDs = new List<string>();

        //Create a global dateTime for this session
        string dateTime = Regex.Replace(DateTime.Now.ToString(), @"['<''>'':''/''\''|''?''*'' ']", "_", RegexOptions.None);

        //Keeps track of the line count in the log
        int lineCount = 1;
        //Stores all logs in a list, for later storage in a text file
        List<string> log = new List<string>();

        //Stores logs temporarily until the message textblock is initiated
        List<string> logTmp = new List<string>();

        //Create a root directory in the user's Documents folder for all generated data
        string rootPath = System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\Conexus";
        //Create a data directory for all text files (HTML.txt, Mods.txt, ModInfo.txt)
        string dataPath = System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\Conexus\\Data";
        //Create a config directory for all user data
        string configPath = System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\Conexus\\Config";
        //Create a directory that will hold Links.txt
        string linksPath = System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\Conexus\\Links";
        //Create a directory that will hold logs
        string logsPath = System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\Conexus\\Logs";

        /*
         * 
         * New config file as of v1.3.0
         * config.ini, stored in Documents\Conexus\Config
         * 
         * [System]
         * Root=\Documents\Conexus
         * Data=\Documents\Conexus\Data
         * Config=\Documents\Conexus\Config
         * Links=\Documents\Conexus\Links
         * Logs="\Documents\Conexus\Logs"
         * 
         * [Directories]
         * Mods=\DarkestDungeon\mods
         * SteamCmd=\steamcmd
         * 
         * [URL]
         * Collection=https://steamcommunity.com
         * 
         * [Misc]
         * Method=steam
         * 
         * [Login]
         * Username=""
         * Password=""
         * 
         */
        INIFile ini;

        string root = "";
        string data = "";
        string config = "";
        string links = "";
        string logs = "";

        string mods = "";
        string steamcmd = "";

        string urlcollection = "";

        //If the user provides this info, we also need to read the Steam username and password
        string username = "";
        string password = "";

        //Ensures that certain messages don't happen until the textblock is initialized
        bool loaded = false;

        //Stores number of download attempts made
        //Maximum allowed: 2
        int dlAttempts = 0;

        //Indicates if downloads were successful
        bool success;

        //Keeps a list of mod IDs that could not be downloaded
        List<string> missingMods = new List<string>();

        #endregion

        public MainWindow()
        {
            InitializeComponent();

            //Very basic exception handling
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);

            ini = new INIFile(configPath + "\\config.ini");

            this.DataContext = this;
        }

        //Very basic exception handling
        void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            ShowMessage("ERROR: exception occured! " + (e.ExceptionObject as Exception).Message);
            ShowMessage("ERROR: Please post your logs on Github! https://github.com/Hypocrita20XX/Conexus/issues");

            //If an exception does happen, I'm assuming Conexus will crash, so here's this just in case
            //Ensure the Logs folder exists
            if (!Directory.Exists(logsPath))
                Directory.CreateDirectory(logsPath);

            //Save logs to file
            WriteToFile(log.ToArray(), Path.Combine(logsPath, dateTime + ".txt"));
        }

        #region Button Functionality

        //Provides functionality to allow the user to select the mods directory
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
            mods = folderBrowser.SelectedPath;

            //Log info relating to what the user wants to do
            ShowMessage("INPUT: Mods directory set to \"" + mods + "\"");

            //Verify that the provided directory is valid, not empty, and contains Darkest.exe
            if (VerifyModDir(folderBrowser.SelectedPath))
            {
                //Set the settings variable to the one selected
                mods = folderBrowser.SelectedPath;

                ini["Directories"]["Mods"] = mods;
                ini.Persist();

                //Log info relating to what the user wants to do
                ShowMessage("VERIFY: Mods directory is valid and has been saved to config file");

                ModDir.Content = mods;
            }
            //If the given path is invalid, let the user know why
            else
            {
                //If the given path contains any info, provide that with the error message
                if (folderBrowser.SelectedPath.Length > 0)
                    ShowMessage("WARN: Invalid mods location: " + folderBrowser.SelectedPath + "!");
                //If the given path is blank, provide that information
                else
                    ShowMessage("WARN: Invalid mods Location: no path given!");
            }

            if (loaded)
                //Save log to file
                WriteToFile(log.ToArray(), Path.Combine(logsPath, dateTime + ".txt"));
        }

        //Provides functionality to allow the user to select the SteamCMD directory
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
            steamcmd = folderBrowser.SelectedPath;

            //Log info relating to what the user wants to do
            ShowMessage("INPUT: SteamCMD directory set to \"" + steamcmd + "\"");

            //Verify that the provided directory is valid, not empty, and contains steamcmd.exe
            if (VerifySteamCMDDir(folderBrowser.SelectedPath))
            {
                string tmp = Path.GetFullPath(folderBrowser.SelectedPath);

                //Set the settings variable to the one selected
                steamcmd = folderBrowser.SelectedPath;

                //Added v1.3.0
                ini["Directories"]["SteamCMD"] = steamcmd;
                ini.Persist();

                //Log info relating to what the user wants to do
                ShowMessage("VERIFY: SteamCMD directory is valid and has been saved to config file");

                SteamCMDDir.Content = steamcmd;
            }
            //If the given path is invalid, let the user know why
            else
            {
                //If the given path contains any info, provide that with the error message
                if (folderBrowser.SelectedPath.Length > 0)
                    ShowMessage("WARN: Invalid SteamCMD location: " + folderBrowser.SelectedPath + "!");
                //If the given path is blank, provide that information
                else
                    ShowMessage("WARN: Invalid SteamCMD location: no path given!");
            }

            if (loaded)
                //Save log to file
                WriteToFile(log.ToArray(), Path.Combine(logsPath, dateTime + ".txt"));
        }

        //Opens a link to Conexus on Nexus Mods
        void URL_Nexus_Click(object sender, RoutedEventArgs e)
        {
            //Attempt to open a link in the user's default browser
            try
            {
                //Let user know what's happening
                ShowMessage("INFO: Attempting to open link to Conexus on Nexus Mods");
                //Attempt to open link
                System.Diagnostics.Process.Start("https://www.nexusmods.com/darkestdungeon/mods/858?");
                //Let user know what happened
                ShowMessage("INFO: Link successfully opened");
            }
            //Exception for no default browser
            catch (System.ComponentModel.Win32Exception noBrowser)
            {
                //Let user know what happened
                if (noBrowser.ErrorCode == -2147467259)
                    ShowMessage("ERROR: No default browser found! " + noBrowser.Message);
            }
            //Unspecified exception
            catch (System.Exception other)
            {
                //Let user know what happened
                ShowMessage("ERROR: Unspecified exception! " + other.Message);
            }
            //Save log file no matter what happens
            finally
            {
                //Save log to file
                WriteToFile(log.ToArray(), Path.Combine(logsPath, dateTime + ".txt"));
            }
        }

        //Opens a link to Conexus on Github
        void URL_Github_Click(object sender, RoutedEventArgs e)
        {
            //Attempt to open a link in the user's default browser
            try
            {
                //Let user know what's happening
                ShowMessage("INFO: Attempting to open link to Conexus' Github repository");
                //Attempt to open link
                System.Diagnostics.Process.Start("https://github.com/Hypocrita20XX/Conexus");
                //Let user know what happened
                ShowMessage("INFO: Link successfully opened");
            }
            //Exception for no default browser
            catch (System.ComponentModel.Win32Exception noBrowser)
            {
                //Let user know what happened
                if (noBrowser.ErrorCode == -2147467259)
                    ShowMessage("ERROR: No default browser found! " + noBrowser.Message);
            }
            //Unspecified exception
            catch (System.Exception other)
            {
                //Let user know what happened
                ShowMessage("ERROR: Unspecified exception! " + other.Message);
            }
            //Save log file no matter what happens
            finally
            {
                //Save log to file
                WriteToFile(log.ToArray(), Path.Combine(logsPath, dateTime + ".txt"));
            }
        }

        //Opens a link to the wiki on Github
        void URL_Wiki_Click(object sender, RoutedEventArgs e)
        {
            //Attempt to open a link in the user's default browser
            try
            {
                //Let user know what's happening
                ShowMessage("INFO: Attempting to open link to Conexus' Github wiki");
                //Attempt to open link
                System.Diagnostics.Process.Start("https://github.com/Hypocrita20XX/Conexus/wiki");
                //Let user know what happened
                ShowMessage("INFO: Link successfully opened");
            }
            //Exception for no default browser
            catch (System.ComponentModel.Win32Exception noBrowser)
            {
                //Let user know what happened
                if (noBrowser.ErrorCode == -2147467259)
                    ShowMessage("ERROR: No default browser found! " + noBrowser.Message);
            }
            //Unspecified exception
            catch (System.Exception other)
            {
                //Let user know what happened
                ShowMessage("ERROR: Unspecified exception! " + other.Message);
            }
            //Save log file no matter what happens
            finally
            {
                //Save log to file
                WriteToFile(log.ToArray(), Path.Combine(logsPath, dateTime + ".txt"));
            }
        }

        //Opens a link to the issue tracker on Github
        void URL_Issue_Click(object sender, RoutedEventArgs e)
        {
            //Attempt to open a link in the user's default browser
            try
            {
                //Let user know what's happening
                ShowMessage("INFO: Attempting to open link to Conexus' Github issue tracker");
                //Attempt to open link
                System.Diagnostics.Process.Start("https://github.com/Hypocrita20XX/Conexus/issues");
                //Let user know what happened
                ShowMessage("INFO: Link successfully opened");
            }
            //Exception for no default browser
            catch (System.ComponentModel.Win32Exception noBrowser)
            {
                //Let user know what happened
                if (noBrowser.ErrorCode == -2147467259)
                    ShowMessage("ERROR: No default browser found! " + noBrowser.Message);
            }
            //Unspecified exception
            catch (System.Exception other)
            {
                //Let user know what happened
                ShowMessage("ERROR: Unspecified exception! " + other.Message);
            }
            //Save log file no matter what happens
            finally
            {
                //Save log to file
                WriteToFile(log.ToArray(), Path.Combine(logsPath, dateTime + ".txt"));
            }
        }

        #endregion

        #region Checkbox Functionality

        //Reveals the username when the checkbox is checked
        void UsernameReveal_Checked(object sender, RoutedEventArgs e)
        {
            //Assign the text in the textbox to the given password
            SteamUsername_TextBox.Text = SteamUsername.Password;
            //Hide the password box
            SteamUsername.Visibility = Visibility.Collapsed;
            //Show the text box
            SteamUsername_TextBox.Visibility = Visibility.Visible;
        }

        //Hides the username when the checkbox is unchecked
        void UsernameReveal_Unchecked(object sender, RoutedEventArgs e)
        {
            //Assign the text in the password to what's in the text box
            SteamUsername.Password = SteamUsername_TextBox.Text;
            //Hide the text box
            SteamUsername_TextBox.Visibility = Visibility.Collapsed;
            //Show the password box
            SteamUsername.Visibility = Visibility.Visible;
        }

        //Reveals the password when the checkbox is unchecked
        void PasswordReveal_Checked(object sender, RoutedEventArgs e)
        {
            //Assign the text in the textbox to the given password
            SteamPassword_TextBox.Text = SteamPassword.Password;
            //Hide the password box
            SteamPassword.Visibility = Visibility.Collapsed;
            //Show the text box
            SteamPassword_TextBox.Visibility = Visibility.Visible;
        }

        //Hides the password when the checkbox is unchecked
        void PasswordReveal_Unchecked(object sender, RoutedEventArgs e)
        {
            //Assign the text in the password to what's in the text box
            SteamPassword.Password = SteamPassword_TextBox.Text;
            //Hide the text box
            SteamPassword_TextBox.Visibility = Visibility.Collapsed;
            //Show the password box
            SteamPassword.Visibility = Visibility.Visible;
        }

        #endregion

        #region Main Functionality 

        //Main workhorse function
        async void OrganizeMods_Click(object sender, RoutedEventArgs e)
        {
            //If this directory is deleted or otherwise not found, it needs to be created, otherwise stuff will break
            if (!Directory.Exists(dataPath))
            {
                ShowMessage("WARN: Conexus\\Data is missing! Creating now");

                Directory.CreateDirectory(dataPath);
            }

            ShowMessage("INFO: Disabling input fields now");

            //Disable input during operation
            URLLink.IsEnabled = false;
            SteamCMDDir.IsEnabled = false;
            ModDir.IsEnabled = false;
            SteamUsername.IsEnabled = false;
            SteamUsername_TextBox.IsEnabled = false;
            UsernameReveal.IsEnabled = false;
            SteamPassword.IsEnabled = false;
            SteamPassword_TextBox.IsEnabled = false;
            PasswordReveal.IsEnabled = false;
            OrganizeMods.IsEnabled = false;

            //Log info relating to what the user wants to do
            ShowMessage("INFO: Using " + System.Environment.OSVersion);

            //Provide further clarification
            if (System.Environment.OSVersion.ToString().Contains("10"))
                ShowMessage("INFO: Using supported Windows version, 10");
            else
                ShowMessage("WARN: Potentially using unsupported OS!");

            //Save log to file
            WriteToFile(log.ToArray(), Path.Combine(logsPath, dateTime + ".txt"));

            //Check internet connection before doing anything

            //Note: ideally this would  be rolled into the CheckForInternetConnection method
            //I don't care enough to make that happen right now, so this will work fine, despite being far too verbose
            //IE turn the method into an int, return the value of valid pings 
            //(Even though what I'm doing may not technically be pinging. You get the idea)
            List<bool> netAttempts = new List<bool>();

            netAttempts.Add(CheckForInternetConnection("google.com"));
            netAttempts.Add(CheckForInternetConnection("facebook.com"));
            netAttempts.Add(CheckForInternetConnection("youtube.com"));
            netAttempts.Add(CheckForInternetConnection("twitter.com"));
            //Specifically chose this one because it's not blocked in China
            netAttempts.Add(CheckForInternetConnection("weibo.com"));

            int t = 0;
            int f = 0;

            //See the ratio of true:false
            for (int i = 0; i < netAttempts.Count; i++)
            {
                if (netAttempts[i])
                    t++;

                if (!netAttempts[i])
                    f++;
            }

            //If we have more false than true, they're lacking an internet connection
            if (f > t)
            {
                //Provide additional logging
                ShowMessage("ERROR: There seems to be a problem with your internet connection!");
                ShowMessage("ERROR: Conexus will now stop processing your list");

                //Save log to file
                WriteToFile(log.ToArray(), Path.Combine(logsPath, dateTime + ".txt"));

                ShowMessage("INFO: Enabling input fields now");

                //Enable input after operation
                URLLink.IsEnabled = true;
                SteamCMDDir.IsEnabled = true;
                ModDir.IsEnabled = true;
                SteamUsername.IsEnabled = true;
                SteamUsername_TextBox.IsEnabled = true;
                UsernameReveal.IsEnabled = true;
                SteamPassword.IsEnabled = true;
                SteamPassword_TextBox.IsEnabled = true;
                PasswordReveal.IsEnabled = true;
                OrganizeMods.IsEnabled = true;

                ShowMessage("WARN: Process could not finish successfully!");

                //Save logs to file
                WriteToFile(log.ToArray(), Path.Combine(logsPath, dateTime + ".txt"));

                //Return and make sure everything else stops
                return;
            }

            //Check to see if the user has selected directories for SteamCMD and Darkest Dungeon\mods
            if ((string)SteamCMDDir.Content == "Select SteamCMD Directory")
                ShowMessage("WARN: You didn't select a SteamCMD directory!");

            if ((string)ModDir.Content == "Select Mods Directory")
                ShowMessage("WARN: You didn't select a mods directory!");

            //If either equal the default value, get out of here
            if ((string)SteamCMDDir.Content == "Select SteamCMD Directory")
            {
                //Save log to file
                WriteToFile(log.ToArray(), Path.Combine(logsPath, dateTime + ".txt"));

                ShowMessage("INFO: Enabling input fields now");

                //Enable input after operation
                URLLink.IsEnabled = true;
                SteamCMDDir.IsEnabled = true;
                ModDir.IsEnabled = true;
                SteamUsername.IsEnabled = true;
                SteamUsername_TextBox.IsEnabled = true;
                UsernameReveal.IsEnabled = true;
                SteamPassword.IsEnabled = true;
                SteamPassword_TextBox.IsEnabled = true;
                PasswordReveal.IsEnabled = true;
                OrganizeMods.IsEnabled = true;

                ShowMessage("WARN: Process could not finish successfully!");

                //Save logs to file
                WriteToFile(log.ToArray(), Path.Combine(logsPath, dateTime + ".txt"));

                //Return and make sure everything else stops
                return;
            }

            //If the user wants to use a Steam collection, ensure all functionality relates to that
            if (URLLink.Text.Length > 0)
            {
                //Log info relating to what the user wants to do
                ShowMessage("INFO: URL found, using a Steam collection");

                //Check the provided URL to make sure it's valid
                if (await VerifyCollectionURLAsync(URLLink.Text, dataPath))
                {
                    urlcollection = URLLink.Text;

                    //It is assumed that at this point, the user has entered a valid URL to the collection
                    if (urlcollection.Length > 0)
                    {
                        ini["URL"]["Collection"] = urlcollection;
                        ini.Persist();

                        //Log info relating to what the user wants to do
                        ShowMessage("PROC: Collection URL has been saved");

                        //Save log to file
                        WriteToFile(log.ToArray(), Path.Combine(logsPath, dateTime + ".txt"));
                    }
                    //Otherwise we need to quit and provide an error message
                    else
                    {
                        //Provide feedback
                        ShowMessage("WARN: Invalild URL! Process has stopped");

                        //Save log to file
                        WriteToFile(log.ToArray(), Path.Combine(logsPath, dateTime + ".txt"));

                        ShowMessage("INFO: Enabling input fields now");

                        //Enable input after operation
                        URLLink.IsEnabled = true;
                        SteamCMDDir.IsEnabled = true;
                        ModDir.IsEnabled = true;
                        SteamUsername.IsEnabled = true;
                        SteamUsername_TextBox.IsEnabled = true;
                        UsernameReveal.IsEnabled = true;
                        SteamPassword.IsEnabled = true;
                        SteamPassword_TextBox.IsEnabled = true;
                        PasswordReveal.IsEnabled = true;
                        OrganizeMods.IsEnabled = true;

                        //Exit out of this function
                        return;
                    }

                    //Provide feedback
                    ShowMessage("INFO: Mod info will now be obtained from the collection link");

                    //Create all necessary text files
                    await DownloadHTMLAsync(urlcollection, dataPath);

                    //Provide feedback
                    ShowMessage("INFO: HTML has been downloaded and processed");

                    //Provide feedback
                    ShowMessage("INFO: Mods will now be downloaded");

                    //Start downloading mods
                    await DownloadModsFromSteamAsync();

                    //Save log to file
                    WriteToFile(log.ToArray(), Path.Combine(logsPath, dateTime + ".txt"));
                    
                }
                //URL is not valid, don't do anything
                else
                {
                    ShowMessage("INFO: Enabling input fields now");

                    //Enable input after operation
                    URLLink.IsEnabled = true;
                    SteamCMDDir.IsEnabled = true;
                    ModDir.IsEnabled = true;
                    SteamUsername.IsEnabled = true;
                    SteamUsername_TextBox.IsEnabled = true;
                    UsernameReveal.IsEnabled = true;
                    SteamPassword.IsEnabled = true;
                    SteamPassword_TextBox.IsEnabled = true;
                    PasswordReveal.IsEnabled = true;
                    OrganizeMods.IsEnabled = true;

                    ShowMessage("WARN: Invalid URL!");

                    //Save log to file
                    WriteToFile(log.ToArray(), Path.Combine(logsPath, dateTime + ".txt"));

                    return;
                }
            }
            //Otherwise, the user wants to use a list of URLs
            else if (URLLink.Text.Length == 0)
            {
                //Log info relating to what the user wants to do
                ShowMessage("INFO: No URL found, using a list of links");

                //Provide feedback
                ShowMessage("INFO: Mod info will now be obtained from the Links file");

                //Parse IDs from the user-populated list
                await ParseFromListAsync(linksPath);

                //Provide feedback
                ShowMessage("INFO: Mod info has been obtained, mods will now be downloaded");

                await DownloadModsFromSteamAsync();
            }

            ShowMessage("INFO: Enabling input fields now");

            //Enable input after operation
            URLLink.IsEnabled = true;
            SteamCMDDir.IsEnabled = true;
            ModDir.IsEnabled = true;
            SteamUsername.IsEnabled = true;
            SteamUsername_TextBox.IsEnabled = true;
            UsernameReveal.IsEnabled = true;
            SteamPassword.IsEnabled = true;
            SteamPassword_TextBox.IsEnabled = true;
            PasswordReveal.IsEnabled = true;
            OrganizeMods.IsEnabled = true;

            if (success)
                ShowMessage("INFO: Process has finished successfully");
            else if (!success)
                ShowMessage("WARN: Process could not finish successfully!");

            //If missing mods were detected, append to the end of the log, for convenience
            if (missingMods.Count > 0)
                foreach (string mods in missingMods)
                    ShowMessage("INFO: Mods that could not be downloaded: " + mods);

            //Reset dlAttempts
            dlAttempts = 0;

            //Save logs to file
            WriteToFile(log.ToArray(), Path.Combine(logsPath, dateTime + ".txt"));
        }

        //Download source HTML from a given Steam collection URL
        async Task DownloadHTMLAsync(string url, string fileDir)
        {
            //If the Data folder does not exist, create it
            if (!Directory.Exists(fileDir))
            {
                ShowMessage("INFO: " + fileDir + " does not exist, creating now");

                Directory.CreateDirectory(fileDir);
            }

            //Reset lists
            if (modInfo.Count > 0)
                modInfo.Clear();
            if (appIDs.Count > 0)
                appIDs.Clear();

            //Overwrite whatever may be in ModInfo.txt, if it exists
            if (File.Exists(dataPath + "\\ModInfo.txt"))
            {
                File.WriteAllText(dataPath + "\\ModInfo.txt", String.Empty);

                ShowMessage("PROC: ModInfo contents have been overwritten");
            }

            //Save logs to file
            WriteToFile(log.ToArray(), Path.Combine(logsPath, dateTime + ".txt"));

            //Create a new WebClient
            WebClient webClient = new WebClient();

            try
            {
                //Download the desired collection and save the file
                await Task.Run(() => webClient.DownloadFile(url, fileDir + "\\HTML.txt"));
            }
            catch (System.Net.WebException e)
            {
                ShowMessage("ERROR: Something went wrong with your network connection! " + e.Message);

                //Indicate failure
                success = false;

                //Get out of here
                return;
            }

            //Free up resources, cleanup
            webClient.Dispose();

            ShowMessage("PROC: Source HTML downloaded successfully");

            //Save logs to file
            WriteToFile(log.ToArray(), Path.Combine(logsPath, dateTime + ".txt"));

            //Move on to parsing through the raw source
            await IterateThroughHTMLAsync(fileDir);
        }

        //Go through the source line by line
        async Task IterateThroughHTMLAsync(string fileDir)
        {
            //Temp variable to store an individual line
            string line;
            //List of strings to store a line that houses all neccesary info for each mod
            List<string> mods = new List<string>();
            //Create a file reader and load the previously saved source file
            StreamReader file = new StreamReader(@fileDir + "\\HTML.txt");

            ShowMessage("PROC: Parsing HTML source now");

            //Iterate through the file one line at a time
            while ((line = file.ReadLine()) != null)
            {
                //If a line contains "a href" and "workshopItemTitle," then this line contains mod information
                if (line.Contains("a href") & line.Contains("workshopItemTitle"))
                {
                    //Add this line to the mods list
                    await Task.Run(() => mods.Add(line.Substring(line.IndexOf("<"))));
                }
            }

            //Provide feedback
            ShowMessage("PROC: Finished search for mod info in provided collection URL");

            //Close file, cleanup
            file.Close();

            //Write this information to a file
            WriteToFile(mods.ToArray(), fileDir + "\\Mods.txt");

            //Provide feedback
            ShowMessage("INFO: Mod info will now be separated into its useful parts");

            //Save logs to file
            WriteToFile(log.ToArray(), Path.Combine(logsPath, dateTime + ".txt"));

            //Move on to parsing out the relevant info
            await SeparateInfoAsync(fileDir);
        }

        //Parses out all relevant info from the source's lines
        async Task SeparateInfoAsync(string fileDir)
        {
            //Temp variable to store an individual line
            string line;
            //Stores the initial folder index
            int folderIndex = 0;
            //Stores the final folder index (with leading zeroes)
            string folderIndex_S = "";
            //Load the previously stored file for further refinement
            StreamReader file = new StreamReader(@fileDir + "\\Mods.txt");

            //Provide feedback
            ShowMessage("PROC: Parsing through source for all relevant data");

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
                await Task.Run(() => modInfo.Add(final));

                //Add the app id to the appIDs list
                await Task.Run(() => appIDs.Add(id));

                //Provide feedback
                ShowMessage("PROC: Found mod info: " + final);

                //Increment folderIndex
                folderIndex++;
            }

            //Provide feedback
            ShowMessage("INFO: Finished finalizing each mods' information");

            //Close file, cleanup
            file.Close();

            //Write the modInfo to a text file
            WriteToFile(modInfo.ToArray(), @fileDir + "\\ModInfo.txt");

            //Save logs to file
            WriteToFile(log.ToArray(), Path.Combine(logsPath, dateTime + ".txt"));
        }

        //Go through the provided text file and find/create relevant mod info
        async Task ParseFromListAsync(string fileDir)
        {
            //Examples:
            // > Format: https://steamcommunity.com/sharedfiles/filedetails/?id=1282438975
            // > Ignore: * 50% Stealth Chance in Veteran Quests

            //Overwrite whatever may be in ModInfo.txt, if it exists
            if (File.Exists(linksPath + "\\ModInfo.txt"))
            {
                File.WriteAllText(linksPath + "\\ModInfo.txt", String.Empty);

                ShowMessage("PROC: ModInfo contents have been overwritten");
            }

            //Reset lists
            if (modInfo.Count > 0)
                modInfo.Clear();
            if (appIDs.Count > 0)
                appIDs.Clear();

            //Save logs to file
            WriteToFile(log.ToArray(), Path.Combine(logsPath, dateTime + ".txt"));

            //Temp variable to store an individual line
            string line;
            //Stores the initial folder index
            int folderIndex = 0;
            //Stores the final folder index (with leading zeroes)
            string folderIndex_S = "";
            //Load the previously stored file for further refinement
            StreamReader file = new StreamReader(@fileDir + "\\Links.txt");

            ShowMessage("PROC: Links.txt will now be parsed for mod info");

            //Temporary lists
            List<string> modInfo_Names = new List<string>();
            List<string> modInfo_IDs = new List<string>();

            string name = "";

            //Iterate through each line the file
            while ((line = file.ReadLine()) != null)
            {
                //If this line contains an asterisk, this is intended to be the mod name
                if (line.Contains("*"))
                    name = Regex.Replace(line, @"['<''>'':''/''\''|''?''*']", "", RegexOptions.None);

                //If the line being looked at is a Steam workshop link, we need to get the ID from this line
                //if (!line.Contains("*"))
                if (line.Contains("steamcommunity.com"))
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

                    //If we found a mod name in a previous line after finding the last link,
                    //we need to append that to what we add to modInfo
                    if (name != "")
                    {
                        //We need to make sure the given modInfo is no more than 260 characters
                        //Practically, let's say it can be no more than 120 characters
                        if ((folderIndex_S + "_" + id + "_" + name).Length > 120)
                        {
                            //Provide feedback
                            ShowMessage("INFO: Mod info exceeds 120 characters! Truncating now");

                            await Task.Run(() => modInfo.Add((folderIndex_S + "_" + id + "_" + name).Substring(0, 119)));

                            //Provide feedback
                            ShowMessage("PROC: Found mod ID and name in Links file: " + (folderIndex_S + "_" + id + "_" + name).Substring(0, 119));
                        }
                        //Otherwise the given mod info is fine
                        else if ((folderIndex_S + "_" + id + "_" + name).Length <= 120)
                        {
                            await Task.Run(() => modInfo.Add(folderIndex_S + "_" + id + "_" + name));

                            //Provide feedback
                            ShowMessage("PROC: Found mod ID and name in Links file: " + folderIndex_S + "_" + id + "_" + name);
                        }

                        //Reset name string
                        name = "";
                    }
                    //Otherwise if no mod name was found this link and the last,
                    //we can just use the old format, numerical order and ID
                    else if (name == "")
                    {
                        await Task.Run(() => modInfo.Add(folderIndex_S + "_" + id));

                        //Provide feedback
                        ShowMessage("PROC: Found only mod ID in Links file: " + folderIndex_S + "_" + id);
                    }

                    //Add this ID to the appIDs list
                    await Task.Run(() => appIDs.Add(id));

                    //Increment folderIndex
                    folderIndex++;
                }
            }

            //Provide feedback
            ShowMessage("INFO: Finished processing mod info in Links file");

            //Write the modInfo to a text file
            WriteToFile(modInfo.ToArray(), @fileDir + "\\ModInfo.txt");

            //Close file, cleanup
            file.Close();

            //Save log to file
            WriteToFile(log.ToArray(), Path.Combine(logsPath, dateTime + ".txt"));
        }

        //Handles downloading mods through SteamCMD
        async Task DownloadModsFromSteamAsync()
        {
            //Only do this if this is the first attempt
            if (dlAttempts == 0)
            {
                //We need to verify that all the needed data text files downloaded successfully
                //If we're working with a URL, we need to verify HTML.txt, ModInfo.txt, and Mods.txt
                //If we're working with a list of links, we only need to verify ModInfo.txt
                //Verification will be simple, just checking that the length of the file is greater than 0
                //So far that's the only bug I've encountered (as of 1.4.0), and not missing info, which would require
                //more extensive verification methods, such as parsing each file 
                //and checking each line against known good values
                FileInfo html_txt = new FileInfo(dataPath + "\\HTML.txt");
                FileInfo mods_txt = new FileInfo(dataPath + "\\Mods.txt");
                FileInfo modinfo_url_txt = new FileInfo(dataPath + "\\ModInfo.txt");
                FileInfo modinfo_links_txt = new FileInfo(linksPath + "\\ModInfo.txt");

                if (URLLink.Text.Length > 0)
                {
                    //If HTML.txt is equal to zero bytes, no data, we need to try once more to get that data
                    //If this is the case, it's safe to assume that the other files are most likely invalid as well
                    if (html_txt.Length == 0)
                    {
                        ShowMessage("WARN: HTML.txt contains no data, trying once again to obtain collection info!");

                        await DownloadHTMLAsync(URLLink.Text, dataPath);

                        //If it's still zero bytes, there's a problem and we can't proceed
                        if (html_txt.Length == 0)
                        {
                            ShowMessage("WARN: HTML.txt still has no data, no mods will be downloaded! Aborting now");

                            //We can (and have done so elsewhere) check this automatically, 
                            //but this is such a specific and what I would think rare issue,
                            //I don't see the point of added overhead
                            //Just throw out the most likely problem and solution, call it a day
                            ShowMessage("DEBUG: Please check that your link is valid and that you're connected to the internet");

                            //Indicate failure
                            success = false;

                            //Get out of here
                            return;
                        }

                        //Next file in line, Mods.txt
                        if (mods_txt.Length == 0)
                        {
                            ShowMessage("WARN: Mods.txt contains no data, trying once again to obtain info!");

                            await IterateThroughHTMLAsync(dataPath);

                            //If it's still zero bytes, there's a problem and we can't proceed
                            if (mods_txt.Length == 0)
                            {
                                ShowMessage("WARN: Mods.txt still has no data, no mods will be downloaded! Aborting now");
                                ShowMessage("DEBUG: Please check that your link is valid and that you're connected to the internet");

                                //Indicate failure
                                success = false;

                                //Get out of here
                                return;
                            }
                        }

                        //Last file, ModInfo.txt
                        if (modinfo_url_txt.Length == 0)
                        {
                            ShowMessage("WARN: ModInfo.txt contains no data, trying once again to obtain info!");

                            await SeparateInfoAsync(dataPath);

                            //If it's still zero bytes, there's a problem and we can't proceed
                            if (modinfo_url_txt.Length == 0)
                            {
                                ShowMessage("WARN: ModInfo.txt still has no data, no mods will be downloaded! Aborting now");
                                ShowMessage("DEBUG: Please check that your link is valid and that you're connected to the internet");

                                //Indicate failure
                                success = false;

                                //Get out of here
                                return;
                            }
                        }
                    }

                    //Next file in line, Mods.txt
                    if (mods_txt.Length == 0)
                    {
                        ShowMessage("WARN: Mods.txt contains no data, trying once again to obtain info!");

                        await IterateThroughHTMLAsync(dataPath);

                        //If it's still zero bytes, there's a problem and we can't proceed
                        if (mods_txt.Length == 0)
                        {
                            ShowMessage("WARN: Mods.txt still has no data, no mods will be downloaded! Aborting now");
                            ShowMessage("DEBUG: Please check that your link is valid and that you're connected to the internet");

                            //Indicate failure
                            success = false;

                            //Get out of here
                            return;
                        }
                    }

                    //Last file, ModInfo.txt
                    if (modinfo_url_txt.Length == 0)
                    {
                        ShowMessage("WARN: ModInfo.txt contains no data, trying once again to obtain info!");

                        await SeparateInfoAsync(dataPath);

                        //If it's still zero bytes, there's a problem and we can't proceed
                        if (modinfo_url_txt.Length == 0)
                        {
                            ShowMessage("WARN: ModInfo.txt still has no data, no mods will be downloaded! Aborting now");
                            ShowMessage("DEBUG: Please check that your link is valid and that you're connected to the internet");

                            //Indicate failure
                            success = false;

                            //Get out of here
                            return;
                        }
                    }

                    //If, after all of that, we've got valid files, we can continue as normal
                    //Provide a message because... Why not
                    ShowMessage("Info: All text files have been verified as valid, proceeding to download!");
                }
                else if (URLLink.Text.Length == 0)
                {
                    //ModInfo.txt for a list of links
                    if (modinfo_links_txt.Length == 0)
                    {
                        await SeparateInfoAsync(linksPath);

                        //If it's still zero bytes, there's a problem and we can't proceed
                        if (modinfo_links_txt.Length == 0)
                        {
                            ShowMessage("WARN: ModInfo.txt still has no data, no mods will be downloaded! Aborting now");
                            ShowMessage("DEBUG: Please check that your link is valid and that you're connected to the internet");

                            //Indicate failure
                            success = false;

                            //Get out of here
                            return;
                        }
                    }

                    //If, after all of that, we've got valid files, we can continue as normal
                    //Provide a message because... Why not
                    ShowMessage("Info: All text files have been verified as valid, proceeding to download!");
                }


                //Check to see if the darkestdungeon\mods folder is empty
                //If not, it needs cleared out to make room for the updated mods
                if (Directory.GetDirectories(mods).Length > 0)
                {
                    //Provide feedback
                    ShowMessage("INFO: Mods detected in " + mods + ", deleting old versions now");

                    await DeleteDirectory(mods);
                }

                /*
                //Commented out because I'm unconvinced this is the best approach
                //For instance if the user downloads 200 mods, but 2 of those have since been taken down
                //This will delete 198 perfectly usable mods
                //Which is awful because those 198 could have taken 30 minutes or longer to download
                //So let's not, as far as I know, having them in this folder isn't an issue.
                //Really becomes an issue with the mods folder, due to the possibility of adding/removing mods
                //And how specific the naming is, the order would get messed up otherwise
                //Leaving this code here just in case, and the comment too as I feel this is useful info

                //Check to see if the 262060 folder is empty
                //If not, it needs cleared out, remnants of a failed download process
                if (Directory.Exists(steamcmd + "\\steamapps\\workshop\\content") && Directory.Exists(steamcmd + "\\steamapps\\workshop\\content\\262060"))
                    if (Directory.GetDirectories(steamcmd + "\\steamapps\\workshop\\content\\262060").Length > 0)
                    {
                        //Provide feedback
                        ShowMessage("INFO: Mods detected in " + steamcmd + "\\steamapps\\workshop\\content\\262060" + ", deleting now");

                        await DeleteDirectory(steamcmd + "\\steamapps\\workshop\\content\\262060");
                    }
                */
            }

            //Stores the proper commands that will be passed to SteamCMD
            string cmdList = "";


            //Only do this if this is the first attempt
            if (dlAttempts == 0) 
                ShowMessage("PROC: Commands will now be obtained");

            //Get a list of commands for each mod stored in a single string
            for (int i = 0; i < appIDs.Count; i++)
            {
                await Task.Run(() => cmdList += "+\"workshop_download_item 262060 " + appIDs[i] + "\" ");

                //Only do this if this is the first attempt
                if (dlAttempts == 0)
                    ShowMessage("PROC: Adding command to list: " + " +\"workshop_download_item 262060 " + appIDs[i] + "\" ");
            }
            

            //Provide feedback
            ShowMessage("INFO: SteamCMD will take over now");

            //Save logs to file
            WriteToFile(log.ToArray(), Path.Combine(logsPath, dateTime + ".txt"));

            //Create a process that will contain all relevant SteamCMD commands for all mods
            ProcessStartInfo processInfo = new ProcessStartInfo("\"" + steamcmd + "\\steamcmd.exe" + "\"", " +login " + SteamUsername.Password + " " + SteamPassword.Password + " " + cmdList + "+quit");

            //Create a wrapper that will run all commands, wait for the process to finish, and then proceed to copying and renaming folders/files
            using (Process process = new Process())
            {
                //Set the commands for this process
                process.StartInfo = processInfo;
                //Start the process with the provided commands
                await Task.Run(() => process.Start());
                //Wait until SteamCMD finishes
                await Task.Run(() => process.WaitForExit());
            }

            //Increment dlAttempts
            dlAttempts++;

            //Provide feedback
            ShowMessage("INFO: SteamCMD has closed");

            bool result = VerifySteamCMDDownload(steamcmd);

            //Verify that mods downloaded
            if (result)
            {
                //Reset dlAttempts
                dlAttempts = 0;

                //Provide feedback
                ShowMessage("INFO: Mods have been successfully downloaded");

                //Indicate success
                success = true;

                //Move on to copying and renaming the mods
                await RenameAndMoveModsAsync();

                //Save log to file
                WriteToFile(log.ToArray(), Path.Combine(logsPath, dateTime + ".txt"));
            }
            else if (!result && dlAttempts < 2)
            {
                //Increment dlAttempts
                dlAttempts++;

                //Provide feedback
                ShowMessage("Warn: Mods were not downloaded successfully! Trying again");
                ShowMessage("Info: Attempt " + dlAttempts.ToString() + " of 2");

                await DownloadModsFromSteamAsync();
            }
            else if (!result && dlAttempts >= 2)
            {
                //Provide feedback
                ShowMessage("Warn: Mods could not be downloaded after two attempts, process will now stop");

                //Indicate failure
                success = false;
            }
        }

        //Creates organized folders in the mods directory, then copies files from the SteaCMD directory to those folders
        //Requires that an operation be specified (DOWNLOAD or UPDATE)
        async Task RenameAndMoveModsAsync()
        {
            //Create source/destination path list variables
            string[] source = new string[appIDs.Count];
            string[] destination = new string[modInfo.Count];

            //Provide feedback
            //ShowMessage("PROC: Acquiring paths to copy from");

            //Save log to file
            WriteToFile(log.ToArray(), Path.Combine(logsPath, dateTime + ".txt"));

            //Get the proper path to copy from
            for (int i = 0; i < appIDs.Count; i++)
                await Task.Run(() => source[i] = Path.Combine(steamcmd + "\\steamapps\\workshop\\content\\262060\\", appIDs[i]));

            //Provide feedback
            //ShowMessage("PROC: Acquiring paths to copy to");

            //Save log to file
            WriteToFile(log.ToArray(), Path.Combine(logsPath, dateTime + ".txt"));

            //Get the proper path that will be copied to
            for (int i = 0; i < modInfo.Count; i++)
                await Task.Run(() => destination[i] = Path.Combine(mods, modInfo[i]));

            //Provide feedback
            ShowMessage("PROC: Copying files, please wait");

            //Save log to file
            WriteToFile(log.ToArray(), Path.Combine(logsPath, dateTime + ".txt"));

            //Copy all folders/files from the SteamCMD directory to the mods directory
            for (int i = 0; i < destination.Length; i++)
            {
                await CopyFoldersAsync(source[i], destination[i]);

                //Provide feedback
                ShowMessage("PROC: Files copied from " + source[i] + " to " + destination[i]);
            }

            /* 
            
            // After some thought, I don't think removing what's in the 262060 folder is the best idea
            // It shouldn't cause any issue, and saves the user time in subsequent uses
             
            //Provide feedback
            ShowMessage("INFO: Files copied, original copy will now be deleted");

            //Save log to file
            WriteToFile(log.ToArray(), Path.Combine(logsPath, dateTime + ".txt"));

            await DeleteDirectory(@steamcmd + "\\steamapps\\workshop\\content\\262060\\");

            //Provide feedback
            ShowMessage("INFO: Mods have now been moved and renamed, originals have been deleted");
            */

            ShowMessage("INFO: Mods have now been moved and renamed");

            //Save log to file
            WriteToFile(log.ToArray(), Path.Combine(logsPath, dateTime + ".txt"));
        }

        //Fix for index out of bounds when stuff gets deleted
        //Handles any deletions of directories and sub-directories
        async Task DeleteDirectory(string directory)
        {
            try
            {
                foreach (var dir in Directory.GetDirectories(directory))
                {
                    await Task.Run(() => Directory.Delete(dir, true));

                    //Provide feedback
                    ShowMessage("PROC: " + dir + " deleted");
                }

                //Provide feedback
                ShowMessage("INFO: Original copies have been deleted");
            }
            catch (System.IO.DirectoryNotFoundException)
            {
                //Provide feedback
                ShowMessage("ERROR: Directory could not be found: " + directory + "!");
            }
            finally
            {
                //Save log to file
                WriteToFile(log.ToArray(), Path.Combine(logsPath, dateTime + ".txt"));
            }
        }

        //A base function that will copy/rename any given folder(s)
        //Can be used recursively for multiple directories
        async Task CopyFoldersAsync(string source, string destination)
        {
            //Check if the directory exists, if not, create it
            if (!Directory.Exists(destination))
                Directory.CreateDirectory(destination);

            //Check to see if a mod was downloaded
            if (!Directory.Exists(source))
            {
                ShowMessage("WARN: " + source + " does not exist! Mod info: " + destination);

                //Add destination to the missingMods list for later adding to the end of the log
                missingMods.Add(destination.Substring(destination.IndexOf("_") + 1));

                //We need to get out of here before stuff breaks
                return;
            }

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
                await Task.Run(() => File.Copy(file, dest, true));
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
                await CopyFoldersAsync(folder, dest);
            }

            //Save log to file
            WriteToFile(log.ToArray(), Path.Combine(logsPath, dateTime + ".txt"));
        }

        //Utility function to write text to a file
        void WriteToFile(string[] text, string fileDir)
        {
            if (!File.Exists(fileDir))
                File.Create(fileDir).Dispose();

            if (File.Exists(fileDir))
                File.WriteAllLines(@fileDir, text);
        }

        //Utility function to handle messages
        void ShowMessage(string msg)
        {
            //Because I insist on proper formatting, here's a series of if statements
            //whose only job is to add leading zeroes
            //Yes, we're going up to a million lines
            //No, no sane person should ever reach this
            //I'm doing it anyway

            //Create a temporary string
            string lcStr = "";
            //If lineCount is less than 10, add six leading zeroes
            if (lineCount < 10)
                lcStr = "000000" + lineCount.ToString();
            //If lineCount is less than 100 and greater than/equal to 10, add five leading zeros
            else if (lineCount < 100 && lineCount >= 10)
                lcStr = "00000" + lineCount.ToString();
            //If lineCount is less than 1000 and greater than/equal to 100, add four leading zeroes
            else if (lineCount < 1000 && lineCount >= 100)
                lcStr = "0000" + lineCount.ToString();
            //If lineCount is less than 10000 and greater than/equal to than 1000, add three leading zeroes
            else if (lineCount < 10000 && lineCount >= 1000)
                lcStr = "000" + lineCount.ToString();
            //If lineCount is less than 100000 and greater than/equal to than 10000, add two leading zeroes
            else if (lineCount < 100000 && lineCount >= 10000)
                lcStr = "00" + lineCount.ToString();
            //If lineCount is less than 1000000 and greater than/equal to than 100000, add one leading zero
            else if (lineCount < 1000000 && lineCount >= 100000)
                lcStr = "0" + lineCount.ToString();
            //If lineCount is greater than 1000000, no leading zeroes are needed
            else if (lineCount >= 1000000)
            {
                //Seriously, how?
                ShowMessage("Impressive, and I thought I liked mods. Nice!");
                lcStr = lineCount.ToString();
            }
            //If it's somehow something else, no leading zeroes
            else
                lcStr = lineCount.ToString();

            //Check to see if the Messages extblock is loaded
            //If so, proceed as normal
            if (Messages != null && Messages.IsLoaded)
            {
                //If logTmp is not empty, then messages were added before the textblock was initiated
                //and those messages should be added now to the textblock
                if (logTmp.Count > 0)
                {
                    //Show desired message with appropriate line count
                    //Messages.Text += logTmp;

                    for (int i = 0; i < logTmp.Count; i++)
                    {
                        //Show desired message without line count or date
                        Messages.Text += logTmp[i].Substring(logTmp[i].IndexOf("*")+1);
                        //Remove the asterisk to provide a properly formatted log message
                        string tmp = Regex.Replace(logTmp[i], @"['*']", " ", RegexOptions.None);
                        //Save this message to the log list
                        log.Add(tmp.Substring(0, tmp.Length - 2));
                    }

                    //Clear out logTmp
                    logTmp.Clear();

                    //This specific part of the program will only hit once, so we can safely do this twice without issue
                    //Add the current message to the textblock and list
                    //Show desired message with appropriate line count
                    Messages.Text += msg + "\n";
                    //Save this message to the log list
                    log.Add("[" + lcStr + "] " + "[" + Regex.Replace(DateTime.Now.ToString(), @"['<''>'':''/''\''|''?''*'' ']", "_", RegexOptions.None) + "] " + msg);

                    //Increment lineCount
                    lineCount++;

                    //Scroll to the end of the scroll viewer
                    MessageScrollViewer.ScrollToEnd();
                }
                //Otherwise, proceed as normal
                else
                {
                    //Show desired message with appropriate line count
                    Messages.Text += msg + "\n";
                    //Save this message to the log list
                    log.Add("[" + lcStr + "] " + "[" + Regex.Replace(DateTime.Now.ToString(), @"['<''>'':''/''\''|''?''*'' ']", "_", RegexOptions.None) + "] " + msg);

                    //Increment lineCount
                    lineCount++;

                    //Scroll to the end of the scroll viewer
                    MessageScrollViewer.ScrollToEnd();
                }
            }
            //Otherwise, the message needs stored until it is loaded and can accept messages
            else
            {
                //Messages textblock has not initiated yet, so we need to store the messages until it is
                //Add an asterisk at the end of the date for later removal of the line count and date for the log window
                logTmp.Add("[" + lcStr + "] " + "[" + Regex.Replace(DateTime.Now.ToString(), @"['<''>'':''/''\'' | '' ? '' * '' ']", "_", RegexOptions.None) + "]*" + msg + "\n");

                //Increment lineCount
                lineCount++;
            }
        }

        #endregion

        #region Verification Functionality

        //Goes through several verification steps to ensure a proper Steam collection URL has been entered
        async Task<bool> VerifyCollectionURLAsync(string url, string fileDir)
        {
            /*
             * 
             * URL verification is a bit tricky
             * This is because Steam has a landing page with "valid" results, 
             * as opposed to a 404 page, or similar.
             * Because of this, the HTML contents of the given URL need to be downloaded
             * so that we can be sure we have a valid collection URL.
             * 
             * This validation only tests for links that have somehow gotten messed up
             * IE https://steamcommunity.com/workshop/filedetails/?id=2362884526 (valid) versus
             * https://steamcommunity.com/workshop/filfadsfafaedetails/?id=2362884526 (invalid)
             * 
             * It does not test for something such as https://steamc431241134ommunity.com/workshop/filedetails/?id=2362884526
             * which will not lead to a Steam site at all (in fact it leads to a completely invalid site)
             * For this, we need another validation, which checks if the link is in any way valid
             * 
             * There also needs to be a check to ensure that the site is actually for Steam
             * For instance someone accidently pastes a Youtube link instead of a Steam collection link.
             * This check basically will search through a line or two of the HTML code
             * and compare it to a known good Steam site
             * 
             * Order of validation:
             * 1.) Check for any valid link
             * 2.) Check to make sure it's a Steam site
             * 3.) Check to make sure it leads to a collection
             * 
             */

            //Create a new WebClient
            WebClient webClient = new WebClient();

            //Assume the URL is valid unless an exception occurs
            bool validURL = true;

            //Attempt to download the HTML from the provided URL
            try
            {
                ShowMessage("INFO: Attempting to download HTML from given collection URL");
                //Download the desired collection and save the file
                await Task.Run(() => webClient.DownloadFile(url, fileDir + "\\HTML.txt"));

                ShowMessage("INFO: Successfully downloaded HTML");
            }
            //Not a valid URL
            catch (WebException)
            {
                //Clear URLLink Text
                URLLink.Text = string.Empty;
                //Flag this URL as invalid
                validURL = false;
                //Provide additional logging
                ShowMessage("ERROR: Provided URL is not valid!");
            }
            //No URL at all, or something else that was unexpected
            catch (ArgumentException)
            {
                //Clear URLLink Text
                URLLink.Text = string.Empty;
                //Flag this URL as invalid
                validURL = false;
                //Provide additional logging
                ShowMessage("ERROR: Provided URL is not valid or does not exist!");
            }
            //I don't know why this triggers, but it does, and it's not for valid reasons
            catch (NotSupportedException)
            {
                //Clear URLLink Text
                URLLink.Text = string.Empty;
                //Flag this URL as invalid
                validURL = false;
                //Provide additional logging
                ShowMessage("ERROR: Provided URL is not valid!");
            }
            //URL is too long
            catch (PathTooLongException)
            {
                //Clear URLLink Text
                URLLink.Text = string.Empty;
                //Flag this URL as invalid
                validURL = false;
                //Provide additional logging
                ShowMessage("ERROR: Provided URL is too long! (Greater than 260 characters)");
            }
            finally
            {
                //Save log to file
                WriteToFile(log.ToArray(), Path.Combine(logsPath, dateTime + ".txt"));
            }

            //If the link is valid, leads to an actual site, we need to check for a valid Steam site
            if (validURL)
            {
                ShowMessage("VERIFY: Given link is valid");

                try
                {
                    //Download the desired collection and save the file
                    await Task.Run(() => webClient.DownloadFile(url, fileDir + "\\HTML.txt"));
                }
                catch (System.Net.WebException e)
                {
                    ShowMessage("ERROR: Something went wrong with your network connection!" + e.Message);

                    //Indicate failure
                    success = false;

                    //Get out of here
                    return false;
                }

                ShowMessage("INFO: HTML source has been downloaded");

                /*
                 * Now we need to check to see if this is a valid Steam site
                 * 
                 * We need something to compare to though, to do this
                 * 
                 * A valid Steam site has various tells, 
                 * most important of which is that it will contain links that start with https://steamcommunity-a.akamaihd.net
                 * These links start on line 8 on several Steam sites I looked like, but start on line 12 in a collection
                 * Because of this slight discrepency, we need to look at a range of lines
                 * Let's say we start at line 0,up to 50 (as this is zero-based, we'll stop at 49)
                 * We shouldn't go further than is needed though, as this will affect overall performance
                 * 
                 * While we're doing this check, we'll also look for the next verification's tell, "Steam Workshop: Darkest Dungeon"
                 * Both checks use the same iteration process and should be combined for performance reasons
                 * Because of this, we'll go further than the previous 50 lines, to 100 (stopping at 99)
                 * The HTML I looked at contained this tell on line 71, but we need to be sure
                 * 
                 */

                //Temp variable to store an individual line
                string line;
                //List of strings to store all ines in a given range
                List<string> lines = new List<string>();
                //Create a file reader and load the saved HTML file
                StreamReader file = new StreamReader(@fileDir + "\\HTML.txt");

                //Keeps track of line count
                int lineCount = 0;

                //Stores the result of the verification check for a valid Steam link
                bool isValidSteam = false;
                //Stores the result of the verification check for a valid Steam collection link
                bool isValidCollection = false;

                //Provide feeback
                ShowMessage("PROC: Searching for valid Steam collection links");

                //Iterate through the given file up to line 100, line by line
                while ((line = file.ReadLine()) != null && lineCount < 100)
                {
                    //Check 2
                    //If we find a line that contains "https://steamcommunity-a.akamaihd.net", we can safely say this is a Steam link
                    if (line.Contains("steamcommunity-a.akamaihd.net"))
                        isValidSteam = true;

                    //If we find a line that contains "Steam Workshop: Darkest Dungeon", we can say this is a Steam Collection link
                    if (line.Contains("Steam Workshop: Darkest Dungeon"))
                        isValidCollection = true;

                    //Increment lineCount
                    lineCount++;
                }

                //Provide feeback
                ShowMessage("PROC: Search complete");

                //Save log to file
                WriteToFile(log.ToArray(), Path.Combine(logsPath, dateTime + ".txt"));

                //If these checks fail, this is not a valid Steam collection link and the user needs to know that
                if (!isValidSteam && !isValidCollection || isValidSteam && !isValidCollection)
                {
                    //Clear URLLink Text
                    URLLink.Text = string.Empty;
                    //Cleanup
                    webClient.Dispose();
                    file.Close();

                    //Provide feedback
                    ShowMessage("WARN: No Steam collection link found, please check the link provided!");

                    //Save log to file
                    WriteToFile(log.ToArray(), Path.Combine(logsPath, dateTime + ".txt"));

                    return false;
                }
                else
                {
                    //Cleanup
                    webClient.Dispose();
                    file.Close();

                    //Provide feedback
                    ShowMessage("VERIFY: A valid Steam collection link has been found");

                    //Save log to file
                    WriteToFile(log.ToArray(), Path.Combine(logsPath, dateTime + ".txt"));

                    return true;
                }
            }
            //Otherwise this is not a valid link and the user needs to know that
            else
            {
                //Clear URLLink Text
                URLLink.Text = string.Empty;
                //Provide feedback
                ShowMessage("WARN: No valid Steam collection link found, please check the link provided!");

                //Cleanup
                webClient.Dispose();

                //Save log to file
                WriteToFile(log.ToArray(), Path.Combine(logsPath, dateTime + ".txt"));

                return false;
            }
        }

        //Goes through several verification steps to ensure that the given SteamCMD directory is valid, contains steamcmd.exe
        bool VerifySteamCMDDir(string fileDir)
        {
            /*
             * 
             * This verification check is fairly straightforward: we check the given directory and see if we can find steamcmd.exe
             * Steamcmd.exe is thankfully located in the root directory, which is what the user is asked to find
             * so we can assume that if it's not in the given directory, the given directory is not valid
             * 
             */

            //Verify if this directory contains steamcmd.exe
            if (File.Exists(fileDir + "\\steamcmd.exe"))
                return true;
            else
                return false;
        }

        //Goes through several verification steps to ensure that the given mods directory is valid, contains Darkest.exe
        bool VerifyModDir(string fileDir)
        {
            /*
             * 
             * This verification check is fairly straightforward: we check the given directory and see if we can find Darkest.exe
             * Unlike steamcmd.exe, this is located in a different folder DarkestDungeon\_windows
             * So we first need to navigate to the root directory, then to _windows and check for the exe
             * 
             */

            try
            {
                //Temp string to store the root directory
                string dirRoot = fileDir.Substring(0, fileDir.Length - 5);
                //Temp string to store the _windows directory
                string win = dirRoot + "\\_windows";

                //Verify if this directory contains steamcmd.exe
                if (File.Exists(win + "\\Darkest.exe"))
                    return true;
                else
                    return false;
            }
            catch (ArgumentOutOfRangeException)
            {
                return false;
            }
        }

        //After SteamCMD has closed, this will check to see if SteamCMD successfully downloaded mods
        bool VerifySteamCMDDownload(string steamCmdDir)
        {
            /*
             * 
             * This verification is mostly straightforward, but comes in two parts, one nested in the other,
             * and becomes more complex as the implications need handled approprietly in a user-oriented manner.
             * 
             * The first check has to verify that steamcmd\steamapps\workshop\content exists
             * The next check needs to go to steamcmd\steamapps\workshop\content\262060 and make sure 
             * that there are indeed folders matching the information found in the modInfo list
             * This needs to be an exact match to what's in that file, otherwise we need to run checks
             * to see what may have gone wrong and let the user know the result of each check
             * 
             * There are several known reasons mods fail to download:
             * 1.) The game is not registered to the user's Steam account (they have it on Epic/GOG, or a pirated version - all unsupported)
             * 2.) Invalid Steam credentials (the user typed in something accidentally)
             * 3.) Invalid mod info (either their links file is invalid/missing or URL is invalid - previous check should handle the URL)
             * 
             * There are also a number of suspected reasons mods could fail to download:
             * 1.) No internet connection
             * 2.) Incorrect or corrupt SteamCMD installation (possibly missing SteamGuard code/account not linked to SteamCMD)
             * 3.) Unsupported OS (only Windows 10 is tested, if tried on Linux, Mac, Android, etc. I'm almost certain Conexus will break)
             * 
             * 
             * Concerning known reasons and how to check them:
             * 1 is unrecoverable, there is no known workaround for Epic/GOG, and I won't support a pirated version
             * 2 is easy enough in theory, just let the user know there was a problem logging in
             * 3 should also be straightforward, let them know that no valid mod information was found
             * and point them in a few directions as to how to fix it, related to their chosen method (collection/list of links)
             * 
             * Concerning suspected reasons:
             * 1 should be simple enough, do a basic WebClient read and see if you get a true result
             * 2 is also simple, let the user know there's an issue (so long as you can detect it) and have them do a manual clean reinstall of SteamCMD
             * the trick is detecting the problem, and it will probably be simplest to just do a blanket, blind check and point them
             * in that direction
             * 3 is already implented elsewhere and we can reuse that code here, just let the user know that their OS is not supported and that I can't help them
             * 
             */

            ShowMessage("INFO: Downloads will now be verified");

            //Will store local variables that will then dictate further verification checks
            //until final returns are needed
            bool e = false;

            if (Directory.Exists(steamCmdDir + "\\steamapps\\workshop\\content"))
            {
                ShowMessage("VERIFY: " + steamCmdDir + "\\steamapps\\workshop\\content was created successfully");

                //Check to see if \\262060 exists
                if (Directory.Exists(steamCmdDir + "\\steamapps\\workshop\\content\\262060"))
                {
                    ShowMessage("VERIFY: " + steamCmdDir + "\\steamapps\\workshop\\content\\262060 was created successfully");

                    //Get an array of all directories in \\262060
                    string[] directories = Directory.GetDirectories(steamCmdDir + "\\steamapps\\workshop\\content\\262060");

                    //Will store how many mods have been correctly downloaded
                    int match = 0;

                    ShowMessage("INFO: Downloaded mods will now be verified");

                    //So, we run through both data sets, what has been downloaded, and what is in the appID list
                    //We compare to make sure we get a 100% identical match in the end
                    for (int x = 0; x < directories.Length; x++)
                    {
                        ShowMessage("PROC: Checking " + directories[x] + "...");

                        for (int y = 0; y < appIDs.Count; y++)
                        {
                            if (directories[x].Contains(appIDs[y]))
                            {
                                ShowMessage("PROC: Match found! " + directories[x] + " for ID " + appIDs[y]);

                                match++;
                            }
                        }
                    }

                    //To make it easier to work with, convert our ints to float 
                    //and store that division in its own float
                    float ratio = (float)match / (float)appIDs.Count;

                    //Now compare what is stored in match with what is expected
                    if (match == appIDs.Count)
                    {
                        ShowMessage("VERIFY: Download successful! " + match.ToString() + " mod(s) out of a total " + appIDs.Count.ToString() + " found");

                        //If match is the same as the count of IDs in the list, the all mods have downloaded
                        e = true;
                    }
                    //Otherwise if the ratio is above or equal to a certain percentage, deem this a success 
                    //(to account for hidden mods that SteamCMD can't download)
                    //I'm assuming an acceptable ratio is 3/4 of a modlist
                    else if (ratio >= 0.75)
                    {
                        ShowMessage("VERIFY: Download was partially successful! Some mods are missing");
                        ShowMessage("INFO: " + match.ToString() + " mod(s) out of a total " + appIDs.Count.ToString() + " found");

                        //If match is the same as the count of IDs in the list, the all mods have downloaded
                        e = true;
                    }
                    //If the match ratio is less than 75%, then we can consider this a download failure
                    else if (ratio < 0.75)
                    {
                        ShowMessage("WARN: Download was not successful! " + match.ToString() + " mod(s) out of a total " + appIDs.Count.ToString() + " found");

                        //Otherwise something has gone wrong
                        e = false;
                    }
                }
                else
                {
                    ShowMessage("WARN: " + steamCmdDir + "\\steamapps\\workshop\\content\\262060 is missing!");

                    e = false;
                }
            }
            else
            {
                ShowMessage("WARN: " + steamCmdDir + "\\steamapps\\workshop\\content is missing!");

                e = false;
            }

            //Second, for any "INVALID" occurences
            //This, again happens if:
            //1.) The content folder is missing (which also means 2 and 3 are true)
            //2.) The 262060 folder is missing (which also means 1 is false and 3 is true)
            //3.) Any or all of the mod folders are missing (which means 1 and 2 are false)
            if (!e && dlAttempts >= 2)
            {
                ShowMessage("INFO: Debug info will now be generated");

                //First, let's find specifics, are content\262060 folders missing?
                if (!Directory.Exists(steamCmdDir + "\\steamapps\\workshop\\content") || !Directory.Exists(steamCmdDir + "\\steamapps\\workshop\\content\\262060"))
                {
                    //Let's provide debug info related to that
                    ShowMessage("DEBUG: 0 - Content and\\or 262060 folder(s) are missing!");
                    ShowMessage("DEBUG: 0 - This happens because SteamCMD could not download mods");

                    //Before we continue, we need to check to make sure the user is online
                    //Please note that, as far as I can tell, there's no reliable way to do this
                    //So we'll do a shotgun approach and let the user know that they may be offline
                    //Or in a country where that website is unavailable

                    //Note: ideally this would  be rolled into the CheckForInternetConnection method
                    //I don't care enough to make that happen right now, so this will work fine, despite being far too verbose
                    //IE turn the method into an int, return the value of valid pings 
                    //(Even though what I'm doing may not technically be pinging. You get the idea)
                    List<bool> netAttempts = new List<bool>();

                    ShowMessage("DEBUG: 1 - Let's check your internet connection");

                    netAttempts.Add(CheckForInternetConnection("google.com"));
                    netAttempts.Add(CheckForInternetConnection("facebook.com"));
                    netAttempts.Add(CheckForInternetConnection("youtube.com"));
                    netAttempts.Add(CheckForInternetConnection("twitter.com"));
                    //Specifically chose this one because it's not blocked in China
                    netAttempts.Add(CheckForInternetConnection("weibo.com"));

                    int t = 0;
                    int f = 0;

                    //See the ratio of true:false
                    for (int i = 0; i < netAttempts.Count; i++)
                    {
                        if (netAttempts[i])
                            t++;

                        if (!netAttempts[i])
                            f++;
                    }

                    //If we have more true than false, I think it's safe to say their internet connection is fine
                    if (t > f)
                        ShowMessage("DEBUG: 2 - It looks like your internet connnection is fine!");
                    //Otherwise there could be an issue with their connection
                    else if (f > t)
                    {
                        ShowMessage("DEBUG: 2 - Unforuntately there seems to be a problem with your internet connection!");
                        ShowMessage("DEBUG: 2 - Please check your connection and try again");

                        //Return and make sure everything else stops
                        return false;
                    }

                    //Save log to file
                    WriteToFile(log.ToArray(), Path.Combine(logsPath, dateTime + ".txt"));

                    ShowMessage("DEBUG: 3 - We've verified your internet connection, so let's check other potential causes");

                    //Specific to a collection
                    if (URLLink.Text.Length > 0)
                    {
                        ShowMessage("DEBUG: 4 - It looks like you're using a Steam collection");
                        ShowMessage("DEBUG: 4 - Please make sure that your collection is set to either unlisted or public");
                        ShowMessage("DEBUG: 4 - Unfortunately if it's set to hidden or friends-only, Conexus can't find it");
                    }
                    //Specific to a list of links
                    else if (URLLink.Text.Length == 0)
                    {
                        ShowMessage("DEBUG: 4 - It looks like you're using a list of links");
                        ShowMessage("DEBUG: 4 - Please make sure that Links.txt is located in Documents\\Conexus\\Links");
                        ShowMessage("DEBUG: 4 - Also please be sure that each comment starts with * and is one its own line");
                        ShowMessage("DEBUG: 4 - And that each URL is also on its own line");
                    }

                    //Save log to file
                    WriteToFile(log.ToArray(), Path.Combine(logsPath, dateTime + ".txt"));

                    //Let's move on to verifying their OS
                    //We can just copy/paste this from earlier in the program
                    if (System.Environment.OSVersion.ToString().Contains("10"))
                        ShowMessage("DEBUG: 5 - You're using a supported OS, Windows 10");
                    else
                        ShowMessage("DEBUG: 5 - Unsupported OS detected, please try again using Windows 10!");

                    //Save log to file
                    WriteToFile(log.ToArray(), Path.Combine(logsPath, dateTime + ".txt"));

                    /*
                     * 
                     * Now for the stuff that is a bit less simple to detect 
                     * (and that I honestly don't want to come up with a solution for,
                     * (stuff like login you could theoretically parse the steamcmd logs for, but.... Ehhhhhhhhhhh)
                     * 
                     * This includes the following:
                     * 1.) Failure to log in
                     * 2.) Invalid/corrupt SteamCMD installation
                     * 3.) Epic/GOG/Pirates
                     * 
                     * This, we just need to provide a large block of text and hope for the best
                     * 
                    */

                    ShowMessage("DEBUG: 6 - Now for the stuff I can't check directly");
                    ShowMessage("DEBUG: 6 - First, make sure you entered your Steam credentials correctly");
                    ShowMessage("DEBUG: 6 - Second, you might want to clear out your SteamCMD installation and start fresh");
                    ShowMessage("DEBUG: 6 - If you need a refresher, visit this link: https://developer.valvesoftware.com/wiki/SteamCMD#Running_SteamCMD");
                    ShowMessage("DEBUG: 6 - Third, if you don't own the game on Steam, unfortunately Conexus won't work");
                    ShowMessage("DEBUG: 6 - Sadly due to technical limitations, I can't support Epic/GOG");
                    ShowMessage("DEBUG: 6 - Please see this for more details: https://github.com/Hypocrita20XX/Conexus/issues/11");
                    ShowMessage("DEBUG: 6 - Also, if you're using a pirated version, I can't help you whatsoever");
                    ShowMessage("DEBUG: 6 - I only support legitimate copies");


                    //And now, the "IDK, send me stuff" series of messages
                    ShowMessage("DEBUG: 7 - If you still can't get Conexus to work, first of all, sorry for the inconvenience!");
                    ShowMessage("DEBUG: 7 - I'll need you to send me some logs and create an issue on Github: https://github.com/Hypocrita20XX/Conexus/issues");
                    ShowMessage("DEBUG: 7 - First, I need the logs located in Documents\\Conexus\\Logs");
                    ShowMessage("DEBUG: 7 - Second, also send the logs in steamcmd\\logs");
                    ShowMessage("DEBUG: 7 - Just send everything in those folders, thanks!");

                    //Save log to file
                    WriteToFile(log.ToArray(), Path.Combine(logsPath, dateTime + ".txt"));
                }
                else
                {
                    ShowMessage("DEBUG: Downloads could not be completed for an unknown reason. Please try again!");
                }
            }

            //Return the appropriate value
            if (e)
                return true;
            else if (!e)
                return false;
            else
                return false;
        }

        //A generic method that takes in a website and appends "generate_204" to the end
        //Modified from the StackOverflow answer so that it can accept any URL
        bool CheckForInternetConnection(string URL)
        {
            try
            {
                using (var client = new WebClient())

                using (client.OpenRead("HTTPS://" + URL + "/generate_204"))
                {
                    ShowMessage("PROC: Connecting to: " + URL + ", successful");
                    return true;
                }
            }
            catch
            {
                ShowMessage("PROC: Connecting to: " + URL + ", failed");
                return false;
            }
        }

        #endregion

        #region Data Saving Functionality

        //Called when the UI window has loaded, used to set proper info in the UI from the settings file
        void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //A bunch of checks to make sure every necessary path exists
            if (!Directory.Exists(rootPath))
            {
                Directory.CreateDirectory(rootPath);
                Directory.CreateDirectory(dataPath);
                Directory.CreateDirectory(configPath);
                Directory.CreateDirectory(linksPath);
                Directory.CreateDirectory(logsPath);

                ShowMessage("INFO: No folder found in User\\Documents");
                ShowMessage("INFO: Created Conexus\\Config, Conexus\\Data, Conexus\\Links, and Conexus\\Logs");
            }

            if (!Directory.Exists(dataPath))
            {
                Directory.CreateDirectory(dataPath);
                ShowMessage("WARN: Conexus\\Data missing! Folder created");
            }

            if (!Directory.Exists(configPath))
            {
                Directory.CreateDirectory(configPath);
                ShowMessage("WARN: Conexus\\Config missing! Folder created");
            }

            if (!Directory.Exists(linksPath))
            {
                Directory.CreateDirectory(linksPath);
                ShowMessage("WARN: Conexus\\Links missing! Folder created");
            }

            if (!Directory.Exists(logsPath))
            {
                Directory.CreateDirectory(logsPath);
                ShowMessage("WARN: Conexus\\Logs missing! Folder created");
            }

            //Make sure that Links.txt exists
            if (!File.Exists(linksPath + "\\Links.txt"))
            {
                File.Create(linksPath + "\\Links.txt").Dispose();
                ShowMessage("VERIFY: Links.txt not found, creating file");
            }
            else
                ShowMessage("VERIFY: Links.txt found");


            if (File.ReadAllBytes(configPath + "\\config.ini").Length == 0)
            {
                //Initialize data structure
                ini["System"]["Root"] = rootPath.Replace(@"\\", @"\");
                ini["System"]["Data"] = dataPath.Replace(@"\\", @"\");
                ini["System"]["Config"] = configPath.Replace(@"\\", @"\");
                ini["System"]["Links"] = linksPath.Replace(@"\\", @"\");
                ini["System"]["Logs"] = logsPath.Replace(@"\\", @"\");

                ini["Directories"]["Mods"] = "";
                ini["Directories"]["SteamCMD"] = "";

                ini["URL"]["Collection"] = "";

                ini["Login"]["Username"] = "";
                ini["Login"]["Password"] = "";

                ini.Persist();

                ShowMessage("VERIFY: Created INI with default settings");
            }
            else
            {
                //Read values from the INI file
                root = ini["System"]["Root"].Replace(@"\\", @"\");
                data = ini["System"]["Data"].Replace(@"\\", @"\");
                config = ini["System"]["Config"].Replace(@"\\", @"\");
                links = ini["System"]["Links"].Replace(@"\\", @"\");
                logs = ini["System"]["Logs"].Replace(@"\\", @"\");

                mods = ini["Directories"]["Mods"].Replace(@"\\", @"\");
                steamcmd = ini["Directories"]["SteamCMD"].Replace(@"\\", @"\");

                urlcollection = ini["URL"]["Collection"].Replace(@"\\", @"\");

                username = ini["Login"]["Username"].Replace(@"\\", @"\");
                password = ini["Login"]["Password"].Replace(@"\\", @"\");

                ShowMessage("VERIFY: Loaded INI");
            }

            //Check the contents of the ini variable in the settings file, if so, set the UI variable to it
            if (urlcollection != "")
            {
                URLLink.Text = urlcollection;
                ShowMessage("VERIFY: Now showing the saved URL on the UI");
            }
            else
            {
                URLLink.Text = string.Empty;
                ShowMessage("VERIFY: No saved collection URL");
            }

            //Check the contents of the ini variable in the settings file, if so, set the UI variable to it
            if (steamcmd != "")
            {
                SteamCMDDir.Content = steamcmd;
                ShowMessage("VERIFY: Saved SteamCMD directory found, now showing on the UI");
            }
            else
            {
                SteamCMDDir.Content = "Select SteamCMD Directory";
                ShowMessage("VERIFY: No saved SteamCMD directory found");
            }

            //Check the contents of the ini variable in the settings file, if so, set the UI variable to it
            if (mods != "")
            {
                ModDir.Content = mods;
                ShowMessage("VERIFY: Saved mods directory found, now showing on the UI");
            }
            else
            {
                ModDir.Content = "Select Mods Directory";
                ShowMessage("VERIFY: No saved mods directory found");
            }

            //Check the contents of the ini variable in the settings file, if so, set the UI variable to it
            if (username != "")
            {
                SteamUsername.Password = username;
                ShowMessage("VERIFY: Saved Steam username found, now showing (obscured) on the UI");
            }
            else
            {
                SteamUsername.Password = "";
                ShowMessage("VERIFY: No saved Steam username found");
            }
            //Check the contents of the ini variable in the settings file, if so, set the UI variable to it
            if (password != "")
            {
                SteamPassword.Password = password;
                ShowMessage("VERIFY: Saved Steam password found, now showing (obscured) on the UI");
            }
            else
            {
                SteamPassword.Password = "";
                ShowMessage("VERIFY: No saved Steam password found");
            }

            loaded = true;
        }

        //Called right after the user indicates they want to close the program (through the use of the "X" button)
        //Used to ensure all proper data is set to their corrosponding variables in the settings file
        void Window_Closing(object sender, EventArgs e)
        {
            //Check to ensure the URLLink content is indeed provided
            //Make sure the variable in the settings file is correct
            if (urlcollection != "")
            {
                ini["URL"]["Collection"] = urlcollection;
                ShowMessage("VERIFY: Chosen collection URL saved");
            }

            //Check to see if no URL was given
            if (URLLink.Text.Length == 0)
            {
                ini["URL"]["Collection"] = "";
                ShowMessage("VERIFY: No URL found, clearing any URL saved in the config");
            }

            //Check to ensure the SteamCMDDir content is indeed provided
            //Make sure the variable in the settings file is correct
            if (steamcmd != "")
            {
                ini["Directories"]["SteamCMD"] = steamcmd;
                ShowMessage("VERIFY: Chosen SteamCMD directory saved");
            }

            //Check to ensure the ModDir content is indeed provided
            //Make sure the variable in the settings file is correct
            if (mods != "")
            {
                ini["Directories"]["Mods"] = mods;
                ShowMessage("VERIFY: Chosen mods directory saved");
            }

            ShowMessage("PROC: All user data has been saved!");
            ShowMessage("INFO: Conexus will close now");

            ini.Persist();

            //Save log to file
            WriteToFile(log.ToArray(), Path.Combine(logsPath, dateTime + ".txt"));
        }

        //Final call that happens right after the window starts to close
        //Used to save all relevant data to the settings file
        void Window_Closed(object sender, EventArgs e)
        {
            //Save all data to the settings file
            //UserSettings.Default.Save();

            //Ensure the Logs folder exists
            if (!Directory.Exists(logsPath))
            {
                ShowMessage("WARN: Logs folder is missing! Creating now");

                Directory.CreateDirectory(logsPath);
            }

            //Save log to file
            WriteToFile(log.ToArray(), Path.Combine(logsPath, dateTime + ".txt"));

            ini.Persist();
        }

        #endregion
    }
}
