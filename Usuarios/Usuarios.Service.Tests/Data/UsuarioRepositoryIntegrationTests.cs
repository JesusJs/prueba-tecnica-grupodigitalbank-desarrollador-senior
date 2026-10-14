using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Usuarios.Services.Business.Mapper;
using Usuarios.Services.Contracts;
using Usuarios.Services.Data;

namespace Usuarios.Service.Tests.Data
{
    [TestFixture]
    [Category("Integration")]
    public class UsuarioRepositoryIntegrationTests
    {
        // Ajusta a tu instancia local o define la variable de entorno USUARIOS_TEST_DB.
        private static readonly string ConnectionString =
            Environment.GetEnvironmentVariable("USUARIOS_TEST_DB")
            ?? @"Server=(localdb)\MSSQLLocalDB;Database=UsuariosDB;Integrated Security=true;";

        private UsuarioRepository _sut;
        private int _idCreado;

        [SetUp]
        public void SetUp() => _sut = new UsuarioRepository(ConnectionString);

        [TearDown]
        public void TearDown()
        {
            // Limpieza: no dejamos basura en la BD.
            if (_idCreado > 0)
                _sut.Eliminar(_idCreado);
            _idCreado = 0;
        }

        private static UsuarioDto NuevoUsuario() => new UsuarioDto
        {
            Nombre = "Test Integración " + Guid.NewGuid().ToString("N").Substring(0, 8),
            FechaNacimiento = new DateTime(1990, 1, 15),
            Sexo = "F"
        };

        [Test]
        public void Constructor_ConexionVacia_LanzaArgumentException()
        {
            Assert.Throws<ArgumentException>(() => new UsuarioRepository(""));
        }

        [Test]
        public void Agregar_UsuarioValido_RetornaIdMayorACero()
        {
            _idCreado = _sut.Agregar(NuevoUsuario().ToEntity());

            Assert.That(_idCreado, Is.GreaterThan(0));
        }

        [Test]
        public void ObtenerPorId_Existente_RetornaLosDatosGuardados()
        {
            var original = NuevoUsuario();
            _idCreado = _sut.Agregar(original.ToEntity());

            var leido = _sut.ObtenerPorId(_idCreado);

            Assert.That(leido, Is.Not.Null);
            Assert.That(leido.Nombre, Is.EqualTo(original.Nombre));
            Assert.That(leido.FechaNacimiento, Is.EqualTo(original.FechaNacimiento));
            Assert.That(leido.Sexo, Is.EqualTo("F"));
        }

        [Test]
        public void ObtenerPorId_NoExiste_RetornaNull()
        {
            Assert.That(_sut.ObtenerPorId(int.MaxValue), Is.Null);
        }

        [Test]
        public void ObtenerTodos_IncluyeElUsuarioCreado()
        {
            _idCreado = _sut.Agregar(NuevoUsuario().ToEntity());

            var todos = _sut.ObtenerTodos();

            Assert.That(todos.Any(u => u.Id == _idCreado), Is.True);
        }

        [Test]
        public void Modificar_Existente_PersisteLosCambios()
        {
            _idCreado = _sut.Agregar(NuevoUsuario().ToEntity());
            var cambio = NuevoUsuario();
            cambio.Id = _idCreado;
            cambio.Sexo = "M";

            var resultado = _sut.Modificar(cambio.ToEntity());

            Assert.That(resultado, Is.True);
            Assert.That(_sut.ObtenerPorId(_idCreado).Sexo, Is.EqualTo("M"));
        }

        [Test]
        public void Modificar_NoExiste_RetornaFalse()
        {
            var usuario = NuevoUsuario();
            usuario.Id = int.MaxValue;

            Assert.That(_sut.Modificar(usuario.ToEntity()), Is.False);
        }

        [Test]
        public void Eliminar_Existente_RetornaTrueYYaNoExiste()
        {
            var id = _sut.Agregar(NuevoUsuario().ToEntity());

            Assert.That(_sut.Eliminar(id), Is.True);
            Assert.That(_sut.ObtenerPorId(id), Is.Null);
        }

        [Test]
        public void Eliminar_NoExiste_RetornaFalse()
        {
            Assert.That(_sut.Eliminar(int.MaxValue), Is.False);
        }
    }
}
