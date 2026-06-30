namespace EduDataApi.Models
{
    public class EstudianteCurso
    {
        public int IdEstudianteCurso { get; set; }
        public int IdEstudiante { get; set; }
        public int IdCurso { get; set; }
        public bool Activo { get; set; } = true;
        public DateTime FechaCreacion { get; set; } = DateTime.Now;
        public string? UsuarioCreacion { get; set; }
    }
}
