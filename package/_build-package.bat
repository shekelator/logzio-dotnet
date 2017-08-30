@ECHO OFF

set framework=%1
set dllsuffix=%2
set project=%dllsuffix%Shipper

call _clear %framework%


echo Making directory tree...
mkdir %framework%
mkdir %framework%\out
mkdir %framework%\package
mkdir %framework%\package\lib
mkdir %framework%\package\lib\net45
mkdir %framework%\package\lib\net46

set ilmerge=~\.nuget\packages\ILMerge\2.14.1208\tools\ILMerge.exe
set nuget=~\.nuget\packages\NuGet.CommandLine\3.4.3\tools\NuGet.exe
set szip="C:\Program Files\7-Zip\7z.exe"

echo Building project...
dotnet build ..\src\%project%\%project%.csproj --framework netstandard2.0 --output=..\..\package\%project%

echo Merging DLLs...
%ilmerge% /out:%framework%\package\lib\net45\Logzio.DotNet.%dllsuffix%.dll %framework%\out\45\Logzio.DotNet.%dllsuffix%.dll %framework%\out\45\Logzio.DotNet.Core.dll 

echo Creating nuget package...
copy %framework%\Logzio.DotNet.%dllsuffix%.nuspec %framework%\package\Logzio.DotNet.%dllsuffix%.nuspec
dotnet pack %framework%\package\Logzio.DotNet.%dllsuffix%.nuspec -OutputDirectory %framework%


REM :nuget setApiKey Your-API-Key
REM :nuget push YourPackage.nupkg -Source c:\Buildium\src\LocalNugetFeed

echo Creating release zip...
%szip% a -r %framework%\Logzio.DotNet.%dllsuffix%.zip .\%framework%\package\lib\*

echo Done