using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Usuarios.Services.Business.Interfaces;
using Usuarios.Services.Contracts;
using Usuarios.Services.Contracts.Interfaces;

namespace Usuarios.Services.Service
{
    public class UsuarioService : IUsuarioService
    {
        public UsuarioService(IUsuarioBusiness business) { }

        public int Agregar(UsuarioDto usuario) => throw new NotImplementedException();
        public void Modificar(UsuarioDto usuario) => throw new NotImplementedException();
        public List<UsuarioDto> Consultar() => throw new NotImplementedException();
        public UsuarioDto ConsultarPorId(int id) => throw new NotImplementedException();
        public void Eliminar(int id) => throw new NotImplementedException();
    }
}