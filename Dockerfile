# Stage 1: build
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy the project file first so the restore layer is cached
COPY src/TicketTracker.Api/TicketTracker.Api.csproj src/TicketTracker.Api/
RUN dotnet restore src/TicketTracker.Api/TicketTracker.Api.csproj

COPY src/ src/
RUN dotnet publish src/TicketTracker.Api/TicketTracker.Api.csproj -c Release -o /app/publish --no-restore

# Stage 2: runtime (no SDK, much smaller)
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 8080
ENTRYPOINT ["dotnet", "TicketTracker.Api.dll"]