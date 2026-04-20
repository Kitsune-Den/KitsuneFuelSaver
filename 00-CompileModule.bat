@echo off
setlocal
set MSBUILD="C:\Program Files\Microsoft Visual Studio\2022\Community\MSBuild\Current\Bin\MSBuild.exe"
if not exist %MSBUILD% set MSBUILD="C:\Program Files (x86)\Microsoft Visual Studio\2019\BuildTools\MSBuild\Current\Bin\MSBuild.exe"
if not exist %MSBUILD% (
    echo MSBuild not found. Install Visual Studio 2019+ or Build Tools.
    exit /b 1
)
%MSBUILD% KitsuneFuelSaver.sln /p:Configuration=Release /v:minimal
endlocal
