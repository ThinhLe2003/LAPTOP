# Build stage
FROM mcr.microsoft.com/dotnet/sdk:.0 AS build
WORKDIR /app

# Copy project file và restore dependency
COPY *.csproj ./
RUN dotnet restore

# Copy toàn bộ source và build
COPY . ./
RUN dotnet publish -c Release -o out

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:.0 AS runtime
WORKDIR /app
COPY --from=build /app/out ./

# Mở port
EXPOSE 8080

# Chạy ứng dụng
ENTRYPOINT ["dotnet", "Laptopshop.dll"]
