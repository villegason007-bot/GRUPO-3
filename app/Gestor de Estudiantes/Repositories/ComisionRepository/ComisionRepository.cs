using Gestor_de_Estudiantes.Config;
using Gestor_de_Estudiantes.Entidades;

namespace Gestor_de_Estudiantes.Repositories
{
    public class ComisionRepository : IComisionRepository
    {
        public List<Comision> ListarComisiones()
        {
            using var conexion = BaseDeDatos.Abrir();
            using var comando = conexion.CreateCommand();
            comando.CommandText = "SELECT id, codigo FROM comisiones ORDER BY codigo;";

            var comisiones = new List<Comision>();
            using var lector = comando.ExecuteReader();
            while (lector.Read())
                comisiones.Add(new Comision
                {
                    Id = Guid.Parse(lector.GetString(0)),
                    Codigo = lector.GetString(1),
                });
            return comisiones;
        }

        public void AgregarComision(Comision comision)
        {
            using var conexion = BaseDeDatos.Abrir();
            using var comando = conexion.CreateCommand();
            comando.CommandText = "INSERT INTO comisiones (id, codigo) VALUES (@id, @codigo);";
            comando.Parameters.AddWithValue("@id", comision.Id.ToString("D"));
            comando.Parameters.AddWithValue("@codigo", comision.Codigo);
            comando.ExecuteNonQuery();
        }

        public bool ExisteComision(string codigo)
        {
            using var conexion = BaseDeDatos.Abrir();
            using var comando = conexion.CreateCommand();
            comando.CommandText = "SELECT COUNT(*) FROM comisiones WHERE codigo = @codigo;";
            comando.Parameters.AddWithValue("@codigo", codigo);
            return Convert.ToInt64(comando.ExecuteScalar()) > 0;
        }
    }
}
