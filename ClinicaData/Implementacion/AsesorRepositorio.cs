


using System.Data.SqlClient;
using System.Data;
using Microsoft.Extensions.Options;
using System.Xml.Linq;
using CentroEmpData.Configuracion;
using CentroEmpEntidades.DTO;
using CentroEmpEntidades;
using System.Numerics;
using CentroEmpData.Contrato;
using System;

namespace CentroEmpData.Implementacion
{
    public class AsesorRepositorio : IAsesorRepositorio
    {
        private readonly ConnectionStrings con;
        public AsesorRepositorio(IOptions<ConnectionStrings> options)
        {
            con = options.Value;
        }
        public async Task<string> Editar(Asesor objeto)
        {
            string respuesta = "";
            using (var conexion = new SqlConnection(con.CadenaSQL))
            {
                await conexion.OpenAsync();
                SqlCommand cmd = new SqlCommand("sp_editarAsesor", conexion);
                cmd.Parameters.AddWithValue("@IdAsesor", objeto.IdAsesor);
                cmd.Parameters.AddWithValue("@NumeroDocumentoIdentidad", objeto.NumeroDocumentoIdentidad);
                cmd.Parameters.AddWithValue("@Nombres", objeto.Nombres);
                cmd.Parameters.AddWithValue("@Apellidos", objeto.Apellidos);
                cmd.Parameters.AddWithValue("@Genero", objeto.Genero);
                cmd.Parameters.AddWithValue("@IdEspecialidad", objeto.Especialidad.IdEspecialidad);
                cmd.Parameters.Add("@msgError", SqlDbType.VarChar, 100).Direction = ParameterDirection.Output;
                cmd.CommandType = CommandType.StoredProcedure;

                try
                {
                    await cmd.ExecuteNonQueryAsync();
                    respuesta = Convert.ToString(cmd.Parameters["@msgError"].Value)!;
                }
                catch
                {
                    respuesta = "Error al editar el asesor";
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
                SqlCommand cmd = new SqlCommand("sp_eliminarAsesor", conexion);
                cmd.Parameters.AddWithValue("@IdAsesor", Id);
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

        public async Task<string> EliminarHorario(int Id)
        {
            string respuesta = "";

            using (var conexion = new SqlConnection(con.CadenaSQL))
            {
                await conexion.OpenAsync();
                SqlCommand cmd = new SqlCommand("sp_eliminarAsesorHorario", conexion);
                cmd.Parameters.AddWithValue("@IdAsesorHorario", Id);
                cmd.Parameters.Add("@msgError", SqlDbType.VarChar, 100).Direction = ParameterDirection.Output;
                cmd.CommandType = CommandType.StoredProcedure;

                try
                {
                    await cmd.ExecuteNonQueryAsync();
                    respuesta = Convert.ToString(cmd.Parameters["@msgError"].Value)!;
                }
                catch
                {
                    respuesta = "Error al eliminar el horario";
                }
            }
            return respuesta;
        }

        public async Task<string> Guardar(Asesor objeto)
        {

            string respuesta = "";
            using (var conexion = new SqlConnection(con.CadenaSQL))
            {
                await conexion.OpenAsync();
                SqlCommand cmd = new SqlCommand("sp_guardarAsesor", conexion);
                cmd.Parameters.AddWithValue("@NumeroDocumentoIdentidad", objeto.NumeroDocumentoIdentidad);
                cmd.Parameters.AddWithValue("@Nombres", objeto.Nombres);
                cmd.Parameters.AddWithValue("@Apellidos", objeto.Apellidos);
                cmd.Parameters.AddWithValue("@Genero", objeto.Genero);
                cmd.Parameters.AddWithValue("@IdEspecialidad", objeto.Especialidad.IdEspecialidad);
                cmd.Parameters.Add("@msgError", SqlDbType.VarChar, 100).Direction = ParameterDirection.Output;
                cmd.CommandType = CommandType.StoredProcedure;

                try
                {
                    await cmd.ExecuteNonQueryAsync();
                    respuesta = Convert.ToString(cmd.Parameters["@msgError"].Value)!;
                }
                catch
                {
                    respuesta = "Error al editar el asesor";
                }
            }
            return respuesta;
        }

        public async Task<List<Asesor>> Lista()
        {
            List<Asesor> lista = new List<Asesor>();

            using (var conexion = new SqlConnection(con.CadenaSQL))
            {
                await conexion.OpenAsync();
                SqlCommand cmd = new SqlCommand("sp_listaAsesor", conexion);
                cmd.CommandType = CommandType.StoredProcedure;

                using (var dr = await cmd.ExecuteReaderAsync())
                {
                    while (await dr.ReadAsync())
                    {
                        lista.Add(new Asesor()
                        {
                            IdAsesor = Convert.ToInt32(dr["IdAsesor"]),
                            NumeroDocumentoIdentidad = dr["NumeroDocumentoIdentidad"].ToString()!,
                            Nombres = dr["Nombres"].ToString()!,
                            Apellidos = dr["Apellidos"].ToString()!,
                            Genero = dr["Genero"].ToString()!,
                            Especialidad = new Especialidad()
                            {
                                IdEspecialidad = Convert.ToInt32(dr["IdEspecialidad"]),
                                Nombre = dr["NombreEspecialidad"].ToString()!,
                            },
                            FechaCreacion = dr["FechaCreacion"].ToString()!
                        });
                    }
                }
            }
            return lista;
        }

        public async Task<List<Cita>> ListaCitasAsignadas(int Id, int IdEstadoCita)
        {
            List<Cita> lista = new List<Cita>();

            using (var conexion = new SqlConnection(con.CadenaSQL))
            {
                await conexion.OpenAsync();
                SqlCommand cmd = new SqlCommand("sp_ListaCitasAsignadas", conexion);
                cmd.Parameters.AddWithValue("@IdAsesor", Id);
                cmd.Parameters.AddWithValue("@IdEstadoCita", IdEstadoCita);
               // cmd.Parameters.AddWithValue("@IdAsesor", 3);
                //cmd.Parameters.AddWithValue("@IdEstadoCita", 2);
                cmd.CommandType = CommandType.StoredProcedure;

                using (var dr = await cmd.ExecuteReaderAsync())
                {
                    while (await dr.ReadAsync())
                    {
                        lista.Add(new Cita()
                        {
                            IdCita = Convert.ToInt32(dr["IdCita"]),
                            FechaCita = dr["FechaCita"].ToString()!,
                            HoraCita = dr["HoraCita"].ToString()!,
                            Usuario = new Usuario()
                            {
                                Nombre = dr["Nombre"].ToString()!,
                                Apellido = dr["Apellido"].ToString()!,
                            },
                            EstadoCita = new EstadoCita()
                            {
                                Nombre = dr["EstadoCita"].ToString()!
                            },
                            Indicaciones = dr["Indicaciones"].ToString()!
                        });
                    }
                }
            }
            return lista;
        }

        public async Task<List<AsesorHorario>> ListaAsesorHorario()
        {
            List<AsesorHorario> lista = new List<AsesorHorario>();

            using (var conexion = new SqlConnection(con.CadenaSQL))
            {
                await conexion.OpenAsync();
                SqlCommand cmd = new SqlCommand("sp_listaAsesorHorario", conexion);
                cmd.CommandType = CommandType.StoredProcedure;

                using (var dr = await cmd.ExecuteReaderAsync())
                {
                    while (await dr.ReadAsync())
                    {
                        lista.Add(new AsesorHorario()
                        {
                            IdAsesorHorario = Convert.ToInt32(dr["IdAsesorHorario"]),
                            Asesor = new Asesor()
                            {
                                NumeroDocumentoIdentidad = dr["NumeroDocumentoIdentidad"].ToString()!,
                                Nombres = dr["Nombres"].ToString()!,
                                Apellidos = dr["Apellidos"].ToString()!,
                            },
                            NumeroMes = Convert.ToInt32(dr["NumeroMes"]),
                            HoraInicioAM = dr["HoraInicioAM"].ToString()!,
                            HoraFinAM = dr["HoraFinAM"].ToString()!,
                            HoraInicioPM = dr["HoraInicioPM"].ToString()!,
                            HoraFinPM = dr["HoraFinPM"].ToString()!,
                            FechaCreacion = dr["FechaCreacion"].ToString()!
                        });
                    }
                }
            }
            return lista;
        }

        public async Task<List<FechaAtencionDTO>> ListaAsesorHorarioDetalle(int Id)

        {
            Console.WriteLine("ID recibido: " + Id);

           

            List<FechaAtencionDTO> lista = new List<FechaAtencionDTO>();
            

            using (var conexion = new SqlConnection(con.CadenaSQL))
            {
                
                await conexion.OpenAsync();
                SqlCommand cmd = new SqlCommand("sp_listaAsesorHorarioDetalle", conexion);// sp_listaAsesorHorarioDetalle


                cmd.Parameters.AddWithValue("@IdAsesor",Id); //@IdAsesor
                cmd.CommandType = CommandType.StoredProcedure;

                using (var dr = await cmd.ExecuteXmlReaderAsync())
                {
                    if (await dr.ReadAsync())
                    {
                        XDocument doc = XDocument.Load(dr);
                        lista = doc.Elements("HorarioAsesor") != null ? (from FechaAtencion in doc.Element("HorarioAsesor")!.Elements("FechaAtencion")
                                                                             select new FechaAtencionDTO()
                                                                             {
                                                                                 Fecha = FechaAtencion.Element("Fecha")!.Value,
                                                                                 HorarioDTO = FechaAtencion.Elements("Horarios") != null ? (from Hora in FechaAtencion.Element("Horarios")!.Elements("Hora")
                                                                                                                                            select new HorarioDTO()
                                                                                                                                            {
                                                                                                                                                IdAsesorHorarioDetalle = Convert.ToInt32(Hora.Element("IdAsesorHorarioDetalle")!.Value),
                                                                                                                                                Turno = Hora.Element("Turno")!.Value,
                                                                                                                                                TurnoHora = Hora.Element("TurnoHora")!.Value
                                                                                                                                            }).ToList() : new List<HorarioDTO>()

                                                                             }).ToList() : new List<FechaAtencionDTO>();

                    }
                }
            }
            
            return lista;
        }

        public async Task<string> RegistrarHorario(AsesorHorario objeto)
        {
            string respuesta = "";
            using (var conexion = new SqlConnection(con.CadenaSQL))
            {
                await conexion.OpenAsync();
                SqlCommand cmd = new SqlCommand("sp_registrarAsesorHorario", conexion);
                cmd.Parameters.AddWithValue("@IdAsesor", objeto.Asesor.IdAsesor);
                cmd.Parameters.AddWithValue("@NumeroMes", objeto.NumeroMes);
                cmd.Parameters.AddWithValue("@HoraInicioAM", objeto.HoraInicioAM);
                cmd.Parameters.AddWithValue("@HoraFinAM", objeto.HoraFinAM);
                cmd.Parameters.AddWithValue("@HoraInicioPM", objeto.HoraInicioPM);
                cmd.Parameters.AddWithValue("@HoraFinPM", objeto.HoraFinPM);
                cmd.Parameters.AddWithValue("@Fechas", objeto.AsesorHorarioDetalle.Fecha);
                cmd.Parameters.Add("@msgError", SqlDbType.VarChar, 100).Direction = ParameterDirection.Output;
                cmd.CommandType = CommandType.StoredProcedure;

                try
                {
                    await cmd.ExecuteNonQueryAsync();
                    respuesta = Convert.ToString(cmd.Parameters["@msgError"].Value)!;
                }
                catch (SqlException ex)
                {
                    respuesta = "Error al registrar el horario: " + ex.Message;
                }
                catch (Exception ex)
                {
                    respuesta = "Error al registrar el horario" + ex.Message;
                }
            }
            return respuesta;
        }

        Task<List<AsesorHorario>> IAsesorRepositorio.ListaAsesorHorario(int id)
        {
            throw new NotImplementedException();
        }
    }
}

