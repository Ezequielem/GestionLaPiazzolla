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
        public async Task<IActionResult> Index(string sortOrder, string cadenaBusqueda)
        {
            ViewData["NombreSortOrder"] = String.IsNullOrEmpty(sortOrder) ? "nombre_desc" : "";
            ViewData["PrecioMensualSortOrder"] = sortOrder == "PrecioMensual" ? "precio_desc" : "PrecioMensual";
            ViewData["DescripcionSortOrder"] = sortOrder == "Descripcion" ? "descrip_desc" : "Descripcion";
            ViewData["ProfesorSortOrder"] = sortOrder == "Profesor" ? "prof_desc" : "Profesor";
            ViewData["FiltroActual"] = cadenaBusqueda;
            var vistaModelo = new CursoIndexData();
            vistaModelo.Cursos = await _context.Cursos
                .Include(p => p.Profesor)
                    .ThenInclude(p => p.Direccion)
                .Include(p => p.Alumnos_X_Cursos)
                    .ThenInclude(p => p.Alumno)
                        .ThenInclude(p => p.Direccion)
                .AsNoTracking()
                .OrderBy(p => p.Nombre)
                .ToListAsync();
            if (!String.IsNullOrEmpty(cadenaBusqueda))
            {
                vistaModelo.Cursos = vistaModelo.Cursos.Where(c => c.Nombre.Contains(cadenaBusqueda));
            }
            switch (sortOrder)
            {
                case "nombre_desc":
                    vistaModelo.Cursos = vistaModelo.Cursos.OrderByDescending(c => c.Nombre);
                    break;
                case "PrecioMensual":
                    vistaModelo.Cursos = vistaModelo.Cursos.OrderBy(c => c.PrecioMensual);
                    break;
                case "precio_desc":
                    vistaModelo.Cursos = vistaModelo.Cursos.OrderByDescending(c => c.PrecioMensual);
                    break;
                case "Descripcion":
                    vistaModelo.Cursos = vistaModelo.Cursos.OrderBy(c => c.Descripcion);
                    break;
                case "descrip_desc":
                    vistaModelo.Cursos = vistaModelo.Cursos.OrderByDescending(c => c.Descripcion);
                    break;
                case "Profesor":
                    vistaModelo.Cursos = vistaModelo.Cursos.OrderBy(c => c.Profesor.Apellido);
                    break;
                case "prof_desc":
                    vistaModelo.Cursos = vistaModelo.Cursos.OrderByDescending(c => c.Profesor.Apellido);
                    break;
                default:
                    vistaModelo.Cursos = vistaModelo.Cursos.OrderBy(c => c.Nombre);
                    break;
            }
            return View(vistaModelo);
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
            if (await TryUpdateModelAsync<Curso>(cursoAModdificar, "", c => c.Nombre, c => c.PrecioMensual, c => c.Descripcion, c => c.ProfesorId))
            {
                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateException /* ex */)
                {
                    //Log the error (uncomment ex variable name and write a log.)
                    ModelState.AddModelError("", "No se pueden guardar los cambios. " +
                "Intente nuevamente, y si el problema persiste, " +
                "consulte a su administrador del sistema.");
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
                                     select new { 
                                     ProfeId=p.ProfesorId,
                                     NombreApellido=p.Nombre + " " + p.Apellido
                                     };
            ViewBag.ProfesorId = new SelectList(consultaProfesores.AsNoTracking(), "ProfeId", "NombreApellido", profesorSeleccionado);
        }
    }
}
