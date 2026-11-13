using System;
using System.Collections.Generic;
using Gestor_de_Estudiantes.Views;

namespace Gestor_de_Estudiantes
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private async void OnVerEstudiantesClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync(nameof(EstudiantesPage));
        }

        private async void OnVerComisionesClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync(nameof(ComisionesPage));
        }

        private async void OnCalendarioClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync(nameof(Gestor_de_Estudiantes.Views.CalendarioPage));
        }

        private async void OnAsistenciaEntregasClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync(nameof(Gestor_de_Estudiantes.Views.AsistenciaEntregasPage));
        }

        private async void OnRiesgoClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync(nameof(RiesgoPage));
        }

        private async void OnConfiguracionRiesgoClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync(nameof(ConfiguracionRiesgoPage));
        }

        private async void OnInicioClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//MainPage");
        }

        private void OnGenerarReporteClicked(object sender, EventArgs e)
        {
            try
            {
                // Llamamos al método que exporta los 500 estudiantes de la base de datos a Excel
                GeneradorReporte.ExportarEstudiantesDesdeBDACSV();

                DisplayAlert("¡Éxito!", "Se generó el reporte masivo de los 500 estudiantes y se abrió en Excel.", "OK");
            }
            catch (Exception ex)
            {
                DisplayAlert("Error", $"No se pudo generar el reporte: {ex.Message}", "OK");
            }
        }
    }
}