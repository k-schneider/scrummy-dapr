#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["src/Frontend/BlazorClient.Host/BlazorClient.Host.csproj", "src/Frontend/BlazorClient.Host/"]
RUN dotnet restore "src/Frontend/BlazorClient.Host/BlazorClient.Host.csproj"
COPY . .
WORKDIR "/src/src/Frontend/BlazorClient.Host"
RUN dotnet build "BlazorClient.Host.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "BlazorClient.Host.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Scrummy.BlazorClient.Host.dll"]
