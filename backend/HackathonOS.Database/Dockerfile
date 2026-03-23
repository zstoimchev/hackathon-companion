FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["backend/HackathonOS.Database/HackathonOS.Database.csproj", "backend/HackathonOS.Database/"]
RUN dotnet restore "backend/HackathonOS.Database/HackathonOS.Database.csproj"
COPY . .
WORKDIR "/src/backend/HackathonOS.Database"
RUN dotnet build "./HackathonOS.Database.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./HackathonOS.Database.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "HackathonOS.Database.dll"]
