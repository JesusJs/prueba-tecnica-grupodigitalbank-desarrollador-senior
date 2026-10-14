using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Usuarios.Services.Contracts;

namespace Usuarios.Web.Gateways
{
    public interface IUsuarioGateway
    {
        int Agregar(UsuarioDto usuario);
        void Modificar(UsuarioDto usuario);
        IReadOnlyList<UsuarioDto> Consultar();
        UsuarioDto ConsultarPorId(int id);
        void Eliminar(int id);
    }
}