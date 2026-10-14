using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Usuarios.Services.Contracts;

namespace Usuarios.Service.Tests.Controllers
{
    [TestFixture]
    public class UsuarioControllerTests
    {
        private Mock<IUsuarioGateway> _gateway;
        private UsuarioController _sut;

        [SetUp]
        public void SetUp()
        {
            _gateway = new Mock<IUsuarioGateway>(MockBehavior.Strict);
            _sut = new UsuarioController(_gateway.Object) { TempData = new TempDataDictionary() };
        }

        private static UsuarioViewModel Modelo(int id = 0) => new UsuarioViewModel
        {
            Id = id,
            Nombre = "Ana",
            FechaNacimiento = new DateTime(2000, 1, 2),
            Sexo = "F"
        };

        private static UsuarioDto Dto(int id = 1) => new UsuarioDto
        {
            Id = id,
            Nombre = "Ana",
            FechaNacimiento = new DateTime(2000, 1, 2),
            Sexo = "F"
        };

        private static UsuarioGatewayException Error(string codigo, params string[] errores) =>
            new UsuarioGatewayException(codigo, "Mensaje del servicio.", errores);

        private static void AssertRedirigeAConsulta(ActionResult resultado)
        {
            var redireccion = resultado as RedirectToRouteResult;
            Assert.That(redireccion, Is.Not.Null);
            Assert.That(redireccion.RouteValues["action"], Is.EqualTo("Consulta"));
        }

        [Test]
        public void Constructor_GatewayNulo_LanzaArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new UsuarioController(null));
        }

        // ---------- Crear ----------

        [Test]
        public void Crear_Get_RetornaVistaConModeloVacio()
        {
            var vista = _sut.Crear() as ViewResult;

            Assert.That(vista, Is.Not.Null);
            Assert.That(vista.Model, Is.TypeOf<UsuarioViewModel>());
        }

        [Test]
        public void Crear_Post_Valido_AgregaYRedirigeAConsulta()
        {
            _gateway.Setup(g => g.Agregar(It.Is<UsuarioDto>(d => d.Nombre == "Ana" && d.Sexo == "F")))
                    .Returns(1);

            var resultado = _sut.Crear(Modelo());

            AssertRedirigeAConsulta(resultado);
            _gateway.Verify(g => g.Agregar(It.IsAny<UsuarioDto>()), Times.Once());
        }

        [Test]
        public void Crear_Post_ModeloInvalido_NoLlamaAlServicio()
        {
            _sut.ModelState.AddModelError("Nombre", "obligatorio");

            var vista = _sut.Crear(Modelo()) as ViewResult;

            Assert.That(vista, Is.Not.Null);
            _gateway.Verify(g => g.Agregar(It.IsAny<UsuarioDto>()), Times.Never());
        }

        [Test]
        public void Crear_Post_ValidacionDelServicio_MuestraTodosLosErrores()
        {
            _gateway.Setup(g => g.Agregar(It.IsAny<UsuarioDto>()))
                    .Throws(Error(UsuarioGatewayException.Validacion, "Error uno.", "Error dos."));

            var vista = _sut.Crear(Modelo()) as ViewResult;

            Assert.That(vista, Is.Not.Null);
            Assert.That(_sut.ModelState.Values.SelectMany(v => v.Errors).Count(), Is.EqualTo(2));
        }

        // ---------- Consulta ----------

        [Test]
        public void Consulta_RetornaVistaConLosUsuarios()
        {
            _gateway.Setup(g => g.Consultar()).Returns(new List<UsuarioDto> { Dto(1), Dto(2) });

            var vista = _sut.Consulta() as ViewResult;

            Assert.That(vista, Is.Not.Null);
            Assert.That(((IEnumerable<UsuarioViewModel>)vista.Model).Count(), Is.EqualTo(2));
        }

        [Test]
        public void Consulta_ErrorDelServicio_RetornaListaVaciaConError()
        {
            _gateway.Setup(g => g.Consultar()).Throws(Error("ERROR_INTERNO"));

            var vista = _sut.Consulta() as ViewResult;

            Assert.That(vista, Is.Not.Null);
            Assert.That(((IEnumerable<UsuarioViewModel>)vista.Model), Is.Empty);
            Assert.That(_sut.ModelState.IsValid, Is.False);
        }

        // ---------- Editar ----------

        [Test]
        public void Editar_Get_Existente_RetornaFormularioConDatos()
        {
            _gateway.Setup(g => g.ConsultarPorId(5)).Returns(Dto(5));

            var vista = _sut.Editar(5) as ViewResult;

            Assert.That(vista, Is.Not.Null);
            Assert.That(vista.ViewName, Is.EqualTo("Crear"));
            Assert.That(((UsuarioViewModel)vista.Model).Id, Is.EqualTo(5));
        }

        [Test]
        public void Editar_Get_NoExiste_RedirigeAConsultaConError()
        {
            _gateway.Setup(g => g.ConsultarPorId(5)).Throws(Error(UsuarioGatewayException.NoEncontrado));

            var resultado = _sut.Editar(5);

            AssertRedirigeAConsulta(resultado);
            Assert.That(_sut.TempData["Error"], Is.Not.Null);
        }

        [Test]
        public void Editar_Post_Valido_ModificaYRedirigeAConsulta()
        {
            _gateway.Setup(g => g.Modificar(It.Is<UsuarioDto>(d => d.Id == 5)));

            var resultado = _sut.Editar(Modelo(5));

            AssertRedirigeAConsulta(resultado);
            _gateway.Verify(g => g.Modificar(It.IsAny<UsuarioDto>()), Times.Once());
        }

        [Test]
        public void Editar_Post_ModeloInvalido_NoLlamaAlServicio()
        {
            _sut.ModelState.AddModelError("Nombre", "obligatorio");

            var vista = _sut.Editar(Modelo(5)) as ViewResult;

            Assert.That(vista, Is.Not.Null);
            Assert.That(vista.ViewName, Is.EqualTo("Crear"));
            _gateway.Verify(g => g.Modificar(It.IsAny<UsuarioDto>()), Times.Never());
        }

        [Test]
        public void Editar_Post_NoExiste_RedirigeAConsultaConError()
        {
            _gateway.Setup(g => g.Modificar(It.IsAny<UsuarioDto>()))
                    .Throws(Error(UsuarioGatewayException.NoEncontrado));

            var resultado = _sut.Editar(Modelo(5));

            AssertRedirigeAConsulta(resultado);
            Assert.That(_sut.TempData["Error"], Is.Not.Null);
        }

        // ---------- Eliminar ----------

        [Test]
        public void Eliminar_Existente_EliminaYRedirigeConMensaje()
        {
            _gateway.Setup(g => g.Eliminar(3));

            var resultado = _sut.Eliminar(3);

            AssertRedirigeAConsulta(resultado);
            _gateway.Verify(g => g.Eliminar(3), Times.Once());
            Assert.That(_sut.TempData["Mensaje"], Is.Not.Null);
        }

        [Test]
        public void Eliminar_NoExiste_RedirigeConError()
        {
            _gateway.Setup(g => g.Eliminar(3)).Throws(Error(UsuarioGatewayException.NoEncontrado));

            var resultado = _sut.Eliminar(3);

            AssertRedirigeAConsulta(resultado);
            Assert.That(_sut.TempData["Error"], Is.Not.Null);
        }
    }
}
}
