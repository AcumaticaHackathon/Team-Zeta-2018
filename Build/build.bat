rmdir "%CD%\..\CstPackages\Temp" /s /q > NUL 2>&1
rmdir "%CD%\..\CstPackages\" /s /q > NUL 2>&1
mkdir "%CD%\..\CstPackages\Temp" > NUL 2>&1

XCOPY %CD%\..\TeamZetaUI "%CD%\..\CstPackages\Temp" /S /E /H /R /Y > NUL

PX.CstBuildProject.exe /website "%CD%\..\..\Site" /in "%CD%\..\CstPackages\Temp" /out "%CD%\..\CstPackages\TeamZeta.zip" ^
/include "%CD%\..\TeamZeta\bin\Debug\TeamZeta.dll" "bin\TeamZeta.dll" ^
/description "Team Zeta 2018"