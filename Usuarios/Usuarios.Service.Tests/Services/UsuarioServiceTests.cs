using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using Usuarios.Services.Business.Interfaces;
using Usuarios.Services.BusinessException;
using Usuarios.Services.Contracts;
using Usuarios.Services.Service;

namespace Usuarios.Service.Tests.Services
{
    [TestFixture]
    public class UsuarioServiceTests
    {
        private Mock<IUsuarioBusiness> _business;
        private UsuarioService _sut;

        [SetUp]
        public void SetUp()
        {
            _business = new Mock<IUsuarioBusiness>(MockBehavior.Strict);
            _sut = new UsuarioService(_business.Object);
        }

        private static UsuarioDto Usuario(int id = 1) => new UsuarioDto
        {
            Id = id,
            Nombre = "Jesús Chávez",
            FechaNacimiento = new DateTime(1995, 5, 10),
            Sexo = "M"
        };

        [Test]
        public void Constructor_BusinessNulo_LanzaArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new UsuarioService(null));
        }

        // ---------- Agregar ----------

        [Test]
        public void Agregar_Valido_RetornaIdDelNegocio()
        {
            var usuario = Usuario(0);
            _business.Setup(b => b.Agregar(usuario)).Returns(10);

            Assert.That(_sut.Agregar(usuario), Is.EqualTo(10));
        }

        [Test]
        public void Agregar_ValidacionException_SeTraduceAFaultConErrores()
        {
            _business.Setup(b => b.Agregar(It.IsAny<UsuarioDto>()))
                     .Throws(new ValidacionException(new[] { "El nombre es obligatorio.", "El sexo debe ser 'M' o 'F'." }));

            var ex = Assert.Throws<FaultException<UsuarioFault>>(() => _sut.Agregar(Usuario(0)));

            Assert.That(ex.Detail.Codigo, Is.EqualTo(CodigosFault.Validacion));
            Assert.That(ex.Detail.Errores, Has.Count.EqualTo(2));
        }

        // ---------- Modificar ----------

        [Test]
        public void Modificar_Valido_DelegaAlNegocio()
        {
            var usuario = Usuario(5);
            _business.Setup(b => b.Modificar(usuario));

            Assert.DoesNotThrow(() => _sut.Modificar(usuario));

            _business.Verify(b => b.Modificar(usuario), Times.Once());
        }

        [Test]
        public void Modificar_NoExiste_SeTraduceAFaultNoEncontrado()
        {
            _business.Setup(b => b.Modificar(It.IsAny<UsuarioDto>()))
                     .Throws(new UsuarioNoEncontradoException(99));

            var ex = Assert.Throws<FaultException<UsuarioFault>>(() => _sut.Modificar(Usuario(99)));

            Assert.That(ex.Detail.Codigo, Is.EqualTo(CodigosFault.NoEncontrado));
        }

        // ---------- Consultar ----------

        [Test]
        public void Consultar_RetornaTodosLosUsuarios()
        {
            _business.Setup(b => b.Consultar()).Returns(new List<UsuarioDto> { Usuario(1), Usuario(2) });

            Assert.That(_sut.Consultar(), Has.Count.EqualTo(2));
        }

        [Test]
        public void ConsultarPorId_Existente_RetornaUsuario()
        {
            var usuario = Usuario(4);
            _business.Setup(b => b.ConsultarPorId(4)).Returns(usuario);

            Assert.That(_sut.ConsultarPorId(4), Is.SameAs(usuario));
        }

        [Test]
        public void ConsultarPorId_NoExiste_SeTraduceAFaultNoEncontrado()
        {
            _business.Setup(b => b.ConsultarPorId(4)).Throws(new UsuarioNoEncontradoException(4));

            var ex = Assert.Throws<FaultException<UsuarioFault>>(() => _sut.ConsultarPorId(4));

            Assert.That(ex.Detail.Codigo, Is.EqualTo(CodigosFault.NoEncontrado));
        }

        // ---------- Eliminar ----------

        [Test]
        public void Eliminar_Existente_DelegaAlNegocio()
        {
            _business.Setup(b => b.Eliminar(3));

            Assert.DoesNotThrow(() => _sut.Eliminar(3));

            _business.Verify(b => b.Eliminar(3), Times.Once());
        }

        [Test]
        public void Eliminar_NoExiste_SeTraduceAFaultNoEncontrado()
        {
            _business.Setup(b => b.Eliminar(3)).Throws(new UsuarioNoEncontradoException(3));

            var ex = Assert.Throws<FaultException<UsuarioFault>>(() => _sut.Eliminar(3));

            Assert.That(ex.Detail.Codigo, Is.EqualTo(CodigosFault.NoEncontrado));
        }

        // ---------- Errores inesperados ----------

        [Test]
        public void ErrorInesperado_SeTraduceAFaultGenericoSinFiltrarDetalles()
        {
            _business.Setup(b => b.Consultar())
                     .Throws(new InvalidOperationException("Server=secreto;Password=1234"));

            var ex = Assert.Throws<FaultException<UsuarioFault>>(() => _sut.Consultar());

            Assert.That(ex.Detail.Codigo, Is.EqualTo(CodigosFault.ErrorInterno));
            Assert.That(ex.Detail.Mensaje, Does.Not.Contain("secreto"));
            Assert.That(ex.Reason.ToString(), Does.Not.Contain("secreto"));
        }
    }
}
