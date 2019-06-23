![Conexus Banner](https://github.com/Hypocrita20XX/Conexus/blob/master/Images/Banner.png)

# Welcome To Conexus!
*Conexus is an all-in-one tool to convert a Steam collection into a load order for the game [Darkest Dungeon](https://store.steampowered.com/app/262060/Darkest_Dungeon/).* 

## Your Load Order, Simplified
*If you've modded Darkest Dungeon for any length of time, you've more than likely discovered that the in-game UI breaks after an unknown  amount of mods are loaded in.*  

**Enter Conexus...** </br>
![Conexus GUI](https://github.com/Hypocrita20XX/Conexus/blob/master/Images/GUI.png)</br>

*With the help of a straightforward GUI, Conexus will anaylze your Steam collection, download or update those mods (you can do both!) and then move those folders to the correct location with names that you can understand, and that the game will read in the exact order your Steam collection is in.*</br>
*Updating your mods with Conexus is just as easy. Just let him know you want to update, and he'll move your mods to the proper location, update them, and move them back for you, all with the same user-friendly names.*</br>

***

### Disclaimers
*Please keep in mind that Conexus was developed on a machine running Windows 10. I don't have any experience with Linux or Mac, and cannot provide support for those operating systems. Thanks to the [license](https://github.com/Hypocrita20XX/Conexus/blob/master/LICENSE) though, feel free to modify the code to create builds for those platforms! Just be sure to link back here, I would appreciate it.*</br>

*Also, you absolutely have to have a Steam collection, otherwise Conexus simply cannot work. Steam collections thankfully provide a functional way to organize one's mods that Darkest Dungeon's UI simply cannot do at this point in time, and so Conexus has a hard requirement of you having one.*

### Requirements
* *A legitimate copy of [Darkest Dungeon](https://store.steampowered.com/app/262060/Darkest_Dungeon/)*
* *[SteamCMD](https://developer.valvesoftware.com/wiki/SteamCMD)*

### Setup
* *You'll need to get SteamCMD up and running, go ahead and download it, then I would recommend extracting to C:\SteamCMD, or any other drive/location you would prefer. Best not to have spaces though.*
* *To get all the necessary files downloaded, you'll need to open the command prompt (Windows key + R, type in "cmd.")*
* *Once you've done that, type "cd: <location of steamcmd>"*
* *Next, type "steamcmd" and hit enter, SteamCMD should do its thing!*
* *Lastly, you'll need to download and install the latest [release}(https://github.com/Hypocrita20XX/Conexus/releases) of Conexus, it'll save to C:\Conexus by default*
* That's all the really hard stuff out of the way, let's get to organizing!

***

### Operating Conexus
*You'll need five bits of information:*
* *The URL of your Steam collection*
* *The directory that SteamCMD was extracted/installed to*
* *The directory of Darkest Dungeon's mod folder*
* *Your Steam credentials (username and password*
*Once you have that, you're ready to begin your adventure with Conexus!*</br></br>

*You'll notice that there are three options at the bottom:*
* *Download mods*
* *Update mods*
* *Save credentials*</br>

* *Download mods, when checked, will do a full download of all mods listed in the provided collection URL. Please note that this assumes your Darkest Dungeon mod directory is empty, otherwise Conexus may explode. Best used when you're on your first adventure.*
* *Update mods, when checked, will move all mods from the Darkest Dungeon mod directory to the SteamCMD/workshop directory, renaming as needed and allowing SteamCMD to ensure your mods are up to date with their counterparts in the Workshop.*
* *Save credentials, when checked, will save your Steam username and password. Please keep in mind that they will be saved unencrypted, in plain text. This is why Conexus does not have this checked by default.*</br>

*Once you have all the needed information stored with Conexus, and your mode of operation has been decided, simply hit the "organize mods" button and Conexus will do his best to ensure your in-game load order is of the highest quality.*
*Do note that this can take some time, depending on how large your mod list is. If you want to make sure everything is operating normally, just navigate to your Darkest Dungeon mod directory and your SteamCMD workshop directory. If you see folders appearing, disappearing, and/or doing a fine jig, everything is as it should be.</br>
Just be patient, quality work takes time.*</br>

####
