using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestionLaPiazzolla.Models
{
    public class Localidad
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int LocalidadId{ get; set; }
        [Required]
        [StringLength(250)]
        public string Nombre { get; set; }
        [Required]
        public int DepartamentoId { get; set; }
        public Departamento Departamento { get; set; }
    }
}
