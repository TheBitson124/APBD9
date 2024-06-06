using Microsoft.EntityFrameworkCore;
using Zadanie9.DTOs;

namespace Zadanie9.Data;

public partial class EntityFrameworkContext:DbContext
{
    public EntityFrameworkContext(){}
    public EntityFrameworkContext(DbContextOptions<EntityFrameworkContext> options) : base(options) {}

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) => optionsBuilder.UseSqlServer("Name=ConnectionStrings:Default");
    
    
    public virtual DbSet<Client> Clients { get; set; }

    public virtual DbSet<ClientTrip> ClientTrips { get; set; }

    public virtual DbSet<Country> Countries { get; set; }

    public virtual DbSet<Trip> Trips { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Client>(entity =>
        {
            entity.HasKey(e => e.IdClient).HasName("Client_pk");
            entity.ToTable("Client");
            entity.Property(e => e.Email).HasMaxLength(120);
            entity.Property(e => e.FirstName).HasMaxLength(120);
            entity.Property(e => e.LastName).HasMaxLength(120);
            entity.Property(e => e.Pesel).HasMaxLength(120);
            entity.Property(e => e.Telephone).HasMaxLength(120);
        });
        
        
        
        
        OnModelCreatingPartial(modelBuilder);
    }
    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}