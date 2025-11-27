# ----- Build Stage -----
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src

# Copy csproj và restore
COPY ["LAPTOP.csproj", "./"]
RUN dotnet restore "LAPTOP.csproj"

# Copy toàn bộ code và build
COPY . .
WORKDIR "/src"
RUN dotnet build "LAPTOP.csproj" -c Release --no-restore

# ----- Publish Stage -----
FROM build AS publish
RUN dotnet publish "LAPTOP.csproj" -c Release -o /app/out --no-restore --self-contained false

# ----- Final Stage -----
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS final
WORKDIR /app
COPY --from=publish /app/out ./

# Expose port
EXPOSE 80

# Run
ENTRYPOINT ["dotnet", "LAPTOP.dll"]