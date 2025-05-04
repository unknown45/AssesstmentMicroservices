# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy csproj and restore
COPY ["AssesstmentMicroservices.API/AssesstmentMicroservices.API.csproj", "AssesstmentMicroservices.API/"]
COPY ["AssesstmentMicroservices.Application/AssesstmentMicroservices.Application.csproj", "AssesstmentMicroservices.Application/"]
RUN dotnet restore "AssesstmentMicroservices.API/AssesstmentMicroservices.API.csproj"

# Copy all files and build
COPY . .
RUN dotnet build "AssesstmentMicroservices.API/AssesstmentMicroservices.API.csproj" -c Release -o /app/build

# Stage 2: Publish
FROM build AS publish
RUN dotnet publish "AssesstmentMicroservices.API/AssesstmentMicroservices.API.csproj" -c Release -o /app/publish

# Stage 3: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AssesstmentMicroservices.API.dll"]