# Download_And_Organize_Mods
A process and set of batch files that allows mods to be downloaded and organized according to a Steam collection\
Intended to be used with Darkest Dungeon


### Caveats
This process is meant to be used in conjunction with a Steam collection\
While it's certainly possible to use this method and batch files without it, I wouldn't recommend it simply because of organization\
A collection allows you to order your mods as needed in a relatively short amount of time (especially compared to the in-game GUI)\
But as long as you get a list of mod IDs, the batch files will take care of the rest for you, regardless of how you get them

It is worth mentioning that the end results are purely functional, you will not be able to get a mod name directly by looking at the folder
This is due in part to how the game loads mods, and also certainly limitations in my understanding of SteamCMD
I would recommend going in-game into the mods UI, if you hover over the mod listing, it will tell you which folder it comes from

Furthermore, I highly recommend using Notepad++ for this method as its RegEx find/replace functionality is used extensively/
You can find a download for Notepad++ [here](https://notepad-plus-plus.org/download/v7.7.html)

All batch files were made with the D drive in mind
If you're doing this on any other drive, you'll need to modify the batch files accordingly
Directories are located in 2_download_mods.bat and 4_rename_mods.bat

Also, don't try to throw over 1,000 mod IDs into this thing, it might work, but it probably won't]
(If you try this, you have issues, [go view some cute puppy pictures and reconsider your life choices](https://www.google.com/search?hl=en&tbm=isch&source=hp&biw=1097&bih=554&ei=q5b-XImEMaW2ggfBhqnwBA&q=cute+puppies&oq=cute+puppies&gs_l=img.3..0l10.1238.2824..3013...0.0..0.457.4259.4-10......0....1..gws-wiz-img.....0.ioYWSAoIT_A))

Lastly, this process was made using a Windows OS\
I mention this because I'll be mentioning key combinations, and I know that varies based on OS

Now for some standard legalities\
I am not responsible for any unintended side-effects that one may come across, \
including (but not limited to) loss of files, excessive download times, explosive drive failure, and/or cute puppies

But serously, don't do weird stuff with this\
Batch files are scary


### Methodology
Aside from the aforementioned Notepad++, you'll also need [SteamCMD](https://developer.valvesoftware.com/wiki/SteamCMD#Windows)\
Of course, download the batch files as well from the [repository](https://github.com/Hypocrita20XX/Download_And_Organize_Mods)\
I highly recommend placing SteamCMD and the batch files in its own folder on your main drive without any spaces
IE C:\steamcmd or D:\steamcmd\
Spaces are evil and will cause catastrophic failure (probably)\
Also the batch files need to be placed in the same directory as steamcmd.exe

As mentioned previously, if you're going to use SteamCMD from any drive other than D, modify all references to that drive\
Again, those directories are located in 2_download_mods.bat and 4_rename_mods.bat

Also, you'll need to provide your Steam credentials in 2_download_mods.bat\
Just replace <USERNAME> and <PASSWORD> with that information

### SteamCMD Setup
1. Open the command prompt with Win+R, then type in cmd
2. Navigate to the location of the steamcmd executable (use cd <directory> if it's on the C drive, otherwise enter <drive>: and then cd <directory> afterwards)
3. Enter steamcmd.exe and let it do its magic (it will hopefully tell you it's finished)

### Initial prep work
1.  Assuming you have a collection, navigate to that collection
2.  Right-click any empty space and select "view source"
3.  Press CTRL+A to select all text, then CTRL+C
4.  Open up a new text file in Notepad++, paste the source code into that
5.  Press CTRL+F to open the find window, search for the first mod name in your collection
6.  Once found, go a line above that and delete everything preceding it
7.  Now enter the name of the last mod in the collection in the find window
8.  Go a line down from that, delete everything that comes afterward
9.  Again in the find window, enter "a href" and press "find all in current documents"
10. A new window should pop up at the bottom of Notepad++, CTRL+A in that window, then copy/paste that result into your current file

To proceed, be sure you've enabled the regular expression option in the find window

### Deleting fluff
1. Switch over to the replace tab in the find window, enter .+?id= into the find text field, make sure the replace field is completey blank, press "replace all"
2. Do the same as above, but with "><div.*, press "replace all"
3. Same as above, this time with 262060">.*
4. Once again with feeling, but with a twist: enter \s+, but make sure the replace field has a single space (very important!)

You should now have a single line list of mod IDS, separated by a single space
If not, you need to go back and do it again until that is your result
Otherwise, the batch files won't work

That's all of the text prep work out of the way
Everything from here on out should be straightforward

### DOS magic
1. In 1_init_download.bat and 3_rename_mods.bat, put that long list of ids in place of <IDs>
2. Double-click to run 1_init_download.bat, you may be prompted to enter a SteamGuard code, once entered, your downloads should start
3. By default, you'll find all downloads in <drive>:\steamcmd\steamapps\workshop\content\262060\ (it's fun to watch the folders appear)
4) Once all downloads are done (it should close automatically) you'll need to double-click to run 3_init_rename.bat
5. This process is super quick, but afterwards you should have folders with incredibly intuitive names such as 000 and 016

### Windows explorer magic
Now all you need to do is move all folders in this directory to the mods directory of Darkest Dungeon\
(Usually in <drive>:\Steam\steamapps\common\

### Game magic
In-game, when you click on the mods button, all mods listed should now be in the exact order as your collection\
If not, do not pass go, do not collect $200\
If so, yay, you did the thing!


### Final thoughts
This method and associated batch files, while intended for Darkest Dungeon, can rather easily be modified to work with any game that has mod support\
I don't know exactly how other games load their mods, but I would assume the naming convention used would work in a fair few cases\
If not, you're more than welcome to modify any and all files/steps to get the desired result\
Just throw a thanks my way if you do, that would be great\
