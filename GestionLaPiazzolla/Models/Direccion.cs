using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace GestionLaPiazzolla.Models
{
    public class Direccion
    {
        public int DireccionId { get; set; }
        [Required]
        [StringLength(200)]
        public string Calle { get; set; }
        [Required]
        public int Altura { get; set; }
        public string Piso { get; set; }
        [Display(Name = "Depto")]
        public string Departamento { get; set; }
        [Required]
        [StringLength(20)]
        [Display(Name = "Codigo Postal")]
        public string CodigoPostal { get; set; }
        [Required]
        public int LocalidadId { get; set; }
        [ValidateNever]
        public Localidad Localidad { get; set; }


    }
}
