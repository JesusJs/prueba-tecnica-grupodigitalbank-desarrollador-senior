using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Usuarios.Services.Business;
using Usuarios.Services.BusinessException;
using Usuarios.Services.Contracts;
using Usuarios.Services.Data.Interfaces;

namespace Usuarios.Service.Tests.Business
{
    [TestFixture]
    public class UsuarioBusinessTests
    {
        private Mock<IUsuarioRepository> _repository;
        private Mock<IUsuarioValidator> _validator;
        private UsuarioBusiness _sut;

        [SetUp]
        public void SetUp()
        {
            _repository = new Mock<IUsuarioRepository>(MockBehavior.Strict);
            _validator = new Mock<IUsuarioValidator>();
            _sut = new UsuarioBusiness(_repository.Object, _validator.Object);
        }

        private static UsuarioDto UsuarioValido(int id = 0) => new UsuarioDto
        {
            Id = id,
            Nombre = "Jesús Chávez",
            FechaNacimiento = new DateTime(1995, 5, 10),
            Sexo = "M"
        };

        private void ValidadorRetornaValido() =>
            _validator.Setup(v => v.Validar(It.IsAny<UsuarioDto>())).Returns(new List<string>());

        private void ValidadorRetornaErrores(params string[] errores) =>
            _validator.Setup(v => v.Validar(It.IsAny<UsuarioDto>())).Returns(errores);

        // ---------- Constructor ----------

        [Test]
        public void Constructor_RepositorioNulo_LanzaArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new UsuarioBusiness(null, _validator.Object));
        }

        [Test]
        public void Constructor_ValidadorNulo_LanzaArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new UsuarioBusiness(_repository.Object, null));
        }

        // ---------- Agregar ----------

        [Test]
        public void Agregar_UsuarioValido_GuardaYRetornaIdGenerado()
        {
            ValidadorRetornaValido();
            var usuario = UsuarioValido();
            _repository.Setup(r => r.Agregar(usuario)).Returns(42);

            var id = _sut.Agregar(usuario);

            Assert.That(id, Is.EqualTo(42));
            _repository.Verify(r => r.Agregar(usuario), Times.Once());
        }

        [Test]
        public void Agregar_UsuarioInvalido_LanzaValidacionExceptionYNoGuarda()
        {
            ValidadorRetornaErrores("El nombre es obligatorio.");

            var ex = Assert.Throws<ValidacionException>(() => _sut.Agregar(UsuarioValido()));

            Assert.That(ex.Errores, Has.Count.EqualTo(1));
            _repository.Verify(r => r.Agregar(It.IsAny<UsuarioDto>()), Times.Never());
        }

        // ---------- Modificar ----------

        [Test]
        public void Modificar_UsuarioValidoExistente_Modifica()
        {
            ValidadorRetornaValido();
            var usuario = UsuarioValido(id: 7);
            _repository.Setup(r => r.Modificar(usuario)).Returns(true);

            Assert.DoesNotThrow(() => _sut.Modificar(usuario));

            _repository.Verify(r => r.Modificar(usuario), Times.Once());
        }

        [Test]
        public void Modificar_UsuarioInvalido_LanzaValidacionExceptionYNoModifica()
        {
            ValidadorRetornaErrores("El sexo debe ser 'M' o 'F'.");

            Assert.Throws<ValidacionException>(() => _sut.Modificar(UsuarioValido(id: 7)));

            _repository.Verify(r => r.Modificar(It.IsAny<UsuarioDto>()), Times.Never());
        }

        [TestCase(0)]
        [TestCase(-1)]
        public void Modificar_IdInvalido_LanzaValidacionExceptionSinTocarRepositorio(int id)
        {
            ValidadorRetornaValido();

            Assert.Throws<ValidacionException>(() => _sut.Modificar(UsuarioValido(id)));

            _repository.Verify(r => r.Modificar(It.IsAny<UsuarioDto>()), Times.Never());
        }

        [Test]
        public void Modificar_UsuarioNoExiste_LanzaUsuarioNoEncontradoException()
        {
            ValidadorRetornaValido();
            var usuario = UsuarioValido(id: 99);
            _repository.Setup(r => r.Modificar(usuario)).Returns(false);

            var ex = Assert.Throws<UsuarioNoEncontradoException>(() => _sut.Modificar(usuario));

            Assert.That(ex.Id, Is.EqualTo(99));
        }

        // ---------- Consultar ----------

        [Test]
        public void Consultar_RetornaTodosLosUsuarios()
        {
            var lista = new List<UsuarioDto> { UsuarioValido(1), UsuarioValido(2) };
            _repository.Setup(r => r.ObtenerTodos()).Returns(lista);

            var resultado = _sut.Consultar();

            Assert.That(resultado, Has.Count.EqualTo(2));
        }

        [Test]
        public void Consultar_SinUsuarios_RetornaListaVacia()
        {
            _repository.Setup(r => r.ObtenerTodos()).Returns(new List<UsuarioDto>());

            Assert.That(_sut.Consultar(), Is.Empty);
        }

        [Test]
        public void ConsultarPorId_Existente_RetornaUsuario()
        {
            var usuario = UsuarioValido(5);
            _repository.Setup(r => r.ObtenerPorId(5)).Returns(usuario);

            var resultado = _sut.ConsultarPorId(5);

            Assert.That(resultado, Is.SameAs(usuario));
        }

        [Test]
        public void ConsultarPorId_NoExiste_LanzaUsuarioNoEncontradoException()
        {
            _repository.Setup(r => r.ObtenerPorId(5)).Returns((UsuarioDto)null);

            Assert.Throws<UsuarioNoEncontradoException>(() => _sut.ConsultarPorId(5));
        }

        [TestCase(0)]
        [TestCase(-3)]
        public void ConsultarPorId_IdInvalido_LanzaValidacionExceptionSinTocarRepositorio(int id)
        {
            Assert.Throws<ValidacionException>(() => _sut.ConsultarPorId(id));

            _repository.Verify(r => r.ObtenerPorId(It.IsAny<int>()), Times.Never());
        }

        // ---------- Eliminar ----------

        [Test]
        public void Eliminar_Existente_Elimina()
        {
            _repository.Setup(r => r.Eliminar(3)).Returns(true);

            Assert.DoesNotThrow(() => _sut.Eliminar(3));

            _repository.Verify(r => r.Eliminar(3), Times.Once());
        }

        [Test]
        public void Eliminar_NoExiste_LanzaUsuarioNoEncontradoException()
        {
            _repository.Setup(r => r.Eliminar(3)).Returns(false);

            var ex = Assert.Throws<UsuarioNoEncontradoException>(() => _sut.Eliminar(3));

            Assert.That(ex.Id, Is.EqualTo(3));
        }

        [TestCase(0)]
        [TestCase(-1)]
        public void Eliminar_IdInvalido_LanzaValidacionExceptionSinTocarRepositorio(int id)
        {
            Assert.Throws<ValidacionException>(() => _sut.Eliminar(id));

            _repository.Verify(r => r.Eliminar(It.IsAny<int>()), Times.Never());
        }
    }
}
