# Base image for ASP.NET Core runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

# Build and compile the client app
FROM node:22 AS client
WORKDIR /client
COPY Client/ .  
# Use 'COPY Client/ .' to copy the content of the Client directory
RUN npm install
ARG ANALYTICS_CLARITY_ID
ARG ANALYTICS_GA_MEASUREMENT_ID
ENV ANALYTICS_CLARITY_ID=${ANALYTICS_CLARITY_ID}
ENV ANALYTICS_GA_MEASUREMENT_ID=${ANALYTICS_GA_MEASUREMENT_ID}
RUN npm run build

# Restore dependencies for the .NET project
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS restore
WORKDIR /src/Ecommerce
COPY API/API.csproj API/
COPY Core/Core.csproj Core/
COPY Infrastructure/Infrastructure.csproj Infrastructure/
RUN dotnet restore API/API.csproj

# Build the .NET project
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src/Ecommerce
COPY . .  
# Copy all files
COPY --from=restore /src/Ecommerce/API/ API/
COPY --from=client /client/dist/client API/wwwroot/
RUN dotnet build API/API.csproj -c Release -o /app/build

# Publish the .NET project
FROM build AS publish
RUN dotnet publish API/API.csproj -c Release -o /app/publish /p:UseAppHost=false

# Final stage - set up the runtime environment
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
COPY --from=build /src/Ecommerce/API/*.db .
ENTRYPOINT ["dotnet", "API.dll"]
