# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy project files and restore
COPY HackathonOS.Domain/HackathonOS.Domain.csproj HackathonOS.Domain/
COPY HackathonOS.Application/HackathonOS.Application.csproj HackathonOS.Application/
COPY HackathonOS.Infrastructure/HackathonOS.Infrastructure.csproj HackathonOS.Infrastructure/
COPY HackathonOS.API/HackathonOS.API.csproj HackathonOS.API/
RUN dotnet restore HackathonOS.API/HackathonOS.API.csproj

# Copy everything and build
COPY . .
RUN dotnet publish HackathonOS.API/HackathonOS.API.csproj -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
ENTRYPOINT ["dotnet", "HackathonOS.API.dll"]
