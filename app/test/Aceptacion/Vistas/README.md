# Pruebas de aceptación de vistas

La suite valida los flujos que las páginas MAUI exponen a través de sus ViewModels y servicios reales. Se ejecuta sobre `net9.0` con MSTest y SQLite temporal, por lo que no requiere un emulador.

## Cobertura

| Vista | Flujo aceptado | Prueba |
|---|---|---|
| `MainPage` | Acceso al espacio de trabajo | Inventario de navegación |
| `ComisionesPage` | Listar y seleccionar comisión | `VistasComisionesAcceptanceTests` |
| `AgregarComisionPage` | Crear comisión | `VistasComisionesAcceptanceTests` |
| `ComisionDetallePage` | Consultar comisión seleccionada | `VistasComisionesAcceptanceTests` |
| `AgregarEstudiantePage` | Alta y validación de estudiante | `VistasComisionesAcceptanceTests` |
| `EstudiantesPage` | Listar estudiantes activos | `VistasComisionesAcceptanceTests` |
| `CalendarioPage` | Consultar calendario y eventos | `VistasCursadaAcceptanceTests` |
| `AsistenciaEntregasPage` | Registrar asistencia y entregas | `VistasCursadaAcceptanceTests` |
| `MateriasPage` | Gestionar materias | `VistasCursadaAcceptanceTests` |
| `ConfiguracionRiesgoPage` | Guardar umbral de riesgo | `VistasRiesgoAcceptanceTests` |
| `RiesgoPage` | Mostrar riesgo, motivo y contacto | `VistasRiesgoAcceptanceTests` |
| `FichaEstudiantePage` | Mostrar ficha y contacto | `VistasFichaAcceptanceTests` |
| `HistorialAsistenciaPage` | Mostrar historial de asistencia | `VistasFichaAcceptanceTests` |
| `HistorialEntregasPage` | Mostrar historial de entregas | `VistasFichaAcceptanceTests` |

## Alcance técnico

Estas son pruebas de aceptación de la capa de presentación: instancian los ViewModels y ejecutan los comandos/servicios que usa cada vista. Para automatizar clics y controles visuales directamente en Windows/Android sería necesario incorporar Appium, WinAppDriver o .NET MAUI UI Tests, además de un entorno de ejecución de la aplicación.
