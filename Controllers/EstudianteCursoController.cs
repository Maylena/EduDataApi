using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using EduDataApi.Models;
using System.Data;

namespace EduDataApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EstudianteCursoController : ControllerBase
    {
        private readonly string _cadenaConexion;

        public EstudianteCursoController(IConfiguration configuracion)
        {
            _cadenaConexion = configuracion.GetConnectionString("DefaultConnection");
        }

        // POST: api/estudiantecurso (Matricular)
        [HttpPost]
        public IActionResult Post([FromBody] EstudianteCurso inscripcion)
        {
            if (inscripcion == null || inscripcion.IdEstudiante <= 0 || inscripcion.IdCurso <= 0)
                return BadRequest(new { mensaje = "Debe enviar un estudiante y un curso válidos." });

            try
            {
                using (SqlConnection con = new SqlConnection(_cadenaConexion))
                {
                    SqlCommand cmd = new SqlCommand("sp_EstudianteCurso_Inscribir", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@IdEstudiante", inscripcion.IdEstudiante);
                    cmd.Parameters.AddWithValue("@IdCurso", inscripcion.IdCurso);
                    cmd.Parameters.AddWithValue("@UsuarioCreacion", string.IsNullOrEmpty(inscripcion.UsuarioCreacion) ? "AdminAPI" : inscripcion.UsuarioCreacion);

                    con.Open();
                    var idGenerado = cmd.ExecuteScalar();
                    inscripcion.IdEstudianteCurso = Convert.ToInt32(idGenerado);
                }
                return Ok(new { mensaje = "Inscripción realizada con éxito", id = inscripcion.IdEstudianteCurso });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error interno", detalle = ex.Message });
            }
        }

        // DELETE: api/estudiantecurso/5 (Desmatricular - Borrado Lógico)
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(_cadenaConexion))
                {
                    SqlCommand cmd = new SqlCommand("sp_EstudianteCurso_Eliminar", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@IdEstudianteCurso", id);

                    con.Open();
                    int filasAfectadas = cmd.ExecuteNonQuery();

                    if (filasAfectadas == 0)
                        return NotFound(new { mensaje = "Inscripción no encontrada." });
                }
                return Ok(new { mensaje = "Inscripción eliminada lógicamente." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error interno", detalle = ex.Message });
            }
        }
    }
}