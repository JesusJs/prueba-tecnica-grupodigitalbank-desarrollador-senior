using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Usuarios.Services.BusinessException
{
    public class ValidacionException : Exception
    {
        public IReadOnlyList<string> Errores { get; }

        public ValidacionException(IReadOnlyList<string> errores)
            : base("Los datos del usuario no son válidos.")
        {
            Errores = errores;
        }
    }
}