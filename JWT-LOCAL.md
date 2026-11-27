# Autenticación local

La aplicación exige `Jwt:Clave`, `Jwt:Emisor`, `Jwt:Audiencia` y `Jwt:Minutos` mediante User Secrets o variables de entorno. Use una clave aleatoria de al menos 32 caracteres; no la incluya en el repositorio. Minutos debe estar entre 1 y 1440. Configure también `ConnectionStrings:TallerMecanico` en User Secrets.

Ejecute `database/TallerMecanico.sql` en SQL Server. Registre dos cuentas mediante POST /api/auth/registro. El servidor asigna Recepcionista. Para la cuenta del jefe, cambie el rol mediante SQL con el UPDATE comentado en el script. Las contraseñas se transforman en hashes BCrypt por la API; no se incluyen claves ni hashes prefabricados en el repositorio.

| Operación | Sin token | Recepcionista | JefeTaller |
|---|---|---|---|
| POST auth/registro, POST auth/login | Sí | Sí | Sí |
| GET Clientes, GET Vehiculos | Sí | Sí | Sí |
| GET auth/perfil | 401 | Sí | Sí |
| GET Clientes/{id}, GET Clientes/{id}/vehiculos, GET Vehiculos/{id} | Sí | Sí | Sí |
| POST y PUT de Clientes y Vehiculos | 401 | Sí | Sí |
| DELETE de Clientes y Vehiculos | 401 | 403 | Sí |

Los dos roles se almacenan en la base; registro nunca acepta el rol. Los permisos concedidos por un token se mantienen hasta su expiración; tras cambiar un rol, inicie sesión nuevamente.
