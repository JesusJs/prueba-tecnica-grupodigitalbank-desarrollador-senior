using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Usuarios.Services.Contracts;
using Usuarios.Web.Models;

namespace Usuarios.Web.Controllers
{
    public class UsuarioController
    {
        private readonly IUsuarioGateway _gateway;

        // Lo usa MVC: compone la dependencia real.
        public UsuarioController() : this(new UsuarioGateway()) { }

        // Lo usan los tests.
        public UsuarioController(IUsuarioGateway gateway)
        {
            _gateway = gateway ?? throw new ArgumentNullException(nameof(gateway));
        }

        // ---------- Página "usuario" ----------

        [HttpGet]
        public ActionResult Crear() => View(new UsuarioViewModel());

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Crear(UsuarioViewModel modelo)
        {
            if (!ModelState.IsValid)
                return View(modelo);

            try
            {
                _gateway.Agregar(ADto(modelo));
                TempData["Mensaje"] = "Usuario creado correctamente.";
                return RedirectToAction("Consulta");
            }
            catch (UsuarioGatewayException ex)
            {
                AgregarErrores(ex);
                return View(modelo);
            }
        }

        // ---------- Página "usuario consulta" ----------

        [HttpGet]
        public ActionResult Consulta()
        {
            try
            {
                var usuarios = _gateway.Consultar().Select(AModelo).ToList();
                return View(usuarios);
            }
            catch (UsuarioGatewayException ex)
            {
                AgregarErrores(ex);
                return View(new List<UsuarioViewModel>());
            }
        }

        // ---------- Modificar ----------

        [HttpGet]
        public ActionResult Editar(int id)
        {
            try
            {
                return View("Crear", AModelo(_gateway.ConsultarPorId(id)));
            }
            catch (UsuarioGatewayException ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction("Consulta");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Editar(UsuarioViewModel modelo)
        {
            if (!ModelState.IsValid)
                return View("Crear", modelo);

            try
            {
                _gateway.Modificar(ADto(modelo));
                TempData["Mensaje"] = "Usuario modificado correctamente.";
                return RedirectToAction("Consulta");
            }
            catch (UsuarioGatewayException ex)
            {
                if (ex.Codigo == UsuarioGatewayException.NoEncontrado)
                {
                    TempData["Error"] = ex.Message;
                    return RedirectToAction("Consulta");
                }

                AgregarErrores(ex);
                return View("Crear", modelo);
            }
        }

        // ---------- Eliminar ----------

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Eliminar(int id)
        {
            try
            {
                _gateway.Eliminar(id);
                TempData["Mensaje"] = "Usuario eliminado correctamente.";
            }
            catch (UsuarioGatewayException ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction("Consulta");
        }

        // ---------- Helpers ----------

        private void AgregarErrores(UsuarioGatewayException ex)
        {
            if (ex.Errores.Count == 0)
                ModelState.AddModelError(string.Empty, ex.Message);

            foreach (var error in ex.Errores)
                ModelState.AddModelError(string.Empty, error);
        }

        private static UsuarioDto ADto(UsuarioViewModel modelo) => new UsuarioDto
        {
            Id = modelo.Id,
            Nombre = modelo.Nombre.Trim(),
            FechaNacimiento = modelo.FechaNacimiento.Value,
            Sexo = modelo.Sexo
        };

        private static UsuarioViewModel AModelo(UsuarioDto dto) => new UsuarioViewModel
        {
            Id = dto.Id,
            Nombre = dto.Nombre,
            FechaNacimiento = dto.FechaNacimiento,
            Sexo = dto.Sexo
        };
    }
}
}