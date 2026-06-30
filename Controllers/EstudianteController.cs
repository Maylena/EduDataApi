using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient; 
using EduDataApi.Models;
using System.Data;

namespace EduDataApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EstudianteController : ControllerBase
    {
        private readonly string _cadenaConexion;

        // Inyectamos la configuración para leer la cadena de conexión desde appsettings.json
        public EstudianteController(IConfiguration configuracion)
        {
            _cadenaConexion = configuracion.GetConnectionString("DefaultConnection");
        }

        // GET: api/estudiante
        [HttpGet]
        public IActionResult Get()
        {
            var estudiantes = new List<Estudiante>();
            try
            {
                using (SqlConnection con = new SqlConnection(_cadenaConexion))
                {
                    SqlCommand cmd = new SqlCommand("sp_Estudiante_Listar", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    con.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            estudiantes.Add(new Estudiante
                            {
                                IdEstudiante = Convert.ToInt32(reader["IdEstudiante"]),
                                Nombre = reader["Nombre"].ToString(),
                                Email = reader["Email"].ToString(),
                                Telefono = reader["Telefono"].ToString(),
                                Activo = Convert.ToBoolean(reader["Activo"]),
                                FechaCreacion = Convert.ToDateTime(reader["FechaCreacion"]),
                                UsuarioCreacion = reader["UsuarioCreacion"].ToString()
                            });
                        }
                    }
                }
                return Ok(estudiantes); // Retorna 200 OK
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error interno del servidor", detalle = ex.Message }); // Retorna 500
            }
        }

        // GET: api/estudiante/5
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            Estudiante estudiante = null;
            try
            {
                using (SqlConnection con = new SqlConnection(_cadenaConexion))
                {
                    SqlCommand cmd = new SqlCommand("sp_Estudiante_ObtenerPorId", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@IdEstudiante", id);
                    con.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            estudiante = new Estudiante
                            {
                                IdEstudiante = Convert.ToInt32(reader["IdEstudiante"]),
                                Nombre = reader["Nombre"].ToString(),
                                Email = reader["Email"].ToString(),
                                Telefono = reader["Telefono"].ToString(),
                                Activo = Convert.ToBoolean(reader["Activo"]),
                                FechaCreacion = Convert.ToDateTime(reader["FechaCreacion"]),
                                UsuarioCreacion = reader["UsuarioCreacion"].ToString()
                            };
                        }
                    }
                }

                if (estudiante == null)
                    return NotFound(new { mensaje = "Estudiante no encontrado o inactivo." }); // Retorna 404

                return Ok(estudiante); // Retorna 200 OK
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error interno del servidor", detalle = ex.Message }); // Retorna 500
            }
        }

        // POST: api/estudiante
        [HttpPost]
        public IActionResult Post([FromBody] Estudiante estudiante)
        {
            if (estudiante == null || string.IsNullOrEmpty(estudiante.Nombre))
                return BadRequest(new { mensaje = "Los datos del estudiante son incorrectos." }); // Retorna 400

            try
            {
                using (SqlConnection con = new SqlConnection(_cadenaConexion))
                {
                    SqlCommand cmd = new SqlCommand("sp_Estudiante_Insertar", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Nombre", estudiante.Nombre);
                    cmd.Parameters.AddWithValue("@Email", estudiante.Email);
                    cmd.Parameters.AddWithValue("@Telefono", string.IsNullOrEmpty(estudiante.Telefono) ? (object)DBNull.Value : estudiante.Telefono);
                    cmd.Parameters.AddWithValue("@UsuarioCreacion", string.IsNullOrEmpty(estudiante.UsuarioCreacion) ? "AdminAPI" : estudiante.UsuarioCreacion);

                    con.Open();
                    // Ejecutamos y obtenemos el ID generado
                    var idGenerado = cmd.ExecuteScalar();
                    estudiante.IdEstudiante = Convert.ToInt32(idGenerado);
                }
                return Ok(new { mensaje = "Estudiante creado exitosamente", id = estudiante.IdEstudiante });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error interno del servidor", detalle = ex.Message });
            }
        }

        // PUT: api/estudiante/5
        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] Estudiante estudiante)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(_cadenaConexion))
                {
                    SqlCommand cmd = new SqlCommand("sp_Estudiante_Actualizar", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@IdEstudiante", id);
                    cmd.Parameters.AddWithValue("@Nombre", estudiante.Nombre);
                    cmd.Parameters.AddWithValue("@Email", estudiante.Email);
                    cmd.Parameters.AddWithValue("@Telefono", string.IsNullOrEmpty(estudiante.Telefono) ? (object)DBNull.Value : estudiante.Telefono);

                    con.Open();
                    int filasAfectadas = cmd.ExecuteNonQuery();

                    if (filasAfectadas == 0)
                        return NotFound(new { mensaje = "Estudiante no encontrado para actualizar." }); // 404
                }
                return Ok(new { mensaje = "Estudiante actualizado exitosamente" }); // 200 OK
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error interno del servidor", detalle = ex.Message }); // 500
            }
        }

        // DELETE: api/estudiante/5
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(_cadenaConexion))
                {
                    SqlCommand cmd = new SqlCommand("sp_Estudiante_Eliminar", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@IdEstudiante", id);

                    con.Open();
                    int filasAfectadas = cmd.ExecuteNonQuery();

                    if (filasAfectadas == 0)
                        return NotFound(new { mensaje = "Estudiante no encontrado." }); // 404
                }
                return Ok(new { mensaje = "Estudiante eliminado lógicamente (Activo = 0)." }); // 200 OK
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error interno del servidor", detalle = ex.Message }); // 500
            }
        }
    }
}
