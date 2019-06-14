@echo off

::Set mod directory here
set modDirectory=<ModDirectory>
::For load-order accurate renaming
set index=0

::Iterate through all folders
:repeatOnArgs
if not "%1" == "" (
	::Feedback
	:echo Processing folder %1
	::Rename the folder appropriately
	rename %modDirectory%\%1 %index%
	::If the folder name is a single digit, add two leading zeroes
	if %index% lss 10 ( rename %modDirectory%\%index% 00%index%)
	::If the folder name are double digits, add one leading zero
	if %index% lss 100 ( if %index% gtr 9 ( rename %modDirectory%\%index% 0%index%))
	::Increment index
	set /a index=index+1
	::Remove the first ID
	shift
	::Repeat on the next argument
	goto repeatOnArgs
)

::Wait for input, close cmd afterwards
pause