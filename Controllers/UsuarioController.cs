using EduDataApi.Models;
using EduDataApi.Repositorios;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc;

namespace EduDataApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuarioController : Controller
    {
        private readonly UsuarioRepository _repository;

        public UsuarioController(UsuarioRepository repository)
        {
            _repository = repository;
        }

        [HttpPost]
        public async Task<IActionResult> CrearUsuario([FromBody] Usuario Usuario)
        {
            await _repository.CrearUsuario(Usuario);
            return CreatedAtAction(nameof(BuscarUsuarioPorId),
                new { id = Usuario.Id }, Usuario); 
        }

        [HttpGet]
        public async Task<ActionResult<List<Usuario>>> LeerUsuarios()
        {
            return await _repository.LeerUsuarios();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> ModificarUsuario(int id, [FromBody] Usuario Usuario)
        {
            if (id != Usuario.Id) 
            {
                return BadRequest();
            }
            await _repository.ModificarUsuario(Usuario);
            return NoContent();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> ElimninarUsuario(int id)
        {
            await _repository.EliminarUsuario(id);
            return NoContent();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> BuscarUsuarioPorId(int id)
        {
            var Usuario = await _repository.BuscarUsuarioPorId(id);
            if (Usuario == null)
            {
                return NotFound();
            }
            return Usuario();
        }
    }
}
