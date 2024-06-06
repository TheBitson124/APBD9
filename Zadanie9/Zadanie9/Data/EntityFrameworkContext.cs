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
            entity.HasKey(e => e.IdClient).HasName("ClientPrimaryKey");
            entity.ToTable("Client");
            entity.Property(e => e.Email).HasMaxLength(120);
            entity.Property(e => e.FirstName).HasMaxLength(120);
            entity.Property(e => e.LastName).HasMaxLength(120);
            entity.Property(e => e.Pesel).HasMaxLength(120);
            entity.Property(e => e.Telephone).HasMaxLength(120);
        });

        modelBuilder.Entity<ClientTrip>(entity =>
        {
            entity.HasKey(e => new { e.IdClient, e.IdTrip }).HasName("ClientTripPrimaryKey");
            entity.ToTable("Client_Trip");
            entity.Property(e => e.PaymentDate).HasColumnType("datetime");
            entity.Property(e => e.RegisteredAt).HasColumnType("datetime");
            entity.HasOne(d => d.IdClientNavigation).WithMany(p => p.ClientTrips)
                .HasForeignKey(d => d.IdClient)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Table_5_Client");
            entity.HasOne(d => d.IdTripNavigation).WithMany(p => p.ClientTrips)
                .HasForeignKey(d => d.IdTrip)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Table_5_Trip");
        });

        modelBuilder.Entity<Country>(entity =>
        {
            entity.HasKey(e => e.IdCountry).HasName("CountryPrimaryKey");
            entity.ToTable("Country");
            entity.Property(e => e.Name).HasMaxLength(120);
            entity.HasMany(e => e.IdTrips).WithMany(c => c.IdCountries)
                .UsingEntity<Dictionary<string, object>>(
                "Country_Trip",
                cr => cr.HasOne<Trip>().WithMany()
                    .HasForeignKey("IdTrip")
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Country_Trip_Trip"),
                tr => tr.HasOne<Country>().WithMany()
                    .HasForeignKey("IdCountry")
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Country_Trip_Country"),
                join =>
                {
                    join.HasKey("IdCountry", "IdTrip").HasName("CountryTripPrimaryKey");
                    join.ToTable("Country_Trip");
                });
        });
        modelBuilder.Entity<Trip>(entity =>
        {
            entity.HasKey(e => e.IdTrip).HasName("TripPrimaryKey");

            entity.ToTable("Trip");

            entity.Property(e => e.DateFrom).HasColumnType("datetime");
            entity.Property(e => e.DateTo).HasColumnType("datetime");
            entity.Property(e => e.Description).HasMaxLength(220);
            entity.Property(e => e.Name).HasMaxLength(120);
        });
        OnModelCreatingPartial(modelBuilder);
    }
    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}