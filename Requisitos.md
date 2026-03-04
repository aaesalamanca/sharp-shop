# PRUEBA TÉCNICA 2026 – NET 8+

Un cliente de Capitole le solicita un pequeño servicio para gestionar los pedidos de su nueva tienda online. La moneda por defecto es el Euro, pero las solicitudes deben dar la opción de cambiar la moneda.

Se pide la creación de una Web API en NET 8 / 10 (en C#) con los siguientes endpoints:

- Crear un pedido
- Añadir productos al pedido
- Eliminar productos del pedido
- Confirmar un pedido
- Enviar un pedido
- Consultar un pedido por Id
- Obtener todos los productos y sus precios.
- Agregar productos al stock (opcional, pero te resultará cómodo tenerlo)

Reglas de negocio:

1. Un pedido solo puede confirmarse si tiene al menos un producto.
2. El total del pedido se calcular como:
  a. Precio unitario * cantidad
  b. Si el total supera 200 €, aplicar un 10% de descuento (no 200 USD).
3. Un pedido confirmado o enviado no puede modificarse.
4. Los precios de los productos están almacenados en euros.
5. Para obtener el cambio en otras monedas, usa [https://www.frankfurter.app](https://www.frankfurter.app) (HTTP GET [https://api.frankfurter.app/latest?from=EUR&to=USD](https://api.frankfurter.app/latest?from=EUR&to=USD)).

Consejos:

- El negocio puede ser el que tu elijas (tienda deportiva, moda, amazon, etc.).
- Ten un catálogo de varios productos.
- Se valorará una arquitectura de capas, aunque si no conoces ninguna hazla como mejor sepas.
- Se valorará la aplicación de principios SOLID y DDD.
- Si sabes hacer unit test, haz al menos uno. Se recomienda el patrón AAA.
- Se valorará las buenas prácticas actuales, organización del código, el nombre de las variables, métodos, clases, etc., limpieza y especialmente la separación de responsabilidades.
- Para la persistencia puedes usar ADO o Entity Framework (recomendado). Como bases de datos, siéntete libre, pero debes proporcionar las instrucciones y/o scripts para generar la base de datos. Tal vez SQLite, resulte lo más cómodo.
- Se da libertad de diseño de como tienen que ser esos endpoints, pero se valora la coherencia y proactividad.
- La solución debe estar en un repositorio. Se valorará la estrategia de commits empleada.
- Asegúrate de que la api carga un swagger para facilitar la interacción con la api. Si tienes problemas, puedes preparar una colección de Postman.
