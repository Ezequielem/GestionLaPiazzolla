using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestionLaPiazzolla.Models
{
    public class Pago
    {
        public int PagoId { get; set; }
        [Required]
        [DataType(DataType.Currency)]
        [Column(TypeName = "money")]
        public decimal Monto { get; set; }
        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Fecha de pago")]
        public DateTime FechaPago { get; set; }
        [StringLength(500)]
        public string Observacion { get; set; }
        [Required]
        public int AlumnoId { get; set; }
        public Alumno Alumno { get; set; }
    }
}
