# ========== Build stage ==========
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src

# Copy csproj vào đúng thư mục
COPY LAPTOP.csproj ./LAPTOP.csproj

# Restore package
RUN dotnet restore LAPTOP.csproj

# Copy toàn bộ source code
COPY . .

# Build và publish
WORKDIR /src
RUN dotnet publish LAPTOP.csproj -c Release -o /app/out

# ========== Runtime stage ==========
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS runtime
WORKDIR /app
COPY --from=build /app/out ./

EXPOSE 8080
ENTRYPOINT ["dotnet", "LAPTOP.dll"]
