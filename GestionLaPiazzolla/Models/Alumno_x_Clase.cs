using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GestionLaPiazzolla.Models
{
    public class Alumno_x_Clase
    {
        public int AlumnoId { get; set; }
        public int ClaseId { get; set; }
        [Required]
        public bool Asistencia { get; set; }
        [StringLength(512)]
        public string Observacion { get; set; }
        [Required]        
        public DateTime FechaYHora { get; set; }
        public Alumno Alumno { get; set; }
        public Clase Clase { get; set; }
    }
}
