using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Usuarios.Services.Contracts;

namespace Usuarios.Services.Data.Interfaces
{
    public interface IUsuarioRepository
    {
        int Agregar(UsuarioDto usuario);
        bool Modificar(UsuarioDto usuario);
        UsuarioDto ObtenerPorId(int id); 
        IReadOnlyList<UsuarioDto> ObtenerTodos();
        bool Eliminar(int id);
    }
}