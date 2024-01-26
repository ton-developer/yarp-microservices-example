FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Blog-microservices-example/Blog-microservices-example.csproj", "Blog-microservices-example/"]
RUN dotnet restore "Blog-microservices-example/Blog-microservices-example.csproj"
COPY . .
WORKDIR "/src/Blog-microservices-example"
RUN dotnet build "Blog-microservices-example.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Blog-microservices-example.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Blog-microservices-example.dll"]
