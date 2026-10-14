using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Usuarios.Services.Business.Interfaces
{
    public interface IUsuarioBusiness
    {
        int Agregar(UsuarioDto usuario);
        void Modificar(UsuarioDto usuario);
        UsuarioDto ConsultarPorId(int id);
        IReadOnlyList<UsuarioDto> Consultar();
        void Eliminar(int id);
    }
}
