echo
echo ">>> Running test..."
echo

export SolutionFile=ValorDolarHoy.sln
export CollectCoverage=true
export CoverletOutput=../CodeCoverage/
export CoverletOutputFormat=opencover

dotnet test $SolutionFile --no-build $CollectCoverage
echo ">>> Build coverage report..."
echo
dotnet /Users/"$USER"/.nuget/packages/reportgenerator/5.4.3/tools/net9.0/ReportGenerator.dll "-reports:CodeCoverage/coverage.opencover.xml" "-targetdir:CodeCoverage/Web" "-assemblyfilters:-Web;-Web.Views;" "-classfilters:-*Exception"

echo
open CodeCoverage/Web/index.html
