FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copiar todos los .csproj primero (mejor cache)
COPY *.sln .
COPY DeveloperHub.API/*.csproj ./DeveloperHub.API/
COPY DeveloperHub.Application/*.csproj ./DeveloperHub.Application/
COPY DeveloperHub.Domain/*.csproj ./DeveloperHub.Domain/
COPY DeveloperHub.Infrastructure/*.csproj ./DeveloperHub.Infrastructure/

RUN dotnet restore

# Copiar todo el codigo
COPY . .

WORKDIR "/src/DeveloperHub.API"
RUN dotnet publish "DeveloperHub.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DeveloperHub.API.dll"]