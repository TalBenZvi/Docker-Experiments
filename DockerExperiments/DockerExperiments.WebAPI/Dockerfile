FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["DockerExperiments.WebAPI/DockerExperiments.WebAPI.csproj", "DockerExperiments.WebAPI/"]
RUN dotnet restore "DockerExperiments.WebAPI/DockerExperiments.WebAPI.csproj"
COPY . .
WORKDIR "/src/DockerExperiments.WebAPI"
RUN dotnet build "./DockerExperiments.WebAPI.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./DockerExperiments.WebAPI.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DockerExperiments.WebAPI.dll"]
