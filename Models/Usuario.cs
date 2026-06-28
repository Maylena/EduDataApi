using System.Data;

namespace EduDataApi.Models
{
    public class Usuario
    {
        //Propiedades 
        public int Id { get; set; }
        public string Apodo {  get; set; }
        public string Correo { get; set; }
        public string Clave { get; set; }
        public bool Estado { get; set; }
        public string Rol {  get; set; }
        public DateTime Fecha_Creacion {  get; set; }
        public string Creado_Por {  get; set; }
        public DateTime? Fecha_Modificacion {  get; set; }
        public string? Modificacion_Por { get; set; }
        public SqlDbType Modificado_Por { get; internal set; }
    }
}
