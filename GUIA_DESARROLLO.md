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

## 7. Arquitectura de DTOs (Data Transfer Objects)
Para separar el modelo de la base de datos de los datos que viajan por la red.

### ¿Por qué usarlos?
*   **Seguridad**: El cliente solo envía y recibe lo que tú permites (evita ataques como *Mass Assignment*).
*   **Mantenimiento**: Puedes cambiar la base de datos sin afectar a los usuarios de la API.

### Implementación en el Controlador:
El controlador ahora actúa como un "traductor" entre el mundo exterior (DTOs) y el mundo interior (Modelos de Base de Datos).

#### 1. Recepción de Datos (`POST`/`PUT`)
El controlador recibe un `ProductoCreateDto`. No usamos el modelo `Producto` directamente para evitar que el usuario intente manipular campos internos como el `Id` o la `FechaDeAlta`.
```csharp
public async Task<ActionResult> Post([FromBody] ProductoCreateDto dto)
{
    // Mapeo manual: Creamos el modelo a partir del DTO
    var producto = new Producto { Nombre = dto.Nombre, Precio = dto.Precio ... };
    _context.Productos.Add(producto);
}
```

#### 2. Envío de Datos (`GET`)
Nunca devolvemos el modelo de la base de datos directamente. Lo convertimos a un `ProductoReadDto`.
```csharp
public async Task<ActionResult<IEnumerable<ProductoReadDto>>> Get()
{
    var productos = await _context.Productos.ToListAsync();
    // Transformación: Proyectamos cada producto a su versión DTO
    return Ok(productos.Select(p => new ProductoReadDto { ... }));
}
```

### ¿Cómo leer los errores de validación?
Al usar DTOs con `DataAnnotations` y el atributo `[ApiController]`, .NET genera automáticamente una respuesta **400 Bad Request** si los datos son inválidos. El formato sigue el estándar **RFC 7807 (Problem Details)**, que desglosa exactamente qué campo falló y por qué.

---

## 8. Paginado y Rendimiento
Para manejar grandes volúmenes de datos de forma eficiente sin saturar el servidor o el cliente.

### Conceptos Clave:
*   **`.Skip(n)`**: Salta los primeros `n` registros de la consulta.
*   **`.Take(m)`**: Toma solo los siguientes `m` registros.
*   **Query Parameters**: Usamos `pageNumber` y `pageSize` en la URL (ej: `api/productos?pageNumber=1&pageSize=10`).

### Paso a paso:
1.  **Crear `PagedResponse<T>`**: Un DTO genérico que envuelva los datos y añada metadatos (página actual, total de registros, total de páginas).
2.  **Implementar lógica en el Controlador**:
    *   Usa `[FromQuery]` para recibir los parámetros.
    *   Obtén el conteo total con `CountAsync()`.
    *   Aplica `OrderBy()`, `Skip()` y `Take()` antes del `ToListAsync()`.
3.  **Seguridad**: Limita siempre el `pageSize` máximo (ej: no permitir más de 50 registros por página) para evitar ataques que intenten descargar toda tu base de datos de una vez.

---

## 9. Relaciones entre Entidades (Relaciones 1:N)
Para conectar diferentes modelos (ej: un `Producto` pertenece a una `Categoria`).

### Paso a paso:
1.  **Crear el modelo padre**: Define una clase con una `ICollection` de los modelos hijos.
    ```csharp
    public class Categoria {
        public int Id { get; set; }
        public ICollection<Producto> Productos { get; set; } = new List<Producto>();
    }
    ```
2.  **Crear el modelo hijo con Foreign Key**: En el modelo hijo, añade el ID del padre y la propiedad de navegación.
    ```csharp
    public int CategoriaId { get; set; }
    public Categoria? Categoria { get; set; }
    ```
3.  **Actualizar DbContext**: Añade el `DbSet` del nuevo modelo.
4.  **Uso de `.Include()`**: En los métodos `GET` del controlador, usa `.Include(p => p.Categoria)` para traer la información relacionada de la base de datos (Eager Loading).
5.  **Migraciones**: Cada vez que añadas una relación, debes generar una nueva migración:
    ```bash
    dotnet ef migrations add NombreRelacion
    dotnet ef database update
    ```
    *Nota: Si ya tienes datos, asegúrate de limpiarlos o asignar categorías válidas antes de aplicar la restricción de llave foránea.*

---

## 10. Servicios y Lógica de Negocio (Service Pattern)
Para mantener el controlador limpio, movemos toda la lógica de datos y negocio a clases de servicio.

### Implementación de Filtrado (Patrón Queryable)
Para implementar filtros eficientes sin sobrecargar la base de datos, usamos `IQueryable`:

1.  **Conversión a Queryable**: Al usar `_context.Productos.AsQueryable()`, estamos creando una "consulta pendiente". Nada se envía a la base de datos todavía.
2.  **Encadenamiento dinámico**: Si el usuario proporciona un filtro, añadimos una condición `Where()` a la consulta existente.
    ```csharp
    var query = _context.Productos.AsQueryable();
    if (!string.IsNullOrWhiteSpace(nombre)) {
        query = query.Where(p => p.Nombre.Contains(nombre));
    }
    ```
3.  **Ejecución**: La consulta real (SQL) se construye y ejecuta en Supabase solo cuando llamamos a `ToListAsync()`. Esto significa que si el usuario pidió filtrar por nombre, **la base de datos solo devolverá los productos que coinciden**, lo cual es extremadamente rápido y eficiente.

*Nota: Siempre aplica los filtros ANTES del `.Skip()` y `.Take()` para asegurarte de paginar sobre los datos ya filtrados.*

### Paso a paso para crear un nuevo servicio:
1.  **Crear la Interface (`Services/I[Entidad]Service.cs`)**: Define los métodos necesarios (`GetAsync`, `CreateAsync`, etc.).
2.  **Crear la Implementación (`Services/[Entidad]Service.cs`)**: Sigue los puntos explicados arriba.
3.  **Registrar en `Program.cs`**:
    ```csharp
    builder.Services.AddScoped<IProductoService, ProductoService>();
    ```
4.  **Inyectar en el Controlador**:
    Sustituye el acceso directo al `DbContext` por tu servicio.

*Nota: Es obligatorio crear tanto la Interface como la Clase de implementación antes de intentar registrarlas en `Program.cs`.*

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
