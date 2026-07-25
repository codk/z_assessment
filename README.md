Rest api for product management with stock history.
Build with .net 10 on PostgreSQL 


## Project arquitecture

Products.api (http\rest)
	↓
Products (services\dto, interfaces)
	↓
Products.Domain (entities\eepository interfaces)
	↓
Products.Infrastructure (EF, repositories)
			
## Projects 
|Products.Api|Asp.net core rest api, openpi|
|Products|Services, dto|
|Products.Domain|Entities, repo interfaces|
|Products.Infrastructure|EF core, PostgreSQL, repositores|
|Products.Tests.Unit|Unit tests with mocks|
|Products.Tests.Integration|Integration tests with testcotainers (PostgreSQL)|



## Prerequisites
* **Visual Studio** or **VS Code** 
* Docker (desktop)

## Usage - locally 
```
 docker-compose up db
```

## API 
Documentation (development env. only): http://localhost:5071/scalar/

## Run tests                                                                     
```bash                                                                          
# Unit tests
dotnet test Tests/Products.Tests.Unit

# Integration tests (requires Docker)
dotnet test Products.Tests.Integration
```
  
  
## EF Migrations
```
 dotnet ef migrations add MigrationName --project Products.Infrastructure  --startup-project Procducts.Api
```