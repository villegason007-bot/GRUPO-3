# Grupo 3 — Gestor de Estudiantes · Seguimiento de cursada y alerta temprana

## Integrantes

- Elias Villegas
- Yesica Quezada
- Isaias Romero
- Ibañez Mario

## Descripción

Seguimiento de cursada y alerta temprana.
Un panel para que quien sigue una comisión sepa a qué estudiantes contactar esta semana, antes de que abandonen.

## Stack tecnológico

<p align="center">
  <img src="docs/assets/csharp.webp" height="60" alt="C#" />
  <img src="docs/assets/MAUI.webp" height="60" alt=".NET MAUI" />
  <img src="docs/assets/SQLite.webp" height="60" alt="SQLite" />
</p>

- **C# / .NET 9** — lenguaje y runtime del sistema.
- **.NET MAUI** — aplicación multiplataforma (Android, iOS y Windows) con el panel de seguimiento.
- **SQLite** — base de datos embebida (`ppii.db`), sin servidor ni instalación aparte.

## Historias de usuario

| HU | Historia | Estado |
|---|---|---|
| H1 | Cargar y ver nómina de estudiantes | Implementada |
| H2 | Definir el calendario de clases y de trabajos | Implementada |
| H3 | Registrar la asistencia de una clase | Implementada |
| H4 | Corregir la asistencia de una fecha ya registrada | Implementada |
| H5 | Registrar la entrega o no entrega de un trabajo | Implementada |
| H6 | Calcular el riesgo de cada estudiante a partir de asistencia y entregas | Implementada |
| H7 | Configurar los umbrales del cálculo sin tocar el código | Implementada |
| H8 | Ver la comisión ordenada por riesgo, con el detalle que justifica cada valor | Implementada |
| H9 | Ver la ficha de un estudiante con su historial | Pendiente |
| H10 | Registrar un contacto con un estudiante y su resultado | Pendiente |
| H11 | Que el panel muestre a quién ya se contactó y cuándo | Pendiente |

## Casos de uso

Dos vistas del alcance según el momento del ciclo: primero cómo se cargan y configuran los datos, después cómo se consume la decisión.

### Registro y configuración de la cursada

Cubre la entrada de datos: nómina (H1, con la carga de estudiantes como extensión), calendario (H2), asistencia (H3, con la corrección de fechas ya registradas como extensión H4), entregas (H5) y umbrales del riesgo (H7).

![Casos de uso — Registro y configuración](docs/diagrams/casos-de-uso-registro.png)

### Panel de seguimiento y alerta temprana

Cubre la salida de decisión: el panel ordenado por riesgo con el detalle que justifica cada valor (H8, que **incluye** el cálculo H6), la ficha del estudiante con su historial (H9), el registro de contactos (H10) y la vista de quién ya fue contactado y cuándo (H11, que **incluye** la vista del panel).

![Casos de uso — Panel de seguimiento](docs/diagrams/casos-de-uso-panel.png)

## Modelo de dominio

El sistema está modelado sobre cinco entidades centrales: **comisiones** (con la cantidad de **semanas** del cuatrimestre), **estudiantes** (con baja lógica mediante `activo`), **clases**, **trabajos** y sus registros de **asistencia** y **entrega**.

Decisiones de modelado que responden a reglas de negocio:

- `Clase.estado` (`EstadoClase` = `Habil` | `NoHabil`): las clases feriadas o suspendidas **no reciben asistencia y se excluyen del cálculo de riesgo** (H2 + H6), de modo que el denominador de inasistencias siempre cuenta solo clases hábiles.
- `Estudiante.activo` (`SiNo`): la baja es **lógica**, no se borran filas; los listados y búsquedas filtran por `activo` para conservar el historial.
- `Asistencia.condicion` (`Presente` | `Ausente`) y `Entrega.entregado` (`SiNo`) son las **dos señales** que alimentan el indicador de riesgo (H6).

![Modelo de dominio](docs/diagrams/dominio.png)

## Modelo de arquitectura

El sistema a producir se organiza en tres capas:

- **Presentación**: App MAUI (páginas + ViewModels) para Android, iOS y Windows.
- **Lógica de aplicación**: siete servicios (`Estudiante`, `Comision`, `Clase`, `Asistencia`, `Trabajo`, `Entrega`, `Riesgo`) y `CalculadoraDeRiesgo`, una clase pura testeable sin base de datos.
- **Acceso a datos**: siete repositorios sobre `BaseDeDatos.Abrir()` (factory; cada operación abre y cierra su conexión con `using`, sin singleton) → SQLite (`ppii.db`, ids `TEXT`/Guid).

Los umbrales del indicador de riesgo viven en la tabla `parametros_riesgo`: **límites absolutos configurables desde la UI** (máx. faltas = 3 · máx. trabajos no entregados = 2) y un **umbral porcentual interno** (50). El panel muestra el detalle como fracción —por ejemplo `2/3` faltas— junto con el motivo del riesgo.

Evolución prevista: la migración a SQL Server se concentra en `CadenaDeConexion`/`Abrir()`; el próximo ciclo agrega el **refactor SOLID (SRP)** de las clases con responsabilidades múltiples y la **migración de las queries a Entity Framework Core**.

![Modelo de arquitectura](docs/diagrams/arquitectura.png)

## La aplicación

Capturas de la app en ejecución (Windows).

**Inicio** — espacio de trabajo y accesos directos:

![Inicio](docs/assets/captura_001.webp)

**Comisiones** — alta de comisiones y nómina de estudiantes (H1):

![Comisiones](docs/assets/captura_002.webp)

**Asistencia y entregas** — control por clase, presentes y ausentes (H3, H5):

![Asistencia y entregas](docs/assets/captura_003.webp)

**Indicador de riesgo** — panel ordenado con el motivo de cada valor y el detalle como fracción `faltas / límite` (H6, H8):

![Indicador de riesgo](docs/assets/captura_004.webp)

**Calendario** — clases y trabajos con su estado (H2):

![Calendario](docs/assets/captura_005.webp)

## Referencias

- [Tablero Trello del equipo](https://trello.com/invite/b/6a88d1f63d0b424a0d777285/ATTIe701e521716581338bbc8c541d8b5d388000D637/grupo3-ppii)
- Minutas: [M001](docs/minutas/M001.md) · [M002](docs/minutas/M002.md) · [M003](docs/minutas/M003.md) · [M004](docs/minutas/M004.md)
- Consigna del proyecto (cátedra): [trabajo-integrador/consigna.md](https://github.com/emigallo-edu/ifts-pp2/blob/main/trabajo-integrador/consigna.md)
