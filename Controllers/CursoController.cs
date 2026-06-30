using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using EduDataApi.Models;
using System.Data;

namespace EduDataApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CursoController : ControllerBase
    {
        private readonly string _cadenaConexion;

        public CursoController(IConfiguration configuracion)
        {
            _cadenaConexion = configuracion.GetConnectionString("DefaultConnection");
        }

        // GET: api/curso
        [HttpGet]
        public IActionResult Get()
        {
            var cursos = new List<Curso>();
            try
            {
                using (SqlConnection con = new SqlConnection(_cadenaConexion))
                {
                    SqlCommand cmd = new SqlCommand("sp_Curso_Listar", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    con.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            cursos.Add(new Curso
                            {
                                IdCurso = Convert.ToInt32(reader["IdCurso"]),
                                Nombre = reader["Nombre"].ToString(),
                                Descripcion = reader["Descripcion"].ToString(),
                                IdProfesor = Convert.ToInt32(reader["IdProfesor"]),
                                Activo = Convert.ToBoolean(reader["Activo"]),
                                FechaCreacion = Convert.ToDateTime(reader["FechaCreacion"]),
                                UsuarioCreacion = reader["UsuarioCreacion"].ToString()
                            });
                        }
                    }
                }
                return Ok(cursos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error interno", detalle = ex.Message });
            }
        }

        // GET: api/curso/5
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            Curso curso = null;
            try
            {
                using (SqlConnection con = new SqlConnection(_cadenaConexion))
                {
                    SqlCommand cmd = new SqlCommand("sp_Curso_ObtenerPorId", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@IdCurso", id);
                    con.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            curso = new Curso
                            {
                                IdCurso = Convert.ToInt32(reader["IdCurso"]),
                                Nombre = reader["Nombre"].ToString(),
                                Descripcion = reader["Descripcion"].ToString(),
                                IdProfesor = Convert.ToInt32(reader["IdProfesor"]),
                                Activo = Convert.ToBoolean(reader["Activo"]),
                                FechaCreacion = Convert.ToDateTime(reader["FechaCreacion"]),
                                UsuarioCreacion = reader["UsuarioCreacion"].ToString()
                            };
                        }
                    }
                }

                if (curso == null) return NotFound(new { mensaje = "Curso no encontrado o inactivo." });
                return Ok(curso);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error interno", detalle = ex.Message });
            }
        }

        // POST: api/curso
        [HttpPost]
        public IActionResult Post([FromBody] Curso curso)
        {
            if (curso == null || string.IsNullOrEmpty(curso.Nombre))
                return BadRequest(new { mensaje = "Datos inválidos." });

            try
            {
                using (SqlConnection con = new SqlConnection(_cadenaConexion))
                {
                    SqlCommand cmd = new SqlCommand("sp_Curso_Insertar", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Nombre", curso.Nombre);
                    cmd.Parameters.AddWithValue("@Descripcion", string.IsNullOrEmpty(curso.Descripcion) ? (object)DBNull.Value : curso.Descripcion);
                    cmd.Parameters.AddWithValue("@IdProfesor", curso.IdProfesor);
                    cmd.Parameters.AddWithValue("@UsuarioCreacion", string.IsNullOrEmpty(curso.UsuarioCreacion) ? "AdminAPI" : curso.UsuarioCreacion);

                    con.Open();
                    var idGenerado = cmd.ExecuteScalar();
                    curso.IdCurso = Convert.ToInt32(idGenerado);
                }
                return Ok(new { mensaje = "Curso creado", id = curso.IdCurso });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error interno", detalle = ex.Message });
            }
        }
    }
}