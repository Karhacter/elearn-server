# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy file csproj và restore dependencies (tận dụng Docker cache)
COPY ["elearn_server.csproj", "./"]
RUN dotnet restore "elearn_server.csproj"

# Copy toàn bộ code và build
COPY . .
RUN dotnet build "elearn_server.csproj" -c Release -o /app/build

# Stage 2: Publish
FROM build AS publish
RUN dotnet publish "elearn_server.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Stage 3: Final Image (Runtime)
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Copy kết quả đã build từ stage publish
COPY --from=publish /app/publish .

# Chạy ứng dụng
ENTRYPOINT ["dotnet", "elearn_server.dll"]