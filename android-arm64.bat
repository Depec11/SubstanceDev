@echo off

dotnet publish Test.Android -c Release -r android-arm64 -p:BuildTarget=Android -o Release\android-arm64
explorer Release\android-arm64
