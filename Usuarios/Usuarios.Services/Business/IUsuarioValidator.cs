using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Usuarios.Services.Contracts;

namespace Usuarios.Services.Business
{
    public interface IUsuarioValidator
    {
        IReadOnlyList<string> Validar(UsuarioDto usuario);
    }
}