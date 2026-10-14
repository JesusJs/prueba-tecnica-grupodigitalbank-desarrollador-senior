using System;

namespace Usuarios.Data.Data.Entities
{
    public class UsuarioEntity
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public DateTime FechaNacimiento { get; set; }
        public string Sexo { get; set; }
    }
}
