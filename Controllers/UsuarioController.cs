using EduDataApi.Models;
using EduDataApi.Repositorios;
using Microsoft.AspNetCore.Mvc;

namespace EduDataApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuarioController : ControllerBase
    {
        private readonly UsuarioRepository _repository;

        public UsuarioController(UsuarioRepository repository)
        {
            _repository = repository;
        }

        // Crear usuario
        [HttpPost]
        public async Task<IActionResult> CrearUsuario([FromBody] Usuario usuario)
        {
            await _repository.CrearUsuario(usuario);

            return CreatedAtAction(
                nameof(BuscarUsuarioPorId),
                new { id = usuario.Id },
                usuario
            );
        }

        // Obtener todos los usuarios
        [HttpGet]
        public async Task<ActionResult<List<Usuario>>> LeerUsuarios()
        {
            var usuarios = await _repository.LeerUsuarios();
            return Ok(usuarios);
        }

        // Obtener usuario por ID
        [HttpGet("{id}")]
        public async Task<IActionResult> BuscarUsuarioPorId(int id)
        {
            var usuario = await _repository.BuscarUsuarioPorId(id);

            if (usuario == null)
            {
                return NotFound();
            }

            return Ok(usuario);
        }

        // Actualizar usuario
        [HttpPut("{id}")]
        public async Task<IActionResult> ModificarUsuario(int id, [FromBody] Usuario usuario)
        {
            if (id != usuario.Id)
            {
                return BadRequest("El ID no coincide");
            }

            await _repository.ModificarUsuario(usuario);
            return NoContent();
        }

        // Eliminar usuario
        [HttpDelete("{id}")]
        public async Task<IActionResult> EliminarUsuario(int id)
        {
            await _repository.EliminarUsuario(id);
            return NoContent();
        }
    }
}