using Microsoft.EntityFrameworkCore;
using OrderManagement.Api.Models;

namespace OrderManagement.Api.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<CompanyProfile> CompanyProfiles => Set<CompanyProfile>();
    public DbSet<D100Declaration> D100Declarations => Set<D100Declaration>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var order = modelBuilder.Entity<Order>();

        order.Property(o => o.CustomerName)
            .HasMaxLength(200)
            .IsRequired();

        order.Property(o => o.Product)
            .HasMaxLength(200)
            .IsRequired();

        order.Property(o => o.Quantity)
            .IsRequired();

        order.Property(o => o.Price)
            .HasPrecision(18, 2);

        order.Property(o => o.Status)
            .HasMaxLength(50)
            .IsRequired();

        order.Property(o => o.Notes)
            .HasMaxLength(1000);

        order.Property(o => o.CreatedAt)
            .IsRequired();

        order.HasIndex(o => o.CreatedAt);
        order.HasIndex(o => o.Status);

        var company = modelBuilder.Entity<CompanyProfile>();
        company.Property(c => c.Cui).HasMaxLength(13).IsRequired();
        company.Property(c => c.Denumire).HasMaxLength(200).IsRequired();
        company.Property(c => c.Adresa).HasMaxLength(1000).IsRequired();
        company.Property(c => c.Telefon).HasMaxLength(15);
        company.Property(c => c.Email).HasMaxLength(200);
        company.Property(c => c.NumeDeclar).HasMaxLength(75).IsRequired();
        company.Property(c => c.PrenumeDeclar).HasMaxLength(75).IsRequired();
        company.Property(c => c.FunctieDeclar).HasMaxLength(50).IsRequired();

        var declaration = modelBuilder.Entity<D100Declaration>();
        declaration.Property(d => d.TotalSales).HasPrecision(18, 2);
        declaration.Property(d => d.CodOblig).HasMaxLength(3).IsRequired();
        declaration.Property(d => d.CodBugetar).HasMaxLength(10).IsRequired();
        declaration.Property(d => d.Scadenta).HasMaxLength(10).IsRequired();
        declaration.Property(d => d.NrEvidenta).HasMaxLength(23).IsRequired();
        declaration.Property(d => d.XmlPath).HasMaxLength(500);
        declaration.Property(d => d.PdfPath).HasMaxLength(500);
        declaration.Property(d => d.ValidationErrors).HasMaxLength(4000);
        declaration.HasIndex(d => new { d.Luna, d.An }).IsUnique();
    }
}
