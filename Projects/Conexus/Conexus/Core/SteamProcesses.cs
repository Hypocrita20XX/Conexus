using Conexus.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Conexus.Core
{
    public class SteamProcesses
    {
        public void DownloadModsFromSteam()
        {
            //Stores the proper commands that will be passed to SteamCMD
            string cmdList = "";

            //Get a list of commands for each mod stored in a single string
            for (int i = 0; i < Variables.appIDs.Count; i++)
                cmdList += "+\"workshop_download_item 262060 " + Variables.appIDs[i] + "\" ";

            //Create a process that will contain all relevant SteamCMD commands for all mods
            //ProcessStartInfo processInfo = new ProcessStartInfo(UserSettings.Default.SteamCMDDir + "\\steamcmd.exe", "+login " + UserSettings.Default.SteamUsername + " " + UserSettings.Default.SteamPassword + " " + cmdList + "+quit");

            ProcessStartInfo processInfo = new ProcessStartInfo(UserSettings.Default.SteamCMDDir + "\\steamcmd.exe", " +login " + Variables.main.SteamUsername.Password + " " + Variables.main.SteamPassword.Password + " " + cmdList + "+quit");

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
                Variables.fileOps.RenameAndMoveMods("DOWNLOAD");
            }
        }

        public void UpdateModsFromSteam()
        {
            //Move all mods from the mods directory to the SteamCMD directory for updating.
            Variables.fileOps.RenameAndMoveMods("UPDATE");

            //Stores the proper commands that will be passed to SteamCMD
            string cmdList = "";

            //Get a list of commamds for each mod stored in a single string
            for (int i = 0; i < Variables.appIDs.Count; i++)
                cmdList += "+\"workshop_download_item 262060 " + Variables.appIDs[i] + "\" ";

            ProcessStartInfo processInfo = new ProcessStartInfo(UserSettings.Default.SteamCMDDir + "\\steamcmd.exe", " +login " + Variables.main.SteamUsername.Password + " " + Variables.main.SteamPassword.Password + " " + cmdList + "+quit");

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
                Variables.fileOps.RenameAndMoveMods("DOWNLOAD");
            }
        }
    }
}
