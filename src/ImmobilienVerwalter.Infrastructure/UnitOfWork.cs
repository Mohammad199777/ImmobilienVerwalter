using ImmobilienVerwalter.Core.Interfaces;
using ImmobilienVerwalter.Infrastructure.Data;
using ImmobilienVerwalter.Infrastructure.Repositories;

namespace ImmobilienVerwalter.Infrastructure;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;

    public UnitOfWork(AppDbContext context)
    {
        _context = context;
        Properties = new PropertyRepository(context);
        Units = new UnitRepository(context);
        Tenants = new TenantRepository(context);
        Leases = new LeaseRepository(context);
        Payments = new PaymentRepository(context);
        Expenses = new ExpenseRepository(context);
        MeterReadings = new MeterReadingRepository(context);
        Documents = new DocumentRepository(context);
        Users = new UserRepository(context);
    }

    public IPropertyRepository Properties { get; }
    public IUnitRepository Units { get; }
    public ITenantRepository Tenants { get; }
    public ILeaseRepository Leases { get; }
    public IPaymentRepository Payments { get; }
    public IExpenseRepository Expenses { get; }
    public IMeterReadingRepository MeterReadings { get; }
    public IDocumentRepository Documents { get; }
    public IUserRepository Users { get; }

    public async Task<int> SaveChangesAsync()
        => await _context.SaveChangesAsync();

    public void Dispose()
        => _context.Dispose();
}
