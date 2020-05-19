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
        [Display(Name = "Fecha y hora de inscipción")]
        public DateTime FechaInscripcion { get; set; }
        [Required]
        [Display(Name = "Estado")]
        public bool Activo { get; set; }
        [StringLength(512)]
        public string Observacion { get; set; }
        public Alumno Alumno { get; set; }
        public Curso Curso { get; set; }
    }
}
