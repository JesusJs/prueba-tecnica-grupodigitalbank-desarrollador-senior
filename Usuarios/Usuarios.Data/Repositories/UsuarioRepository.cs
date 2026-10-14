using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using Usuarios.Data.Entities;
using Usuarios.Services.Data.Interfaces;

namespace Usuarios.Services.Data
{
    public class UsuarioRepository : IUsuarioRepository
    {
        private const string NombreProcedimiento = "dbo.usp_Usuario_CRUD";
        private const int ErrorUsuarioNoEncontrado = 50001;

        private readonly string _connectionString;

        public UsuarioRepository(string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
                throw new ArgumentException("La cadena de conexión es obligatoria.", nameof(connectionString));

            _connectionString = connectionString;
        }

        public int Agregar(UsuarioEntity usuario)
        {
            using (var conexion = new SqlConnection(_connectionString))
            using (var comando = CrearComando(conexion, 'C'))
            {
                AgregarParametrosDeDatos(comando, usuario);
                conexion.Open();
                return (int)comando.ExecuteScalar();
            }
        }

        public bool Modificar(UsuarioEntity usuario)
        {
            using (var conexion = new SqlConnection(_connectionString))
            using (var comando = CrearComando(conexion, 'U'))
            {
                comando.Parameters.Add("@Id", SqlDbType.Int).Value = usuario.Id;
                AgregarParametrosDeDatos(comando, usuario);
                return EjecutarSinResultado(conexion, comando);
            }
        }

        public UsuarioEntity ObtenerPorId(int id)
        {
            using (var conexion = new SqlConnection(_connectionString))
            using (var comando = CrearComando(conexion, 'R'))
            {
                comando.Parameters.Add("@Id", SqlDbType.Int).Value = id;
                conexion.Open();

                using (var lector = comando.ExecuteReader())
                {
                    return lector.Read() ? Mapear(lector) : null;
                }
            }
        }

        public IReadOnlyList<UsuarioEntity> ObtenerTodos()
        {
            var usuarios = new List<UsuarioEntity>();

            using (var conexion = new SqlConnection(_connectionString))
            using (var comando = CrearComando(conexion, 'R'))
            {
                conexion.Open();

                using (var lector = comando.ExecuteReader())
                {
                    while (lector.Read())
                        usuarios.Add(Mapear(lector));
                }
            }

            return usuarios;
        }

        public bool Eliminar(int id)
        {
            using (var conexion = new SqlConnection(_connectionString))
            using (var comando = CrearComando(conexion, 'D'))
            {
                comando.Parameters.Add("@Id", SqlDbType.Int).Value = id;
                return EjecutarSinResultado(conexion, comando);
            }
        }

    // Helpers
        private static SqlCommand CrearComando(SqlConnection conexion, char accion)
        {
            var comando = new SqlCommand(NombreProcedimiento, conexion)
            {
                CommandType = CommandType.StoredProcedure,
                CommandTimeout = 30
            };
            comando.Parameters.Add("@Accion", SqlDbType.Char, 1).Value = accion.ToString();
            return comando;
        }

        private static void AgregarParametrosDeDatos(SqlCommand comando, UsuarioEntity usuario)
        {
            comando.Parameters.Add("@Nombre", SqlDbType.NVarChar, 100).Value = usuario.Nombre;
            comando.Parameters.Add("@FechaNacimiento", SqlDbType.Date).Value = usuario.FechaNacimiento.Date;
            comando.Parameters.Add("@Sexo", SqlDbType.Char, 1).Value = usuario.Sexo;
        }

        // Devuelve false si el SP indica que el usuario no existe (THROW 50001).
        private static bool EjecutarSinResultado(SqlConnection conexion, SqlCommand comando)
        {
            try
            {
                conexion.Open();
                comando.ExecuteNonQuery();
                return true;
            }
            catch (SqlException ex) when (ex.Number == ErrorUsuarioNoEncontrado)
            {
                return false;
            }
        }

        private static UsuarioEntity Mapear(IDataRecord registro)
        {
            return new UsuarioEntity
            {
                Id = registro.GetInt32(registro.GetOrdinal("Id")),
                Nombre = registro.GetString(registro.GetOrdinal("Nombre")),
                FechaNacimiento = registro.GetDateTime(registro.GetOrdinal("FechaNacimiento")),
                Sexo = registro.GetString(registro.GetOrdinal("Sexo"))
            };
        }
    }
}