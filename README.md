# OWASP Top 10 Demo (.NET / C#) + Mini Frontend

Proyecto de ejemplo ASP.NET Core Web API para **probar los riesgos OWASP Top 10 (2021)**
con código **inseguro vs más seguro**, más un **mini frontend** en HTML/JS que consume
la API y muestra el resultado visualmente.

> ⚠️ Este código contiene endpoints INSEGUROS a propósito, solo para laboratorio.

---

## 1. Requisitos

- SDK **.NET 8** instalado.
- Visual Studio 2022+ (o `dotnet` CLI).
- Navegador moderno (Chrome/Edge/Firefox).

---

## 2. Cómo ejecutar el backend

### Opción A: Visual Studio

1. Abre `OwaspTop10Demo.csproj` en Visual Studio.
2. Selecciona el perfil `OwaspTop10Demo`.
3. Ejecuta con **F5**.
4. Se abrirá Swagger en una URL tipo `https://localhost:7243/swagger`.

### Opción B: `dotnet` CLI

```bash
cd OwaspTop10Demo
dotnet restore
dotnet run
```

---

## 3. Mini frontend (wwwroot/index.html)

El proyecto incluye un frontend estático en:

- `wwwroot/index.html`

Cuando la API está levantada, puedes abrir:

- `https://localhost:7243/index.html` (o el puerto HTTPS que indique la consola)

Desde esa página podrás:

- Jugar con la **SQL Injection real** (A03) contra una BD SQLite local.
- Ver cómo se guardan contraseñas en modo inseguro vs seguro (A02).
- Ver el impacto de un control de acceso roto (A01).

Todo con llamadas `fetch` a la API.

---

## 4. Laboratorio de SQL Injection con BD real (SQLite)

Al arrancar la API, se crea el archivo:

- `SqlInjectionDemo.db` (en el directorio de trabajo)

Con la tabla y datos:

```sql
CREATE TABLE Users (
  Id INTEGER PRIMARY KEY AUTOINCREMENT,
  UserName TEXT NOT NULL,
  Password TEXT NOT NULL
);

-- Datos de ejemplo:
-- alice / Password123
-- bob   / Secret456
-- admin / AdminPass!
```

### Endpoints clave

- `GET /api/injection/insecure-db?username=alice&password=Password123`
- `GET /api/injection/secure-db?username=alice&password=Password123`

En el frontend (`index.html`) hay una pestaña **A03 – SQL Injection** que llama a
estos endpoints, te enseña la **consulta ejecutada** y las filas devueltas.

Prueba a usar como usuario:

```text
' OR 1=1 --
```

y cualquier valor en `password` para ver cómo la versión insegura devuelve todas las
filas, mientras la segura no.

---

## 5. Otros riesgos visibles desde el frontend

### A02 – Crypto (contraseñas)

- Endpoints: `/api/crypto/insecure-register` y `/api/crypto/secure-register`
- El frontend te deja registrar un usuario y ver el JSON de respuesta.
  - Inseguro: la propiedad `PasswordHash` contiene la contraseña en claro.
  - Seguro: `PasswordHash` es un hash PBKDF2 con salt.

### A01 – Broken Access Control

- Endpoints: `/api/accesscontrol/insecure/{id}` y `/api/accesscontrol/secure/{id}?currentUserId=`
- En la pestaña A01 del frontend puedes simular el “usuario actual” y pedir datos de otro id.
  - Versión insegura: siempre devuelve el usuario pedido.
  - Versión segura: devuelve 403 si `currentUserId != id`.

---

## 6. Resto de endpoints en la API

Aunque el frontend solo cubre 3 riesgos, la API incluye ejemplos para casi todo
el OWASP Top 10:

- A03 – Injection → `InjectionController`
- A04 – Insecure Design → `InsecureDesignController`
- A05 – Security Misconfiguration → CORS en `Program.cs`
- A06 – Componentes desactualizados → `OwaspTop10Demo.csproj` (Newtonsoft.Json 9.0.1)
- A07 – Auth & Identification Failures → `AuthController`
- A08 – Integrity Failures → `IntegrityController`
- A09 – Logging & Monitoring Failures → `LoggingController`
- A10 – SSRF → `SsrfController`

Puedes explorarlos desde Swagger.

---

Disfruta el laboratorio y úsalo para demos internas, formaciones y katas de seguridad. 🔐
