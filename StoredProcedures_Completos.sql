-- =============================================
-- STORED PROCEDURES PARA SISTEMA CENTRO EMPRENDIMIENTO
-- =============================================

USE DBEmprendimiento_Citas;
GO

-- =============================================
-- 1. STORED PROCEDURE PARA LOGIN DE USUARIO
-- =============================================
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_loginUsuario]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[sp_loginUsuario]
GO

CREATE PROCEDURE sp_loginUsuario
    @DocumentoIdentidad NVARCHAR(20),
    @Clave NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        u.IdUsuario,
        u.NumeroDocumentoIdentidad,
        u.Nombre,
        u.Apellido,
        u.Correo,
        u.Clave,
        r.IdRolUsuario,
        r.Nombre AS NombreRol
    FROM Usuario u
    INNER JOIN RolUsuario r ON u.IdRolUsuario = r.IdRolUsuario
    WHERE u.NumeroDocumentoIdentidad = @DocumentoIdentidad
END
GO

-- =============================================
-- 2. STORED PROCEDURE PARA GUARDAR USUARIO
-- =============================================
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_guardarUsuario]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[sp_guardarUsuario]
GO

CREATE PROCEDURE sp_guardarUsuario
    @NumeroDocumentoIdentidad NVARCHAR(20),
    @Nombre NVARCHAR(100),
    @Apellido NVARCHAR(100),
    @Correo NVARCHAR(100),
    @Clave NVARCHAR(100),
    @IdRolUsuario INT,
    @MsgError NVARCHAR(100) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRY
        -- Verificar si ya existe el documento
        IF EXISTS (SELECT 1 FROM Usuario WHERE NumeroDocumentoIdentidad = @NumeroDocumentoIdentidad)
        BEGIN
            SET @MsgError = 'Ya existe un usuario con este documento'
            RETURN
        END
        
        -- Verificar si ya existe el correo
        IF EXISTS (SELECT 1 FROM Usuario WHERE Correo = @Correo)
        BEGIN
            SET @MsgError = 'Ya existe un usuario con este correo'
            RETURN
        END
        
        -- Insertar nuevo usuario
        INSERT INTO Usuario (NumeroDocumentoIdentidad, Nombre, Apellido, Correo, Clave, IdRolUsuario, FechaCreacion)
        VALUES (@NumeroDocumentoIdentidad, @Nombre, @Apellido, @Correo, @Clave, @IdRolUsuario, GETDATE())
        
        SET @MsgError = ''
    END TRY
    BEGIN CATCH
        SET @MsgError = 'Error al guardar usuario: ' + ERROR_MESSAGE()
    END CATCH
END
GO

-- =============================================
-- 3. STORED PROCEDURE PARA EDITAR USUARIO
-- =============================================
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_editarUsuario]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[sp_editarUsuario]
GO

CREATE PROCEDURE sp_editarUsuario
    @IdUsuario INT,
    @NumeroDocumentoIdentidad NVARCHAR(20),
    @Nombre NVARCHAR(100),
    @Apellido NVARCHAR(100),
    @Correo NVARCHAR(100),
    @Clave NVARCHAR(100),
    @IdRolUsuario INT,
    @MsgError NVARCHAR(100) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRY
        -- Verificar si existe otro usuario con el mismo documento
        IF EXISTS (SELECT 1 FROM Usuario WHERE NumeroDocumentoIdentidad = @NumeroDocumentoIdentidad AND IdUsuario != @IdUsuario)
        BEGIN
            SET @MsgError = 'Ya existe otro usuario con este documento'
            RETURN
        END
        
        -- Verificar si existe otro usuario con el mismo correo
        IF EXISTS (SELECT 1 FROM Usuario WHERE Correo = @Correo AND IdUsuario != @IdUsuario)
        BEGIN
            SET @MsgError = 'Ya existe otro usuario con este correo'
            RETURN
        END
        
        -- Actualizar usuario
        UPDATE Usuario 
        SET NumeroDocumentoIdentidad = @NumeroDocumentoIdentidad,
            Nombre = @Nombre,
            Apellido = @Apellido,
            Correo = @Correo,
            Clave = @Clave,
            IdRolUsuario = @IdRolUsuario
        WHERE IdUsuario = @IdUsuario
        
        SET @MsgError = ''
    END TRY
    BEGIN CATCH
        SET @MsgError = 'Error al editar usuario: ' + ERROR_MESSAGE()
    END CATCH
END
GO

-- =============================================
-- 4. STORED PROCEDURE PARA ELIMINAR USUARIO
-- =============================================
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_eliminarUsuario]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[sp_eliminarUsuario]
GO

CREATE PROCEDURE sp_eliminarUsuario
    @IdUsuario INT
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRY
        -- Verificar si tiene citas asociadas
        IF EXISTS (SELECT 1 FROM Cita WHERE IdEmprendedor = @IdUsuario OR IdAsesor = @IdUsuario)
        BEGIN
            RAISERROR ('No se puede eliminar el usuario porque tiene citas asociadas', 16, 1)
            RETURN
        END
        
        DELETE FROM Usuario WHERE IdUsuario = @IdUsuario
    END TRY
    BEGIN CATCH
        RAISERROR ('Error al eliminar usuario: %s', 16, 1, ERROR_MESSAGE())
    END CATCH
END
GO

-- =============================================
-- 5. STORED PROCEDURE PARA LISTAR USUARIOS
-- =============================================
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_listaUsuario]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[sp_listaUsuario]
GO

CREATE PROCEDURE sp_listaUsuario
    @IdRolUsuario INT = 0
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        u.IdUsuario,
        u.NumeroDocumentoIdentidad,
        u.Nombre,
        u.Apellido,
        u.Correo,
        u.Clave,
        u.IdRolUsuario,
        r.Nombre AS NombreRol,
        u.FechaCreacion
    FROM Usuario u
    INNER JOIN RolUsuario r ON u.IdRolUsuario = r.IdRolUsuario
    WHERE (@IdRolUsuario = 0 OR u.IdRolUsuario = @IdRolUsuario)
    ORDER BY u.Nombre, u.Apellido
END
GO

-- =============================================
-- 6. STORED PROCEDURE PARA LISTAR ASESORES
-- =============================================
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_listaAsesor]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[sp_listaAsesor]
GO

CREATE PROCEDURE sp_listaAsesor
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        a.IdAsesor,
        a.IdUsuario,
        a.IdEspecialidad,
        u.Nombre,
        u.Apellido,
        u.Correo,
        e.Nombre AS NombreEspecialidad
    FROM Asesor a
    INNER JOIN Usuario u ON a.IdUsuario = u.IdUsuario
    INNER JOIN Especialidad e ON a.IdEspecialidad = e.IdEspecialidad
    ORDER BY u.Nombre, u.Apellido
END
GO

-- =============================================
-- 7. STORED PROCEDURE PARA LISTAR ESPECIALIDADES
-- =============================================
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_listaEspecialidad]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[sp_listaEspecialidad]
GO

CREATE PROCEDURE sp_listaEspecialidad
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        IdEspecialidad,
        Nombre,
        Descripcion
    FROM Especialidad
    ORDER BY Nombre
END
GO

-- =============================================
-- 8. STORED PROCEDURE PARA LISTAR CITAS
-- =============================================
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_listaCita]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[sp_listaCita]
GO

CREATE PROCEDURE sp_listaCita
    @IdEmprendedor INT = 0,
    @IdAsesor INT = 0
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        c.IdCita,
        c.IdEmprendedor,
        c.IdAsesor,
        c.IdEspecialidad,
        c.FechaCita,
        c.HoraCita,
        c.IdEstadoCita,
        c.Observaciones,
        -- Datos del emprendedor
        emp.Nombre AS NombreEmprendedor,
        emp.Apellido AS ApellidoEmprendedor,
        -- Datos del asesor
        ase.Nombre AS NombreAsesor,
        ase.Apellido AS ApellidoAsesor,
        -- Especialidad
        e.Nombre AS NombreEspecialidad,
        -- Estado
        ec.Nombre AS NombreEstado
    FROM Cita c
    INNER JOIN Usuario emp ON c.IdEmprendedor = emp.IdUsuario
    INNER JOIN Usuario ase ON c.IdAsesor = ase.IdUsuario
    INNER JOIN Especialidad e ON c.IdEspecialidad = e.IdEspecialidad
    INNER JOIN EstadoCita ec ON c.IdEstadoCita = ec.IdEstadoCita
    WHERE (@IdEmprendedor = 0 OR c.IdEmprendedor = @IdEmprendedor)
      AND (@IdAsesor = 0 OR c.IdAsesor = @IdAsesor)
    ORDER BY c.FechaCita DESC, c.HoraCita DESC
END
GO

-- =============================================
-- 9. STORED PROCEDURE PARA GUARDAR CITA
-- =============================================
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_guardarCita]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[sp_guardarCita]
GO

CREATE PROCEDURE sp_guardarCita
    @IdEmprendedor INT,
    @IdAsesor INT,
    @IdEspecialidad INT,
    @FechaCita DATE,
    @HoraCita TIME,
    @Observaciones NVARCHAR(500),
    @MsgError NVARCHAR(100) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRY
        -- Verificar si el asesor está disponible en esa fecha y hora
        IF EXISTS (
            SELECT 1 FROM Cita 
            WHERE IdAsesor = @IdAsesor 
              AND FechaCita = @FechaCita 
              AND HoraCita = @HoraCita
              AND IdEstadoCita IN (1, 2) -- Pendiente o Confirmada
        )
        BEGIN
            SET @MsgError = 'El asesor no está disponible en esa fecha y hora'
            RETURN
        END
        
        -- Verificar si el emprendedor ya tiene una cita en esa fecha y hora
        IF EXISTS (
            SELECT 1 FROM Cita 
            WHERE IdEmprendedor = @IdEmprendedor 
              AND FechaCita = @FechaCita 
              AND HoraCita = @HoraCita
              AND IdEstadoCita IN (1, 2) -- Pendiente o Confirmada
        )
        BEGIN
            SET @MsgError = 'Ya tienes una cita programada en esa fecha y hora'
            RETURN
        END
        
        -- Insertar nueva cita
        INSERT INTO Cita (IdEmprendedor, IdAsesor, IdEspecialidad, FechaCita, HoraCita, IdEstadoCita, Observaciones)
        VALUES (@IdEmprendedor, @IdAsesor, @IdEspecialidad, @FechaCita, @HoraCita, 1, @Observaciones) -- 1 = Pendiente
        
        SET @MsgError = ''
    END TRY
    BEGIN CATCH
        SET @MsgError = 'Error al guardar cita: ' + ERROR_MESSAGE()
    END CATCH
END
GO

-- =============================================
-- 10. STORED PROCEDURE PARA ACTUALIZAR ESTADO CITA
-- =============================================
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_actualizarEstadoCita]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[sp_actualizarEstadoCita]
GO

CREATE PROCEDURE sp_actualizarEstadoCita
    @IdCita INT,
    @IdEstadoCita INT,
    @Observaciones NVARCHAR(500) = NULL,
    @MsgError NVARCHAR(100) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRY
        -- Verificar que la cita existe
        IF NOT EXISTS (SELECT 1 FROM Cita WHERE IdCita = @IdCita)
        BEGIN
            SET @MsgError = 'La cita no existe'
            RETURN
        END
        
        -- Actualizar estado de la cita
        UPDATE Cita 
        SET IdEstadoCita = @IdEstadoCita,
            Observaciones = ISNULL(@Observaciones, Observaciones)
        WHERE IdCita = @IdCita
        
        SET @MsgError = ''
    END TRY
    BEGIN CATCH
        SET @MsgError = 'Error al actualizar estado de cita: ' + ERROR_MESSAGE()
    END CATCH
END
GO

-- =============================================
-- MENSAJE DE CONFIRMACIÓN
-- =============================================
PRINT '============================================='
PRINT 'STORED PROCEDURES CREADOS EXITOSAMENTE'
PRINT '============================================='
PRINT '1. sp_loginUsuario - Para autenticación'
PRINT '2. sp_guardarUsuario - Para crear usuarios'
PRINT '3. sp_editarUsuario - Para editar usuarios'
PRINT '4. sp_eliminarUsuario - Para eliminar usuarios'
PRINT '5. sp_listaUsuario - Para listar usuarios'
PRINT '6. sp_listaAsesor - Para listar asesores'
PRINT '7. sp_listaEspecialidad - Para listar especialidades'
PRINT '8. sp_listaCita - Para listar citas'
PRINT '9. sp_guardarCita - Para crear citas'
PRINT '10. sp_actualizarEstadoCita - Para cambiar estado de citas'
PRINT '============================================='
PRINT '¡El sistema está listo para funcionar!'
PRINT '============================================='
GO
