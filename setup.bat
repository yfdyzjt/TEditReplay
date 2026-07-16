@echo off
setlocal enabledelayedexpansion

set "SRC=src"
set "DST=TEdit\src\TEdit\Editor\Plugins"

set FILES=ReplayRecorderPlugin.cs ReplayRecorderPluginFile.cs ReplayRecorderPluginRecorder.cs ReplayRecorderPluginPlayer.cs ReplayRecorderPluginRecorderView.xaml ReplayRecorderPluginRecorderView.xaml.cs ReplayRecorderPluginPlayerView.xaml ReplayRecorderPluginPlayerView.xaml.cs

for %%f in (%FILES%) do (
    if exist "%SRC%\%%f" if exist "%DST%\%%f" (
        for %%a in ("%SRC%\%%f") do set src_time=%%~ta
        for %%b in ("%DST%\%%f") do set dst_time=%%~tb
        if "!src_time!" gtr "!dst_time!" (
            copy /Y "%SRC%\%%f" "%DST%\%%f" >nul 2>&1
        ) else if "!dst_time!" gtr "!src_time!" (
            copy /Y "%DST%\%%f" "%SRC%\%%f" >nul 2>&1
        )
    ) else if exist "%SRC%\%%f" (
        copy /Y "%SRC%\%%f" "%DST%\%%f" >nul 2>&1
    ) else if exist "%DST%\%%f" (
        copy /Y "%DST%\%%f" "%SRC%\%%f" >nul 2>&1
    )
)
