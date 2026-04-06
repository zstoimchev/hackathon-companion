FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["backend/HackathonOS.DatabaseAPI/HackathonOS.DatabaseAPI.csproj", "backend/HackathonOS.DatabaseAPI/"]
COPY ["backend/HackathonOS.Infrastructure/HackathonOS.Infrastructure.csproj", "backend/HackathonOS.Infrastructure/"]
COPY ["backend/HackathonOS.Domain/HackathonOS.Domain.csproj", "backend/HackathonOS.Domain/"]
RUN dotnet restore "backend/HackathonOS.DatabaseAPI/HackathonOS.DatabaseAPI.csproj"
COPY . .
WORKDIR "/src/backend/HackathonOS.DatabaseAPI"
RUN dotnet build "./HackathonOS.DatabaseAPI.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./HackathonOS.DatabaseAPI.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "HackathonOS.DatabaseAPI.dll"]
