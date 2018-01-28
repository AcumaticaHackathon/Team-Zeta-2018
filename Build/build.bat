rmdir "%CD%\..\Packages\Temp" /s /q > NUL 2>&1
rmdir "%CD%\..\Packages\" /s /q > NUL 2>&1
mkdir "%CD%\..\Packages\Temp" > NUL 2>&1

XCOPY %CD%\..\TeamZetaUI "%CD%\..\Packages\Temp" /S /E /H /R /Y > NUL

PX.CstBuildProject.exe /website "C:\Acumatica\17.204.0019\Hackathon2018" /in "%CD%\..\Packages\Temp" /out "%CD%\..\Packages\TeamZeta.zip" ^
/include "%CD%\..\TeamZeta\bin\Debug\TeamZeta.dll" "bin\TeamZeta.dll" ^
/description "Team Zeta 2018"