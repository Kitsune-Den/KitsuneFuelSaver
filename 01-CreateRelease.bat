@echo off
setlocal
pushd "%~dp0"
dotnet build -c Release || goto :fail
echo.
echo Mod folder ready at: %CD%\KitsuneFuelSaver
echo Copy that folder into ^<7D2D^>\Mods\
popd
exit /b 0
:fail
popd
exit /b 1
