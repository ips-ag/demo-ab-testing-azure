FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS restore
WORKDIR /src/Ecommerce
COPY API/API.csproj API/API.csproj
COPY Core/Core.csproj Core/Core.csproj
COPY Infrastructure/Infrastructure.csproj Infrastructure/Infrastructure.csproj
RUN dotnet restore API/API.csproj

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src/Ecommerce
COPY API API
COPY Core Core
COPY Infrastructure Infrastructure
COPY --from=restore /src/Ecommerce/API API
COPY --from=restore /src/Ecommerce/Core Core
COPY --from=restore /src/Ecommerce/Infrastructure Infrastructure
RUN dotnet build API/API.csproj -c Release -o /app/build

FROM build AS publish
RUN dotnet publish API/API.csproj -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app/API
COPY --from=publish /app/publish .
COPY --from=build /src/Ecommerce/API/*.db  .
ENTRYPOINT ["dotnet", "API.dll"]
