# Build stage
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /app

# Copy file project (đúng đường dẫn của bạn)
COPY LAPTOP/LAPTOP.csproj ./
RUN dotnet restore

# Copy toàn bộ code
COPY . .

# Publish
RUN dotnet publish LAPTOP/LAPTOP.csproj -c Release -o /app/publish --no-restore

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS final
WORKDIR /app
COPY --from=build /app/publish ./

EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "LAPTOP.dll"]