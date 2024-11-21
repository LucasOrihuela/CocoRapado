USE db_cocorapado; -- Cambia esto por el nombre de tu base de datos

-- Eliminar todas las constraints
DECLARE @sql NVARCHAR(MAX) = '';

-- Generar el script para eliminar las constraints de CHECK
SELECT @sql += 'ALTER TABLE ' + QUOTENAME(t.name) + ' DROP CONSTRAINT ' + QUOTENAME(c.name) + ';' + CHAR(13)
FROM sys.tables t
INNER JOIN sys.check_constraints c ON t.object_id = c.parent_object_id;

-- Generar el script para eliminar las constraints de FOREIGN KEY
SELECT @sql += 'ALTER TABLE ' + QUOTENAME(t.name) + ' DROP CONSTRAINT ' + QUOTENAME(f.name) + ';' + CHAR(13)
FROM sys.tables t
INNER JOIN sys.foreign_keys f ON t.object_id = f.parent_object_id;

-- Ejecutar el script para eliminar las constraints
EXEC sp_executesql @sql;

-- Truncar todas las tablas
SET @sql = '';

SELECT @sql += 'TRUNCATE TABLE ' + QUOTENAME(t.name) + ';' + CHAR(13)
FROM sys.tables t;

-- Ejecutar el script para truncar las tablas
EXEC sp_executesql @sql;
USE master
DROP DATABASE db_cocorapado