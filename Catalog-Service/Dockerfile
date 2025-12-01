FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 5000

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["Catalog-Service/CatalogService.csproj", "Catalog-Service/"]
COPY ["Consul-Shared/Consul-Shared.csproj", "Consul-Shared/"]
RUN dotnet restore "Catalog-Service/CatalogService.csproj"
COPY . .
WORKDIR "/src/Catalog-Service"
RUN dotnet build "CatalogService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "CatalogService.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CatalogService.dll"]

