using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GestionLaPiazzolla.Models
{
    public class DetalleDePago
    {
        public int DetalleDePagoId { get; set; }
        [Required]
        public string Concepto { get; set; }
        [Required]
        public int Cantidad { get; set; }
        [Required]
        public int PagoId { get; set; }
        public Pago Pago { get; set; }
    }
}
