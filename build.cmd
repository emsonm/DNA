@echo off
call "C:\Program Files (x86)\Microsoft Visual Studio\2017\Professional\Common7\Tools\VsDevCmd.bat" 
msbuild.exe ".\dna.sln" /t:rebuild /property:Configuration=Release /property:Platform=x86

