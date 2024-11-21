USE db_cocorapado

BEGIN TRANSACTION;

BEGIN TRY
	INSERT into sucursales values ('Sucursal Martinez','Libertador 2000','Martinez','/assets/img/sucursales/local_martinez.jpg','+5491122334455')
	INSERT into sucursales values ('Sucursal Beccar','Av. Santa fe','Beccar','/assets/img/sucursales/local_beccar.jpg','+5491122378941')
	INSERT into sucursales values ('Sucursal Tigre','Av. Liniers','Tigre','/assets/img/sucursales/local_beccar.jpg','+5491122378941')

	INSERT into perfiles values ('Cliente',0,0,0)
	INSERT into perfiles values ('Profesional',0,0,1)
	INSERT into perfiles values ('Administrador',1,0,1)
	INSERT into perfiles values ('Super administrador',1,1,1)
	
	--INSERT into usuarios values (1,1,'juancito22@gmail.com','Juancito','/assets/img/team/1.jpg','juan','costa','1122334455')
	--INSERT into usuarios values (1,1,'carolina@gmail.com','caro22','/assets/img/team/2.jpg','carolina','olivera','1122334455')
	INSERT into usuarios (id_perfil,id_sucursal,correo,password_hash,imagen,nombre,apellido,telefono,fecha_nacimiento,security_stamp) values (2,1,'thiago@gmail.com','asdasdasdasdasdasdasddas','/assets/img/team/1.jpg','thiago','gonzalez','1122334455','20/12/1995','')
	INSERT into usuarios (id_perfil,id_sucursal,correo,password_hash,imagen,nombre,apellido,telefono,fecha_nacimiento,security_stamp) values (2,1,'matias15@gmail.com','asdasdasdasdasdasdasddas','/assets/img/team/1.jpg','matias','Berticelli','1122334455','20/12/1995','')
	--INSERT into usuarios values (3,1,'jimena@gmail.com','jime','/assets/img/team/1.jpg','jimena','baron','1122334455')
	--INSERT into usuarios values (4,1,'lucas@gmail.com','elPela','/assets/img/team/1.jpg','lucas','Orihuela','1122334455')
	--INSERT into usuarios values (1,2,'sebastian@gmail.com','seba123','/assets/img/team/2.jpg','sebastian','otero','1122334455')
	--INSERT into usuarios values (1,2,'ramiro@gmail.com','rama','/assets/img/team/1.jpg','ramiro','busato','1122334455')
	--INSERT into usuarios values (2,2,'valentin@gmail.com','osobuco','/assets/img/team/1.jpg','valentin','marticelli','1122334455')
	--INSERT into usuarios values (2,2,'jesus@gmail.com','cine','/assets/img/team/2.jpg','jesus','gimenez','1122334455')
	--INSERT into usuarios values (3,2,'mateo14@gmail.com','Regulus','/assets/img/team/1.jpg','mateo','beccaria','1122334455')
	--INSERT into usuarios values (4,2,'matias_cast22@gmail.com','LaBestia','pass','/assets/img/team/2.jpg','matias','castorina','1122334455')


	INSERT into horarios values (1,'Lunes','08:00','12:00','14:30','20:00')
	INSERT into horarios values (1,'Martes','08:00','12:00','14:30','20:00')
	INSERT into horarios values (1,'Miercoles','08:00','12:00','14:30','20:00')
	INSERT into horarios values (1,'Jueves','08:00','12:00','14:30','20:00')
	INSERT into horarios values (1,'Viernes','08:00','12:00','14:30','20:00')
	INSERT into horarios values (1,'Sabado','08:00','12:00','14:30','20:00')
	INSERT into horarios values (1,'Domingo','08:00','12:00','14:30','20:00')
	INSERT into horarios values (2,'Lunes','09:00','13:00','15:00','20:30')
	INSERT into horarios values (2,'Martes','09:00','13:00','15:00','20:30')
	INSERT into horarios values (2,'Miercoles','09:00','13:00','15:00','20:30')
	INSERT into horarios values (2,'Jueves','09:00','13:00','15:00','20:30')
	INSERT into horarios values (2,'Viernes','09:00','13:00','15:00','20:30')
	INSERT into horarios values (2,'Sabado','09:00','13:00','15:00','20:30')
	INSERT into horarios values (2,'Domingo','09:00','13:00','15:00','20:30')


	INSERT into servicios values ('Corte de pelo','Corte de pelo simple',30,8000,0,'',1)
	INSERT into servicios values ('Corte y barba','Corte de pelo simple mas corte y delineado de barba',45,10000,0,'',1)
	INSERT into servicios values ('Ritual de barba','el barbero baila alrededor de la silla mientras te hace la barba',40,5000,0,'',1)
	INSERT into servicios values ('Color global','Tintura completa unico color',60,30000,.50000,'',1)
	INSERT into servicios values ('Perfilado de cejas','perfilado de cejas',15,4000,0,'',1)
	INSERT into servicios values ('Mechas o claritos','tintuta parcial del pelo',30,30000,50000,'',1)
	INSERT into servicios values ('Baño de luz','caño de luz',45,18000,25000,'',1)
	INSERT into servicios values ('Corte y dibujo',' Corte simple mas un dibujo a eleccion',40,9000,0,'',1)
	INSERT into servicios values ('Permanente','Planchado o ondulado permanente',45,20000,40000,'',1)
	
	--INSERT into servicio_x_profesional values (1,3)
	--INSERT into servicio_x_profesional values (2,3)
	--INSERT into servicio_x_profesional values (3,3)
	--INSERT into servicio_x_profesional values (4,3)
	--INSERT into servicio_x_profesional values (5,3)
	--INSERT into servicio_x_profesional values (6,3)
	--INSERT into servicio_x_profesional values (7,3)
	--INSERT into servicio_x_profesional values (8,3)
	--INSERT into servicio_x_profesional values (1,4)
	--INSERT into servicio_x_profesional values (2,4)
	--INSERT into servicio_x_profesional values (3,4)
	--INSERT into servicio_x_profesional values (4,4)
	--INSERT into servicio_x_profesional values (5,4)
	--INSERT into servicio_x_profesional values (6,4)
	--INSERT into servicio_x_profesional values (7,4)
	--INSERT into servicio_x_profesional values (1,9)
	--INSERT into servicio_x_profesional values (2,9)
	--INSERT into servicio_x_profesional values (6,9)
	--INSERT into servicio_x_profesional values (7,9)
	--INSERT into servicio_x_profesional values (1,10)
	--INSERT into servicio_x_profesional values (3,10)
	--INSERT into servicio_x_profesional values (5,10)
	--INSERT into servicio_x_profesional values (7,10)
	--INSERT into servicio_x_profesional values (8,10)
	--INSERT into servicio_x_profesional values (9,10)


	--INSERT into turnos values ('2024-09-27','09:00:00',1,3,50000,0)
	--INSERT into turnos values ('2024-09-27','10:00:00',1,4,20000,0)
	--INSERT into turnos values ('2024-09-28','11:30:00',1,4,8000,0)
	--INSERT into turnos values ('2024-09-28','14:00:00',1,3,18000,0)
	--INSERT into turnos values ('2024-09-27','09:00:00',2,10,50000,0)
	--INSERT into turnos values ('2024-09-27','10:00:00',2,9,20000,0)
	--INSERT into turnos values ('2024-09-28','11:30:00',2,9,8000,0)
	--INSERT into turnos values ('2024-09-28','14:00:00',2,10,18000,0)


    COMMIT TRANSACTION;
END TRY
BEGIN CATCH
    ROLLBACK TRANSACTION;
    SELECT ERROR_MESSAGE() AS Error_Mensaje;
END CATCH;