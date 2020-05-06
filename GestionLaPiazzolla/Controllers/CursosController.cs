using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using GestionLaPiazzolla.Data;
using GestionLaPiazzolla.Models;
using GestionLaPiazzolla.Models.LaPiazzollaViewModels;
using Microsoft.AspNetCore.Authorization;

namespace GestionLaPiazzolla.Controllers
{
    [Authorize]
    public class CursosController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CursosController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Cursos
        public async Task<IActionResult> Index(int? id, int? alumnoId)
        {
            var ViewModel = new CursoIndexData();
            ViewModel.Cursos = await _context.Cursos
                .Include(p => p.Profesor)
                    .ThenInclude(p => p.Direccion)
                .Include(p => p.Alumnos_X_Cursos)
                    .ThenInclude(p => p.Alumno)
                        .ThenInclude(p => p.Direccion)
                .AsNoTracking()
                .OrderBy(p => p.Nombre)
                .ToListAsync();
            //var cursos = _context.Cursos.Include(c=>c.Profesor).AsNoTracking();

            if (id != null)
            {
                ViewData["CursoId"] = id.Value;
                Curso curso = ViewModel.Cursos.Where(c => c.CursoId == id.Value).Single();
                ViewModel.Alumnos = curso.Alumnos_X_Cursos.Select(s => s.Alumno);
            }

            if (alumnoId != null)
            {
                ViewData["AlumnoId"] = alumnoId.Value;
                ViewModel.AlumnosCursos = ViewModel.Alumnos.Where(a => a.AlumnoId == alumnoId).Single().Alumnos_X_Cursos;
            }

            return View(ViewModel/*await _context.Cursos.ToListAsync()*/);
        }

        // GET: Cursos/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var curso = await _context.Cursos.Include(p => p.Profesor).AsNoTracking()
                .FirstOrDefaultAsync(m => m.CursoId == id);
            if (curso == null)
            {
                return NotFound();
            }

            return View(curso);
        }

        public IActionResult Create()
        {
            listadoDeProfesores();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CursoId,Nombre,PrecioMensual,Descripcion,ProfesorId")] Curso curso)
        {
            if (ModelState.IsValid)
            {
                _context.Add(curso);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            listadoDeProfesores(curso.ProfesorId);
            return View(curso);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var curso = await _context.Cursos.AsNoTracking().FirstOrDefaultAsync(c => c.CursoId == id);
            if (curso == null)
            {
                return NotFound();
            }
            listadoDeProfesores(curso.ProfesorId);
            return View(curso);
        }

        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPost(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var cursoAModdificar = await _context.Cursos.FirstOrDefaultAsync(c => c.CursoId == id);
            if (await TryUpdateModelAsync<Curso>(cursoAModdificar, "", c => c.Nombre, c => c.PrecioMensual, c => c.Descripcion, c => c.Profesor))
            {
                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateException /* ex */)
                {
                    //Log the error (uncomment ex variable name and write a log.)
                    ModelState.AddModelError("", "Unable to save changes. " +
                "Try again, and if the problem persists, " +
                "see your system administrator.");
                }
                return RedirectToAction(nameof(Index));
            }
            listadoDeProfesores(cursoAModdificar.ProfesorId);
            return View(cursoAModdificar);
        }

        // GET: Cursos/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var curso = await _context.Cursos.Include(p => p.Profesor).AsNoTracking()
                .FirstOrDefaultAsync(m => m.CursoId == id);
            if (curso == null)
            {
                return NotFound();
            }

            return View(curso);
        }

        // POST: Cursos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var curso = await _context.Cursos.FindAsync(id);
            _context.Cursos.Remove(curso);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CursoExists(int id)
        {
            return _context.Cursos.Any(e => e.CursoId == id);
        }

        public void listadoDeProfesores(object profesorSeleccionado = null)
        {
            var consultaProfesores = from p in _context.Profesores
                                     orderby p.Nombre
                                     select p;
            ViewBag.ProfesorId = new SelectList(consultaProfesores.AsNoTracking(), "ProfesorId", "Nombre", profesorSeleccionado);
        }
    }
}
