# KeyStock - Sistema de Gestión de Inventario

KeyStock es una aplicación web robusta diseñada para el control y administración de inventarios. Permite gestionar productos, realizar seguimiento de movimientos (entradas y salidas), administrar proveedores y controlar el acceso de usuarios mediante roles.

El proyecto está construido con una arquitectura desacoplada, utilizando **ASP.NET Core** para la API RESTful y **React** para la interfaz de usuario, integrados para un despliegue unificado.

##  Demo en Vivo

Puedes ver la aplicación desplegada y funcionando en el siguiente enlace:

 **[http://www.keystock.somee.com/](http://www.keystock.somee.com/)**

---

## 🛠 Tecnologías Utilizadas

### Backend
* **Framework:** ASP.NET Core 8 Web API
* **Lenguaje:** C#
* **ORM:** Entity Framework Core (Code First)
* **Base de Datos:** SQL Server
* **Documentación API:** Swagger / OpenAPI

### Frontend
* **Librería:** React.js (v18)
* **Routing:** React Router DOM (v6)
* **Estilos:** Bootstrap 5 & CSS personalizado
* **HTTP Client:** Axios

### Despliegue
* **Hosting:** Somee.com
* **Estrategia:** Frontend estático servido a través del middleware `UseStaticFiles` de .NET (carpeta `wwwroot`).

---

##  Características Principales

* **Dashboard Interactivo:** Visualización de KPIs en tiempo real (Total de productos, movimientos del día, proveedores activos).
* **Gestión de Productos:** CRUD completo (Crear, Leer, Actualizar, Eliminar) con control de stock.
* **Bitácora de Movimientos:** Registro de entradas, salidas y ajustes de inventario que actualizan automáticamente el stock.
* **Gestión de Proveedores:** Administración de la base de datos de proveedores y contactos.
* **Seguridad y Usuarios:**
    * Sistema de Login y Autenticación.
    * Gestión de Usuarios y Roles.
    * Encriptación de contraseñas (Hashing SHA256).
* **Diseño Responsivo:** Interfaz adaptable a dispositivos móviles y escritorio.

---

##  Configuración e Instalación Local

Sigue estos pasos para ejecutar el proyecto en tu entorno local.

### Prerrequisitos
* .NET SDK 8.0 o superior.
* Node.js y npm.
* SQL Server (LocalDB o instancia completa).

### 1. Configuración del Backend (API)

1.  Clona el repositorio.
2.  Navega a la carpeta del proyecto de la API (ej. `SistemaInventario.Api`).
3.  Configura tu cadena de conexión en `appsettings.json`:
    ```json
    "ConnectionStrings": {
      "DefaultConnection": "Server=TU_SERVIDOR;Database=KeyStockDB;Trusted_Connection=True;TrustServerCertificate=True;"
    }
    ```
4.  Ejecuta las migraciones para crear la base de datos:
    ```bash
    dotnet ef database update
    ```
5.  Ejecuta la API:
    ```bash
    dotnet run
    ```

### 2. Configuración del Frontend (React)

1.  Navega a la carpeta del frontend (ej. `sistema-inventario-frontend`).
2.  Instala las dependencias:
    ```bash
    npm install
    ```
3.  Ejecuta el servidor de desarrollo:
    ```bash
    npm start
    ```
    *La aplicación abrirá en `http://localhost:3000`.*

---

##  Despliegue (Build)

Para desplegar la aplicación como una sola unidad (como se ve en Somee):

1.  Ejecuta el build de React:
    ```bash
    npm run build
    ```
2.  Copia el contenido de la carpeta `build` generada.
3.  Pega el contenido dentro de la carpeta `wwwroot` en el proyecto de ASP.NET Core.
4.  Publica la solución de .NET.

---

## Licencia

Este proyecto está bajo la Licencia MIT.

---

**Desarrollado por Fernando Molinares, Andrea Arce**
