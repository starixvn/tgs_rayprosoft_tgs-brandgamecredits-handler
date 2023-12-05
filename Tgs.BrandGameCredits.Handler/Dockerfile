#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["nuget.config", "."]
COPY ["Tgs.BrandGameCredits.Handler/Tgs.BrandGameCredits.Handler.csproj", "Tgs.BrandGameCredits.Handler/"]
COPY ["Tgs.BrandGameCredits.Handler.Common/Tgs.BrandGameCredits.Handler.Common.csproj", "Tgs.BrandGameCredits.Handler.Common/"]
COPY ["Tgs.BrandGameCredits.Handler.Core/Tgs.BrandGameCredits.Handler.Core.csproj", "Tgs.BrandGameCredits.Handler.Core/"]
COPY ["Tgs.BrandGameCredits.Handler.Repositories/Tgs.BrandGameCredits.Handler.Repositories.csproj", "Tgs.BrandGameCredits.Handler.Repositories/"]
RUN dotnet restore "./Tgs.BrandGameCredits.Handler/./Tgs.BrandGameCredits.Handler.csproj"
COPY . .
WORKDIR "/src/Tgs.BrandGameCredits.Handler"
RUN dotnet build "./Tgs.BrandGameCredits.Handler.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Tgs.BrandGameCredits.Handler.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Tgs.BrandGameCredits.Handler.dll"]