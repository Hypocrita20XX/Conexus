using Conexus.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Conexus.Core
{
    public class FileOperations
    {
        //Creates organized folders in the mods directory, then copies files from the SteaCMD directory to those folders
        //Requires that an operation be specified (DOWNLOAD or UPDATE)
        public void RenameAndMoveMods(string DownloadOrUpdate)
        {
            //Create source/destination path list variables
            string[] source = new string[Variables.appIDs.Count];
            string[] destination = new string[Variables.modInfo.Count];

            //If the user has downloaded/Updated mods, copy all files/folders from the SteamCMD directory to the mod directory
            if (DownloadOrUpdate == "DOWNLOAD")
            {
                //Get the proper path to copy from
                for (int i = 0; i < Variables.appIDs.Count; i++)
                    source[i] = Path.Combine(UserSettings.Default.SteamCMDDir + "\\steamapps\\workshop\\content\\262060\\", Variables.appIDs[i]);

                //Get the proper path that will be copied to
                for (int i = 0; i < Variables.modInfo.Count; i++)
                    destination[i] = Path.Combine(UserSettings.Default.ModsDir, Variables.modInfo[i]);

                //Copy all folders/files from the SteamCMD directory to the mods directory
                for (int i = 0; i < destination.Length; i++)
                    CopyFolders(source[i], destination[i]);

                //Check to ensure the last mod is in the destination directory
                if (Directory.Exists(destination[Variables.modInfo.Count - 1]) && Variables.modInfo.Count != 0)
                {
                    //If so, delete all folders/files in the source destination
                    for (int i = 0; i < Variables.appIDs.Count; i++)
                    {
                        if (Directory.Exists(source[i]))
                        {
                            //Delete the directory
                            Directory.Delete(source[i], true);
                        }
                    }
                }

                //Added v1.2.0
                //Let the user know this process has finished
                Variables.main.Messages.Text = "Downloading has finished.\n" + "Mods are now downloded, moved, and renamed.\n" + "You're ready to play!";
            }

            //If the userwants to update mods, copy all files/folders from the mod directory to the SteamCMD directory
            if (DownloadOrUpdate == "UPDATE")
            {
                //Get the proper path to copy from
                for (int i = 0; i < Variables.appIDs.Count; i++)
                    source[i] = Path.Combine(UserSettings.Default.ModsDir + "\\", Variables.modInfo[i]);

                //Get the proper path that will be copied to
                for (int i = 0; i < Variables.modInfo.Count; i++)
                    destination[i] = Path.Combine(UserSettings.Default.SteamCMDDir + "\\steamapps\\workshop\\content\\262060\\", Variables.appIDs[i]);

                //Copy all folders/files from the SteamCMD directory to the mods directory
                for (int i = 0; i < destination.Length; i++)
                    CopyFolders(source[i], destination[i]);

                //Check to ensure the last mod is in the destination directory
                if (Directory.Exists(destination[Variables.modInfo.Count - 1]) && Variables.modInfo.Count != 0)
                {
                    //If so, delete all folders/files in the source destination
                    for (int i = 0; i < Variables.appIDs.Count; i++)
                    {
                        if (Directory.Exists(source[i]))
                        {
                            //Delete the directory
                            Directory.Delete(source[i], true);
                        }
                    }
                }

                //Added v1.2.0
                //Let the user know this process has finished
                Variables.main.Messages.Text = "Updating has finished.\n" + "Mods are now updated, moved, and renamed.\n" + "You're ready to play!";
            }
        }

        //A base function that will copy/rename any given folder(s)
        //Can be used recursively for multiple directories
        public void CopyFolders(string source, string destination)
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
        public void WriteToFile(string[] text, string fileDir)
        {
            File.WriteAllLines(@fileDir, text);
        }
    }
}
