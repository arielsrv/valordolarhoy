dotnet test /p:CollectCoverage=true /p:CoverletOutput="../CodeCoverage/" /p:CoverletOutputFormat="opencover"
reportgenerator "-reports:CodeCoverage/coverage.opencover.xml" "-targetdir:CodeCoverage/Web" "-assemblyfilters:-ValorDolarHoy;-ValorDolarHoy.Views" 