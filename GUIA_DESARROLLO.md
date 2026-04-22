# 🛠 Guía de Desarrollo del Proyecto

Este archivo sirve como referencia rápida para la configuración y mantenimiento de este proyecto Web API de .NET.

---

## 🔍 Configuración de Swagger (UI)

Para habilitar la interfaz visual de Swagger en proyectos .NET 9/10 que usan la nueva librería OpenAPI:

### 1. Instalar el Paquete NuGet
Ejecuta el siguiente comando en la terminal:
```bash
dotnet add package Swashbuckle.AspNetCore
```

### 2. Configurar `Program.cs`
Añade los servicios y el middleware correspondientes:

```csharp
// 1. Agregar el servicio generador de Swagger
builder.Services.AddSwaggerGen();

var app = builder.Build();

// 2. Habilitar Swagger solo en entorno de Desarrollo
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();   // Genera el archivo JSON de la API
    app.UseSwaggerUI(); // Genera la interfaz visual (UI)
}
```

### 3. Configurar el Inicio Automático
En el archivo `Properties/launchSettings.json`, modifica cada perfil (`http` y `https`) con estas propiedades:
```json
"launchBrowser": true,
"launchUrl": "swagger",
```

---

## 📂 Control de Versiones (Git)

### Configuración de `.gitignore`
Se ha añadido un archivo `.gitignore` estándar para proyectos .NET para evitar subir archivos innecesarios al repositorio:
*   `bin/` y `obj/` (Binarios y archivos temporales de compilación).
*   `.vs/` y `.vscode/` (Configuraciones de IDE).
*   `appsettings.Development.json` (Configuraciones locales).

---

## 🛠 CRUD en Memoria (Productos)

Se ha implementado un controlador `ProductosController` que simula una base de datos utilizando una **lista estática**. Esto permite probar la API sin necesidad de configurar una base de datos real.

### 1. El Controlador (`Controllers/ProductosController.cs`)
El controlador utiliza los verbos HTTP estándar para realizar operaciones sobre la lista de productos:

*   **GET `/api/productos`**: Devuelve la lista completa de productos.
*   **GET `/api/productos/{id}`**: Busca un producto específico por su ID.
*   **POST `/api/productos`**: Añade un nuevo producto a la lista (devuelve `"creado"`).
*   **PUT `/api/productos/{id}`**: Actualiza los datos de un producto existente (devuelve `"actualizado"`).
*   **DELETE `/api/productos/{id}`**: Elimina un producto de la lista (devuelve `"borrado"`).

### 2. Conceptos Clave
*   **`private static List<Producto> _productos`**: Al ser una variable `static`, la información se mantiene mientras la aplicación esté ejecutándose (se pierde al reiniciar).
*   **`[FromBody]`**: Indica que los datos del producto vienen en el cuerpo de la petición JSON.
*   **`ActionResult<T>`**: Permite devolver tanto los datos como códigos de estado HTTP (200 OK, 404 Not Found, etc.).

---

## 📝 Notas Adicionales
*   **TargetFramework:** `net10.0`
*   **OpenAPI Nativa:** Este proyecto utiliza `Microsoft.AspNetCore.OpenApi` junto con `Swashbuckle` para la interfaz visual.
