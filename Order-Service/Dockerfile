FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 5001

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["Order-Service/OrderService.csproj", "Order-Service/"]
COPY ["Consul-Shared/Consul-Shared.csproj", "Consul-Shared/"]
RUN dotnet restore "Order-Service/OrderService.csproj"
COPY . .
WORKDIR "/src/Order-Service"
RUN dotnet build "OrderService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "OrderService.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "OrderService.dll"]