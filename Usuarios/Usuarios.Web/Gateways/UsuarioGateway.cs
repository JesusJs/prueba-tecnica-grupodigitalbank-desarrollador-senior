using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Web;
using Usuarios.Web.UsuarioServiceReference;

namespace Usuarios.Web.Gateways
{
    public class UsuarioGateway : IUsuarioGateway
    {
        public int Agregar(UsuarioDto usuario) => Llamar(c => c.Agregar(usuario));

        public void Modificar(UsuarioDto usuario) => Llamar(c => { c.Modificar(usuario); return 0; });

        public IReadOnlyList<UsuarioDto> Consultar() =>
            Llamar(c => (IReadOnlyList<UsuarioDto>)c.Consultar().ToList());

        public UsuarioDto ConsultarPorId(int id) => Llamar(c => c.ConsultarPorId(id));

        public void Eliminar(int id) => Llamar(c => { c.Eliminar(id); return 0; });

        private static T Llamar<T>(Func<UsuarioServiceClient, T> operacion)
        {
            var cliente = new UsuarioServiceClient();
            try
            {
                var resultado = operacion(cliente);
                cliente.Close();
                return resultado;
            }
            catch (FaultException<UsuarioFault> ex)
            {
                cliente.Abort();
                throw new UsuarioGatewayException(ex.Detail.Codigo, ex.Detail.Mensaje, ex.Detail.Errores);
            }
            catch (Exception)
            {
                cliente.Abort();
                throw;
            }
        }
    }
}