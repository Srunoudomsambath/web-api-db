# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

COPY *.csproj ./
RUN dotnet restore

COPY . ./
RUN dotnet publish -c Release -o /app/publish

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .

# Create images directory
RUN mkdir -p /app/wwwroot/images

ENV PORT=10000
EXPOSE 10000

ENTRYPOINT ["dotnet", "WebAPIDB.dll"]