# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy project files and restore
COPY src/HackathonOS.Domain/HackathonOS.Domain.csproj src/HackathonOS.Domain/
COPY src/HackathonOS.Application/HackathonOS.Application.csproj src/HackathonOS.Application/
COPY src/HackathonOS.Infrastructure/HackathonOS.Infrastructure.csproj src/HackathonOS.Infrastructure/
COPY src/HackathonOS.Api/HackathonOS.Api.csproj src/HackathonOS.Api/
RUN dotnet restore src/HackathonOS.Api/HackathonOS.Api.csproj

# Copy everything and build
COPY . .
RUN dotnet publish src/HackathonOS.Api/HackathonOS.Api.csproj -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
ENTRYPOINT ["dotnet", "HackathonOS.Api.dll"]
