FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["Cardofun.Core/", "Cardofun.Core/"]
COPY ["Cardofun.Domain/", "Cardofun.Domain/"]
COPY ["Cardofun.Interfaces/", "Cardofun.Interfaces/"]
COPY ["Cardofun.DataContext/", "Cardofun.DataContext/"]
COPY ["Cardofun.Infrastructure.ServiceDefinitions/Repositories/LocationRepository/", "Cardofun.Infrastructure.ServiceDefinitions/Repositories/LocationRepository/"]
COPY ["Cardofun.Microservices/Repositories/Cardofun.Microservices.Repositories.Location/", "Cardofun.Microservices/Repositories/Cardofun.Microservices.Repositories.Location/"]
RUN dotnet restore "Cardofun.Microservices/Repositories/Cardofun.Microservices.Repositories.Location/Cardofun.Microservices.Repositories.Location.csproj"
WORKDIR /src
RUN dotnet build "Cardofun.Microservices/Repositories/Cardofun.Microservices.Repositories.Location/Cardofun.Microservices.Repositories.Location.csproj" -c Release -o /app/build

FROM build AS publish
WORKDIR /src
RUN dotnet publish "Cardofun.Microservices/Repositories/Cardofun.Microservices.Repositories.Location/Cardofun.Microservices.Repositories.Location.csproj" -c Release -o /app/publish

FROM base AS final
EXPOSE 5000
EXPOSE 5001
EXPOSE 1433
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Cardofun.Microservices.Repositories.Location.dll"]