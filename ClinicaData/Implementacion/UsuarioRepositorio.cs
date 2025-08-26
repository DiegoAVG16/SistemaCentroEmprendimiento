using CentroEmpData.Configuracion;
using CentroEmpData.Contrato;
using CentroEmpEntidades;
using Microsoft.Extensions.Options;
using System.Data;
using System.Data.SqlClient;
using static System.Runtime.InteropServices.JavaScript.JSType;
using CentroEmpData.Seguridad;

namespace CentroEmpData.Implementacion
{
    public class UsuarioRepositorio : IUsuarioRepositorio
    {
        private readonly ConnectionStrings con;
        public UsuarioRepositorio(IOptions<ConnectionStrings> options)
        {
            con = options.Value;
        }
        public async Task<string> Editar(Usuario objeto)
        {
            string respuesta = "";
            using (var conexion = new SqlConnection(con.CadenaSQL))
            {
                await conexion.OpenAsync();
                SqlCommand cmd = new SqlCommand("sp_editarUsuario", conexion);
                cmd.Parameters.AddWithValue("@IdUsuario", objeto.IdUsuario);
                cmd.Parameters.AddWithValue("@NumeroDocumentoIdentidad", objeto.NumeroDocumentoIdentidad);
                cmd.Parameters.AddWithValue("@Nombre", objeto.Nombre);
                cmd.Parameters.AddWithValue("@Apellido", objeto.Apellido);
                cmd.Parameters.AddWithValue("@Correo", objeto.Correo);
                // Hash si no viene hasheada (compatibilidad con existentes)
                var storedPassword = SecurityUtils.IsHashed(objeto.Clave) ? objeto.Clave : SecurityUtils.HashPassword(objeto.Clave);
                cmd.Parameters.AddWithValue("@Clave", storedPassword);
                cmd.Parameters.AddWithValue("@IdRolUsuario", objeto.RolUsuario.IdRolUsuario);
                cmd.Parameters.Add("@MsgError", SqlDbType.VarChar, 100).Direction = ParameterDirection.Output;
                cmd.CommandType = CommandType.StoredProcedure;

                try
                {
                    await cmd.ExecuteNonQueryAsync();
                    respuesta = Convert.ToString(cmd.Parameters["@MsgError"].Value)!;
                }
                catch
                {
                    respuesta = "Error al editar usuario";
                }

            }
            return respuesta;
        }

        public async Task<int> Eliminar(int Id)
        {
            int respuesta = 1;
            using (var conexion = new SqlConnection(con.CadenaSQL))
            {
                await conexion.OpenAsync();
                SqlCommand cmd = new SqlCommand("sp_eliminarUsuario", conexion);
                cmd.Parameters.AddWithValue("@IdUsuario", Id);
                cmd.CommandType = CommandType.StoredProcedure;

                try
                {
                    await cmd.ExecuteNonQueryAsync();
                }
                catch
                {
                    respuesta = 0;
                }

            }
            return respuesta;
        }

        public async Task<string> Guardar(Usuario objeto)
        {
            string respuesta = "";
            using (var conexion = new SqlConnection(con.CadenaSQL))
            {
                await conexion.OpenAsync();
                SqlCommand cmd = new SqlCommand("sp_guardarUsuario", conexion);
                cmd.Parameters.AddWithValue("@NumeroDocumentoIdentidad", objeto.NumeroDocumentoIdentidad);
                cmd.Parameters.AddWithValue("@Nombre", objeto.Nombre);
                cmd.Parameters.AddWithValue("@Apellido", objeto.Apellido);
                cmd.Parameters.AddWithValue("@Correo", objeto.Correo);
                var storedPasswordEdit = SecurityUtils.IsHashed(objeto.Clave) ? objeto.Clave : SecurityUtils.HashPassword(objeto.Clave);
                cmd.Parameters.AddWithValue("@Clave", storedPasswordEdit);
                cmd.Parameters.AddWithValue("@IdRolUsuario", objeto.RolUsuario.IdRolUsuario);
                cmd.Parameters.Add("@MsgError", SqlDbType.VarChar, 100).Direction = ParameterDirection.Output;
                cmd.CommandType = CommandType.StoredProcedure;

                try
                {
                    await cmd.ExecuteNonQueryAsync();
                    respuesta = Convert.ToString(cmd.Parameters["@MsgError"].Value)!;
                }
                catch
                {
                    respuesta = "Error al guardar usuario";
                }

            }
            return respuesta;
        }

        public async Task<List<Usuario>> Lista(int IdRolUsuario = 0)
        {
            List<Usuario> lista = new List<Usuario>();

            using (var conexion = new SqlConnection(con.CadenaSQL))
            {
                await conexion.OpenAsync();
                SqlCommand cmd = new SqlCommand("sp_listaUsuario", conexion);
                cmd.Parameters.AddWithValue("@IdRolUsuario",IdRolUsuario);
                cmd.CommandType = CommandType.StoredProcedure;

                using (var dr = await cmd.ExecuteReaderAsync())
                {
                    while (await dr.ReadAsync())
                    {
                        lista.Add(new Usuario()
                        {
                            IdUsuario = Convert.ToInt32(dr["IdUsuario"]),
                            NumeroDocumentoIdentidad = dr["NumeroDocumentoIdentidad"].ToString()!,
                            Nombre = dr["Nombre"].ToString()!,
                            Apellido = dr["Apellido"].ToString()!,
                            Correo = dr["Correo"].ToString()!,
                            Clave = dr["Clave"].ToString()!,
                            RolUsuario = new RolUsuario()
                            {
                                IdRolUsuario = Convert.ToInt32(dr["IdRolUsuario"]),
                                Nombre = dr["NombreRol"].ToString()!,
                            },
                            FechaCreacion = dr["FechaCreacion"].ToString()!
                        });
                    }
                }
            }
            return lista;
        }

        public async Task<Usuario> Login(string DocumentoIdentidad, string Clave)
        {
            Usuario objeto = null!;

            using (var conexion = new SqlConnection(con.CadenaSQL))
            {
                await conexion.OpenAsync();
                SqlCommand cmd = new SqlCommand("sp_loginUsuario", conexion);
                cmd.Parameters.AddWithValue("@DocumentoIdentidad", DocumentoIdentidad);
                // El SP debe validar por aplicación: recuperamos hash y verificamos aquí
                cmd.Parameters.AddWithValue("@Clave", "__app_verifica_hash__");
                cmd.CommandType = CommandType.StoredProcedure;

                using (var dr = await cmd.ExecuteReaderAsync())
                {
                    while (await dr.ReadAsync())
                    {
                        objeto = new Usuario()
                        {
                            IdUsuario = Convert.ToInt32(dr["IdUsuario"]),
                            NumeroDocumentoIdentidad = dr["NumeroDocumentoIdentidad"].ToString()!,
                            Nombre = dr["Nombre"].ToString()!,
                            Apellido = dr["Apellido"].ToString()!,
                            Correo = dr["Correo"].ToString()!,
                            Clave = dr["Clave"].ToString()!,
                            RolUsuario = new RolUsuario
                            {
                                Nombre = dr["NombreRol"].ToString()!,
                            }
                        };
                    }
                }
            }
            // Verificar hash o texto plano; si coincide en texto plano, migrar a hash
            if (objeto != null && !string.IsNullOrEmpty(objeto.Clave))
            {
                bool ok = false;
                if (SecurityUtils.IsHashed(objeto.Clave))
                {
                    ok = SecurityUtils.VerifyPassword(Clave, objeto.Clave);
                }
                else
                {
                    ok = (Clave == objeto.Clave);
                    if (ok)
                    {
                        // Migración automática a hash
                        objeto.Clave = SecurityUtils.HashPassword(Clave);
                        await Editar(objeto);
                    }
                }
                if (!ok) return null!;
                objeto.Clave = string.Empty;
            }
            return objeto;
        }

        public async Task<int> MigrarHashes()
        {
            int migrados = 0;
            var usuarios = await Lista(0);
            foreach (var u in usuarios)
            {
                if (!SecurityUtils.IsHashed(u.Clave))
                {
                    u.Clave = SecurityUtils.HashPassword(u.Clave);
                    var r = await Editar(u);
                    if (r == "") migrados++;
                }
            }
            return migrados;
        }
    }
}
