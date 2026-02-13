# AuthApi - Sistema de Autenticación y Gestión de Usuarios

Este proyecto es una API RESTful construida con **.NET 10** que implementa un sistema robusto de autenticación y autorización basado en **RBAC (Role-Based Access Control)**.

## 🔐 Modelo de Autenticación

El sistema utiliza **JWT (JSON Web Tokens)** para la autenticación y **Refresh Tokens** para mantener la sesión activa sin necesidad de re-login frecuente.

### Flujo de Autenticación

1.  **Login**: El usuario envía sus credenciales (`email`, `password`).
2.  **Respuesta**: Si son válidas, el servidor devuelve:
    - `accessToken`: Token JWT de corta duración (ej. 15-60 min). Se debe enviar en el header `Authorization: Bearer <token>` en cada petición.
    - `refreshToken`: Token opaco de larga duración (ej. 7 días). Se usa para obtener un nuevo `accessToken` cuando este expira.
3.  **Renovación (Refresh Token)**: Cuando el `accessToken` expira, el cliente envía el `refreshToken` y el `accessToken` expirado al endpoint de renovación para obtener un nuevo par de tokens.

---

## �️ Modelo de Autorización (RBAC)

El control de acceso se basa en Roles, Permisos y Módulos.

### Entidades Principales

- **User**: Usuario del sistema. Puede tener múltiples roles.
- **Role**: Conjunto de permisos (ej. "Admin", "User").
- **Module**: Agrupación lógica de funcionalidades (ej. "Configuración", "Gestión Clínica").
- **Route**: Recurso específico dentro de un módulo (ej. `/config/roles`).
- **Permission**: Acción específica (Ver, Crear, Editar, Eliminar).
- **RolePermission**: Asigna permisos a un Rol sobre una Ruta específica.

### Roles Predefinidos

- **Admin**: Tiene acceso total a todos los módulos y permisos.
- **User**: Tiene acceso limitado (por defecto al módulo "Gestión Clínica").

---

## � Endpoints y Requerimientos

### 1. Autenticación (`/api/Auth`)

| Endpoint         | Método | Descripción             | Auth Requerida | Body Requerido                         |
| :--------------- | :----- | :---------------------- | :------------- | :------------------------------------- |
| `/register`      | POST   | Registrar nuevo usuario | No             | `email`, `password`, `confirmPassword` |
| `/login`         | POST   | Iniciar sesión          | No             | `email`, `password`                    |
| `/refresh-token` | POST   | Renovar Access Token    | No             | `accessToken`, `refreshToken`          |

### 2. Usuarios (`/api/Users`)

| Endpoint | Método | Descripción               | Auth Requerida     | Params/Body |
| :------- | :----- | :------------------------ | :----------------- | :---------- |
| `/`      | GET    | Listar todos los usuarios | **Sí** (AdminOnly) | -           |
| `/me`    | GET    | Obtener perfil propio     | **Sí**             | -           |

### 3. Roles (`/api/Roles`)

| Endpoint | Método | Descripción            | Auth Requerida | Params/Body                       |
| :------- | :----- | :--------------------- | :------------- | :-------------------------------- |
| `/`      | GET    | Listar todos los roles | **Sí**         | -                                 |
| `/{id}`  | GET    | Obtener detalle de rol | **Sí**         | `id` (int)                        |
| `/`      | POST   | Crear nuevo rol        | **Sí**         | `name`, `description`, `isActive` |
| `/{id}`  | PATCH  | Actualizar rol         | **Sí**         | `id` (int), campos a actualizar   |

---

## 👤 Usuarios por Defecto (Seed Data)

Al iniciar la aplicación por primera vez, se crean automáticamente los siguientes usuarios para pruebas:

| Rol       | Email               | Contraseña  |
| :-------- | :------------------ | :---------- |
| **Admin** | `admin@authapi.com` | `Admin123!` |
| **User**  | `user@authapi.com`  | `User123!`  |

---

---

## 🚀 Ejecución del Proyecto

### 1. Requisitos Previos

- Tener instalado el **SDK de .NET 10**.

### 2. Clonar el Repositorio

```bash
git clone <url-del-repositorio>
cd AuthApi
```

### 3. Ejecutar el Proyecto

Para iniciar la aplicación, ejecuta los siguientes comandos en la raíz del proyecto.

1.  **Restaurar y Actualizar Base de Datos**:

    ```bash
    dotnet ef database update
    ```

2.  **Iniciar Servidor**:
    ```bash
    dotnet run
    ```

### 4. Acceder

Una vez iniciada, puedes acceder a:

- **API**: `http://localhost:5263`
- **Swagger UI**: [http://localhost:5263/swagger/index.html](http://localhost:5263/swagger/index.html)

---

## 🛠️ Tecnologías

- **Framework**: .NET 10
- **ORM**: Entity Framework Core
- **Base de Datos**: SQLite (Por defecto para desarrollo)
- **Documentación**: [Swagger UI](http://localhost:5263/swagger/index.html)
