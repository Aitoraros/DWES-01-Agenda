# 📱 Gestión de una Agenda de Contactos

## 📋 Descripción

Desarrollar una aplicación para la **gestión de una agenda de contactos**, utilizando una arquitectura por capas y aplicando buenas prácticas de diseño, persistencia, testing y gestión de errores.

La aplicación se ejecutará desde **terminal**, sin necesidad de implementar ningún menú ni interfaz de usuario.

> ⚠️ **Importante:** el proyecto **no será una API REST**.

---

## 🏗️ Arquitectura

La aplicación deberá estar organizada siguiendo una **arquitectura por capas**, separando correctamente las responsabilidades de cada componente.

Como mínimo, deberá contar con las siguientes capas o componentes:

* 📦 **Model** → Representación de las entidades del dominio.
* 🗄️ **Repository** → Acceso y persistencia de los datos.
* ⚙️ **Service** → Gestión de la lógica de negocio.
* 🧠 **Cache** → Gestión de la caché LRU.
* 🛠️ **Utils** → Utilidades y componentes comunes.

La estructura deberá favorecer la **separación de responsabilidades**, facilitando el mantenimiento, testing y evolución del proyecto.

---

## 👤 Modelo `Contacto`

Se deberá crear un modelo `Contacto` que represente los contactos almacenados en la agenda.

El modelo deberá contener, como mínimo:

* 🆔 `Id`
* 👤 `Nombre`
* 📞 `Teléfono`
* 📧 `email`
* 🔖 `Alias`

Los campos podrán ampliarse si se considera necesario.

---

## 🗃️ Persistencia con SQLite

Los datos de los contactos deberán almacenarse utilizando **SQLite**.

El acceso a la base de datos deberá realizarse desde la capa correspondiente, evitando que la lógica de negocio acceda directamente a SQLite.

Se deberán implementar las operaciones necesarias para:

* ➕ Crear contactos.
* 🔎 Consultar contactos.
* 🆔 Buscar un contacto por ID.
* 🔖 Buscar un contacto por alias.
* ✏️ Actualizar contactos.
* 🗑️ Eliminar contactos.

---

## 🔎 Funcionalidades

### 📄 Buscar contactos

Se deberá implementar una operación para obtener los contactos almacenados.

La búsqueda deberá utilizar **paginación**, permitiendo especificar:

* 🔢 Número de página.
* 📏 Tamaño de página.

Por ejemplo:

```text
Página: 2
Tamaño: 10
```

La operación deberá devolver únicamente los contactos correspondientes a la página solicitada.

---

### 🆔 Buscar por ID

Se deberá poder obtener un contacto utilizando su identificador único.

Ejemplo:

```text
buscarPorId(15)
```

Si el contacto no existe, deberá gestionarse correctamente la situación.

---

### 🔖 Buscar por alias

Se deberá poder buscar un contacto utilizando su alias.

Ejemplo:

```text
buscarPorAlias("Robertito")
```

La búsqueda deberá estar gestionada desde la capa de servicio y utilizar el repositorio correspondiente para acceder a los datos.

---

### ➕ Crear contacto

Se deberá implementar la creación de nuevos contactos.

La aplicación deberá validar los datos necesarios antes de almacenarlos y gestionar correctamente posibles errores o conflictos.

---

### ✏️ Actualizar contacto

Se deberá poder modificar un contacto existente.

La aplicación deberá comprobar que el contacto existe antes de realizar la actualización.

---

### 🗑️ Borrar contacto

Se deberá poder eliminar un contacto mediante su identificador.

Si el contacto no existe, la operación deberá devolver el resultado correspondiente.

---

## 🧠 Caché LRU

La aplicación deberá implementar una **caché LRU (Least Recently Used)**.

La caché tendrá un tamaño máximo y almacenará determinados resultados de las consultas realizadas.

Cuando la caché alcance su capacidad máxima, deberá eliminarse automáticamente el elemento que lleve más tiempo sin utilizarse.

El objetivo será reducir consultas innecesarias a SQLite y mejorar el rendimiento de determinadas operaciones.

Por ejemplo, las búsquedas por ID podrán utilizar la caché:

```text
buscarPorId(15)
       │
       ▼
¿Está en caché?
   │         │
  Sí         No
   │         │
   ▼         ▼
Resultado  SQLite
             │
             ▼
      Guardar en caché
```

También deberá tenerse en cuenta la **invalidación de la caché** cuando se creen, actualicen o eliminen contactos.

---

## 🚦 Gestión de errores

La aplicación deberá disponer de un sistema para representar y gestionar correctamente los resultados de las operaciones.

Se utilizarán códigos de resultado para identificar diferentes situaciones:

|  Código  | Significado                       |
| :------: | --------------------------------- |
| 🟢 `200` | Operación realizada correctamente |
| 🟢 `201` | Contacto creado correctamente     |
| 🟡 `400` | Datos de entrada incorrectos      |
| 🟠 `404` | Contacto no encontrado            |
| 🔴 `500` | Error interno o de acceso a datos |

> ⚠️ Estos códigos se utilizarán únicamente como **códigos de resultado de la aplicación**. No representan respuestas HTTP.

Por lo tanto, **no se deberá implementar una API REST**.

Los errores deberán gestionarse correctamente, evitando que excepciones o errores de la base de datos lleguen directamente hasta la aplicación sin tratar.

---

## 📝 Logging

La aplicación deberá utilizar un sistema de **logging** para registrar información relevante durante su ejecución.

Se deberán utilizar diferentes niveles de log cuando sea necesario:

* 🐛 `DEBUG`
* ℹ️ `INFO`
* ⚠️ `WARNING`
* ❌ `ERROR`

Ejemplo:

```text
[INFO] Buscando contacto con ID: 15
[INFO] Contacto encontrado correctamente
[WARNING] Contacto con ID 25 no encontrado
[INFO] Contacto creado correctamente
[ERROR] Error al acceder a la base de datos
```

El logging deberá utilizarse para facilitar la **depuración, monitorización y seguimiento de la aplicación**.

---

## 🧪 Testing

El proyecto deberá estar correctamente **testeado mediante tests automatizados**.

Se deberán realizar pruebas, como mínimo, sobre:

* ➕ Creación de contactos.
* 🔎 Búsqueda de contactos.
* 📄 Búsqueda paginada.
* 🆔 Búsqueda por ID.
* 🔖 Búsqueda por alias.
* ✏️ Actualización de contactos.
* 🗑️ Eliminación de contactos.
* ❓ Contactos inexistentes.
* ✅ Validación de datos.
* 🚦 Códigos de resultado.
* 🧠 Funcionamiento de la caché LRU.
* 🔄 Invalidación de la caché.
* 💥 Casos de error relacionados con la persistencia.

Los tests deberán cubrir tanto los **casos correctos** como los **casos de error**.

---

## 💻 Ejecución

La aplicación deberá poder ejecutarse desde **terminal**.

No será necesario implementar:

* ❌ Menús.
* ❌ Interfaces gráficas.
* ❌ API REST.

La lógica de la aplicación deberá poder ser utilizada mediante los componentes y servicios correspondientes.

---

## 🔒 Restricciones

El proyecto deberá cumplir las siguientes condiciones:

* 💻 Aplicación ejecutada desde terminal.
* 🚫 Sin menú de usuario.
* 🚫 Sin interfaz gráfica.
* 🚫 Sin API REST.
* 🗃️ Persistencia mediante SQLite.
* 🏗️ Arquitectura por capas.
* 🧠 Implementación de una caché LRU.
* 📝 Sistema de logging.
* 🚦 Códigos de resultado.
* 🧪 Tests automatizados.

---

## 🚀 Posibles mejoras

Una vez completados todos los requisitos, se podrán plantear diferentes ampliaciones.

### 🐳 Docker

Como posible mejora, se podrá **dockerizar la aplicación**, creando la configuración necesaria para poder ejecutar el proyecto mediante Docker.

### 💡 Otras mejoras

También se podrían implementar:

* 🌐 Convertir posteriormente la aplicación en una API REST.
* 🖥️ Añadir una interfaz gráfica.
* 🔎 Incorporar nuevos filtros de búsqueda.
* 📥 Añadir importación y exportación de contactos.
* 🗄️ Sustituir SQLite por otro sistema gestor de bases de datos.
* 📊 Añadir métricas sobre el uso de la caché.
* ✅ Mejorar el sistema de validación.
* 🧠 Añadir nuevas estrategias de caché.

---

## 🎯 Objetivos

El objetivo principal del ejercicio es desarrollar una aplicación sencilla pero estructurada que permita poner en práctica:

* 🏗️ Arquitectura por capas.
* 🧱 Programación orientada a objetos.
* 📦 Patrón Repository.
* ⚙️ Lógica de negocio mediante Services.
* 🗃️ Persistencia con SQLite.
* 🔄 Operaciones CRUD.
* 📄 Paginación.
* 🧠 Caché LRU.
* 🚦 Gestión de errores.
* 🔢 Códigos de resultado.
* 📝 Logging.
* 🧪 Testing automatizado.
* ✨ Buenas prácticas de organización y diseño del código.
