
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



--BASE DE DATOS DE ASIGNACIÓN 1--------------------------------------------------------------------------------
--Profesor
CREATE TABLE Profesor (
    IdProfesor INT IDENTITY(1,1) PRIMARY KEY,
    Nombre NVARCHAR(100) NOT NULL,
    Email NVARCHAR(100) NOT NULL UNIQUE,
    Especialidad NVARCHAR(100) NOT NULL,
    Activo BIT DEFAULT 1 NOT NULL,
    -- Campos de Auditoría
    FechaCreacion DATETIME DEFAULT GETDATE(),
    UsuarioCreacion NVARCHAR(50)
);

--Estudiante
CREATE TABLE Estudiante (
    IdEstudiante INT IDENTITY(1,1) PRIMARY KEY,
    Nombre NVARCHAR(100) NOT NULL,
    Email NVARCHAR(100) NOT NULL UNIQUE,
    Telefono NVARCHAR(20),
    Activo BIT DEFAULT 1 NOT NULL,
    -- Campos de Auditoría
    FechaCreacion DATETIME DEFAULT GETDATE(),
    UsuarioCreacion NVARCHAR(50)
);

--Curso
CREATE TABLE Curso (
    IdCurso INT IDENTITY(1,1) PRIMARY KEY,
    Nombre NVARCHAR(100) NOT NULL,
    Descripcion NVARCHAR(255),
    IdProfesor INT NOT NULL,
    Activo BIT DEFAULT 1 NOT NULL,
    
    FechaCreacion DATETIME DEFAULT GETDATE(),
    UsuarioCreacion NVARCHAR(50),
    CONSTRAINT FK_Curso_Profesor FOREIGN KEY (IdProfesor) REFERENCES Profesor(IdProfesor)
);

CREATE INDEX IX_Curso_IdProfesor ON Curso(IdProfesor);

--EstudianteCurso
CREATE TABLE EstudianteCurso (
    IdEstudianteCurso INT IDENTITY(1,1) PRIMARY KEY,
    IdEstudiante INT NOT NULL,
    IdCurso INT NOT NULL,
    Activo BIT DEFAULT 1 NOT NULL,
    
    FechaCreacion DATETIME DEFAULT GETDATE(),
    UsuarioCreacion NVARCHAR(50),
    CONSTRAINT FK_EstudianteCurso_Estudiante FOREIGN KEY (IdEstudiante) REFERENCES Estudiante(IdEstudiante),
    CONSTRAINT FK_EstudianteCurso_Curso FOREIGN KEY (IdCurso) REFERENCES Curso(IdCurso)
);

CREATE INDEX IX_EstudianteCurso_IdEstudiante ON EstudianteCurso(IdEstudiante);
CREATE INDEX IX_EstudianteCurso_IdCurso ON EstudianteCurso(IdCurso);




--PROCEDIMIENTOS ALMACENADOS--
-- INSERTAR
GO 

CREATE PROCEDURE sp_Estudiante_Insertar
    @Nombre NVARCHAR(100),
    @Email NVARCHAR(100),
    @Telefono NVARCHAR(20),
    @UsuarioCreacion NVARCHAR(50)
AS
BEGIN
    INSERT INTO Estudiante (Nombre, Email, Telefono, UsuarioCreacion)
    VALUES (@Nombre, @Email, @Telefono, @UsuarioCreacion);
    
    -- Retornar el ID generado
    SELECT SCOPE_IDENTITY() AS IdEstudiante;
END;
GO

-- OBTENER POR ID (Solo activos)
CREATE PROCEDURE sp_Estudiante_ObtenerPorId
    @IdEstudiante INT
AS
BEGIN
    SELECT IdEstudiante, Nombre, Email, Telefono, Activo, FechaCreacion, UsuarioCreacion
    FROM Estudiante
    WHERE IdEstudiante = @IdEstudiante AND Activo = 1;
END;
GO

-- LISTAR TODOS (Solo activos)
CREATE PROCEDURE sp_Estudiante_Listar
AS
BEGIN
    SELECT IdEstudiante, Nombre, Email, Telefono, Activo, FechaCreacion, UsuarioCreacion
    FROM Estudiante
    WHERE Activo = 1;
END;
GO

-- ACTUALIZAR
CREATE PROCEDURE sp_Estudiante_Actualizar
    @IdEstudiante INT,
    @Nombre NVARCHAR(100),
    @Email NVARCHAR(100),
    @Telefono NVARCHAR(20)
AS
BEGIN
    UPDATE Estudiante
    SET Nombre = @Nombre,
        Email = @Email,
        Telefono = @Telefono
    WHERE IdEstudiante = @IdEstudiante AND Activo = 1;
END;
GO

-- ELIMINACIÓN LÓGICA
CREATE PROCEDURE sp_Estudiante_Eliminar
    @IdEstudiante INT
AS
BEGIN
    UPDATE Estudiante
    SET Activo = 0
    WHERE IdEstudiante = @IdEstudiante;
END;
GO


--PROFESOR--
-- INSERTAR
CREATE PROCEDURE sp_Profesor_Insertar
    @Nombre NVARCHAR(100),
    @Email NVARCHAR(100),
    @Especialidad NVARCHAR(100),
    @UsuarioCreacion NVARCHAR(50)
AS
BEGIN
    INSERT INTO Profesor (Nombre, Email, Especialidad, UsuarioCreacion)
    VALUES (@Nombre, @Email, @Especialidad, @UsuarioCreacion);
    
    SELECT SCOPE_IDENTITY() AS IdProfesor;
END;
GO

-- OBTENER POR ID
CREATE PROCEDURE sp_Profesor_ObtenerPorId
    @IdProfesor INT
AS
BEGIN
    SELECT IdProfesor, Nombre, Email, Especialidad, Activo, FechaCreacion, UsuarioCreacion
    FROM Profesor
    WHERE IdProfesor = @IdProfesor AND Activo = 1;
END;
GO

-- LISTAR TODOS
CREATE PROCEDURE sp_Profesor_Listar
AS
BEGIN
    SELECT IdProfesor, Nombre, Email, Especialidad, Activo, FechaCreacion, UsuarioCreacion
    FROM Profesor
    WHERE Activo = 1;
END;
GO

--CURSO--
-- INSERTAR
CREATE PROCEDURE sp_Curso_Insertar
    @Nombre NVARCHAR(100),
    @Descripcion NVARCHAR(255),
    @IdProfesor INT,
    @UsuarioCreacion NVARCHAR(50)
AS
BEGIN
    INSERT INTO Curso (Nombre, Descripcion, IdProfesor, UsuarioCreacion)
    VALUES (@Nombre, @Descripcion, @IdProfesor, @UsuarioCreacion);
    
    SELECT SCOPE_IDENTITY() AS IdCurso;
END;
GO

-- OBTENER POR ID
CREATE PROCEDURE sp_Curso_ObtenerPorId
    @IdCurso INT
AS
BEGIN
    SELECT IdCurso, Nombre, Descripcion, IdProfesor, Activo, FechaCreacion, UsuarioCreacion
    FROM Curso
    WHERE IdCurso = @IdCurso AND Activo = 1;
END;
GO

-- LISTAR TODOS
CREATE PROCEDURE sp_Curso_Listar
AS
BEGIN
    SELECT IdCurso, Nombre, Descripcion, IdProfesor, Activo, FechaCreacion, UsuarioCreacion
    FROM Curso
    WHERE Activo = 1;
END;
GO

--ESTUDIANTE CURSO--
-- INSCRIBIR ESTUDIANTE EN CURSO
CREATE PROCEDURE sp_EstudianteCurso_Inscribir
    @IdEstudiante INT,
    @IdCurso INT,
    @UsuarioCreacion NVARCHAR(50)
AS
BEGIN
    INSERT INTO EstudianteCurso (IdEstudiante, IdCurso, UsuarioCreacion)
    VALUES (@IdEstudiante, @IdCurso, @UsuarioCreacion);
    
    SELECT SCOPE_IDENTITY() AS IdEstudianteCurso;
END;
GO

-- ELIMINAR INSCRIPCIÓN (Lógica)
CREATE PROCEDURE sp_EstudianteCurso_Eliminar
    @IdEstudianteCurso INT
AS
BEGIN
    UPDATE EstudianteCurso
    SET Activo = 0
    WHERE IdEstudianteCurso = @IdEstudianteCurso;
END;
GO


