-- VeterinariaDB: propietarios y sus mascotas (relación 1:N)
-- Ejecuta este script DENTRO de la base ya creada (VeterinariaDB).
--   Local:  docker compose lo ejecuta automáticamente sobre VeterinariaDB.
--   Nube:   en Azure SQL la base se crea desde el portal; abre el editor de consultas
--           sobre esa base y ejecuta este script tal cual (no usa CREATE DATABASE ni USE).

IF OBJECT_ID('Mascotas', 'U') IS NOT NULL DROP TABLE Mascotas;
IF OBJECT_ID('Propietarios', 'U') IS NOT NULL DROP TABLE Propietarios;
GO

CREATE TABLE Propietarios (
    Id        INT IDENTITY(1,1) PRIMARY KEY,
    Nombres   NVARCHAR(100) NOT NULL,
    Dni       CHAR(8)       NOT NULL,
    Telefono  NVARCHAR(15)  NOT NULL,
    Email     NVARCHAR(100) NULL,
    CONSTRAINT UQ_Propietarios_Dni UNIQUE (Dni)
);

CREATE TABLE Mascotas (
    Id            INT IDENTITY(1,1) PRIMARY KEY,
    Nombre        NVARCHAR(50)  NOT NULL,
    Especie       NVARCHAR(30)  NOT NULL,
    Raza          NVARCHAR(50)  NULL,
    Edad          INT           NOT NULL,
    Peso          DECIMAL(5,2)  NOT NULL,
    PropietarioId INT           NOT NULL,
    CONSTRAINT FK_Mascotas_Propietarios FOREIGN KEY (PropietarioId) REFERENCES Propietarios (Id),
    CONSTRAINT UQ_Mascotas_Propietario_Nombre UNIQUE (PropietarioId, Nombre)
);
GO

INSERT INTO Propietarios (Nombres, Dni, Telefono, Email) VALUES
    ('María López Quispe',   '45678912', '987654321', 'maria.lopez@gmail.com'),
    ('Carlos Ramírez Torres','71234568', '912345678', NULL),
    ('Lucía Fernández Rojas','40987123', '956123789', 'lucia.fr@hotmail.com');

INSERT INTO Mascotas (Nombre, Especie, Raza, Edad, Peso, PropietarioId) VALUES
    ('Firulais', 'Perro', 'Labrador',        5, 28.50, 1),
    ('Michi',    'Gato',  'Siamés',          3,  4.20, 1),
    ('Rocky',    'Perro', 'Bulldog Francés', 2, 11.80, 2),
    ('Luna',     'Gato',  NULL,              1,  3.10, 3);
GO

SELECT * FROM Propietarios;
SELECT * FROM Mascotas;
