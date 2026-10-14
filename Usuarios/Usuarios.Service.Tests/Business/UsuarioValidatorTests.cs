using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Usuarios.Services.Business;
using Usuarios.Services.Contracts;

namespace Usuarios.Service.Tests.Business
{
    [TestFixture]
    public class UsuarioValidatorTests
    {
        private UsuarioValidator _sut;
        [SetUp]
        public void SetUp() => _sut = new UsuarioValidator();

        private static UsuarioDto UsuarioValido() => new UsuarioDto
        {
            Nombre = "Jesús Chávez",
            FechaNacimiento = new DateTime(1995, 5, 10),
            Sexo = "M"
        };

        [Test]
        public void Validar_UsuarioValido_NoRetornaErrores()
        {
            var errores = _sut.Validar(UsuarioValido());

            Assert.That(errores, Is.Empty);
        }

        [Test]
        public void Validar_UsuarioNulo_RetornaError()
        {
            var errores = _sut.Validar(null);

            Assert.That(errores, Has.Count.EqualTo(1));
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        public void Validar_NombreVacio_RetornaError(string nombre)
        {
            var usuario = UsuarioValido();
            usuario.Nombre = nombre;

            var errores = _sut.Validar(usuario);

            Assert.That(errores.Any(e => e.Contains("nombre")), Is.True);
        }

        [Test]
        public void Validar_NombreMayorA100_RetornaError()
        {
            var usuario = UsuarioValido();
            usuario.Nombre = new string('a', 101);

            var errores = _sut.Validar(usuario);

            Assert.That(errores.Any(e => e.Contains("100")), Is.True);
        }

        [Test]
        public void Validar_NombreDe100Caracteres_EsValido()
        {
            var usuario = UsuarioValido();
            usuario.Nombre = new string('a', 100);

            Assert.That(_sut.Validar(usuario), Is.Empty);
        }

        [Test]
        public void Validar_FechaFutura_RetornaError()
        {
            var usuario = UsuarioValido();
            usuario.FechaNacimiento = DateTime.Today.AddDays(1);

            var errores = _sut.Validar(usuario);

            Assert.That(errores.Any(e => e.Contains("futura")), Is.True);
        }

        [Test]
        public void Validar_FechaDeHoy_EsValida()
        {
            var usuario = UsuarioValido();
            usuario.FechaNacimiento = DateTime.Today;

            Assert.That(_sut.Validar(usuario), Is.Empty);
        }

        [Test]
        public void Validar_FechaPorDefecto_RetornaError()
        {
            var usuario = UsuarioValido();
            usuario.FechaNacimiento = default(DateTime);

            var errores = _sut.Validar(usuario);

            Assert.That(errores, Is.Not.Empty);
        }

        [TestCase("M")]
        [TestCase("F")]
        public void Validar_SexoValido_NoRetornaErrores(string sexo)
        {
            var usuario = UsuarioValido();
            usuario.Sexo = sexo;

            Assert.That(_sut.Validar(usuario), Is.Empty);
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("X")]
        [TestCase("MF")]
        [TestCase("m")]
        public void Validar_SexoInvalido_RetornaError(string sexo)
        {
            var usuario = UsuarioValido();
            usuario.Sexo = sexo;

            var errores = _sut.Validar(usuario);

            Assert.That(errores.Any(e => e.Contains("sexo")), Is.True);
        }

        [Test]
        public void Validar_VariosCamposInvalidos_RetornaTodosLosErrores()
        {
            var usuario = new UsuarioDto
            {
                Nombre = "",
                FechaNacimiento = DateTime.Today.AddDays(5),
                Sexo = "Z"
            };

            var errores = _sut.Validar(usuario);

            Assert.That(errores, Has.Count.EqualTo(3));
        }
    }
}
