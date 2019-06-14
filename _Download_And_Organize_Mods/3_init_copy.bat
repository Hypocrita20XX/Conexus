@echo off

::Set mod directory here
set modDirectory=<ModDirectory>

::Copy all folders/mods in steamcmd to DarkestDungeon\mods
xcopy /s D:\steamcmd\steamapps\workshop\content\262060 %modDirectory%