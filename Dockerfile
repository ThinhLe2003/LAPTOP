# ========== Build stage ==========
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src

# Copy file project
COPY LAPTOP/LAPTOP.csproj ./LAPTOP/
RUN dotnet restore ./LAPTOP/LAPTOP.csproj

# Copy toàn bộ source và build
COPY . .
WORKDIR /src/LAPTOP
RUN dotnet publish -c Release -o /app/out

# ========== Runtime stage ==========
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS runtime
WORKDIR /app
COPY --from=build /app/out ./

# Render thường dùng port 8080
EXPOSE 8080
ENTRYPOINT ["dotnet", "LAPTOP.dll"]
