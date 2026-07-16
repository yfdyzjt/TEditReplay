@echo off
setlocal enabledelayedexpansion

set "SRC=src"
set "DST=TEdit\src\TEdit\Editor\Plugins"

for %%f in ("%SRC%\*") do (
    set "name=%%~nxf"
    if exist "%DST%\!name!" (
        for %%a in ("%SRC%\!name!") do set src_time=%%~ta
        for %%b in ("%DST%\!name!") do set dst_time=%%~tb
        if "!src_time!" gtr "!dst_time!" (
            copy /Y "%SRC%\!name!" "%DST%\!name!" >nul 2>&1
        ) else if "!dst_time!" gtr "!src_time!" (
            copy /Y "%DST%\!name!" "%SRC%\!name!" >nul 2>&1
        )
    ) else (
        copy /Y "%SRC%\!name!" "%DST%\!name!" >nul 2>&1
    )
)
