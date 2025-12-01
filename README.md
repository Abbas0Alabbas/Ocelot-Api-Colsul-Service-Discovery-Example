# Microservices API Gateway with Consul Service Discovery

A microservices architecture demonstration using .NET 10.0, Ocelot API Gateway, and Consul for service discovery. This project showcases how to build a distributed system with automatic service registration, discovery, load balancing, and rate limiting.

## Architecture

```
┌─────────────┐
│   Client    │
└──────┬──────┘
       │
       ▼
┌─────────────────┐
│  API Gateway    │  (Ocelot)
│  Port: 7000     │
└────────┬────────┘
         │
         ├─────────────────┐
         │                 │
         ▼                 ▼
┌──────────────┐   ┌──────────────┐
│Order Service │   │Catalog      │
│Port: 5001    │   │Service      │
└──────┬───────┘   │Port: 5000   │
       │           └──────┬───────┘
       │                  │
       └──────────┬───────┘
                  │
                  ▼
         ┌────────────────┐
         │  Consul        │
         │  Cluster       │
         │  (3 Servers +  │
         │   1 Client)    │
         └────────────────┘
```

## 📦 Components

### 1. **API Gateway** (`API-Gateway`)
- **Technology**: Ocelot API Gateway
- **Port**: 7000
- **Features**:
  - Service discovery via Consul
  - Round-robin load balancing
  - Global rate limiting (100 requests/second)
  - Routes requests to downstream services

### 2. **Order Service** (`Order-Service`)
- **Port**: 5001
- **Endpoints**:
  - `POST /api/order` - Create a new order
  - `GET /health` - Health check endpoint

### 3. **Catalog Service** (`Catalog-Service`)
- **Port**: 5000
- **Endpoints**:
  - `GET /api/product/{id}` - Get product by ID
  - `GET /health` - Health check endpoint

### 4. **Consul Cluster**
- **Consul Servers**: 3 nodes (consul-server-1, consul-server-2, consul-server-3)
- **Consul Client**: 1 node (consul-client)
- **Consul UI**: Available at `http://localhost:8500`
- **Port**: 8500 (server-1)

### 5. **Consul-Shared Library**
- Shared library for Consul service registration and discovery
- Used by all microservices for automatic registration

## 🚀 Prerequisites

- **.NET SDK 10.0** or later
- **Docker Desktop** (for running Consul and services in containers)
- **Docker Compose** (usually included with Docker Desktop)

## 📋 How to Run

### Option 1: Run Everything with Docker Compose (Recommended)

1. **Clone/Navigate to the project directory**:
   ```bash
   docker-compose up --build
   ```

   This will:
   - Build Docker images for API Gateway, Order Service, and Catalog Service
   - Start the Consul cluster (3 servers + 1 client)
   - Start all microservices
   - Register services with Consul automatically

3. **Verify services are running**:
   ```bash
   docker-compose ps
   ```

4. **Check Consul UI**:
   - Open browser: `http://localhost:8500`
   - Navigate to "Services" to see registered services

### Option 2: Run Services Individually (Development)

1. **Start Consul** (in one terminal):
   ```bash
   docker-compose up consul-server-1 consul-server-2 consul-server-3 consul-client
   ```

2. **Run Catalog Service** (in another terminal):
   ```bash
   cd catalog-service
   dotnet run
   ```

3. **Run Order Service** (in another terminal):
   ```bash
   cd order-service
   dotnet run
   ```

4. **Run API Gateway** (in another terminal):
   ```bash
   cd API-Gateway
   dotnet run
   ```

## 🔌 API Endpoints

### Through API Gateway (Port 7000)

#### Order Service Routes
- **Create Order**: `POST http://localhost:7000/api/order/order`
  ```json
  {
    "Items": [
      { "ProductId": "1", "Quantity": 2 },
      { "ProductId": "2", "Quantity": 1 }
    ]
  }
  ```

#### Catalog Service Routes
- **Get Product**: `GET http://localhost:7000/api/catalog/product/{id}`
  - Example: `GET http://localhost:7000/api/catalog/product/1`

### Direct Service Access

#### Order Service (Port 5001)
- `POST http://localhost:5001/api/order`
- `GET http://localhost:5001/health`

#### Catalog Service (Port 5000)
- `GET http://localhost:5000/api/product/{id}`
- `GET http://localhost:5000/health`

## ⚙️ Configuration

### API Gateway Configuration (`API-Gateway/ocelot.json`)

- **Service Discovery**: Consul at `consul-client:8500`
- **Load Balancing**: Round-robin algorithm
- **Rate Limiting**: 100 requests per second globally
- **Routes**:
  - `/api/order/*` → OrderService
  - `/api/catalog/*` → CatalogService

### Service Configuration

Each service automatically registers with Consul on startup using configuration in `appsettings.json`:

```json
{
  "Consul": {
    "Address": "http://consul-client:8500",
    "ServiceName": "OrderService",
    "ServiceId": "OrderService-1",
    "ServiceAddress": "order-service",//should be localhost in development without containers
    "ServicePort": 5001,
    "CheckInterval": 10
  }
}
```

## 🧪 Testing

### Test Order Creation
```bash
curl -X POST http://localhost:7000/api/order/order \
  -H "Content-Type: application/json" \
  -d '{
    "Items": [
      {"ProductId": "1", "Quantity": 2},
      {"ProductId": "2", "Quantity": 1}
    ]
  }'
```

### Test Product Retrieval
```bash
curl http://localhost:7000/api/catalog/product/1
```

### Check Service Health
```bash
curl http://localhost:5001/health
curl http://localhost:5000/health
```

### View Registered Services in Consul
```bash
curl http://localhost:8500/v1/health/service/OrderService?passing
curl http://localhost:8500/v1/health/service/CatalogService?passing
```

## 🔍 Monitoring & Debugging

### Consul UI
- Access at: `http://localhost:8500`
- View registered services, health checks, and service discovery

### Service Logs
```bash
# View all logs
docker-compose logs -f

# View specific service logs
docker-compose logs -f api-gateway
docker-compose logs -f order-service
docker-compose logs -f catalog-service
```

### Check Service Status
```bash
docker-compose ps
```

## 🛠️ Key Features

1. **Service Discovery**: Automatic service registration and discovery via Consul
2. **Load Balancing**: Round-robin algorithm for distributing requests
3. **Rate Limiting**: Global rate limit of 100 requests/second
4. **Health Checks**: Automatic health monitoring and service deregistration
5. **High Availability**: Consul cluster with 3 servers for fault tolerance
6. **Docker Support**: All services containerized for easy deployment

## 📁 Project Structure

```
.
├── API-Gateway/           # Ocelot API Gateway
│   ├── ocelot.json        # Gateway routing configuration
│   ├── Program.cs         # Gateway startup
│   └── Dockerfile
├── Order-Service/         # Order microservice
│   ├── Program.cs
│   ├── appsettings.json
│   └── Dockerfile
├── Catalog-Service/       # Catalog microservice
│   ├── Program.cs
│   ├── appsettings.json
│   └── Dockerfile
├── Consul-Shared/        # Shared Consul library
│   ├── ConsulService.cs
│   └── Models/
└── docker-compose.yml    # Docker Compose configuration
```

## 🐛 Troubleshooting

### Services not registering with Consul
- Check that Consul is running: `docker-compose ps`
- Verify network connectivity: Services must be on the same Docker network
- Check service logs: `docker-compose logs <service-name>`

### API Gateway can't find services
- Verify services are registered in Consul UI: `http://localhost:8500`
- Check Ocelot configuration in `ocelot.json`
- Ensure service names match exactly (case-sensitive)

### Connection refused errors
- Ensure all services are running: `docker-compose ps`
- Check service ports are not conflicting
- Verify Docker network configuration

## 📚 Technologies Used

- **.NET 10.0**: Application framework
- **Ocelot 24.0.1**: API Gateway
- **Ocelot.Provider.Consul 24.0.0**: Consul service discovery provider
- **Consul 1.15.4**: Service discovery and configuration
- **Docker & Docker Compose**: Containerization and orchestration
- **Swashbuckle.AspNetCore**: API documentation (Swagger)

## 📝 Notes

- Services automatically register with Consul on startup
- Services automatically deregister when stopped
- Health checks run every 10 seconds
- Services are deregistered after 1 minute if health checks fail
- Rate limiting applies globally to all requests through the API Gateway

## 🔗 Useful Links

- [Ocelot Documentation](https://ocelot.readthedocs.io/)
- [Consul Documentation](https://www.consul.io/docs)
- [Docker Compose Documentation](https://docs.docker.com/compose/)

---