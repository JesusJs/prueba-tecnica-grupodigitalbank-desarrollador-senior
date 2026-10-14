using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Usuarios.Services.Business.Interfaces;
using Usuarios.Services.Contracts;
using Usuarios.Services.Data.Interfaces;

namespace Usuarios.Services.Business
{
    public class UsuarioBusiness : IUsuarioBusiness
    {
        private readonly IUsuarioRepository _repository;
        private readonly IUsuarioValidator _validator;

        public UsuarioBusiness(IUsuarioRepository repository, IUsuarioValidator validator)
        {
            _repository = repository;
            _validator = validator;
        }

        public int Agregar(UsuarioDto usuario) => throw new NotImplementedException();
        public void Modificar(UsuarioDto usuario) => throw new NotImplementedException();
        public UsuarioDto ConsultarPorId(int id) => throw new NotImplementedException();
        public IReadOnlyList<UsuarioDto> Consultar() => throw new NotImplementedException();
        public void Eliminar(int id) => throw new NotImplementedException();
    }
}