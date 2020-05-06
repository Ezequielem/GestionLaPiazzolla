using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using GestionLaPiazzolla.Data;
using GestionLaPiazzolla.Models;
using Microsoft.AspNetCore.Authorization;

namespace GestionLaPiazzolla.Controllers
{
    [Authorize]
    public class AlumnosController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AlumnosController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Alumnos
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Alumnos.Include(a => a.Direccion).Include(a => a.Sexo);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Alumnos/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var alumno = await _context.Alumnos
                .Include(a => a.Direccion)
                .Include(a => a.Sexo)
                .FirstOrDefaultAsync(m => m.AlumnoId == id);
            if (alumno == null)
            {
                return NotFound();
            }

            return View(alumno);
        }

        // GET: Alumnos/Create
        public IActionResult Create()
        {
            ViewData["DireccionId"] = new SelectList(_context.Direcciones, "DireccionId", "Calle");
            ViewData["SexoId"] = new SelectList(_context.Sexo, "SexoId", "Descripcion");
            return View();
        }

        // POST: Alumnos/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("AlumnoId,Nombre,Apellido,Dni,FechaDeNacimiento,Email,SexoId,DireccionId")] Alumno alumno)
        {
            if (ModelState.IsValid)
            {
                _context.Add(alumno);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["DireccionId"] = new SelectList(_context.Direcciones, "DireccionId", "Calle", alumno.DireccionId);
            ViewData["SexoId"] = new SelectList(_context.Sexo, "SexoId", "Descripcion", alumno.SexoId);
            return View(alumno);
        }

        // GET: Alumnos/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var alumno = await _context.Alumnos.FindAsync(id);
            if (alumno == null)
            {
                return NotFound();
            }
            ViewData["DireccionId"] = new SelectList(_context.Direcciones, "DireccionId", "Calle", alumno.DireccionId);
            ViewData["SexoId"] = new SelectList(_context.Sexo, "SexoId", "Descripcion", alumno.SexoId);
            return View(alumno);
        }

        // POST: Alumnos/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("AlumnoId,Nombre,Apellido,Dni,FechaDeNacimiento,Email,SexoId,DireccionId")] Alumno alumno)
        {
            if (id != alumno.AlumnoId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(alumno);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AlumnoExists(alumno.AlumnoId))
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
            ViewData["DireccionId"] = new SelectList(_context.Direcciones, "DireccionId", "Calle", alumno.DireccionId);
            ViewData["SexoId"] = new SelectList(_context.Sexo, "SexoId", "Descripcion", alumno.SexoId);
            return View(alumno);
        }

        // GET: Alumnos/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var alumno = await _context.Alumnos
                .Include(a => a.Direccion)
                .Include(a => a.Sexo)
                .FirstOrDefaultAsync(m => m.AlumnoId == id);
            if (alumno == null)
            {
                return NotFound();
            }

            return View(alumno);
        }

        // POST: Alumnos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var alumno = await _context.Alumnos.FindAsync(id);
            _context.Alumnos.Remove(alumno);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AlumnoExists(int id)
        {
            return _context.Alumnos.Any(e => e.AlumnoId == id);
        }
    }
}
