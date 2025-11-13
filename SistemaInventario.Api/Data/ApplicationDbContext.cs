// Data/ApplicationDbContext.cs (Versión COMPLETA)
using Microsoft.EntityFrameworkCore;
using SistemaInventario.Api.Models;

namespace SistemaInventario.Api.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // --- AÑADIR TODOS LOS DbSet ---
        public DbSet<Producto> Productos { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Rol> Roles { get; set; }
        public DbSet<Proveedor> Proveedores { get; set; }

        // --- LOS DbSet FALTANTES QUE CAUSARON LOS ERRORES ---
        public DbSet<ProductoProveedor> ProductosProveedores { get; set; }
        public DbSet<MovimientoInventario> MovimientosInventario { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Mapeo para Producto
            modelBuilder.Entity<Producto>(entity =>
            {
                entity.HasKey(e => e.ProductoID);
                entity.HasIndex(e => e.SKU).IsUnique();
            });

            // Mapeo para Usuario
            modelBuilder.Entity<Usuario>(entity =>
            {
                entity.HasKey(e => e.UsuarioID);
                entity.HasIndex(e => e.Email).IsUnique();

                entity.HasOne(u => u.Rol)
                      .WithMany(r => r.Usuarios)
                      .HasForeignKey(u => u.RoleID);
            });

            // Mapeo para Rol
            modelBuilder.Entity<Rol>(entity =>
            {
                entity.HasKey(e => e.RoleID);
                entity.HasIndex(e => e.NombreRol).IsUnique();
            });

            // Mapeo para Proveedor
            modelBuilder.Entity<Proveedor>(entity =>
            {
                entity.HasKey(e => e.ProveedorID);
                entity.HasIndex(e => e.NombreEmpresa).IsUnique();
            });

            // Mapeo para ProductoProveedor (Tabla de unión)
            modelBuilder.Entity<ProductoProveedor>(entity =>
            {
                // Define la clave primaria compuesta
                entity.HasKey(pp => new { pp.ProductoID, pp.ProveedorID });

                // Relación con Producto
                entity.HasOne(pp => pp.Producto)
                      .WithMany(p => p.ProductosProveedores) 
                      .HasForeignKey(pp => pp.ProductoID);

                // Relación con Proveedor
                entity.HasOne(pp => pp.Proveedor)
                      .WithMany(pr => pr.ProductosProveedores)
                      .HasForeignKey(pp => pp.ProveedorID);
            });

            // Mapeo para MovimientoInventario
            modelBuilder.Entity<MovimientoInventario>(entity =>
            {
                entity.HasKey(e => e.MovimientoID);
                entity.Property(e => e.FechaHora).HasDefaultValueSql("GETDATE()");

                // Relaciones
                entity.HasOne(mi => mi.Producto)
                      .WithMany() // Un Producto puede tener muchos movimientos
                      .HasForeignKey(mi => mi.ProductoID);

                entity.HasOne(mi => mi.Usuario)
                      .WithMany() // Un Usuario puede registrar muchos movimientos
                      .HasForeignKey(mi => mi.UsuarioID); // Asegúrate que coincida con tu modelo

                entity.HasOne(mi => mi.Proveedor)
                      .WithMany(pr => pr.MovimientosInventario) // Un Proveedor puede tener muchos movimientos
                      .HasForeignKey(mi => mi.ProveedorID);
            });
        }
    }
}