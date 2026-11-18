using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Diagnostics;
using Microsoft.Data.Sqlite;
using Gestor_de_Estudiantes.Config;

namespace Gestor_de_Estudiantes
{
    public static class GeneradorReporte
    {
        public static void ExportarEstudiantesDesdeBDACSV()
        {
            try
            {
                var culturaEs = new CultureInfo("es-AR");
                var reporteFilas = new List<(string Id, string Comision, string Nombre, string Dni, string Legajo, string RiesgoPorcentaje, string DiasYFechasFalto)>();

                using var conexion = BaseDeDatos.Abrir();

                // 1. Traemos todos los estudiantes con su comisión
                var estudiantes = new List<(string Id, string Comision, string Nombre, string Dni, string Legajo)>();
                using (var cmdEst = conexion.CreateCommand())
                {
                    cmdEst.CommandText = """
                        SELECT e.id, c.codigo, e.nombre, e.dni, e.legajo 
                        FROM estudiantes e 
                        JOIN comisiones c ON e.comision_id = c.id
                        ORDER BY c.codigo, e.nombre;
                        """;
                    using var reader = cmdEst.ExecuteReader();
                    while (reader.Read())
                    {
                        estudiantes.Add((
                            reader.GetString(0),
                            reader.GetString(1),
                            reader.GetString(2),
                            reader.IsDBNull(3) ? "S/D" : reader.GetString(3),
                            reader.GetString(4)
                        ));
                    }
                }

                // 2. Traemos todas las asistencias vinculadas a las fechas de clase
                var asistenciasPorEstudiante = new Dictionary<string, List<(DateTime Fecha, string Condicion)>>();
                using (var cmdAsis = conexion.CreateCommand())
                {
                    cmdAsis.CommandText = """
                        SELECT a.estudiante_id, cl.fecha, a.condicion
                        FROM asistencias a
                        JOIN clases cl ON a.clase_id = cl.id
                        WHERE cl.estado = 'Habil'
                        ORDER BY cl.fecha ASC;
                        """;
                    using var reader = cmdAsis.ExecuteReader();
                    while (reader.Read())
                    {
                        string estId = reader.GetString(0);
                        DateTime fecha = DateTime.Parse(reader.GetString(1));
                        string condicion = reader.GetString(2);

                        if (!asistenciasPorEstudiante.ContainsKey(estId))
                            asistenciasPorEstudiante[estId] = new List<(DateTime, string)>();

                        asistenciasPorEstudiante[estId].Add((fecha, condicion));
                    }
                }

                // 3. Calculamos el % de riesgo y formateamos los días y fechas de ausencia para cada alumno
                foreach (var est in estudiantes)
                {
                    double porcentajeRiesgo = 0;
                    string detalleFaltas = "Sin inasistencias";

                    if (asistenciasPorEstudiante.TryGetValue(est.Id, out var historial) && historial.Count > 0)
                    {
                        int totalClases = historial.Count;
                        var ausencias = historial.Where(h => h.Condicion == "Ausente").ToList();

                        porcentajeRiesgo = Math.Round(((double)ausencias.Count / totalClases) * 100, 1);

                        if (ausencias.Count > 0)
                        {
                            var textosFechas = ausencias.Select(a =>
                            {
                                string diaSemana = culturaEs.TextInfo.ToTitleCase(a.Fecha.ToString("dddd", culturaEs));
                                return $"{diaSemana} {a.Fecha:dd/MM/yyyy}";
                            });
                            detalleFaltas = string.Join(" | ", textosFechas);
                        }
                    }

                    reporteFilas.Add((
                        est.Id,
                        est.Comision,
                        est.Nombre,
                        est.Dni,
                        est.Legajo,
                        $"{porcentajeRiesgo:0.0}%",
                        detalleFaltas
                    ));
                }

                // 4. Exportamos ordenado a CSV separado por punto y coma para Excel
                string carpetaDocumentos = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                string rutaArchivo = Path.Combine(carpetaDocumentos, $"Reporte_Riesgo_Estudiantes_{DateTime.Now:yyyyMMdd_HHmmss}.csv");

                var csvContent = new StringBuilder();
                csvContent.AppendLine("ID;Comisión;Estudiante;DNI;Legajo;% de Riesgo;Días y Fechas de Inasistencia");

                foreach (var fila in reporteFilas)
                {
                    csvContent.AppendLine($"{fila.Id};{fila.Comision};{fila.Nombre};{fila.Dni};{fila.Legajo};{fila.RiesgoPorcentaje};{fila.DiasYFechasFalto}");
                }

                File.WriteAllText(rutaArchivo, csvContent.ToString(), new UTF8Encoding(true));

                Process.Start(new ProcessStartInfo(rutaArchivo) { UseShellExecute = true });
            }
            catch (Exception ex)
            {
                throw new Exception($"No se pudo crear el archivo: {ex.Message}");
            }
        }
    }
}