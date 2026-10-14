using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Usuarios.Services.Business.Interfaces;
using Usuarios.Services.BusinessException;
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
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _validator = validator ?? throw new ArgumentNullException(nameof(validator));
        }

        public int Agregar(UsuarioDto usuario)
        {
            ValidarUsuario(usuario);
            return _repository.Agregar(usuario);
        }

        public void Modificar(UsuarioDto usuario)
        {
            var errores = new List<string>(_validator.Validar(usuario));

            if (usuario != null && usuario.Id <= 0)
                errores.Add("El Id del usuario no es válido.");

            if (errores.Count > 0)
                throw new ValidacionException(errores);

            if (!_repository.Modificar(usuario))
                throw new UsuarioNoEncontradoException(usuario.Id);
        }

        public UsuarioDto ConsultarPorId(int id)
        {
            ValidarId(id);

            var usuario = _repository.ObtenerPorId(id);
            if (usuario == null)
                throw new UsuarioNoEncontradoException(id);

            return usuario;
        }

        public IReadOnlyList<UsuarioDto> Consultar()
        {
            return _repository.ObtenerTodos();
        }

        public void Eliminar(int id)
        {
            ValidarId(id);

            if (!_repository.Eliminar(id))
                throw new UsuarioNoEncontradoException(id);
        }

        private void ValidarUsuario(UsuarioDto usuario)
        {
            var errores = _validator.Validar(usuario);
            if (errores.Count > 0)
                throw new ValidacionException(errores);
        }

        private static void ValidarId(int id)
        {
            if (id <= 0)
                throw new ValidacionException(new[] { "El Id del usuario no es válido." });
        }
    }
}