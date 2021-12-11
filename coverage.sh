dotnet test /p:CollectCoverage=true /p:CoverletOutput="../CodeCoverage/" /p:CoverletOutputFormat="opencover"
dotnet /Users/"$USER"/.nuget/packages/reportgenerator/5.0.0/tools/net6.0/ReportGenerator.dll "-reports:CodeCoverage/coverage.opencover.xml" "-targetdir:CodeCoverage/Web" "-assemblyfilters:-ValorDolarHoy;-ValorDolarHoy.Views;" "-classfilters:-*Exception"
