![Conexus Banner](https://github.com/Hypocrita20XX/Conexus/blob/master/Images/Banner.png)

# Welcome To Conexus!
*Conexus is an all-in-one tool to convert a Steam collection or list of links into a load order for the game [Darkest Dungeon](https://store.steampowered.com/app/262060/Darkest_Dungeon/).* 

## Your Load Order, Simplified
*If you've modded Darkest Dungeon for any length of time, you've more than likely discovered that the in-game UI breaks after an unknown  amount of mods are loaded in.*  

**Enter Conexus...** </br>
![Conexus GUI](https://github.com/Hypocrita20XX/Conexus/blob/master/Images/Conexus__1_2_1_UI.png)</br>

*With the help of a straightforward GUI, Conexus will anaylze your Steam collection, download or update those mods (you can do both!) and then move those folders to the correct location with names that you can understand, and that the game will read in the exact order your Steam collection is in.*</br>
*Updating your mods with Conexus is just as easy. Just let him know you want to update, and he'll move your mods to the proper location, update them, and move them back for you, all with the same user-friendly names.*</br>
*Not using a Steam collection? No problem, Conexus has got your back! Just provide links in a text file, and Conexus will make sure your mods are organized as you decide.*</br>

***

### Disclaimers
*Please keep in mind that Conexus was developed on a machine running Windows 10. I don't have any experience with Linux or Mac, and cannot provide support for those operating systems. Thanks to the [MIT license](https://github.com/Hypocrita20XX/Conexus/blob/master/LICENSE) though, feel free to modify the code to create builds for those platforms! Just be sure to link back here, I would appreciate it.*</br>

*Ideally, you'll have a Steam collection where you've got all your mods organized in whatever order you like. However, with version 1.1.0, you can now provide a list of links, and Conexus will work all the same.*</br>
*Also be aware that if you're using a Steam collection, it needs the vsibility set to either unlisted or public. Private or friends-only unfortunately won't work.*

*It was discovered during the development of v1.2.0 that Epic and GOG are beyond Conexus' abilities. You can get more info on that [here](https://github.com/Hypocrita20XX/Conexus/issues/11)*

### Requirements
* *A legitimate copy of [Darkest Dungeon](https://store.steampowered.com/app/262060/Darkest_Dungeon/)*
* *[SteamCMD](https://developer.valvesoftware.com/wiki/SteamCMD)*

### Setup
* *You'll need to get SteamCMD up and running, go ahead and download it, then I would recommend extracting to C:\SteamCMD, or any other drive/location you would prefer. Best not to have spaces though.*
* *To get all the necessary files downloaded, you'll need to open the command prompt (Windows key + R, type in "cmd.")*
* *Once you've done that, type "cd: <location of steamcmd>"*
* *Next, type "steamcmd" and hit enter, SteamCMD should do its thing!*
* *Lastly, you'll need to download the latest [release](https://github.com/Hypocrita20XX/Conexus/releases) of Conexus. As of version 1.1.0, the installation is portable, put him wherever you want!*
* *That's all the really hard stuff out of the way, let's get to organizing!*

***

### Operating Conexus
*You'll need three bits of information:*
* *The URL of your Steam collection or list of links (saved in DarkestDungeon\mods\Links.txt)*
* *The directory that SteamCMD was extracted/installed to*
* *The directory of Darkest Dungeon's mod folder*
*Once you have that, you're ready to begin your adventure with Conexus!*</br>

*You'll notice that there are two options, via dropdown menus, at the bottom:*
* *Download Mods/Update Mods*
* *Steam Collection/List*</br></br>

* *Download mods, when selected, will do a full download of all mods listed in the provided collection URL. Please note that this assumes your Darkest Dungeon mod directory is empty, otherwise Conexus may explode. Best used when you're on your first adventure.*
* *Update mods, when selected, will move all mods from the Darkest Dungeon mod directory to the SteamCMD/workshop directory, renaming as needed and allowing SteamCMD to ensure your mods are up to date with their counterparts in the Workshop.*
* *Steam Collection, when selected, will parse through the given Steam collection, via URL, and ensure your mods are organized as specified in the collection.*
* *List, when selected, will parse through the links you've provided in DarkestDungeon\mods\Links.txt and make sure your mods reflect the order given in the file.*</br>

*Once you have all the needed information stored with Conexus, and your mode of operation has been decided, simply hit the "organize mods" button and Conexus will do his best to ensure your in-game load order is of the highest quality.*
*Do note that this can take some time, depending on how large your mod list is. As of v1.2.0, Conexus will now tell you what he's working on and when. However if you want to make sure everything is operating normally, just navigate to your Darkest Dungeon mod directory and your SteamCMD workshop directory. If you see folders appearing, disappearing, and/or doing a fine jig, everything is as it should be.</br>
Just be patient, quality work takes time!*</br>

*As of version 1.1.0, Conexus now offers a second method to utilize his services. This comes in the form of a text file, that you can enter URLs to all your mods. This file is located in DarkestDungeon\mods\Links.txt, and all you need to do is provide one URL per line. For organization, you can also add comments, with any line starting with an asterisk* * *. Useful for titles of each mod, or any other bit of snazzy info you would like!*

![Links Text File](https://github.com/Hypocrita20XX/Conexus/blob/master/Images/Links%20Text%20File.png)</br>

#### Potential Issues
*Sometimes Conexus gets too caught up in his work, and can leave behind folders in places they shouldn't be.*</br>
*After an operation, it's always best to ensure your SteamCMD workshop directory is clean of any excess folders.*</br>
*If you encounter any other issues, do let me know! Unfortunately I don't have a PHD in pest control, so bugs could happen spontaneously.*</br></br>
*As of v1.2.0, there's now a log file that gets created after Conexus has done his job. This is located in DarkestDungeon\mods\_Logs. If you have an issue, send that to me via the [Github issue tracker!](https://github.com/Hypocrita20XX/Conexus/issues)*

#### Closing Thoughts
*As I mentioned earlier, this entire project, source code included, is [licensed under MIT](https://github.com/Hypocrita20XX/Conexus/blob/master/LICENSE)*
*This means that if you, or anyone else, knows of a game that could use Conexus, you're more than welcome to modify him as needed to work with those games.*
*I'm also very much open to others taking what I've done here and making it even better. Knowledge should be open to everyone, and I intend on doing my best to share what I can without any limitations.*</br>

## I Hope Conexus Will Take You On Many Fulfilling Adventures!</br>

![Conexus Banner](https://github.com/Hypocrita20XX/Conexus/blob/master/Images/Banner.png)
