@echo off
cmd /c "cd /d ValorDolarHoy\ClientApp && npm cache verify"	
dotnet restore
dotnet build
dotnet test /p:CollectCoverage=true /p:CoverletOutput="../CodeCoverage/" /p:CoverletOutputFormat="opencover" --logger:"console;verbosity=detailed"
dotnet %USERPROFILE%\.nuget\packages\reportgenerator\5.2.0\tools\net8.0\ReportGenerator.dll "-reports:CodeCoverage\coverage.opencover.xml" "-targetdir:CodeCoverage\Web" "-assemblyfilters:-ValorDolarHoy;-ValorDolarHoy.Views;" "-classfilters:-*Exception"