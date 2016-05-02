@echo off

call ".\psake\psake.cmd" "releaseDeploy.ps1 -parameters @{\"projectNameParam\"=\"JsonFile\"}"

pause