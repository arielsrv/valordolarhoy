#!/bin/bash
( cd ValorDolarHoy/ClientApp || exit ; npm cache verify )
dotnet restore
dotnet build
./coverage.sh

