using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace GestionLaPiazzolla.Models
{
    public class Sexo
    {
        public int SexoId { get; set; }
        [Required]
        [StringLength(20)]
        public string Descripcion { get; set; }
    }
}
