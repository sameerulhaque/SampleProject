# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/SampleProject.API/SampleProject.API.csproj", "src/SampleProject.API/"]
COPY ["src/SampleProject.Application/SampleProject.Application.csproj", "src/SampleProject.Application/"]
COPY ["src/SampleProject.Core/SampleProject.Core.csproj", "src/SampleProject.Core/"]
COPY ["src/SampleProject.Infrastructure/SampleProject.Infrastructure.csproj", "src/SampleProject.Infrastructure/"]
COPY ["src/SampleProject.Shared/SampleProject.Shared.csproj", "src/SampleProject.Shared/"]
RUN dotnet restore "./src/SampleProject.API/SampleProject.API.csproj"
COPY . .
WORKDIR "/src/src/SampleProject.API"
RUN dotnet build "./SampleProject.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./SampleProject.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SampleProject.API.dll"]