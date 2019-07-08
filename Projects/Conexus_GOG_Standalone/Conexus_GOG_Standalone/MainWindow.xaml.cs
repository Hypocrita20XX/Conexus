using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using System.Net;

namespace Conexus_GOG_Standalone
{
    public partial class MainWindow : Window
    {
        List<ModInfo> items = new List<ModInfo>();
        int index = 0;

        public MainWindow()
        {
            //Init MainWindow
            InitializeComponent();

            //Create a new blank listing
            items.Add(new ModInfo { UserIndex = index.ToString(), UserModInfo = "Provide A Url"});
            //Add that listing to the listView
            ModInfo_ListView.Items.Add(items[index]);
            //Increment index
            index++;
        }

        //Only fired when the enter(return) key is pressed
        private void UserIndex_TextChanged(object sender, KeyEventArgs e)
        {
            //Check for the proper key press
            if (e.Key.Equals(Key.Return))
            {
                //Query provided URL to ensure it is a proper URL
                //Create a WebClient
                WebClient webClient = new WebClient();
                //Create a TextBox
                TextBox textBox = (TextBox)sender;
                //Query information from the provided URL
                String urlInfo = webClient.DownloadString(textBox.Text);
                //Create a tooltip that will contain the mod description
                ToolTip tooltip = new ToolTip();

                //Parse through the provided URL, look for <title>Steam Workshop
                if (urlInfo.Contains("<title>Steam Workshop"))
                {
                    //If this information is found, we need to get just the title of the mod
                    //First get everything after "<title>Steam Workshop :: "
                    string tmp = urlInfo.Substring(urlInfo.IndexOf("<title>Steam Workshop :: "));
                    //Second, get everything up to "</title>"
                    tmp = tmp.Substring(24, tmp.IndexOf("</title>") - 24);
                    //Finally, assign the textbox text to the mod title
                    (sender as TextBox).Text = tmp;
                    
                    //Parse through the provided URL, this time looking for the mod description
                    string description = urlInfo.Substring(urlInfo.IndexOf("Steam Workshop: Darkest Dungeon"));
                    tmp = description.Substring(35, description.IndexOf("\">") - 35);

                    //Assign the tooltip content
                    tooltip.Content = tmp;
                    //Assign the tooltip to the TextBox
                    textBox.ToolTip = tooltip;

                    //Now a new entry needs to be added below this one (NOT WORKING)
                    items.Add(new ModInfo { UserIndex = index.ToString(), UserModInfo = "Provide A Url" });
                    //Add that blank entry to the listView
                    ModInfo_ListView.Items.Add(items[index]);
                    //Increment index
                    index++;
                }

                //Clear focus of the text box
                Keyboard.ClearFocus();
            }
        }
    }

    public class ModInfo
    {
        public string UserIndex { get; set; }
        public string UserModInfo { get; set; }
    }
}