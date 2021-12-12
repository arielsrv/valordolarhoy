#!/bin/bash
( cd ValorDolarHoy/ClientApp ; npm cache verify )
dotnet restore
dotnet build
./coverage.sh

