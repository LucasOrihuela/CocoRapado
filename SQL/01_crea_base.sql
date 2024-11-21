CREATE DATABASE db_cocorapado
GO
USE db_cocorapado
GO
CREATE TABLE perfiles(
id int PRIMARY KEY IDENTITY(1,1),
rol varchar(50),
permiso_admin int,
permiso_super_admin int,
permiso_profesional int
)

GO

CREATE TABLE sucursales(
id int PRIMARY KEY IDENTITY(1,1),
nombre varchar(100),
direccion varchar(200),
localidad varchar(100),
imagen varchar(500),
telefono varchar(50),
precioAbono int
)

GO

CREATE TABLE horarios(
id_sucursal int,
dia varchar(100),
horario_apertura varchar(100),
horario_cierre_mediodia varchar(100),
horario_apertura_mediodia varchar(100),
horario_cierre varchar(100),
UNIQUE(id_sucursal,dia),
CONSTRAINT FK_Horario_Sucursal FOREIGN KEY (id_sucursal) REFERENCES sucursales(id)
)

GO

CREATE TABLE usuarios (
    id INT PRIMARY KEY IDENTITY(1,1),
    id_perfil INT,
    correo VARCHAR(255) UNIQUE,
    password_hash VARCHAR(255),
    imagen VARCHAR(500),
    nombre VARCHAR(50),
    apellido VARCHAR(50),
    telefono VARCHAR(30),
	fecha_nacimiento DATE,
	turnos_cancelados INT,
	bloqueado INT,
    normalized_email AS UPPER(correo),
    security_stamp VARCHAR(255),
    CONSTRAINT FK_Usuario_Perfil FOREIGN KEY (id_perfil) REFERENCES perfiles(id)
);


GO

CREATE TABLE servicios(
id int PRIMARY KEY IDENTITY(1,1),
servicio varchar(200),
descripcion varchar(500),
duracion_min int,
precio_min int,
precio_max int,
imagen varchar(500)
CONSTRAINT FK_Servicio_Sucursal FOREIGN KEY (id_sucursal) REFERENCES sucursales(id)
)

GO

CREATE TABLE servicio_x_profesional(
id_servicio int not null,
id_profesional int not null,
UNIQUE(id_servicio,id_profesional),
CONSTRAINT FK_Servicio_x_Profesional_Servicio FOREIGN KEY (id_servicio) REFERENCES servicios(id),
CONSTRAINT FK_Servicio_x_Profesional_Perfil FOREIGN KEY (id_profesional) REFERENCES usuarios(id)
)

GO

CREATE TABLE servicio_x_sucursal(
id_servicio int not null,
id_sucursal int not null,
UNIQUE(id_servicio,id_sucursal),
CONSTRAINT FK_Servicio_x_Sucursal_Servicio FOREIGN KEY (id_servicio) REFERENCES servicios(id),
CONSTRAINT FK_Servicio_x_Sucursal_Sucursal FOREIGN KEY (id_sucursal) REFERENCES sucursales(id)
)

GO

CREATE TABLE profesional_x_sucursal(
id_profesional int not null,
id_sucursal int not null,
UNIQUE(id_profesional,id_sucursal),
CONSTRAINT FK_Profesional_x_Sucursal_Servicio FOREIGN KEY (id_profesional) REFERENCES usuarios(id),
CONSTRAINT FK_Profesional_x_Sucursal_Sucursal FOREIGN KEY (id_sucursal) REFERENCES sucursales(id)
)

GO

CREATE TABLE turnos(
id int PRIMARY KEY IDENTITY(1,1),
fecha DATE,
hora TIME,
id_sucursal int,
id_profesional int,
id_cliente int,
precio int,
duracion_min int,
ausente int,
CONSTRAINT FK_Turnos_Sucursal FOREIGN KEY (id_sucursal) REFERENCES sucursales(id),
CONSTRAINT FK_Turnos_Profesional FOREIGN KEY (id_profesional) REFERENCES usuarios(id),
CONSTRAINT FK_Turnos_Cliente FOREIGN KEY (id_cliente) REFERENCES usuarios(id)
)

GO

CREATE TABLE servicios_x_turno(
id_servicio int not null,
id_turno int not null,
UNIQUE(id_servicio,id_turno),
CONSTRAINT FK_Servicio_x_Turno_Servicio FOREIGN KEY (id_servicio) REFERENCES servicios(id),
CONSTRAINT FK_Servicio_x_Turno_Perfil FOREIGN KEY (id_turno) REFERENCES turnos(id)
)

GO

CREATE TABLE profesionales_favoritos(
id_usuario int not null,
id_profesional int not null,
UNIQUE(id_profesional,id_usuario),
CONSTRAINT FK_Cliente_favorito FOREIGN KEY (id_usuario) REFERENCES usuarios(id),
CONSTRAINT FK_Profesional_favorito FOREIGN KEY (id_profesional) REFERENCES usuarios(id)
)

GO


CREATE TABLE abono_cliente_x_sucursal(
id_cliente int not null,
id_sucursal int not null,
UNIQUE(id_cliente,id_sucursal),
CONSTRAINT FK_Cliente_x_Sucursal_Abono FOREIGN KEY (id_cliente) REFERENCES usuarios(id),
CONSTRAINT FK_Sucursal_x_Sucursal_Abono FOREIGN KEY (id_sucursal) REFERENCES sucursales(id)
)