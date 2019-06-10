@echo off

::Iterate through all folders
set index=0

::Iterate through all folders
:repeatOnArgs
if not "%1" == "" (
	::Feedback
	:echo Processing folder %1
	rename D:\steamcmd\steamapps\workshop\content\262060\%1 %index%
	::If the folder name is a single digit, add two leading zeroes
	if %index% lss 10 ( rename D:\steamcmd\steamapps\workshop\content\262060\%index% 00%index%)
	::If the folder name are double digits, add one leading zero
	if %index% lss 100 ( if %index% gtr 9 ( rename D:\steamcmd\steamapps\workshop\content\262060\%index% 0%index%))
	::Increment index
	set /a index=index+1
	::Remove the first ID
	shift
	::Repeat on the next argument
	goto repeatOnArgs
)

::Wait for input, close cmd afterwards
pause