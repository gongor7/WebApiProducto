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

## 📝 Notas Adicionales
*   **TargetFramework:** `net10.0`
*   **OpenAPI Nativa:** Este proyecto utiliza `Microsoft.AspNetCore.OpenApi` junto con `Swashbuckle` para la interfaz visual.
