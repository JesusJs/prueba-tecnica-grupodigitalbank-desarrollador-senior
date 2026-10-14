using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Web;

namespace Usuarios.Services.Contracts.Interfaces
{
    [ServiceContract(Namespace = "http://Jesusjs.dev/usuarios/v1")]
    public interface IUsuarioService
    {
        [OperationContract]
        [FaultContract(typeof(UsuarioFault))]
        int Agregar(UsuarioDto usuario);

        [OperationContract]
        [FaultContract(typeof(UsuarioFault))]
        void Modificar(UsuarioDto usuario);

        [OperationContract]
        [FaultContract(typeof(UsuarioFault))]
        List<UsuarioDto> Consultar();

        [OperationContract]
        [FaultContract(typeof(UsuarioFault))]
        UsuarioDto ConsultarPorId(int id);

        [OperationContract]
        [FaultContract(typeof(UsuarioFault))]
        void Eliminar(int id);
    }
}