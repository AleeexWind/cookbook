# Build context must be the repository root.
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY Cookbook.sln ./
COPY src/Cookbook.Domain/Cookbook.Domain.csproj src/Cookbook.Domain/
COPY src/Cookbook.Application/Cookbook.Application.csproj src/Cookbook.Application/
COPY src/Cookbook.Infrastructure/Cookbook.Infrastructure.csproj src/Cookbook.Infrastructure/
COPY src/Cookbook.Api/Cookbook.Api.csproj src/Cookbook.Api/

RUN dotnet restore src/Cookbook.Api/Cookbook.Api.csproj

COPY src/ ./src/
RUN dotnet publish src/Cookbook.Api/Cookbook.Api.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Cookbook.Api.dll"]
