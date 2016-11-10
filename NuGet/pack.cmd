@echo off
@IF EXIST "_stage" ( goto :error )
mkdir _stage
mkdir _stage\tools
rem xcopy ..\JsonMerge.Cli\bin\Release\JsonMerge.Cli.exe _stage\tools\ /Y
rem xcopy ..\JsonMerge.Cli\bin\Release\JsonMerge.Cli.pdb _stage\tools\ /Y 
rem xcopy ..\JsonMerge.Cli\bin\Release\Newtonsoft.Json.dll _stage\tools\ /Y
rem xcopy ..\JsonMerge.Cli\bin\Release\Newtonsoft.Json.xml _stage\tools\ /Y

xcopy JsonMerge.Cli.nuspec _stage\ /Y
..\packages\ILRepack.1.25.0\tools\ILRepack.exe /out:_stage\tools\JsonMerge.Cli.exe ..\JsonMerge.Cli\bin\Release\JsonMerge.Cli.exe ..\JsonMerge.Cli\bin\Release\Newtonsoft.Json.dll

cd _stage\
..\..\.nuget\nuget.exe pack JsonMerge.Cli.nuspec
cd ..

exit

:error
echo ERROR: Remove "_stage" dir first
