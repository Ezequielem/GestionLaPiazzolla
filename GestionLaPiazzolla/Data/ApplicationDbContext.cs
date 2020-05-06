using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using GestionLaPiazzolla.Models;

namespace GestionLaPiazzolla.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Alumno_x_Curso>().HasKey(x => new { x.AlumnoId, x.CursoId });
            modelBuilder.Entity<Alumno_x_Curso>().HasOne(x => x.Alumno).WithMany(z => z.Alumnos_X_Cursos).OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Alumno_x_Curso>().HasOne(x => x.Curso).WithMany(z => z.Alumnos_X_Cursos).OnDelete(DeleteBehavior.Restrict);
        }

        public DbSet<Sexo> Sexo { get; set; }
        public DbSet<Direccion> Direcciones { get; set; }
        public DbSet<Pago> Pagos { get; set; }
        public DbSet<Alumno> Alumnos { get; set; }
        public DbSet<Curso> Cursos { get; set; }
        public DbSet<Alumno_x_Curso> Alumnos_X_Cursos { get; set; }
        public DbSet<Profesor> Profesores { get; set; }
        public DbSet<Provincia> Provincias { get; set; }
        public DbSet<Departamento> Departamentos { get; set; }
        public DbSet<Localidad> Localidades { get; set; }
    }
}
