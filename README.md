# Taller Mecánico · API

![Portada de la API Taller Mecánico](assets/portada-taller-mecanico.png)

API REST para gestionar clientes y vehículos de un taller mecánico, con autenticación JWT y permisos por rol. Proyecto académico en **.NET 10**, **SQL Server**, **Entity Framework Core**, **AutoMapper** y **BCrypt**.

## Qué resuelve

Permite identificar al propietario de cada vehículo, evitar documentos y placas duplicados y controlar quién modifica los registros. Un **cliente** puede tener varios **vehículos**; cada vehículo pertenece a un cliente. No se puede eliminar un cliente que todavía tenga vehículos asociados.

Las contraseñas se guardan como hashes BCrypt. Al registrarse, cada usuario recibe el rol `Recepcionista`; el rol `JefeTaller` se asigna administrativamente en la base de datos.

## Arquitectura

| Proyecto | Responsabilidad |
| --- | --- |
| `TallerMecanico.Api` | Controladores HTTP, autenticación, autorización y Swagger. |
| `TallerMecanico.Negocio` | Servicios, reglas de negocio, emisión de JWT y `MappingProfile`. |
| `TallerMecanico.Datos` | Contexto de EF Core y repositorios para SQL Server. |
| `TallerMecanico.Entidades` | Entidades y DTOs de entrada y salida. |

Las entidades de clientes y vehículos se generaron a partir de la base de datos con EF Core. `Usuario` se añadió para autenticación. Los controladores exponen DTOs en lugar de entidades de persistencia.

## Requisitos

- SDK de **.NET 10**.
- SQL Server accesible desde tu equipo.
- Un cliente SQL para ejecutar [`database/TallerMecanico.sql`](database/TallerMecanico.sql).

## Puesta en marcha

1. Clona el repositorio.

   ```bash
   git clone https://github.com/DalexisHub/TallerMecanico.git
   cd TallerMecanico
   ```

2. Ejecuta [`database/TallerMecanico.sql`](database/TallerMecanico.sql) en SQL Server. El script crea la base, las tres tablas y un cliente y vehículo de ejemplo. Se puede volver a ejecutar sin recrear tablas existentes.

3. Configura los valores locales con **User Secrets** del proyecto API. Sustituye los textos entre `< >` por tus valores; no los añadas a Git.

   ```bash
   dotnet user-secrets set "ConnectionStrings:TallerMecanico" "<cadena de conexión>" --project TallerMecanico.Api
   dotnet user-secrets set "Jwt:Clave" "<clave aleatoria de al menos 32 caracteres>" --project TallerMecanico.Api
   dotnet user-secrets set "Jwt:Emisor" "TallerMecanico" --project TallerMecanico.Api
   dotnet user-secrets set "Jwt:Audiencia" "TallerMecanico.Clientes" --project TallerMecanico.Api
   dotnet user-secrets set "Jwt:Minutos" "60" --project TallerMecanico.Api
   ```

4. Restaura dependencias e inicia la API.

   ```bash
   dotnet restore
   dotnet run --project TallerMecanico.Api --launch-profile https
   ```

5. En desarrollo, abre [Swagger](https://localhost:7166/swagger). Para confiar en el certificado local de desarrollo, usa `dotnet dev-certs https --trust`.

## Autenticación

1. Crea una cuenta con `POST /api/Auth/registro` enviando `correo` y `clave`; el servidor asigna `Recepcionista`.
2. Inicia sesión con `POST /api/Auth/login`; la respuesta contiene un JWT y su expiración.
3. En Swagger, pulsa **Authorize** y pega el token. En otro cliente HTTP, envía `Authorization: Bearer <token>`.
4. Para preparar un `JefeTaller`, registra otra cuenta y cambia su rol con el `UPDATE` comentado en el script SQL. Inicia sesión de nuevo para obtener un token con el rol actualizado.

Ejemplo de cuerpo para registro o login:

```json
{
  "correo": "recepcion@example.test",
  "clave": "UnaClaveDePrueba123!"
}
```

El registro no acepta un rol enviado por el cliente. La API autoriza según el rol del token firmado. Hay más detalles en [`JWT-LOCAL.md`](JWT-LOCAL.md).

## Endpoints y permisos

| Método y ruta | Sin token | Recepcionista | JefeTaller |
| --- | :---: | :---: | :---: |
| `POST /api/Auth/registro` | Sí | Sí | Sí |
| `POST /api/Auth/login` | Sí | Sí | Sí |
| `GET /api/Auth/perfil` | 401 | Sí | Sí |
| `GET /api/Clientes` | Sí | Sí | Sí |
| `GET /api/Clientes/{id}` | 401 | Sí | Sí |
| `POST /api/Clientes` | 401 | Sí | Sí |
| `PUT /api/Clientes/{id}` | 401 | Sí | Sí |
| `DELETE /api/Clientes/{id}` | 401 | 403 | Sí |
| `GET /api/Clientes/{clienteId}/vehiculos` | 401 | Sí | Sí |
| `GET /api/Vehiculos` | Sí | Sí | Sí |
| `GET /api/Vehiculos/{id}` | 401 | Sí | Sí |
| `POST /api/Vehiculos` | 401 | Sí | Sí |
| `PUT /api/Vehiculos/{id}` | 401 | Sí | Sí |
| `DELETE /api/Vehiculos/{id}` | 401 | 403 | Sí |

`GET /api/Vehiculos` acepta filtros opcionales `clienteId`, `placa` y `marca`. Las operaciones también pueden devolver `400` por datos inválidos, `404` si el recurso no existe y `409` cuando se incumple una regla de negocio, como un documento o una placa duplicados.

## Seguridad de la configuración

El repositorio incluye el script SQL y el código para reproducir la aplicación. La cadena de conexión, la clave de firma JWT y las contraseñas se configuran fuera del repositorio. No publiques tokens de acceso reales.
