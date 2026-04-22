# 📘 Manual de Configuración: Web API .NET

Este manual explica detalladamente cómo configurar las funcionalidades esenciales de una Web API profesional. Úsalo como guía paso a paso para tus proyectos.

---

## 1. Configuración de Documentación (Swagger)
Swagger permite visualizar y probar los endpoints de tu API de forma interactiva.

### Paso a paso:
1.  **Instalar el paquete**:
    ```bash
    dotnet add package Swashbuckle.AspNetCore
    ```
2.  **Registrar el servicio** en `Program.cs`:
    ```csharp
    builder.Services.AddSwaggerGen();
    ```
3.  **Habilitar la interfaz** en el pipeline (dentro de `if (app.Environment.IsDevelopment())`):
    ```csharp
    app.UseSwagger();
    app.UseSwaggerUI();
    ```
4.  **Auto-inicio**: En `Properties/launchSettings.json`, busca los perfiles "http" y "https" y añade:
    ```json
    "launchBrowser": true,
    "launchUrl": "swagger"
    ```

---

## 2. Estructura de Datos (Modelos y Controladores)
Define cómo se ven tus datos y cómo se accede a ellos.

### Paso a paso:
1.  **Crear el Modelo**: Crea una clase en la carpeta `Models/` (ej: `Producto.cs`). Define sus propiedades (`Id`, `Nombre`, `Precio`).
2.  **Crear el Controlador**: Crea una clase en `Controllers/` que herede de `ControllerBase` y tenga el atributo `[ApiController]`.
3.  **CRUD en Memoria**: Para pruebas rápidas sin base de datos, declara una lista estática dentro del controlador:
    ```csharp
    private static List<Producto> _productos = new List<Producto>();
    ```

---

## 3. Conexión a Base de Datos (PostgreSQL + EF Core)
Uso de Entity Framework Core para persistencia real de datos.

### Paso a paso:
1.  **Instalar drivers de PostgreSQL**:
    ```bash
    dotnet add package Npgsql.EntityFrameworkCore.PostgreSQL
    dotnet add package Microsoft.EntityFrameworkCore.Design
    ```
2.  **Crear el Contexto (`DbContext`)**: Crea una carpeta `Data/` y una clase `ApplicationDbContext.cs`. Debe heredar de `DbContext` y contener el `DbSet<TuModelo>`.
3.  **Configurar en `Program.cs`**:
    ```csharp
    builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseNpgsql(cadenaDeConexion));
    ```

---

## 4. Seguridad con Variables de Entorno (.env)
Protege tus contraseñas y secretos para que no se filtren en repositorios públicos.

### Paso a paso:
1.  **Instalar DotNetEnv**:
    ```bash
    dotnet add package DotNetEnv
    ```
2.  **Crear el archivo `.env`**: En la raíz del proyecto, crea un archivo llamado `.env` y guarda tus secretos:
    ```env
    DB_CONNECTION_STRING="Host=servidor;Database=db;Username=user;Password=pass"
    ```
3.  **Ignorar en Git**: Abre `.gitignore` y añade una línea con `.env` al final.
4.  **Cargar variables**: En la primera línea de `Program.cs`, añade:
    ```csharp
    DotNetEnv.Env.Load();
    var connectionString = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING");
    ```

---

## 5. Gestión de Tablas (Migraciones con EF Core)
Las migraciones son el mecanismo para sincronizar tus clases de C# con las tablas físicas de SQL en Supabase.

### Paso a paso:

1.  **Instalar la herramienta CLI de Entity Framework**:
    Es necesario instalar esta herramienta de forma global en tu máquina (solo se hace una vez):
    ```bash
    dotnet tool install --global dotnet-ef
    ```

2.  **Crear una Migración**:
    Cada vez que cambies algo en tu modelo (añadir un campo, crear una tabla), genera un nuevo archivo de migración:
    ```bash
    dotnet ef migrations add InitialCreate -o Data/Migrations
    ```
    *   `InitialCreate`: Es el nombre de la migración.
    *   `-o Data/Migrations`: Indica que los archivos se guarden en esa carpeta.

3.  **Aplicar cambios a la Base de Datos**:
    Este comando toma las migraciones pendientes y ejecuta el código SQL en tu servidor remoto de Supabase:
    ```bash
    dotnet ef database update
    ```

---

## 6. Validaciones de Datos (Data Annotations)
Para asegurar que la información recibida es correcta antes de guardarla en la base de datos.

### Paso a paso:
1.  **Añadir Atributos al Modelo**: En tu clase del modelo (ej: `Producto.cs`), usa los siguientes atributos sobre las propiedades:
    *   `[Required]`: El campo no puede estar vacío.
    *   `[StringLength(max, MinimumLength = min)]`: Limita los caracteres.
    *   `[Range(min, max)]`: Valida valores numéricos.
    *   `[EmailAddress]`, `[Phone]`, etc.: Para formatos específicos.
2.  **Mensajes Personalizados**: Puedes añadir `ErrorMessage = "Tu mensaje"` dentro de los atributos.
3.  **Actualizar DB**: Como estas anotaciones cambian la estructura de las tablas (ej: de `text` a `varchar(100)`), siempre debes crear una migración después:
    ```bash
    dotnet ef migrations add AddValidation
    dotnet ef database update
    ```

---

## 🛠 Solución de Problemas Comunes
*   **Archivo bloqueado al compilar**: Si recibes un error diciendo que no se puede acceder a `WebApiProducto.exe`, es porque la aplicación se está ejecutando. Debes detenerla (cerrar la terminal de ejecución o el proceso) antes de aplicar migraciones.
*   **Error de conexión**: Asegúrate de que el archivo `.env` tenga la contraseña correcta de Supabase y que no tenga espacios innecesarios.

---

## 📂 Glosario de Archivos Importantes
*   **`Program.cs`**: El corazón de la app donde se configuran todos los servicios.
*   **`appsettings.json`**: Configuraciones generales (no sensibles).
*   **`.gitignore`**: Lista de archivos que Git debe ignorar (como `obj/`, `bin/` y `.env`).
*   **`.env`**: Archivo local secreto con tus contraseñas de base de datos.
