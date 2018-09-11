@echo off

call "..\psake\psake.cmd" "clean.ps1 -parameters @{\"projectNameParam\"=\"PoFile\";\"versionParam\"=\"15\"}"

pause