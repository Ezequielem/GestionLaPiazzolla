using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestionLaPiazzolla.Models
{
    public class Clase
    {
        public int ClaseId { get; set; }
        [Required]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "El nombre debe tener entre 3 y 50 caracteres")]
        public string Nombre { get; set; }
        [Required]
        [Display(Name = "Fecha y hora")]
        public DateTime FechaYHora { get; set; }
        [Display(Name = "Observación")]
        public string Observacion { get; set; }
        [Required]
        public int CursoId { get; set; }
        [Required]
        public Curso Curso { get; set; }
        public List<Alumno_x_Clase> Alumnos_X_Clases { get; set; }
    }
}
