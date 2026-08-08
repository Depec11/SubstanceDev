@echo off

dotnet publish Test.Android -c Release -r android-arm64 -o Release\android-arm64
explorer Release\android-arm64
