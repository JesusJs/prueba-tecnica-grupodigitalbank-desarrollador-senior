using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Usuarios.Web.Gateways
{
    public class UsuarioGatewayException : Exception
    {
        public const string Validacion = "VALIDACION";
        public const string NoEncontrado = "NO_ENCONTRADO";

        public string Codigo { get; }
        public IReadOnlyList<string> Errores { get; }

        public UsuarioGatewayException(string codigo, string mensaje, IReadOnlyList<string> errores)
            : base(mensaje)
        {
            Codigo = codigo;
            Errores = errores ?? new List<string>();
        }
    }
}