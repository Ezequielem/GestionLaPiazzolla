using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestionLaPiazzolla.Models
{
    public class Curso
    {
        public int CursoId { get; set; }
        [Required]
        [StringLength(50, MinimumLength =3, ErrorMessage ="El nombre debe tener entre 3 y 50 caracteres")]
        public string Nombre { get; set; }
        [Required]        
        [Display(Name ="Precio")]
        [DataType(DataType.Currency)]
        [Column(TypeName = "money")]
        public float PrecioMensual { get; set; }
        [Required]
        [StringLength(512, ErrorMessage ="La descripción puede tener 512 caracteres" )]
        [Display(Name ="Descripción")]
        public string Descripcion { get; set; }
        [Required]
        public int ProfesorId { get; set; }
        public Profesor Profesor { get; set; }
        public List<Alumno_x_Curso> Alumnos_X_Cursos { get; set; }
    }
}
