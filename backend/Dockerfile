FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY OrderManagement.sln ./
COPY OrderManagement.Api/OrderManagement.Api.csproj OrderManagement.Api/
RUN dotnet restore OrderManagement.Api/OrderManagement.Api.csproj
COPY OrderManagement.Api/ OrderManagement.Api/
RUN dotnet publish OrderManagement.Api/OrderManagement.Api.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
RUN apt-get update \
    && apt-get install -y --no-install-recommends curl \
    && rm -rf /var/lib/apt/lists/*
WORKDIR /app
ENV ASPNETCORE_URLS=http://+:5000
EXPOSE 5000
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "OrderManagement.Api.dll"]
