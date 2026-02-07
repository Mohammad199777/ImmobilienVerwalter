using ImmobilienVerwalter.Core.Entities;
using ImmobilienVerwalter.Core.Interfaces;
using ImmobilienVerwalter.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ImmobilienVerwalter.Infrastructure.Repositories;

public class PropertyRepository : Repository<Property>, IPropertyRepository
{
    public PropertyRepository(AppDbContext context) : base(context) { }

    public async Task<IEnumerable<Property>> GetByOwnerIdAsync(Guid ownerId)
        => await _dbSet.Where(p => p.OwnerId == ownerId)
                       .Include(p => p.Units)
                       .OrderBy(p => p.Name)
                       .ToListAsync();

    public async Task<Property?> GetWithUnitsAsync(Guid id)
        => await _dbSet.Include(p => p.Units)
                       .FirstOrDefaultAsync(p => p.Id == id);

    public async Task<Property?> GetWithAllDetailsAsync(Guid id)
        => await _dbSet.Include(p => p.Units)
                           .ThenInclude(u => u.Leases)
                               .ThenInclude(l => l.Tenant)
                       .Include(p => p.Units)
                           .ThenInclude(u => u.Leases)
                               .ThenInclude(l => l.Payments)
                       .Include(p => p.Expenses)
                       .FirstOrDefaultAsync(p => p.Id == id);
}

public class UnitRepository : Repository<Unit>, IUnitRepository
{
    public UnitRepository(AppDbContext context) : base(context) { }

    public async Task<IEnumerable<Unit>> GetByPropertyIdAsync(Guid propertyId)
        => await _dbSet.Where(u => u.PropertyId == propertyId)
                       .Include(u => u.Leases.Where(l => l.Status == LeaseStatus.Aktiv))
                           .ThenInclude(l => l.Tenant)
                       .OrderBy(u => u.Name)
                       .ToListAsync();

    public async Task<Unit?> GetWithLeaseAsync(Guid id)
        => await _dbSet.Include(u => u.Leases)
                           .ThenInclude(l => l.Tenant)
                       .Include(u => u.Property)
                       .FirstOrDefaultAsync(u => u.Id == id);

    public async Task<IEnumerable<Unit>> GetVacantUnitsAsync(Guid? propertyId = null)
    {
        var query = _dbSet.Where(u => u.Status == UnitStatus.Leer);
        if (propertyId.HasValue)
            query = query.Where(u => u.PropertyId == propertyId.Value);
        return await query.Include(u => u.Property).ToListAsync();
    }
}

public class TenantRepository : Repository<Tenant>, ITenantRepository
{
    public TenantRepository(AppDbContext context) : base(context) { }

    public async Task<Tenant?> GetWithLeasesAsync(Guid id)
        => await _dbSet.Include(t => t.Leases)
                           .ThenInclude(l => l.Unit)
                               .ThenInclude(u => u.Property)
                       .FirstOrDefaultAsync(t => t.Id == id);

    public async Task<IEnumerable<Tenant>> SearchAsync(string searchTerm)
    {
        var term = searchTerm.ToLower();
        return await _dbSet.Where(t =>
            t.FirstName.ToLower().Contains(term) ||
            t.LastName.ToLower().Contains(term) ||
            t.Email.ToLower().Contains(term))
            .ToListAsync();
    }
}

public class LeaseRepository : Repository<Lease>, ILeaseRepository
{
    public LeaseRepository(AppDbContext context) : base(context) { }

    public async Task<IEnumerable<Lease>> GetByUnitIdAsync(Guid unitId)
        => await _dbSet.Where(l => l.UnitId == unitId)
                       .Include(l => l.Tenant)
                       .OrderByDescending(l => l.StartDate)
                       .ToListAsync();

    public async Task<IEnumerable<Lease>> GetByTenantIdAsync(Guid tenantId)
        => await _dbSet.Where(l => l.TenantId == tenantId)
                       .Include(l => l.Unit).ThenInclude(u => u.Property)
                       .OrderByDescending(l => l.StartDate)
                       .ToListAsync();

    public async Task<IEnumerable<Lease>> GetActiveLeaseAsync()
        => await _dbSet.Where(l => l.Status == LeaseStatus.Aktiv)
                       .Include(l => l.Tenant)
                       .Include(l => l.Unit).ThenInclude(u => u.Property)
                       .ToListAsync();

    public async Task<IEnumerable<Lease>> GetExpiringLeasesAsync(int withinDays)
    {
        var cutoff = DateTime.UtcNow.AddDays(withinDays);
        return await _dbSet.Where(l => l.Status == LeaseStatus.Aktiv &&
                                       l.EndDate != null &&
                                       l.EndDate <= cutoff)
                           .Include(l => l.Tenant)
                           .Include(l => l.Unit)
                           .ToListAsync();
    }

    public async Task<Lease?> GetWithPaymentsAsync(Guid id)
        => await _dbSet.Include(l => l.Payments.OrderByDescending(p => p.PaymentDate))
                       .Include(l => l.Tenant)
                       .Include(l => l.Unit).ThenInclude(u => u.Property)
                       .FirstOrDefaultAsync(l => l.Id == id);
}

public class PaymentRepository : Repository<Payment>, IPaymentRepository
{
    public PaymentRepository(AppDbContext context) : base(context) { }

    public async Task<IEnumerable<Payment>> GetByLeaseIdAsync(Guid leaseId)
        => await _dbSet.Where(p => p.LeaseId == leaseId)
                       .OrderByDescending(p => p.PaymentDate)
                       .ToListAsync();

    public async Task<IEnumerable<Payment>> GetOverduePaymentsAsync()
        => await _dbSet.Where(p => p.Status == PaymentStatus.Ausstehend &&
                                   p.DueDate < DateTime.UtcNow)
                       .Include(p => p.Lease).ThenInclude(l => l.Tenant)
                       .Include(p => p.Lease).ThenInclude(l => l.Unit)
                       .ToListAsync();

    public async Task<IEnumerable<Payment>> GetByMonthAsync(int year, int month)
        => await _dbSet.Where(p => p.PaymentYear == year && p.PaymentMonth == month)
                       .Include(p => p.Lease).ThenInclude(l => l.Tenant)
                       .ToListAsync();

    public async Task<decimal> GetTotalIncomeAsync(int year, int? month = null)
    {
        var query = _dbSet.Where(p => p.PaymentYear == year &&
                                      p.Status == PaymentStatus.Eingegangen);
        if (month.HasValue)
            query = query.Where(p => p.PaymentMonth == month.Value);
        return await query.SumAsync(p => p.Amount);
    }
}

public class ExpenseRepository : Repository<Expense>, IExpenseRepository
{
    public ExpenseRepository(AppDbContext context) : base(context) { }

    public async Task<IEnumerable<Expense>> GetByPropertyIdAsync(Guid propertyId)
        => await _dbSet.Where(e => e.PropertyId == propertyId)
                       .OrderByDescending(e => e.Date)
                       .ToListAsync();

    public async Task<IEnumerable<Expense>> GetByCategoryAsync(ExpenseCategory category)
        => await _dbSet.Where(e => e.Category == category)
                       .OrderByDescending(e => e.Date)
                       .ToListAsync();

    public async Task<decimal> GetTotalExpensesAsync(int year, int? month = null)
    {
        var query = _dbSet.Where(e => e.Date.Year == year);
        if (month.HasValue)
            query = query.Where(e => e.Date.Month == month.Value);
        return await query.SumAsync(e => e.Amount);
    }

    public async Task<IEnumerable<Expense>> GetAllocatableExpensesAsync(Guid propertyId, int year)
        => await _dbSet.Where(e => e.PropertyId == propertyId &&
                                   e.IsAllocatable &&
                                   e.Date.Year == year)
                       .ToListAsync();
}

public class MeterReadingRepository : Repository<MeterReading>, IMeterReadingRepository
{
    public MeterReadingRepository(AppDbContext context) : base(context) { }

    public async Task<IEnumerable<MeterReading>> GetByUnitIdAsync(Guid unitId)
        => await _dbSet.Where(m => m.UnitId == unitId)
                       .OrderByDescending(m => m.ReadingDate)
                       .ToListAsync();

    public async Task<MeterReading?> GetLatestReadingAsync(Guid unitId, MeterType meterType)
        => await _dbSet.Where(m => m.UnitId == unitId && m.MeterType == meterType)
                       .OrderByDescending(m => m.ReadingDate)
                       .FirstOrDefaultAsync();
}

public class DocumentRepository : Repository<Document>, IDocumentRepository
{
    public DocumentRepository(AppDbContext context) : base(context) { }

    public async Task<IEnumerable<Document>> GetByPropertyIdAsync(Guid propertyId)
        => await _dbSet.Where(d => d.PropertyId == propertyId).ToListAsync();

    public async Task<IEnumerable<Document>> GetByTenantIdAsync(Guid tenantId)
        => await _dbSet.Where(d => d.TenantId == tenantId).ToListAsync();

    public async Task<IEnumerable<Document>> GetByLeaseIdAsync(Guid leaseId)
        => await _dbSet.Where(d => d.LeaseId == leaseId).ToListAsync();
}

public class UserRepository : Repository<User>, IUserRepository
{
    public UserRepository(AppDbContext context) : base(context) { }

    public async Task<User?> GetByEmailAsync(string email)
        => await _dbSet.FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());
}
