namespace ImmobilienVerwalter.Core.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IPropertyRepository Properties { get; }
    IUnitRepository Units { get; }
    ITenantRepository Tenants { get; }
    ILeaseRepository Leases { get; }
    IPaymentRepository Payments { get; }
    IExpenseRepository Expenses { get; }
    IMeterReadingRepository MeterReadings { get; }
    IDocumentRepository Documents { get; }
    IUserRepository Users { get; }

    Task<int> SaveChangesAsync();
}
