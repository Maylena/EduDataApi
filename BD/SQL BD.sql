
create database EduData;
go
USE EduData;
go

------------------------------------------
--TABLA DE USUARIOS
------------------------------------------
CREATE TABLE Usuarios (
	id INT PRIMARY KEY IDENTITY (1,1),
	Apodo NVARCHAR(50) UNIQUE NOT NULL,
	Correo NVARCHAR(100) UNIQUE NOT NULL,
	Clave NVARCHAR(255) NOT NULL, -- Almacenar hash de contraseña
	Estado BIT NOT NULL DEFAULT 1, --1= Activo, 0=Inactivo
	Rol NVARCHAR(20) NOT NULL DEFAULT 'Usuario',
	Fecha_Creacion DATETIME NOT NULL DEFAULT GETDATE(),
	Creado_Por NVARCHAR(50) NOT NULL,
	Fecha_Modificacion DATETIME NULL,
	Modificado_Por NVARCHAR(50) NULL
);
go

------------------------------------------
--PROCEDIMIENTOS PARA GESTION DE USUARIOS
------------------------------------------

--1. Crear usuario
--Inserta información de un usuario nuevo
CREATE PROCEDURE sp_CrearUsuario
	@Apodo NVARCHAR(50),
	@Correo NVARCHAR(100),
	@Clave NVARCHAR(255), --Ya debe venir hasheada
	@Estado BIT = 1,
	@Rol NVARCHAR(20) = 'Usuario',
	@Creado_Por NVARCHAR(50)
AS
BEGIN
	INSERT INTO Usuarios (Apodo, Correo, Clave, Estado, Rol,
							Fecha_Creacion, Creado_Por)
	VALUES (@Apodo, @Correo, @Clave, @Estado,
			@Rol, GETDATE(), @Creado_Por);
END
go

--3. Modificar usuario
--Actualiza datos del usuario, si canbia la clave, el código lo modifica en la base
CREATE PROCEDURE sp_ModificarUsuario
	@Id INT,
	@Apodo NVARCHAR(50),
	@Correo NVARCHAR(100),
	@Estado BIT = 1,
	@Rol NVARCHAR(20),
	@Modificado_Por NVARCHAR(50)
AS
BEGIN
	UPDATE Usuarios
	SET Apodo = @Apodo,
		Correo = @Correo,
		Estado = @Estado,
		Rol = @Rol,
		Fecha_modificacion = GETDATE(),
		Modificado_Por = @Modificado_Por
	WHERE Id = @Id;
END
go


--4. Cambiar contraseña
--Este procedimiento elimina un usuario usando su ID. Borras un registro específico de manera permanente.
CREATE PROCEDURE sp_CambiarClave
	@Id INT,
	@Nueva_Clave NVARCHAR(255), --Ya debeb venir hasheada
	@Modificado_Por NVARCHAR(50)
AS
BEGIN
	UPDATE Usuarios
	SET Clave = @Nueva_Clave,
		Fecha_Modificacion = GETDATE(),
		Modificado_Por = @Modificado_Por
	WHERE Id = @Id;
END
go


--5. Leer todos los usuarios
CREATE PROCEDURE sp_LeerUsuarios
AS
BEGIN
	SELECT Id, Apodo, Correo, Estado, Rol, 
			Fecha_Creacion, Creado_Por,
			Fecha_Modificacion, Modificado_Por
	FROM Usuarios;
END
GO

--6. Buscar usuario por ID
CREATE PROCEDURE sp_BuscarUsuarioPorId
	@Id INT
AS
BEGIN
	SELECT Id, Apodo, Correo, Clave, Estado, Rol,
			Fecha_Creacion, Creado_Por, 
			Fecha_Modificacion,Modificado_Por
	FROM Usuarios
	WHERE Id = @Id;
END
go

--7. Buscar usuario por apodo
CREATE PROCEDURE sp_BuscarUsuarioPorApodo
	@Apodo NVARCHAR(50)
AS
BEGIN
	SELECT Id, Apodo, Correo, Clave, Estado, Rol,
			Fecha_Creacion, Creado_Por,
			Fecha_Modificacion, Modificado_Por
	FROM Usuario
	WHERE Apodo LIKE '%' + '%';
END
go
