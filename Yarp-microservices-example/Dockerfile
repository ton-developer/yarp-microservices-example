FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Yarp-microservices-example/Yarp-microservices-example.csproj", "Yarp-microservices-example/"]
RUN dotnet restore "Yarp-microservices-example/Yarp-microservices-example.csproj"
COPY . .
WORKDIR "/src/Yarp-microservices-example"
RUN dotnet build "Yarp-microservices-example.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Yarp-microservices-example.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Yarp-microservices-example.dll"]
