echo
echo ">>> Running test..."
echo
dotnet test /p:CollectCoverage=true /p:CoverletOutput="../CodeCoverage/" /p:CoverletOutputFormat="opencover" --logger:"console;verbosity=detailed"
echo ">>> Build coverage report..."
echo
dotnet /Users/"$USER"/.nuget/packages/reportgenerator/5.0.2/tools/net6.0/ReportGenerator.dll "-reports:CodeCoverage/coverage.opencover.xml" "-targetdir:CodeCoverage/Web" "-assemblyfilters:-ValorDolarHoy;-ValorDolarHoy.Views;" "-classfilters:-*Exception"
echo
open CodeCoverage/Web/index.html

