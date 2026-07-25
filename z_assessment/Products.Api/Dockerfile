FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ["Products.Api/Products.Api.csproj", "Products.Api/"]
COPY ["Products.Infrastructure/Products.Infrastructure.csproj", "Products.Infrastructure/"]
COPY ["Products.Domain/Products.Domain.csproj", "Products.Domain/"]
COPY ["Products/Products.csproj", "Products/"]
RUN dotnet restore "./Products.Api/Products.Api.csproj"

COPY . .
WORKDIR "/src/Products.Api"
RUN dotnet build "./Products.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Products.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Products.Api.dll"]
