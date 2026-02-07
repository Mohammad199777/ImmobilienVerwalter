using ImmobilienVerwalter.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ImmobilienVerwalter.Infrastructure.Data.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(u => u.Id);
        builder.Property(u => u.Email).IsRequired().HasMaxLength(256);
        builder.HasIndex(u => u.Email).IsUnique();
        builder.Property(u => u.PasswordHash).IsRequired();
        builder.Property(u => u.FirstName).IsRequired().HasMaxLength(100);
        builder.Property(u => u.LastName).IsRequired().HasMaxLength(100);
        builder.Property(u => u.Phone).HasMaxLength(30);
        builder.Property(u => u.Company).HasMaxLength(200);
        builder.Property(u => u.TaxId).HasMaxLength(50);
    }
}

public class PropertyConfiguration : IEntityTypeConfiguration<Property>
{
    public void Configure(EntityTypeBuilder<Property> builder)
    {
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Name).IsRequired().HasMaxLength(200);
        builder.Property(p => p.Street).IsRequired().HasMaxLength(200);
        builder.Property(p => p.HouseNumber).IsRequired().HasMaxLength(20);
        builder.Property(p => p.ZipCode).IsRequired().HasMaxLength(10);
        builder.Property(p => p.City).IsRequired().HasMaxLength(100);
        builder.Property(p => p.TotalArea).HasPrecision(10, 2);
        builder.Property(p => p.PurchasePrice).HasPrecision(14, 2);

        builder.HasOne(p => p.Owner)
               .WithMany(u => u.Properties)
               .HasForeignKey(p => p.OwnerId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(p => p.Units)
               .WithOne(u => u.Property)
               .HasForeignKey(u => u.PropertyId);
    }
}

public class UnitConfiguration : IEntityTypeConfiguration<Unit>
{
    public void Configure(EntityTypeBuilder<Unit> builder)
    {
        builder.HasKey(u => u.Id);
        builder.Property(u => u.Name).IsRequired().HasMaxLength(200);
        builder.Property(u => u.Area).HasPrecision(10, 2);
        builder.Property(u => u.TargetRent).HasPrecision(10, 2);
        builder.Ignore(u => u.ActiveLease);
    }
}

public class TenantConfiguration : IEntityTypeConfiguration<Tenant>
{
    public void Configure(EntityTypeBuilder<Tenant> builder)
    {
        builder.HasKey(t => t.Id);
        builder.Property(t => t.FirstName).IsRequired().HasMaxLength(100);
        builder.Property(t => t.LastName).IsRequired().HasMaxLength(100);
        builder.Property(t => t.Email).IsRequired().HasMaxLength(256);
        builder.Property(t => t.Iban).HasMaxLength(34);
        builder.Property(t => t.MonthlyIncome).HasPrecision(10, 2);
    }
}

public class LeaseConfiguration : IEntityTypeConfiguration<Lease>
{
    public void Configure(EntityTypeBuilder<Lease> builder)
    {
        builder.HasKey(l => l.Id);
        builder.Property(l => l.ColdRent).HasPrecision(10, 2);
        builder.Property(l => l.AdditionalCosts).HasPrecision(10, 2);
        builder.Property(l => l.DepositAmount).HasPrecision(10, 2);
        builder.Property(l => l.DepositPaid).HasPrecision(10, 2);
        builder.Ignore(l => l.TotalRent);
        builder.Ignore(l => l.DepositFullyPaid);
        builder.Ignore(l => l.IsActive);

        builder.HasOne(l => l.Tenant)
               .WithMany(t => t.Leases)
               .HasForeignKey(l => l.TenantId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(l => l.Unit)
               .WithMany(u => u.Leases)
               .HasForeignKey(l => l.UnitId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}

public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> builder)
    {
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Amount).HasPrecision(10, 2);
        builder.Property(p => p.ExpectedAmount).HasPrecision(10, 2);
        builder.Ignore(p => p.Difference);

        builder.HasOne(p => p.Lease)
               .WithMany(l => l.Payments)
               .HasForeignKey(p => p.LeaseId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(p => new { p.PaymentYear, p.PaymentMonth });
    }
}

public class ExpenseConfiguration : IEntityTypeConfiguration<Expense>
{
    public void Configure(EntityTypeBuilder<Expense> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Title).IsRequired().HasMaxLength(300);
        builder.Property(e => e.Amount).HasPrecision(12, 2);

        builder.HasOne(e => e.Property)
               .WithMany(p => p.Expenses)
               .HasForeignKey(e => e.PropertyId)
               .OnDelete(DeleteBehavior.SetNull);
    }
}

public class MeterReadingConfiguration : IEntityTypeConfiguration<MeterReading>
{
    public void Configure(EntityTypeBuilder<MeterReading> builder)
    {
        builder.HasKey(m => m.Id);
        builder.Property(m => m.MeterNumber).IsRequired().HasMaxLength(50);
        builder.Property(m => m.Value).HasPrecision(14, 4);
        builder.Property(m => m.PreviousValue).HasPrecision(14, 4);
        builder.Ignore(m => m.Consumption);

        builder.HasOne(m => m.Unit)
               .WithMany(u => u.MeterReadings)
               .HasForeignKey(m => m.UnitId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}

public class DocumentConfiguration : IEntityTypeConfiguration<Document>
{
    public void Configure(EntityTypeBuilder<Document> builder)
    {
        builder.HasKey(d => d.Id);
        builder.Property(d => d.FileName).IsRequired().HasMaxLength(500);
        builder.Property(d => d.OriginalFileName).IsRequired().HasMaxLength(500);
        builder.Property(d => d.ContentType).IsRequired().HasMaxLength(100);
        builder.Property(d => d.StoragePath).IsRequired().HasMaxLength(1000);

        builder.HasOne(d => d.UploadedBy)
               .WithMany()
               .HasForeignKey(d => d.UploadedById)
               .OnDelete(DeleteBehavior.Restrict);
    }
}
