using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using GestionLaPiazzolla.Data;
using GestionLaPiazzolla.Models;

namespace GestionLaPiazzolla.Controllers
{
    public class InscripcionesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public InscripcionesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Inscripciones
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Alumnos_X_Cursos.Include(a => a.Alumno).Include(a => a.Curso);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Inscripciones/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var alumno_x_Curso = await _context.Alumnos_X_Cursos
                .Include(a => a.Alumno)
                .Include(a => a.Curso)
                .FirstOrDefaultAsync(m => m.AlumnoId == id);
            if (alumno_x_Curso == null)
            {
                return NotFound();
            }

            return View(alumno_x_Curso);
        }

        // GET: Inscripciones/Create
        public IActionResult Create()
        {
            listaAlumnos();
            listaCursos();
            return View();
        }

        // POST: Inscripciones/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("AlumnoId,CursoId,FechaInscripcion,Activo,Observacion")] Alumno_x_Curso alumno_x_Curso)
        {
            if (Alumno_x_CursoExists(alumno_x_Curso.AlumnoId, alumno_x_Curso.CursoId))
            {
                ModelState.AddModelError("", "No se pueden guardar los cambios. " +
                "El Alumno, ya se encuentra inscripto en este Curso.");
            }            
            if (ModelState.IsValid)
            {
                alumno_x_Curso.FechaInscripcion = DateTime.Now;
                alumno_x_Curso.Activo = true;
                _context.Add(alumno_x_Curso);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            listaAlumnos(alumno_x_Curso);
            listaCursos(alumno_x_Curso);
            return View(alumno_x_Curso);
        }

        // GET: Inscripciones/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var alumno_x_Curso = await _context.Alumnos_X_Cursos.FindAsync(id);
            if (alumno_x_Curso == null)
            {
                return NotFound();
            }
            listaAlumnos();
            listaCursos();
            return View(alumno_x_Curso);
        }

        // POST: Inscripciones/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("AlumnoId,CursoId,FechaInscripcion,Activo,Observacion")] Alumno_x_Curso alumno_x_Curso)
        {
            if (id != alumno_x_Curso.AlumnoId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(alumno_x_Curso);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!Alumno_x_CursoExists(alumno_x_Curso.AlumnoId, alumno_x_Curso.CursoId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            listaAlumnos(alumno_x_Curso);
            listaCursos(alumno_x_Curso);
            return View(alumno_x_Curso);
        }

        // GET: Inscripciones/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var alumno_x_Curso = await _context.Alumnos_X_Cursos
                .Include(a => a.Alumno)
                .Include(a => a.Curso)
                .FirstOrDefaultAsync(m => m.AlumnoId == id);
            if (alumno_x_Curso == null)
            {
                return NotFound();
            }

            return View(alumno_x_Curso);
        }

        // POST: Inscripciones/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var alumno_x_Curso = await _context.Alumnos_X_Cursos.FindAsync(id);
            _context.Alumnos_X_Cursos.Remove(alumno_x_Curso);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool Alumno_x_CursoExists(int alumnoId, int cursoId)
        {
            return _context.Alumnos_X_Cursos.Any(e => e.AlumnoId == alumnoId && e.CursoId == cursoId);
        }

        private void listaAlumnos(object alumnoSeleccionado = null)
        {
            var consultaAlumno = from a in _context.Alumnos
                                 orderby a.Apellido
                                 select new
                                 {
                                     Identificador=a.AlumnoId,
                                     NombreApellido=a.Nombre + " " + a.Apellido
                                 };
            ViewBag.AlumnoId = new SelectList(consultaAlumno.AsNoTracking(), "Identificador", "NombreApellido", alumnoSeleccionado);
        }

        private void listaCursos(object cursoSeleccionado = null)
        {
            var consultaCurso = from c in _context.Cursos
                                orderby c.Nombre
                                select c;
            ViewBag.CursoId = new SelectList(consultaCurso.AsNoTracking(), "CursoId", "Nombre", cursoSeleccionado);
        }
    }
}
