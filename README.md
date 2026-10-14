# Prueba técnica: Gestión de usuarios - Grupo Digital Bank

Sistema por capas en .NET Framework 4.8 - ASP.NET MVC 5:
presentación, servicio WCF negocio y SQL Server datos.

## Mapa de capas:
#### Presentacion, proyecto web `Usuarios/Usuarios.Web`

#### Negocio, servicio WCF `Usuarios/Usuarios.Services`

#### Datos `Usuarios/Usuarios.Data`

---
## Script base de datos:
`database/01_usuario_table_and_sp.sql` Tabla `Usuario` y procedimiento `usp_Usuario_CRUD`.

---
## Cumplimiento de la prueba tecnica:
Página "usuario": Nombre: texto, Fecha de nacimiento: calendario, Sexo: DropDownList

Página "usuario consulta": grilla con Modificar y Eliminar `/Usuario/Consulta`: tabla con botón Modificar, reutiliza el formulario y Eliminar POST con confirmación

Servicio WCF: Agregar, Modificar, Consultar, Eliminar `IUsuarioService` con las cuatro operaciones

Conexión a BD desde la capa de negocio: El servicio lee la cadena de su `Web.config` y construye el repositorio `UsuarioService.CrearBusiness`

Tabla: Nombre - 100, Fecha de nacimiento - fecha, Sexo - texto fijo - 1 `NVARCHAR(100)`, `DATE`, `CHAR(1)`

Procedimiento almacenado con el CRUD `usp_Usuario_CRUD` con parámetro `@Accion` C, R, U, D

Desarrollado en .NET - .NET Framework 4.8


## Decisiones técnicas adicionales

- `Id` autoincremental: permite identificar de forma única cada registro y realizar las operaciones de modificación y eliminación desde la grilla.
    
- `ConsultarPorId`: operación adicional del servicio que permite recuperar un usuario específico y precargar sus datos en el formulario de modificación.


## Arquitectura:
Arquitectura por capas 3 capas, con dependencias en un solo sentido y sin ciclos:
```
Usuarios.Web --(SOAP/WSDL)--> Usuarios.Services --> Usuarios.Data --> SQL Server Presentación Negocio (WCF) Acceso a datos Tabla + SP
```
- Presentación: ASP.NET MVC 5. Solo conoce el servicio por su WSDL "Service Reference", no por referencia de proyecto. 
- Negocio: servicio WCF con validaciones y reglas. Es la única capa que abre la conexión a la base de datos.
- Datos: `Usuarios.Data` "repositorio ADO.NET" y el script SQL con la tabla y el procedimiento almacenado. 

Patrones usados: Repository, DTO por capa, Gateway, Adapter/Facade "servicio WCF", Mapper manual e inyección de dependencias por constructor. 


## Cómo ejecutar

Requisitos: Visual Studio con el workload ASP.NET and web development, el targeting pack de .NET Framework 4.8 y SQL Server LocalDB o cualquier instancia de SQL Server.

1. Ejecutar `database/01_usuario_table_and_sp.sql` es idempotente: se puede repetir sin perder datos.
2. Si no usas `(localdb)\MSSQLLocalDB`, ajustar `connectionStrings` en `Usuarios/Usuarios.Services/Web.config`.
3. Abrir `Usuarios/Usuarios.slnx` Visual Studio 2022 17.13 o superior.
4. Configurar Multiple startup projects: `Usuarios.Services` y `Usuarios.Web` en Start.
5. Ejecutar (F5). La web abre en `/Usuario/Crear`. Navegación: menú superior "Usuario" / "Usuario consulta".
6. Si el puerto del servicio cambia, actualizar el `address` del `<endpoint>` en `Usuarios.Web/Web.config`.

## Decisiones técnicas

.NET Framework 4.8: la prueba tecnica exige WCF, que solo es nativo en .NET Framework

ASP.NET MVC 5: Controlador testeable, `DropDownList` y vistas Razor nativas

basicHttpBinding: Simple e interoperable, suficiente para la prueba

ADO.NET + procedimiento almacenado: la prueba tecnica pide un SP.

Service Reference + `IUsuarioGateway`: La web no conoce el servicio por referencia de proyecto, solo por WSDL, y el gateway permite simular el servicio en pruebas.

Namespace WCF propio y versionado: Evita `tempuri.org` y permite evolucionar el contrato (`/v1`)


## Base de datos

- `NVARCHAR(100)` en lugar de `VARCHAR` para soportar tildes y ñ.
- `CHECK (Sexo IN ('M','F'))` como defensa en profundidad.
- Un único procedimiento con `@Accion`, como pide la prueba tecnica.
- `THROW 50001` cuando el registro no existe: el repositorio lo traduce a `false` y el negocio a una excepción propia, sin depender de códigos de SQL Server en las capas superiores.

## Un modelo por capa

`UsuarioEntity` /datos, `UsuarioDto` contrato WCF, `[DataContract]` y `UsuarioViewModel` vista, con DataAnnotations. 
El mapeo es **manual** `UsuarioMapper`: con una sola entidad de cuatro campos, AutoMapper añadiría dependencia y configuración sin beneficio.

## Conexión a la base de datos desde el servicio

La cadena vive solo en el `Web.config` del servicio. `UsuarioService` la lee y se la entrega al repositorio, de modo que la conexión se configura y se inicia desde la capa de negocio, como pide el enunciado. El código ADO.NET queda aislado en `Usuarios.Data`. La web nunca conoce la cadena ni la base de datos.


## Validaciones

Navegador: `maxlength=100`, `type="date"`, lista cerrada de sexo

UsuarioViewModel: Nombre obligatorio y de máximo 100, fecha obligatoria, sexo obligatorio

`UsuarioValidator`:  Usuario no nulo, nombre no vacío y de máximo 100, fecha obligatoria y no futura, sexo `M` o `F` "devuelve todos" los errores a la vez

UsuarioBusiness: d mayor que cero en modificar, eliminar y consultar por id

Base de datos: `NOT NULL`, tipos, `CHECK` de sexo, `PRIMARY KEY`

## Seguridad

- Parámetros SQL tipados y con tamaño: sin concatenar SQL "inyección SQL"
- Eliminar solo por `POST` con `[ValidateAntiForgeryToken]` "CSRF". Los formularios de crear y editar también llevan token.
- Razor codifica la salida por defecto (XSS).
- `includeExceptionDetailInFaults="false"` y errores inesperados con mensaje genérico, el detalle solo se registra en el servidor "divulgación de información".
- `directoryBrowse` desactivado.
- Cadena de conexión con `Integrated Security`: sin usuario ni clave en el repositorio.

## Principios SOLID y patrones

**SOLID**

- **S**: cada clase tiene una responsabilidad: `UsuarioValidator` valida, `UsuarioBusiness` orquesta, `UsuarioRepository` accede a datos, `UsuarioMapper` mapea, `UsuarioService` traduce excepciones a faults.
- **O** parcial: cambiar el repositorio o añadir un validador no obliga a modificar `UsuarioBusiness`, porque depende de interfaces. Las reglas del validador están en un solo método, no son reglas intercambiables.
- **L**: los dobles de prueba sustituyen a las implementaciones sin alterar el comportamiento esperado.
- **I**: interfaces pequeñas (`IUsuarioValidator`, `IUsuarioBusiness`, `IUsuarioRepository`, `IUsuarioGateway`).
- **D**: todas las dependencias entran por constructor y se declaran como interfaces.

## Pruebas

- Enfoque **TDD** "rojo, verde" con NUnit y Moq: validador, negocio "repositorio simulado", servicio "traducción de excepciones a faults" y controlador "gateway simulado".
- Pruebas de integración del repositorio con `[Category("Integration")]`: requieren la base de datos y se excluyen con `TestCategory!=Integration`.


## Como iniciar el proyecto:

Atajo: clic derecho en la solución > **Configure Startup Projects** > *Multiple startup projects* > `Usuarios.Services` y `Usuarios.Web` en **Start** > **F5**. La web abre en `/Usuario/Crear`. El servicio puede mostrar un 403 en su raíz, es normal vive en `/Usuarios.svc`.



# EMPRESA:
ENTITY DATA