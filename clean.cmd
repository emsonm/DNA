@echo off
call "C:\Program Files (x86)\Microsoft Visual Studio\2017\Professional\Common7\Tools\VsDevCmd.bat" 
msbuild.exe ".\corelib\corelib.csproj" /t:clean
msbuild.exe ".\native\dna.vcxproj" /t:clean

