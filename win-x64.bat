@echo off

dotnet publish Test.Desktop -c Release -r win-x64 -p:BuildTarget=Windows -o Release\win-x64
explorer Release\win-x64
