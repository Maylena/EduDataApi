using EduDataApi.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace EduDataApi.Repositorios
{
    public class UsuarioRepository
    {
        private readonly AppDbContext _context;

        public UsuarioRepository(AppDbContext context)
        {
            _context = context;
        }

        // Crear usuario
        public async Task CrearUsuario(Usuario usuario)
        {
            var parameters = new[]
            {
                new SqlParameter("@Apodo", usuario.Apodo),
                new SqlParameter("@Correo", usuario.Correo),
                new SqlParameter("@Clave", usuario.Clave),
                new SqlParameter("@Estado", usuario.Estado),
                new SqlParameter("@Rol", usuario.Rol),
                new SqlParameter("@Creado_Por", usuario.Creado_Por)
            };

            await _context.Database.ExecuteSqlRawAsync(
                "EXEC sp_CrearUsuario @Apodo, @Correo, @Clave, @Estado, @Rol, @Creado_Por",
                parameters
            );
        }

        // Modificar usuario
        public async Task ModificarUsuario(Usuario usuario)
        {
            var parameters = new[]
            {
                new SqlParameter("@Id", usuario.Id),
                new SqlParameter("@Apodo", usuario.Apodo),
                new SqlParameter("@Correo", usuario.Correo),
                new SqlParameter("@Estado", usuario.Estado),
                new SqlParameter("@Rol", usuario.Rol),
                new SqlParameter("@Modificado_Por", usuario.Modificado_Por)
            };

            await _context.Database.ExecuteSqlRawAsync(
                "EXEC sp_ModificarUsuario @Id, @Apodo, @Correo, @Estado, @Rol, @Modificado_Por",
                parameters
            );
        }

        // Eliminar usuario
        public async Task EliminarUsuario(int id)
        {
            var parameter = new SqlParameter("@Id", id);

            await _context.Database.ExecuteSqlRawAsync(
                "EXEC sp_EliminarUsuario @Id",
                parameter
            );
        }

        // Obtener todos los usuarios
        public async Task<List<Usuario>> LeerUsuarios()
        {
            return await _context.Usuarios
                .FromSqlRaw("EXEC sp_LeerUsuarios")
                .ToListAsync();
        }

        // Obtener usuario por ID
        public async Task<Usuario?> BuscarUsuarioPorId(int id)
        {
            var parameter = new SqlParameter("@Id", id);

            return await _context.Usuarios
                .FromSqlRaw("EXEC sp_BuscarUsuarioPorId @Id", parameter)
                .AsNoTracking()
                .FirstOrDefaultAsync();
        }
    }
}