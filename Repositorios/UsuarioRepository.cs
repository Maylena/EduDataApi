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
        public async Task CrearUsuario(Usuario Usuario)
        {
            var parameters = new[]
            {
            new SqlParameter("@Apodo", Usuario.Apodo),
            new SqlParameter("@Correo", Usuario.Correo),
            new SqlParameter("@Clave", Usuario.Clave),
            new SqlParameter("@Estado", Usuario.Estado),
            new SqlParameter("@Rol", Usuario.Rol),
            new SqlParameter("@Creado_Por", Usuario.Creado_Por)
            
        }:
            await _context.Database.ExecuteSqlRawAsync("EXEC sp_CrearUsuario @Apodo, @Correo," +
                " @Clave, @Estado, @Rol, @Creado_Por", parameters);
        }


        public async Task ModificarUsuario(Usuario Usuario)
        {
            var parameters = new[]
            {
            new SqlParameter("@Id", Usuario.Id),
            new SqlParameter("@Apodo", Usuario.Apodo),
            new SqlParameter("@Correo", Usuario.Correo),
            new SqlParameter("@Estado", Usuario.Estado),
            new SqlParameter("@Rol", Usuario.Rol),
            new SqlParameter("@Modificado_Por", Usuario.Modificado_Por)

        }:
            await _context.Database.ExecuteSqlRawAsync("EXEC sp_ModificarUsuario @Id, @Apodo, @Correo," +
                " @Estado, @Rol, @Modificado_Por", parameters);
        }


        public async Task EliminarUsuario(int id)
        {
            var parameter = new SqlParameter("@Id", id);
            await _context.Database.ExecuteSqlRawAsync("EXEC sp_ElimianrUsuario @Id", parameter);
        }


        public async Task LeerUsuario(Usuario Usuario)
        {
            var parameters = new[]
            {
            new SqlParameter("@Id", Usuario.Id),
            new SqlParameter("@Apodo", Usuario.Apodo),
            new SqlParameter("@Correo", Usuario.Correo),
            new SqlParameter("@Estado", Usuario.Estado),
            new SqlParameter("@Rol", Usuario.Rol),

        }:
            await _context.Database.ExecuteSqlRawAsync("EXEC sp_LeerUsuario @Id, @Apodo, @Correo," +
                " @Estado, @Rol", parameters);
        }

        internal async Task BuscarUsuarioPorId(int id)
        {
            throw new NotImplementedException();
        }
    }
}
