using HotelSuite.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace HotelSuite.Infrastructure.Data;

public class HotelDbContext : IdentityDbContext<ApplicationUser>
{
    public HotelDbContext(DbContextOptions<HotelDbContext> options) : base(options)
    {
    }

    // DbSets
    public DbSet<Hotel> Hoteles { get; set; }
    public DbSet<Habitacion> Habitaciones { get; set; }
    public DbSet<Huesped> Huespedes { get; set; }
    public DbSet<Reserva> Reservas { get; set; }
    public DbSet<Pago> Pagos { get; set; }
    public DbSet<Empleado> Empleados { get; set; }
    public DbSet<Departamento> Departamentos { get; set; }
    public DbSet<TareaDepartamento> TareasDepartamento { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
 // Llamar a la configuración base de Identity
   base.OnModelCreating(modelBuilder);

      // Configuración de entidades
        
        // Hotel
        modelBuilder.Entity<Hotel>(entity =>
        {
            entity.HasKey(h => h.Id);
            entity.Property(h => h.Nombre).IsRequired().HasMaxLength(200);
     entity.Property(h => h.Direccion).IsRequired().HasMaxLength(300);
         entity.Property(h => h.Telefono).IsRequired().HasMaxLength(20);
        entity.Property(h => h.Categoria).IsRequired();
 });

 // Habitacion
        modelBuilder.Entity<Habitacion>(entity =>
      {
            entity.HasKey(h => h.Id);
        entity.Property(h => h.Numero).IsRequired().HasMaxLength(10);
  entity.Property(h => h.Tipo).IsRequired().HasMaxLength(50);
        entity.Property(h => h.PrecioPorNoche).HasColumnType("decimal(18,2)");
      entity.Property(h => h.Estado).IsRequired().HasMaxLength(50);

     entity.HasOne(h => h.Hotel)
      .WithMany(hotel => hotel.Habitaciones)
     .HasForeignKey(h => h.IdHotel)
       .OnDelete(DeleteBehavior.Restrict);
        });

        // Huesped
        modelBuilder.Entity<Huesped>(entity =>
     {
      entity.HasKey(h => h.Id);
            entity.Property(h => h.Nombres).IsRequired().HasMaxLength(100);
            entity.Property(h => h.Apellidos).IsRequired().HasMaxLength(100);
         entity.Property(h => h.Email).IsRequired().HasMaxLength(150);
         entity.Property(h => h.Telefono).IsRequired().HasMaxLength(20);
        entity.Property(h => h.DocumentoIdentidad).IsRequired().HasMaxLength(50);

            entity.HasIndex(h => h.Email).IsUnique();
            entity.HasIndex(h => h.DocumentoIdentidad).IsUnique();
        });

        // Reserva
        modelBuilder.Entity<Reserva>(entity =>
        {
 entity.HasKey(r => r.Id);
            entity.Property(r => r.FechaReserva).IsRequired();
    entity.Property(r => r.FechaEntrada).IsRequired();
            entity.Property(r => r.FechaSalida).IsRequired();
            entity.Property(r => r.Estado).IsRequired().HasMaxLength(50);

  entity.HasOne(r => r.Huesped)
           .WithMany(h => h.Reservas)
      .HasForeignKey(r => r.IdHuesped)
                .OnDelete(DeleteBehavior.Restrict);

     entity.HasOne(r => r.Habitacion)
    .WithMany(h => h.Reservas)
     .HasForeignKey(r => r.IdHabitacion)
    .OnDelete(DeleteBehavior.Restrict);
     });

        // Pago
        modelBuilder.Entity<Pago>(entity =>
   {
  entity.HasKey(p => p.Id);
            entity.Property(p => p.Monto).HasColumnType("decimal(18,2)");
            entity.Property(p => p.FechaPago).IsRequired();
  entity.Property(p => p.Metodo).IsRequired().HasMaxLength(50);

            entity.HasOne(p => p.Reserva)
      .WithMany(r => r.Pagos)
      .HasForeignKey(p => p.IdReserva)
   .OnDelete(DeleteBehavior.Restrict);
});

        // Departamento
        modelBuilder.Entity<Departamento>(entity =>
        {
    entity.HasKey(d => d.Id);
    entity.Property(d => d.Nombre).IsRequired().HasMaxLength(100);
            entity.Property(d => d.Descripcion).HasMaxLength(500);
      });

        // Empleado
        modelBuilder.Entity<Empleado>(entity =>
        {
            entity.HasKey(e => e.Id);
        entity.Property(e => e.Nombres).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Apellidos).IsRequired().HasMaxLength(100);
  entity.Property(e => e.Cargo).IsRequired().HasMaxLength(100);
 entity.Property(e => e.Email).IsRequired().HasMaxLength(150);
    entity.Property(e => e.Telefono).IsRequired().HasMaxLength(20);

     entity.HasIndex(e => e.Email).IsUnique();

    entity.HasOne(e => e.Departamento)
   .WithMany(d => d.Empleados)
    .HasForeignKey(e => e.IdDepartamento)
    .OnDelete(DeleteBehavior.Restrict);
        });

        // TareaDepartamento
        modelBuilder.Entity<TareaDepartamento>(entity =>
     {
entity.HasKey(t => t.Id);
      entity.Property(t => t.Titulo).IsRequired().HasMaxLength(200);
 entity.Property(t => t.Descripcion).HasMaxLength(1000);
    entity.Property(t => t.Estado).IsRequired().HasMaxLength(50);
      entity.Property(t => t.Prioridad).IsRequired().HasMaxLength(50);
  entity.Property(t => t.FechaAsignacion).IsRequired();

 entity.HasOne(t => t.Departamento)
  .WithMany(d => d.TareasDepartamento)
      .HasForeignKey(t => t.IdDepartamento)
         .OnDelete(DeleteBehavior.Restrict);

       entity.HasOne(t => t.Empleado)
.WithMany(e => e.TareasDepartamento)
       .HasForeignKey(t => t.IdEmpleadoAsignado)
  .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
