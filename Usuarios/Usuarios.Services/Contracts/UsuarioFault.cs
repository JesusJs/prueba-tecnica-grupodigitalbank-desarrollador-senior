using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Usuarios.Services.Contracts
{
    [DataContract]
    public class UsuarioFault
    {
        [DataMember] public string Codigo { get; set; }
        [DataMember] public string Mensaje { get; set; }
        [DataMember] public List<string> Errores { get; set; } = new List<string>();
    }

    public static class CodigosFault
    {
        public const string Validacion = "VALIDACION";
        public const string NoEncontrado = "NO_ENCONTRADO";
        public const string ErrorInterno = "ERROR_INTERNO";
    }
}