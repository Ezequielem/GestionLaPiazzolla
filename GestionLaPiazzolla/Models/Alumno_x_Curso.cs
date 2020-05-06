using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace GestionLaPiazzolla.Models
{
    public class Alumno_x_Curso
    {
        public int AlumnoId { get; set; }
        public int CursoId { get; set; }
        [Required]
        public DateTime FechaInscripcion { get; set; }
        [Required]
        public bool Activo { get; set; }
        [Required]
        [StringLength(512)]
        public string Observacion { get; set; }
        [Required]
        public Alumno Alumno { get; set; }
        [Required]  
        public Curso Curso { get; set; }
    }
}
