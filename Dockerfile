# Giai đoạn Build
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /app

# Thay vì dùng *.csproj, hãy chỉ định tên file cụ thể để tránh lỗi cache
# Vì file LAPTOP.csproj nằm cùng cấp với Dockerfile
COPY LAPTOP.csproj ./

# Restore các thư viện
RUN dotnet restore "LAPTOP.csproj"

# Copy toàn bộ source code vào image
COPY . ./

# Build và Publish ra thư mục /app/publish
# Thêm cờ /p:UseAppHost=false để đảm bảo đầu ra là file .dll
RUN dotnet publish "LAPTOP.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Giai đoạn Runtime
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS final
WORKDIR /app
COPY --from=build /app/publish ./
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
ENTRYPOINT ["dotnet", "LAPTOP.dll"]