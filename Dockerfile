FROM mcr.microsoft.com/dotnet/sdk:6.0-focal AS build
ARG BUILD_CONFIG=Release
WORKDIR /src
COPY ["src/TodoApi/TodoApi.csproj", "TodoApi/"]
RUN dotnet restore "TodoApi/TodoApi.csproj"
COPY src .
WORKDIR /src/TodoApi
RUN dotnet publish --no-restore -c $BUILD_CONFIG -o /app/publish /p:GenerateDocumentationFile=true

FROM mcr.microsoft.com/dotnet/aspnet:6.0-focal AS final

RUN apt-get update \
  && apt-get install -y --no-install-recommends libgdiplus libc6-dev \
  && apt-get clean \
  && rm -rf /var/lib/apt/lists/*

WORKDIR /app
COPY --from=build /app/publish .


ENTRYPOINT ["dotnet", "TodoApi.dll"]

