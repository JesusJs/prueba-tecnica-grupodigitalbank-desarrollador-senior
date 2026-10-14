IF DB_ID('UsuariosDB') IS NULL
    CREATE DATABASE UsuariosDB;
GO
USE UsuariosDB;
GO

IF OBJECT_ID('dbo.Usuario', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.Usuario
    (
        Id              INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Usuario PRIMARY KEY,
        Nombre          NVARCHAR(100)     NOT NULL,
        FechaNacimiento DATE              NOT NULL,
        Sexo            CHAR(1)           NOT NULL,
        CONSTRAINT CK_Usuario_Sexo CHECK (Sexo IN ('M', 'F'))
    );
END
GO

CREATE OR ALTER PROCEDURE dbo.usp_Usuario_CRUD
    @Accion          CHAR(1),            -- C=Create, R=Read, U=Update, D=Delete
    @Id              INT           = NULL,
    @Nombre          NVARCHAR(100) = NULL,
    @FechaNacimiento DATE          = NULL,
    @Sexo            CHAR(1)       = NULL
AS
BEGIN
    SET NOCOUNT ON;

    IF @Accion = 'C'
    BEGIN
        INSERT INTO dbo.Usuario (Nombre, FechaNacimiento, Sexo)
        VALUES (@Nombre, @FechaNacimiento, @Sexo);

        SELECT CAST(SCOPE_IDENTITY() AS INT) AS Id;
        RETURN;
    END

    IF @Accion = 'R'
    BEGIN
        SELECT Id, Nombre, FechaNacimiento, Sexo
        FROM dbo.Usuario
        WHERE (@Id IS NULL OR Id = @Id)
        ORDER BY Id;
        RETURN;
    END

    IF @Accion = 'U'
    BEGIN
        UPDATE dbo.Usuario
        SET Nombre = @Nombre,
            FechaNacimiento = @FechaNacimiento,
            Sexo = @Sexo
        WHERE Id = @Id;

        IF @@ROWCOUNT = 0 THROW 50001, 'Usuario no encontrado.', 1;
        RETURN;
    END

    IF @Accion = 'D'
    BEGIN
        DELETE FROM dbo.Usuario WHERE Id = @Id;

        IF @@ROWCOUNT = 0 THROW 50001, 'Usuario no encontrado.', 1;
        RETURN;
    END

    THROW 50000, 'Acción inválida. Use C, R, U o D.', 1;
END
GO