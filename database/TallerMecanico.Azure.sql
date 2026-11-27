-- Conectarse directamente a la base Azure SQL Database llamada TallerMecanico.
-- Crear la base previamente en Azure; este script no crea ni cambia de base.
-- Protección para no modificar master ni una base de otro proyecto.
IF DB_NAME() <> N'TallerMecanico'
    THROW 50001, N'Conecte a TallerMecanico antes de ejecutar este script.', 1;
GO

IF OBJECT_ID(N'dbo.Clientes', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Clientes (
        Id int IDENTITY(1,1) NOT NULL CONSTRAINT PK_Clientes PRIMARY KEY,
        Nombre nvarchar(100) NOT NULL,
        Documento nvarchar(20) NOT NULL,
        Telefono nvarchar(20) NULL,
        CONSTRAINT CK_Clientes_Nombre CHECK (LEN(TRIM(Nombre)) > 0),
        CONSTRAINT CK_Clientes_Documento CHECK (LEN(TRIM(Documento)) > 0)
    );
    CREATE UNIQUE INDEX UX_Clientes_Documento ON dbo.Clientes(Documento);
END;
GO
IF OBJECT_ID(N'dbo.Vehiculos', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Vehiculos (
        Id int IDENTITY(1,1) NOT NULL CONSTRAINT PK_Vehiculos PRIMARY KEY,
        Placa nvarchar(15) NOT NULL,
        Marca nvarchar(50) NOT NULL,
        Modelo nvarchar(50) NOT NULL,
        Anio int NOT NULL,
        ClienteId int NOT NULL,
        CONSTRAINT CK_Vehiculos_Placa CHECK (LEN(TRIM(Placa)) > 0),
        CONSTRAINT CK_Vehiculos_Marca CHECK (LEN(TRIM(Marca)) > 0),
        CONSTRAINT CK_Vehiculos_Modelo CHECK (LEN(TRIM(Modelo)) > 0),
        CONSTRAINT CK_Vehiculos_Anio CHECK (Anio BETWEEN 1900 AND 2100),
        CONSTRAINT FK_Vehiculos_Clientes FOREIGN KEY (ClienteId) REFERENCES dbo.Clientes(Id) ON DELETE NO ACTION
    );
    CREATE UNIQUE INDEX UX_Vehiculos_Placa ON dbo.Vehiculos(Placa);
    CREATE INDEX IX_Vehiculos_ClienteId ON dbo.Vehiculos(ClienteId);
END;
GO
IF NOT EXISTS (SELECT 1 FROM dbo.Clientes WHERE Documento = N'LAB13-001')
    INSERT dbo.Clientes (Nombre, Documento, Telefono) VALUES (N'Cliente de ejemplo', N'LAB13-001', N'999111222');
IF NOT EXISTS (SELECT 1 FROM dbo.Vehiculos WHERE Placa = N'LAB13-ABC')
    INSERT dbo.Vehiculos (Placa, Marca, Modelo, Anio, ClienteId)
    SELECT N'LAB13-ABC', N'Toyota', N'Corolla', 2020, Id FROM dbo.Clientes WHERE Documento = N'LAB13-001';
GO

IF OBJECT_ID(N'dbo.Usuarios', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Usuarios (
        Id int IDENTITY PRIMARY KEY,
        Correo nvarchar(254) NOT NULL,
        ClaveHash nvarchar(100) NOT NULL,
        Rol nvarchar(30) NOT NULL
    );
    CREATE UNIQUE INDEX UX_Usuarios_Correo ON dbo.Usuarios(Correo);
END;
GO
-- Registre usuarios con la API. El registro asigna Recepcionista.
-- La promoción administrativa a JefeTaller debe acotarse al correo correcto.
