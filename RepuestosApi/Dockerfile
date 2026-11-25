FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY RepuestosApi/RepuestosApi.csproj RepuestosApi/
RUN dotnet restore RepuestosApi/RepuestosApi.csproj
COPY RepuestosApi/ RepuestosApi/
RUN dotnet publish RepuestosApi/RepuestosApi.csproj -c Release --no-restore -o /app

FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build /app .
ENV ASPNETCORE_HTTP_PORTS=
ENV PORT=8080
EXPOSE 8080
ENTRYPOINT ["dotnet", "RepuestosApi.dll"]
