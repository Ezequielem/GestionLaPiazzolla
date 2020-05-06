using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GestionLaPiazzolla.Models.LaPiazzollaViewModels
{
    public class CursoIndexData
    {
        public IEnumerable<Curso> Cursos { get; set; }
        public IEnumerable<Alumno> Alumnos { get; set; }
        public IEnumerable<Alumno_x_Curso> AlumnosCursos { get; set; }
    }
}
