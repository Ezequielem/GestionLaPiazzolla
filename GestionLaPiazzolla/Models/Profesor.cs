using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace GestionLaPiazzolla.Models
{
    public class Profesor
    {
        public int ProfesorId { get; set; }
        [Required]
        [StringLength(200)]
        public string Nombre { get; set; }
        [Required]
        [StringLength(200)]
        public string Apellido { get; set; }
        [Required]
        [StringLength(20)]
        public string Dni { get; set; }
        [Required]
        [Display(Name = "Fecha de nacimiento")]
        [DataType(DataType.Date)]
        public DateTime FechaDeNacimiento { get; set; }
        [Required]
        [EmailAddress]
        [StringLength(200)]
        public string Email { get; set; }
        [Required]
        public int SexoId { get; set; }
        [Required]
        public int DireccionId { get; set; }
        public Sexo Sexo { get; set; }
        public Direccion Direccion { get; set; }
        public List<Curso> Cursos { get; set; }
    }
}
