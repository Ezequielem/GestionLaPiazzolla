using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestionLaPiazzolla.Models
{
    public class Provincia
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ProvinciaId { get; set; }
        [Required]
        [StringLength(30)]
        public string Nombre { get; set; }
        public List<Departamento> Departamentos { get; set; }       
    }
}
