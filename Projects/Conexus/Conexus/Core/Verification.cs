using Conexus.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Conexus.Core
{
    public class Verification
    {
        //Added v1.2.0
        //Goes through several verification steps to ensure a proper Steam collection URL has been entered
        public bool VerifyCollectionURL(string url, string fileDir)
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
                //Download the desired collection and save the file
                webClient.DownloadFile(url, fileDir + "\\HTML.txt");
            }
            //Not a valid URL
            catch (WebException)
            {
                //Clear URLLink Text
                Variables.main.URLLink.Text = string.Empty;
                //Provide a message to the user
                Variables.main.URLLink.Watermark = "Not a valid URL: " + url;
                //Flag this URL as invalid
                validURL = false;
            }
            //No URL at all, or something else that was unexpected
            catch (ArgumentException)
            {
                //Clear URLLink Text
                Variables.main.URLLink.Text = string.Empty;
                //Provide a message to the user
                Variables.main.URLLink.Watermark = "Not a valid URL: " + url;
                //Flag this URL as invalid
                validURL = false;
            }
            //I don't know why this triggers, but it does, and it's not for valid reasons
            catch (NotSupportedException)
            {
                //Clear URLLink Text
                Variables.main.URLLink.Text = string.Empty;
                //Provide a message to the user
                Variables.main.URLLink.Watermark = "Not a valid URL: " + url;
                //Flag this URL as invalid
                validURL = false;
            }
            //URL is too long
            catch (PathTooLongException)
            {
                //Clear URLLink Text
                Variables.main.URLLink.Text = string.Empty;
                //Provide a message to the user
                Variables.main.URLLink.Watermark = "URL is too long (more than 260 characters)";
                //Flag this URL as invalid
                validURL = false;
            }

            //If the link is valid, leads to an actual site, we need to check for a valid Steam site
            if (validURL)
            {
                //Download the desired collection and save the file
                webClient.DownloadFile(url, fileDir + "\\HTML.txt");

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

                //If these checks fail, this is not a valid Steam collection link and the user needs to know that
                if (!isValidSteam && !isValidCollection || isValidSteam && !isValidCollection)
                {
                    //Clear URLLink Text
                    Variables.main.URLLink.Text = string.Empty;
                    //Provide a message to the user
                    Variables.main.URLLink.Watermark = "Not a valid URL: " + url;

                    //Cleanup
                    webClient.Dispose();
                    file.Close();

                    return false;
                }
                else
                {
                    //Cleanup
                    webClient.Dispose();
                    file.Close();

                    return true;
                }
            }
            //Otherwise this is not a valid link and the user needs to know that
            else
            {
                //Clear URLLink Text
                Variables.main.URLLink.Text = string.Empty;
                //Provide a message to the user
                Variables.main.URLLink.Watermark = "Not a valid URL: " + url;

                //Cleanup
                webClient.Dispose();

                return false;
            }
        }

        //Added v1.2.0
        //Goes through several verification steps to ensure that the given SteamCMD directory is valid, contains steamcmd.exe
        public bool VerifySteamCMDDir(string fileDir)
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

        //Added v1.2.0
        //Goes through several verification steps to ensure that the given mods directory is valid, contains Darkest.exe
        public bool VerifyModDir(string fileDir)
        {
            /*
             * 
             * This verification check is fairly straightforward: we check the given directory and see if we can find Darkest.exe
             * Unlike steamcmd.exe, this is located in a different folder DarkestDungeon\_windows
             * So we first need to navigate to the root directory, then to _windows and check for the exe
             * 
             */

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
    }
}
