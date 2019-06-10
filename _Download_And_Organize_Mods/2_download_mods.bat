@echo off

::Enter Steam credentials
set username=<USERNAME>
set password=<PASSWORD>

::Iterate through all IDs
:repeatOnArgs
if not "%1" == "" (
	::Feedback
	:echo Processing ID %1
	::Create a "list" of commands for SteamCMD to download
	set list=%list% +"workshop_download_item 262060 %1" validate
	::Create a "List" of names to later rename the folders according to load order
	set names=%names% %1
	::Remove the first ID
	shift
	::Repeat on the next argument
	goto repeatOnArgs
)

::Request download from Steam for this ID
steamcmd +login %username% %password% %list% +quit