#!/bin/bash
echo
echo ">>> Running test..."
echo

export SolutionFile=ValorDolarHoy.sln
export CollectCoverage=true
export CoverletOutput=../CodeCoverage/
export CoverletOutputFormat=opencover

dotnet test $SolutionFile --no-build /p:CollectCoverage=true /p:CoverletOutput=$CoverletOutput /p:CoverletOutputFormat=$CoverletOutputFormat
echo ">>> Build coverage report..."
echo
REPORTGENERATOR_PATH=$(find /Users/"$USER"/.nuget/packages/reportgenerator -name ReportGenerator.dll | grep -E "net(9|10)\.0" | head -n 1)

if [ -z "$REPORTGENERATOR_PATH" ]; then
  echo ">>> ReportGenerator.dll not found. Please install it with 'dotnet tool install -g dotnet-reportgenerator-globaltool' or ensure it's in NuGet cache."
  exit 1
fi

dotnet "$REPORTGENERATOR_PATH" "-reports:CodeCoverage/coverage.opencover.xml" "-targetdir:CodeCoverage/Web" "-assemblyfilters:-Web;-Web.Views;" "-classfilters:-*Exception"

echo
open CodeCoverage/Web/index.html
