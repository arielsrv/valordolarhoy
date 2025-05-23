﻿FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build

# Install Node.js
RUN apt-get update && apt-get install -y ca-certificates curl gnupg \
&& curl -fsSL https://deb.nodesource.com/gpgkey/nodesource-repo.gpg.key | gpg --dearmor -o /etc/apt/keyrings/nodesource.gpg \
&& NODE_MAJOR=20 \
&& echo "deb [signed-by=/etc/apt/keyrings/nodesource.gpg] https://deb.nodesource.com/node_$NODE_MAJOR.x nodistro main" | tee /etc/apt/sources.list.d/nodesource.list \
&& apt-get update && apt-get install nodejs -y

WORKDIR /src
COPY ["ValorDolarHoy/ValorDolarHoy.csproj", "ValorDolarHoy/"]
RUN dotnet restore "ValorDolarHoy/ValorDolarHoy.csproj"
COPY . .
WORKDIR "/src/ValorDolarHoy"
RUN dotnet build "ValorDolarHoy.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ValorDolarHoy.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Install the agent
RUN apt-get update && apt-get install -y wget ca-certificates gnupg \
&& echo 'deb http://apt.newrelic.com/debian/ newrelic non-free' | tee /etc/apt/sources.list.d/newrelic.list \
&& wget https://download.newrelic.com/548C16BF.gpg \
&& apt-key add 548C16BF.gpg \
&& apt-get update \
&& apt-get install -y newrelic-dotnet-agent \
&& rm -rf /var/lib/apt/lists/* 

# Enable the agent
ENV CORECLR_ENABLE_PROFILING=1 \
CORECLR_PROFILER={36032161-FFC0-4B61-B559-F6C5D41BAE5A} \
CORECLR_NEWRELIC_HOME=/usr/local/newrelic-dotnet-agent \
CORECLR_PROFILER_PATH=/usr/local/newrelic-dotnet-agent/libNewRelicProfiler.so \
NEW_RELIC_LICENSE_KEY=2fe9cb4919c003a782ef3028322128dda528NRAL \
NEW_RELIC_APP_NAME=todaycurrency

ENTRYPOINT ["dotnet", "./ValorDolarHoy.dll"]
