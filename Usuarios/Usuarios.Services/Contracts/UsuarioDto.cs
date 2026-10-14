using System;
using System.Runtime.Serialization;

namespace Usuarios.Services.Contracts
{
        [DataContract]
        public class UsuarioDto
        {
            [DataMember] public int Id { get; set; }
            [DataMember] public string Nombre { get; set; }
            [DataMember] public DateTime FechaNacimiento { get; set; }
            [DataMember] public string Sexo { get; set; }   // "M" o "F"
        }
}