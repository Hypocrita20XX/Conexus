![Conexus Banner](https://staticdelivery.nexusmods.com/mods/804/images/858/858-1614678343-1747320964.png)

# Welcome To Conexus!
*Conexus is an all-in-one tool to convert a Steam collection or list of links into a load order for the game [Darkest Dungeon](https://store.steampowered.com/app/262060/Darkest_Dungeon/).* 

## Your Load Order, Simplified
*If you've modded Darkest Dungeon for any length of time, you've more than likely discovered that the in-game UI breaks after an unknown  amount of mods are loaded in.*  

**Enter Conexus...** </br>
![Conexus GUI](https://staticdelivery.nexusmods.com/mods/804/images/858/858-1614678357-1856980801.png)</br>

*With the help of a straightforward GUI, Conexus will anaylze your Steam collection, download or update those mods (you can do both!) and then move those folders to the correct location with names that you can understand, and that the game will read in the exact order your Steam collection is in.*</br>
*Not using a Steam collection? No problem, Conexus has got your back! Just provide links in a text file (located in Documents\Conexus\Links), and Conexus will make sure your mods are organized as you decide!*</br>

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
*You'll need four bits of information:*
* *The URL of your Steam collection or list of links (saved in DarkestDungeon\mods\Links.txt)*
* *The directory that SteamCMD was extracted/installed to*
* *The directory of Darkest Dungeon's mod folder*
* *Your Steam credentials*

*Once you have that, you're ready to begin your adventure with Conexus!*</br>

*Starting with v1.4.0, there's no longer an need to select modes or methods, everything regarding those is now automatic! If you want to use a list of links, just make sure you don't have a URL in that text field, and Conexus will load your Links.txt from Documents\Conexus\Links.*

*Once you have all the needed information stored with Conexus, simply hit the "organize mods" button and Conexus will do his best to ensure your in-game load order is of the highest quality.*
*Do note that this can take some time, depending on how large your mod list is. As of v1.2.0, Conexus will now tell you what he's working on and when. However if you want to make sure everything is operating normally, just navigate to your Darkest Dungeon mod directory and your SteamCMD workshop directory. If you see folders appearing, disappearing, and/or doing a fine jig, everything is as it should be.</br>
Just be patient, quality work takes time!*</br>

*As of version 1.1.0, Conexus now offers a second method to utilize his services. This comes in the form of a text file, that you can enter URLs to all your mods. This file is located in Documents\Conexus\Links\Links.txt, and all you need to do is provide one URL per line. You can add mod names, starting with v1.4.0, using an asterisk* * *, and Conexus will append that to the end of the folder name. Other than that, you're free to format your file in whatever suits you best!*
*For more information, please see [Understanding Links.txt](https://github.com/Hypocrita20XX/Conexus/wiki/P-07:-Understanding-Links.txt) on the wiki, in the [user guide](https://github.com/Hypocrita20XX/Conexus/blob/master/Documentation/Guides/Conexus%20User's%20Guide%20v1.4.0.pdf), or in the video [Understanding Links.txt](https://youtu.be/pM-9bJrp4M4) *

![Links Text File](https://staticdelivery.nexusmods.com/mods/804/images/858/858-1614678334-1685874311.png)</br>

*As of version 1.3.0, Conexus saves all data to Documents\Conexus. All user data is saved to Documents\Conexus\Config\config.ini*
*For more information regarding the config file, please see [Understanding Config.INI](https://github.com/Hypocrita20XX/Conexus/wiki/P-06:-Understanding-Config.INI) on the wiki, in the [user guide](https://github.com/Hypocrita20XX/Conexus/blob/master/Documentation/Guides/Conexus%20User's%20Guide%20v1.4.0.pdf), or in the video [Understanding Config.INI](https://youtu.be/MwJfOzasrcc)*

![Config INI](https://staticdelivery.nexusmods.com/mods/804/images/858/858-1614681261-794366316.png)</br>

*Starting with v1.3.0, He'll no longer save your Steam credentials, but you can modify Config.ini to include that information, and he'll load them just fine!*

#### Potential Issues
*If you encounter any issues, do let me know! Unfortunately I don't have a PHD in pest control, so bugs could happen spontaneously.*</br></br>
*As of v1.2.0, there's now a log file that gets created after Conexus has done his job. This is located in Documents\Conexus\Logs. If you have an issue, send that to me via the [Github issue tracker!](https://github.com/Hypocrita20XX/Conexus/issues)*

#### Documentation
*I've done my very best to provide adequate documentation in various forms. If you're more into videos, you can check out [the video tab on Nexus Mods](https://www.nexusmods.com/darkestdungeon/mods/858?tab=videos) or [this playlist on Youtube](https://www.youtube.com/playlist?list=PLet2vPZn40UGuVOUWylHQwKDY6rYxZoKb) for a series of videos covering every aspect of Conexus.*</br> 
*If you would prefer text, you can check out the PDF guides available [in the files tab on Nexus Mods](https://www.nexusmods.com/darkestdungeon/mods/858?tab=files) as well as [on Github](https://github.com/Hypocrita20XX/Conexus/tree/master/Documentation/Guides)*</br>
*Every release archive also contains a short quickstart guide that should get you up and running, if you prefer shorter documentation. If you don't like Youtube or just want a copy of the video series, that's also available in the files tab.*</br>
*I've made two archives available, one without captions, and another with captions baked into each video. There's also a transcript, if you would like that as well!*</br>
*Lastly, if you're a developer, I've got an archive [on Nexus Mods](https://www.nexusmods.com/darkestdungeon/mods/858?tab=files) containing source code for several recent releases. The code for the latest release is available here on Github and in archive form [in the releases section](https://github.com/Hypocrita20XX/Conexus/releases), basic informating concerning setting up a dev environment can be found [in this readme](https://github.com/Hypocrita20XX/Conexus/blob/master/Project/README.md).*</br>
*Please keep in mind that I can't help you set up a dev environment. I know how to set it up on my system, but have no experience doing so for anyone else.*</br>
*Sorry about that!*

#### Closing Thoughts
*As I mentioned earlier, this entire project, source code included, is [licensed under MIT](https://github.com/Hypocrita20XX/Conexus/blob/master/LICENSE)*
*This means that if you, or anyone else, knows of a game that could use Conexus, you're more than welcome to modify him as needed to work with those games.*
*I'm also very much open to others taking what I've done here and making it even better. Knowledge should be open to everyone, and I intend on doing my best to share what I can without any limitations. Just be sure to provide credit where it's due.*</br>

## I Hope Conexus Will Take You On Many Fulfilling Adventures!</br>

![Conexus Banner](https://staticdelivery.nexusmods.com/mods/804/images/858/858-1614678343-1747320964.png)
