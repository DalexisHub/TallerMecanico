FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY GatewayApi/GatewayApi.csproj GatewayApi/
RUN dotnet restore GatewayApi/GatewayApi.csproj
COPY GatewayApi/ GatewayApi/
RUN dotnet publish GatewayApi/GatewayApi.csproj -c Release --no-restore -o /app

FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build /app .
ENV ASPNETCORE_HTTP_PORTS=
ENV PORT=8080
EXPOSE 8080
ENTRYPOINT ["dotnet", "GatewayApi.dll"]
