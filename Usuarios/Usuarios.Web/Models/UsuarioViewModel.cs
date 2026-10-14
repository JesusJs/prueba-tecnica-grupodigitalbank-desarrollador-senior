using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Usuarios.Web.Models
{
    public class UsuarioViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(100, ErrorMessage = "El nombre no puede superar los 100 caracteres.")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "La fecha de nacimiento es obligatoria.")]
        [DataType(DataType.Date)]
        [Display(Name = "Fecha de nacimiento")]
        public DateTime? FechaNacimiento { get; set; }

        [Required(ErrorMessage = "Seleccione el sexo.")]
        public string Sexo { get; set; }

        public static IEnumerable<SelectListItem> OpcionesSexo => new[]
        {
            new SelectListItem { Value = "M", Text = "Masculino" },
            new SelectListItem { Value = "F", Text = "Femenino" }
        };
    }
}