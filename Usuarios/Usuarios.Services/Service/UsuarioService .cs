using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.ServiceModel;
using System.Web;
using Usuarios.Services.Business;
using Usuarios.Services.Business.Interfaces;
using Usuarios.Services.BusinessException;
using Usuarios.Services.Contracts;
using Usuarios.Services.Contracts.Interfaces;
using Usuarios.Services.Data;

namespace Usuarios.Services.Service
{
    public class UsuarioService : IUsuarioService
    {
        private readonly IUsuarioBusiness _business;

        public UsuarioService() : this(CrearBusiness()) { }

        public UsuarioService(IUsuarioBusiness business)
        {
            _business = business ?? throw new ArgumentNullException(nameof(business));
        }

        public int Agregar(UsuarioDto usuario) => Ejecutar(() => _business.Agregar(usuario));

        public void Modificar(UsuarioDto usuario) => Ejecutar(() => _business.Modificar(usuario));

        public List<UsuarioDto> Consultar() => Ejecutar(() => _business.Consultar().ToList());

        public UsuarioDto ConsultarPorId(int id) => Ejecutar(() => _business.ConsultarPorId(id));

        public void Eliminar(int id) => Ejecutar(() => _business.Eliminar(id));


        private static IUsuarioBusiness CrearBusiness()
        {
            var cadena = ConfigurationManager.ConnectionStrings["UsuariosDb"]?.ConnectionString;
            return new UsuarioBusiness(new UsuarioRepository(cadena), new UsuarioValidator());
        }


        private static void Ejecutar(Action accion)
        {
            Ejecutar<object>(() => { accion(); return null; });
        }

        private static T Ejecutar<T>(Func<T> accion)
        {
            try
            {
                return accion();
            }
            catch (ValidacionException ex)
            {
                throw CrearFault(CodigosFault.Validacion, ex.Message, ex.Errores);
            }
            catch (UsuarioNoEncontradoException ex)
            {
                throw CrearFault(CodigosFault.NoEncontrado, ex.Message);
            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.ToString());
                throw CrearFault(CodigosFault.ErrorInterno, "Ocurrió un error inesperado. Intente nuevamente.");
            }
        }

        private static FaultException<UsuarioFault> CrearFault(
            string codigo, string mensaje, IEnumerable<string> errores = null)
        {
            var detalle = new UsuarioFault
            {
                Codigo = codigo,
                Mensaje = mensaje,
                Errores = errores != null ? errores.ToList() : new List<string>()
            };
            return new FaultException<UsuarioFault>(detalle, new FaultReason(mensaje));
        }
    }
}