using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Usuarios.Services.BusinessException
{
    public class UsuarioNoEncontradoException : Exception
    {
        public int Id { get; }

        public UsuarioNoEncontradoException(int id)
            : base($"No existe un usuario con Id {id}.")
        {
            Id = id;
        }
    }
}