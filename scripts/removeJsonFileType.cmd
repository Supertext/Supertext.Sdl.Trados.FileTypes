@echo off

call ".\psake\psake.cmd" "removePlugin.ps1 -parameters @{\"projectNameParam\"=\"JsonFile\"}"

pause