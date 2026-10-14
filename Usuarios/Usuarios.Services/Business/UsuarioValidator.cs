using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Usuarios.Services.Contracts;

namespace Usuarios.Services.Business
{
    public class UsuarioValidator : IUsuarioValidator
    {
        private const int LongitudMaximaNombre = 100;

        public IReadOnlyList<string> Validar(UsuarioDto usuario)
        {
            var errores = new List<string>();

            if (usuario == null)
            {
                errores.Add("El usuario es obligatorio.");
                return errores;
            }

            if (string.IsNullOrWhiteSpace(usuario.Nombre))
                errores.Add("El nombre es obligatorio.");
            else if (usuario.Nombre.Length > LongitudMaximaNombre)
                errores.Add($"El nombre no puede superar los {LongitudMaximaNombre} caracteres.");

            if (usuario.FechaNacimiento == default(DateTime))
                errores.Add("La fecha de nacimiento es obligatoria.");
            else if (usuario.FechaNacimiento.Date > DateTime.Today)
                errores.Add("La fecha de nacimiento no puede ser futura.");

            if (usuario.Sexo != "M" && usuario.Sexo != "F")
                errores.Add("El sexo debe ser 'M' o 'F'.");

            return errores;
        }
    }
}