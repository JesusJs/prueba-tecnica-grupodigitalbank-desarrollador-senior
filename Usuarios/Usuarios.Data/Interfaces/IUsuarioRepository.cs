using System.Collections.Generic;
using Usuarios.Data.Entities;

namespace Usuarios.Services.Data.Interfaces
{
    public interface IUsuarioRepository
    {
        int Agregar(UsuarioEntity usuario);
        bool Modificar(UsuarioEntity usuario);
        UsuarioEntity ObtenerPorId(int id); 
        IReadOnlyList<UsuarioEntity> ObtenerTodos();
        bool Eliminar(int id);
    }
}