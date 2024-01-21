FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app


FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["User-microservices-example/User-microservices-example.csproj", "User-microservices-example/"]
RUN dotnet restore "User-microservices-example/User-microservices-example.csproj"
COPY . .
WORKDIR "/src/User-microservices-example"
RUN dotnet build "User-microservices-example.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "User-microservices-example.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "User-microservices-example.dll"]
