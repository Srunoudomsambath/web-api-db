# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

COPY WebAPIDB/WebAPIDB.csproj ./WebAPIDB/
RUN dotnet restore ./WebAPIDB/WebAPIDB.csproj

COPY WebAPIDB/. ./WebAPIDB/
RUN dotnet publish ./WebAPIDB/WebAPIDB.csproj -c Release -o /app/publish

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .

RUN mkdir -p /app/wwwroot/images

ENV PORT=10000
EXPOSE 10000

ENTRYPOINT ["dotnet", "WebAPIDB.dll"]