@echo off

call ".\psake\psake.cmd" "deploy.ps1 -parameters @{\"projectNameParam\"=\"PoFile\"}"

pause