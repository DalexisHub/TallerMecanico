FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY TallerMecanico.Api/TallerMecanico.Api.csproj TallerMecanico.Api/
COPY TallerMecanico.Negocio/TallerMecanico.Negocio.csproj TallerMecanico.Negocio/
COPY TallerMecanico.Datos/TallerMecanico.Datos.csproj TallerMecanico.Datos/
COPY TallerMecanico.Entidades/TallerMecanico.Entidades.csproj TallerMecanico.Entidades/
RUN dotnet restore TallerMecanico.Api/TallerMecanico.Api.csproj
COPY TallerMecanico.Api/ TallerMecanico.Api/
COPY TallerMecanico.Negocio/ TallerMecanico.Negocio/
COPY TallerMecanico.Datos/ TallerMecanico.Datos/
COPY TallerMecanico.Entidades/ TallerMecanico.Entidades/
RUN dotnet publish TallerMecanico.Api/TallerMecanico.Api.csproj -c Release --no-restore -o /app

FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build /app .
ENV ASPNETCORE_HTTP_PORTS=
ENV PORT=8080
EXPOSE 8080
ENTRYPOINT ["dotnet", "TallerMecanico.Api.dll"]
