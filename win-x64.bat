@echo off

dotnet publish Test.Windows -c Release -r win-x64 -o Release\win-x64
explorer Release\win-x64
