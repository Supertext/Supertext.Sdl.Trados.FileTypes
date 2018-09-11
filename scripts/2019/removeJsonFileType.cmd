@echo off

call "..\psake\psake.cmd" "clean.ps1 -parameters @{\"projectNameParam\"=\"JsonFile\";\"versionParam\"=\"15\"}"

pause