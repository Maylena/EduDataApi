using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using EduDataApi.Models;
using System.Data;

namespace EduDataApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProfesorController : ControllerBase
    {
        private readonly string _cadenaConexion;

        public ProfesorController(IConfiguration configuracion)
        {
            _cadenaConexion = configuracion.GetConnectionString("DefaultConnection");
        }

        // GET: api/profesor
        [HttpGet]
        public IActionResult Get()
        {
            var profesores = new List<Profesor>();
            try
            {
                using (SqlConnection con = new SqlConnection(_cadenaConexion))
                {
                    SqlCommand cmd = new SqlCommand("sp_Profesor_Listar", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    con.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            profesores.Add(new Profesor
                            {
                                IdProfesor = Convert.ToInt32(reader["IdProfesor"]),
                                Nombre = reader["Nombre"].ToString(),
                                Email = reader["Email"].ToString(),
                                Especialidad = reader["Especialidad"].ToString(),
                                Activo = Convert.ToBoolean(reader["Activo"]),
                                FechaCreacion = Convert.ToDateTime(reader["FechaCreacion"]),
                                UsuarioCreacion = reader["UsuarioCreacion"].ToString()
                            });
                        }
                    }
                }
                return Ok(profesores);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error interno", detalle = ex.Message });
            }
        }

        // GET: api/profesor/5
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            Profesor profesor = null;
            try
            {
                using (SqlConnection con = new SqlConnection(_cadenaConexion))
                {
                    SqlCommand cmd = new SqlCommand("sp_Profesor_ObtenerPorId", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@IdProfesor", id);
                    con.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            profesor = new Profesor
                            {
                                IdProfesor = Convert.ToInt32(reader["IdProfesor"]),
                                Nombre = reader["Nombre"].ToString(),
                                Email = reader["Email"].ToString(),
                                Especialidad = reader["Especialidad"].ToString(),
                                Activo = Convert.ToBoolean(reader["Activo"]),
                                FechaCreacion = Convert.ToDateTime(reader["FechaCreacion"]),
                                UsuarioCreacion = reader["UsuarioCreacion"].ToString()
                            };
                        }
                    }
                }

                if (profesor == null) return NotFound(new { mensaje = "Profesor no encontrado o inactivo." });
                return Ok(profesor);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error interno", detalle = ex.Message });
            }
        }

        // POST: api/profesor
        [HttpPost]
        public IActionResult Post([FromBody] Profesor profesor)
        {
            if (profesor == null || string.IsNullOrEmpty(profesor.Nombre))
                return BadRequest(new { mensaje = "Datos inválidos." });

            try
            {
                using (SqlConnection con = new SqlConnection(_cadenaConexion))
                {
                    SqlCommand cmd = new SqlCommand("sp_Profesor_Insertar", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Nombre", profesor.Nombre);
                    cmd.Parameters.AddWithValue("@Email", profesor.Email);
                    cmd.Parameters.AddWithValue("@Especialidad", profesor.Especialidad);
                    cmd.Parameters.AddWithValue("@UsuarioCreacion", string.IsNullOrEmpty(profesor.UsuarioCreacion) ? "AdminAPI" : profesor.UsuarioCreacion);

                    con.Open();
                    var idGenerado = cmd.ExecuteScalar();
                    profesor.IdProfesor = Convert.ToInt32(idGenerado);
                }
                return Ok(new { mensaje = "Profesor creado", id = profesor.IdProfesor });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error interno", detalle = ex.Message });
            }
        }
    }
}
