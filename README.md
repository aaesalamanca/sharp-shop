# SharpShop API

SharpShop es una API REST para la gestión de pedidos de una librería. Permite crear pedidos, gestionar el inventario de libros y actualizar el estado de los pedidos.

## Requisitos previos

- [.NET 10 SDK](https://dotnet.microsoft.com/download) o superior.

## Ejecutar la API

Para iniciar la aplicación, navega al directorio del proyecto WebApi y ejecuta:

```bash
cd src/SharpShop.WebApi
dotnet run
```

La API estará disponible en `http://localhost:5018`.

## Acceder a Swagger UI

Swagger UI proporciona una interfaz visual para explorar y probar los endpoints de la API. Una vez que la aplicación esté en ejecución, accede a:

```text
http://localhost:5018/swagger
```

Desde esta interfaz puedes ver todos los endpoints disponibles, sus parámetros, modelos de respuesta y probar las llamadas directamente desde el navegador.

## Probar la API con el archivo .http

El proyecto incluye un archivo `SharpShop.WebApi.http` con ejemplos de solicitudes HTTP preconfiguradas. Para utilizar este archivo:

1. Abre el archivo `src/SharpShop.WebApi/SharpShop.WebApi.http` en Visual Studio o VS Code.
2. Asegúrate de que la API esté ejecutándose en `http://localhost:5018`.
3. Haz clic en los enlaces "Send Request" que aparecen sobre cada bloque de código para ejecutar las solicitudes.

El archivo incluye ejemplos para:

- Obtener todos los libros (`GET /api/books`).
- Crear un nuevo pedido (`POST /api/orders`).
- Obtener un pedido por su ID (`GET /api/orders/{orderId}`).
- Agregar libros a un pedido (`POST /api/orders/{orderId}/books`).
- Eliminar libros de un pedido (`DELETE /api/orders/{orderId}/books/{bookId}`).
- Actualizar el estado del pedido (`PATCH /api/orders/{orderId}`).
- Agregar nuevos libros al inventario (`POST /api/books`).
- Casos de error para probar validación.

### Conversión de moneda

Puedes especificar la moneda para los precios utilizando el parámetro `currency`. Por ejemplo:

```text
GET http://localhost:5018/api/books?currency=USD
GET http://localhost:5018/api/orders/{orderId}?currency=GBP
```

## Base de datos SQLite

La aplicación utiliza SQLite como base de datos. El archivo de la base de datos se crea automáticamente en `src/SharpShop.WebApi/sharpshop.db` sin necesidad de ejecutar ningún script adicional.

La generación automática de la base de datos ocurre en el método `Main` de `Program.cs`:

- En entorno de desarrollo: se ejecuta el método `SeedAsync` que borra la base de datos si ya existía previamente y la crea de nuevo insertando datos de ejemplo (5 libros de programación).
- En otros entornos: se ejecuta `EnsureCreatedAsync` que crea las tablas sin datos iniciales.

La cadena de conexión está configurada en `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=sharpshop.db"
  }
}
```

No es necesario ejecutar migraciones ni scripts SQL. La primera vez que ejecutes la aplicación, se creará automáticamente el archivo `sharpshop.db` con el esquema y los datos iniciales.

## Ejecutar los tests unitarios

El proyecto de tests se encuentra en `test/SharpShop.UnitTests` y utiliza xUnit como framework de testing. Para ejecutar los tests, utiliza el siguiente comando desde la raíz del proyecto:

```bash
dotnet test
```

Este comando ejecutará todos los tests unitarios del proyecto y mostrará los resultados en la consola.

## Estructura del proyecto

El proyecto está organizado en las siguientes capas:

- **SharpShop.Domain**: Entidades y excepciones del dominio.
- **SharpShop.Application**: Servicios y DTOs de aplicación.
- **SharpShop.Infrastructure**: Configuración de base de datos y acceso a datos.
- **SharpShop.WebApi**: API REST y endpoints.
- **SharpShop.UnitTests**: Tests unitarios.
