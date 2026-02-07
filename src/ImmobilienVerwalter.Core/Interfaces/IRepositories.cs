using ImmobilienVerwalter.Core.Entities;

namespace ImmobilienVerwalter.Core.Interfaces;

public interface IPropertyRepository : IRepository<Property>
{
    Task<IEnumerable<Property>> GetByOwnerIdAsync(Guid ownerId);
    Task<Property?> GetWithUnitsAsync(Guid id);
    Task<Property?> GetWithAllDetailsAsync(Guid id);
}

public interface IUnitRepository : IRepository<Unit>
{
    Task<IEnumerable<Unit>> GetByPropertyIdAsync(Guid propertyId);
    Task<Unit?> GetWithLeaseAsync(Guid id);
    Task<IEnumerable<Unit>> GetVacantUnitsAsync(Guid? propertyId = null);
}

public interface ITenantRepository : IRepository<Tenant>
{
    Task<Tenant?> GetWithLeasesAsync(Guid id);
    Task<IEnumerable<Tenant>> SearchAsync(string searchTerm);
}

public interface ILeaseRepository : IRepository<Lease>
{
    Task<IEnumerable<Lease>> GetByUnitIdAsync(Guid unitId);
    Task<IEnumerable<Lease>> GetByTenantIdAsync(Guid tenantId);
    Task<IEnumerable<Lease>> GetActiveLeaseAsync();
    Task<IEnumerable<Lease>> GetExpiringLeasesAsync(int withinDays);
    Task<Lease?> GetWithPaymentsAsync(Guid id);
}

public interface IPaymentRepository : IRepository<Payment>
{
    Task<IEnumerable<Payment>> GetByLeaseIdAsync(Guid leaseId);
    Task<IEnumerable<Payment>> GetOverduePaymentsAsync();
    Task<IEnumerable<Payment>> GetByMonthAsync(int year, int month);
    Task<decimal> GetTotalIncomeAsync(int year, int? month = null);
}

public interface IExpenseRepository : IRepository<Expense>
{
    Task<IEnumerable<Expense>> GetByPropertyIdAsync(Guid propertyId);
    Task<IEnumerable<Expense>> GetByCategoryAsync(ExpenseCategory category);
    Task<decimal> GetTotalExpensesAsync(int year, int? month = null);
    Task<IEnumerable<Expense>> GetAllocatableExpensesAsync(Guid propertyId, int year);
}

public interface IMeterReadingRepository : IRepository<MeterReading>
{
    Task<IEnumerable<MeterReading>> GetByUnitIdAsync(Guid unitId);
    Task<MeterReading?> GetLatestReadingAsync(Guid unitId, MeterType meterType);
}

public interface IDocumentRepository : IRepository<Document>
{
    Task<IEnumerable<Document>> GetByPropertyIdAsync(Guid propertyId);
    Task<IEnumerable<Document>> GetByTenantIdAsync(Guid tenantId);
    Task<IEnumerable<Document>> GetByLeaseIdAsync(Guid leaseId);
}

public interface IUserRepository : IRepository<User>
{
    Task<User?> GetByEmailAsync(string email);
}
