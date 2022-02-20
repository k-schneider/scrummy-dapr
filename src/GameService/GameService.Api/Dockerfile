#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["nuget.config", "."]
COPY ["src/GameService/GameService.Api/GameService.Api.csproj", "src/GameService/GameService.Api/"]
COPY ["src/DataContracts/AppContracts/AppContracts.csproj", "src/DataContracts/AppContracts/"]
COPY ["src/BuildingBlocks/EventBus/EventBus.csproj", "src/BuildingBlocks/EventBus/"]
COPY ["src/BuildingBlocks/Healthchecks/Healthchecks.csproj", "src/BuildingBlocks/Healthchecks/"]
RUN dotnet restore "src/GameService/GameService.Api/GameService.Api.csproj"
COPY . .
WORKDIR "/src/src/GameService/GameService.Api"
RUN dotnet build "GameService.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "GameService.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Scrummy.GameService.Api.dll"]
