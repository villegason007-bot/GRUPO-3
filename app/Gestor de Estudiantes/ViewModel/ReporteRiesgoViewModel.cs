using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Gestor_de_Estudiantes.Entidades;
using Gestor_de_Estudiantes.Services;

namespace Gestor_de_Estudiantes.ViewModel;

public class ReporteRiesgoViewModel : INotifyPropertyChanged
{
    private readonly IComisionService _comisionService;
    private readonly IReporteRiesgoService _reporteRiesgoService;

    private Comision? _comisionSeleccionada;
    private ReporteRiesgoComision? _reporteActual;

    public ObservableCollection<Comision> Comisiones { get; } = new();

    public Comision? ComisionSeleccionada
    {
        get => _comisionSeleccionada;
        set
        {
            if (_comisionSeleccionada != value)
            {
                _comisionSeleccionada = value;
                OnPropertyChanged();
                CargarReporte();
            }
        }
    }

    public ReporteRiesgoComision? ReporteActual
    {
        get => _reporteActual;
        set
        {
            _reporteActual = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(TieneReporte));
        }
    }

    public bool TieneReporte => ReporteActual is not null;

    public ReporteRiesgoViewModel(
        IComisionService comisionService,
        IReporteRiesgoService reporteRiesgoService)
    {
        _comisionService = comisionService;
        _reporteRiesgoService = reporteRiesgoService;
    }

    public void Inicializar()
    {
        Comisiones.Clear();
        var lista = _comisionService.ListarComisiones();
        foreach (var comision in lista)
        {
            Comisiones.Add(comision);
        }

        // Selecciona la primera comisión por defecto si existe
        if (Comisiones.Count > 0)
        {
            ComisionSeleccionada = Comisiones[0];
        }
    }

    private void CargarReporte()
    {
        if (ComisionSeleccionada is null)
        {
            ReporteActual = null;
            return;
        }

        ReporteActual = _reporteRiesgoService.ObtenerReportePorComision(ComisionSeleccionada.Codigo);
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}
