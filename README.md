	# Grupo 3

## Integrantes

- Elias Villegas
- Yesica Quezada
- Isaias Romero
- Ibañez Mario

## Descripción

Seguimiento de cursada y alerta temprana.
Un panel para que quien sigue una comisión sepa a qué estudiantes contactar esta semana, antes de que abandonen.

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
