FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build-env
WORKDIR /app

# Copy csproj and restore as distinct layers
COPY ./src/TodoApi/*.csproj ./
RUN dotnet restore

# Copy everything else and build
COPY ./src/TodoApi/* ./
RUN dotnet publish -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app
#comment 
COPY --from=build-env /app/out .
CMD ASPNETCORE_URLS=http://*:$PORT dotnet TodoApi.dll
