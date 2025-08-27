-- =============================================
-- STORED PROCEDURE CORREGIDO PARA LOGIN
-- =============================================
USE DBEmprendimiento_Citas;
GO

-- Eliminar el stored procedure existente
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_loginUsuario]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[sp_loginUsuario]
GO

-- Crear el stored procedure corregido
CREATE PROCEDURE sp_loginUsuario
    @DocumentoIdentidad NVARCHAR(20),
    @Clave NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Buscar usuario por documento de identidad
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

PRINT 'Stored Procedure sp_loginUsuario corregido exitosamente!'
GO
