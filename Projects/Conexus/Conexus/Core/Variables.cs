using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Conexus.Core
{
    public static class Variables
    {
        //Create an instance of each class
        public static MainWindow main = new MainWindow();
        /*
        public static DataProcesses dataPrc = new DataProcesses();
        public static FileOperations fileOps = new FileOperations();
        public static SteamProcesses steamPrc = new SteamProcesses();
        public static Verification verify = new Verification();
        */

        //Lists to store info related to the mods that will/are downloaded
        public static List<string> modInfo;
        public static List<string> appIDs;

        //Bools to store the value of each combobox
        public static bool downloadMods;
        public static bool updateMods;

        //Bool to store which method the user has selected
        public static bool steam;
    }
}
