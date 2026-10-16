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

Capas: Presentación (MAUI + consola) → Lógica (Services + calculadora pura) → Acceso a datos (repositorios con `using` + factory `BaseDeDatos.Abrir()`) → SQLite. La migración futura a SQL Server se concentra en `Abrir()` / `CadenaDeConexion` sin tocar el resto de las capas.

![Modelo de arquitectura](docs/diagrams/arquitectura.png)
