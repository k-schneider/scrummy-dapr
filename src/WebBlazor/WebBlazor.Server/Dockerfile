#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
RUN curl -sL https://deb.nodesource.com/setup_16.x |  bash -
RUN apt-get install -y nodejs
WORKDIR /src
COPY ["nuget.config", "."]
COPY ["src/WebBlazor/WebBlazor.Server/WebBlazor.Server.csproj", "src/WebBlazor/WebBlazor.Server/"]
COPY ["src/WebBlazor/WebBlazor.Client/WebBlazor.Client.csproj", "src/WebBlazor/WebBlazor.Client/"]
COPY ["src/DataContracts/AppContracts/AppContracts.csproj", "src/DataContracts/AppContracts/"]
RUN dotnet restore "src/WebBlazor/WebBlazor.Server/WebBlazor.Server.csproj"
COPY ["src/WebBlazor/WebBlazor.Client/package.json", "src/WebBlazor/WebBlazor.Client/"]
COPY ["src/WebBlazor/WebBlazor.Client/package-lock.json", "src/WebBlazor/WebBlazor.Client/"]
WORKDIR "/src/src/WebBlazor/WebBlazor.Client"
RUN npm ci
WORKDIR /src
COPY . .
WORKDIR "/src/src/WebBlazor/WebBlazor.Server"
RUN dotnet build "WebBlazor.Server.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "WebBlazor.Server.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Scrummy.WebBlazor.Server.dll"]
