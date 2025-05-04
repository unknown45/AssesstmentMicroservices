# 🛠️ .NET 8 Microservice with RabbitMQ

This is a simple RESTful backend microservice built with **.NET 8**, featuring clean architecture, RabbitMQ integration (publisher and consumer), Swagger support, and Dockerized message broker.

## 🚀 Features

- ✅ .NET 8 Web API
- ✅ Clean Architecture (Domain, Application, Infrastructure, API)
- ✅ RabbitMQ Publisher
- ✅ Swagger UI for testing
- ✅ Docker Compose for RabbitMQ
- ✅ Structured logging and exception handling

---

## 📦 Requirements

- [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
- [Docker](https://www.docker.com/)
- Git

---

## ⚙️ Getting Started

### 1. Clone this repository

### 2. Run RabbitMQ via Docker
```
docker-compose up -d
```

Access the RabbitMQ UI:

    http://localhost:15672

    Username: guest

    Password: guest

### 3. Run the application
```
dotnet build
dotnet run --project src/MyMicroservice.Api
```

### 4. Test via Swagger

Visit: https://localhost:5001/swagger
Or: http://localhost:5000/swagger

Try POST /api/order/publish with this sample payload:
```
{
  "orderId": 1,
  "customerId": 101,
  "productIds": [1001, 1002],
  "createdAt": "2025-05-04T10:00:00"
}
```


## 🧪 Project Structure
src/
├── MyMicroservice.Api           → Entry point (.NET Web API)
├── MyMicroservice.Application   → Interfaces, DTOs, business logic
├── MyMicroservice.Infrastructure → RabbitMQ, persistence, logging
└── MyMicroservice.Domain        → Core entities, events
