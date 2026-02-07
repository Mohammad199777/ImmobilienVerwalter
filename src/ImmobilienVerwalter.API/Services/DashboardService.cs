using ImmobilienVerwalter.API.DTOs;
using ImmobilienVerwalter.Core.Interfaces;

namespace ImmobilienVerwalter.API.Services;

public interface IDashboardService
{
    Task<DashboardDto> GetDashboardAsync(Guid ownerId);
}

public class DashboardService : IDashboardService
{
    private readonly IUnitOfWork _uow;

    public DashboardService(IUnitOfWork uow) => _uow = uow;

    public async Task<DashboardDto> GetDashboardAsync(Guid ownerId)
    {
        var now = DateTime.UtcNow;
        var properties = await _uow.Properties.GetByOwnerIdAsync(ownerId);
        var propertyList = properties.ToList();

        var totalUnits = propertyList.Sum(p => p.Units.Count);
        var occupiedUnits = propertyList.Sum(p =>
            p.Units.Count(u => u.Status == Core.Entities.UnitStatus.Vermietet));
        var vacantUnits = totalUnits - occupiedUnits;
        var occupancyRate = totalUnits > 0 ? (decimal)occupiedUnits / totalUnits * 100 : 0;

        var monthlyIncome = await _uow.Payments.GetTotalIncomeAsync(now.Year, now.Month);
        var monthlyExpenses = await _uow.Expenses.GetTotalExpensesAsync(now.Year, now.Month);
        var yearlyIncome = await _uow.Payments.GetTotalIncomeAsync(now.Year);
        var yearlyExpenses = await _uow.Expenses.GetTotalExpensesAsync(now.Year);

        var overduePayments = (await _uow.Payments.GetOverduePaymentsAsync()).Count();
        var expiringLeases = await _uow.Leases.GetExpiringLeasesAsync(90);
        var recentPayments = (await _uow.Payments.GetByMonthAsync(now.Year, now.Month)).Take(10);

        return new DashboardDto(
            TotalProperties: propertyList.Count,
            TotalUnits: totalUnits,
            OccupiedUnits: occupiedUnits,
            VacantUnits: vacantUnits,
            OccupancyRate: Math.Round(occupancyRate, 1),
            MonthlyIncome: monthlyIncome,
            MonthlyExpenses: monthlyExpenses,
            MonthlyProfit: monthlyIncome - monthlyExpenses,
            YearlyIncome: yearlyIncome,
            YearlyExpenses: yearlyExpenses,
            OverduePayments: overduePayments,
            ExpiringLeases: expiringLeases.Count(),
            RecentPayments: recentPayments.Select(p => new PaymentDto(
                p.Id, p.LeaseId, p.Lease?.Tenant?.FullName ?? "",
                p.Amount, p.PaymentDate, p.DueDate,
                p.PaymentMonth, p.PaymentYear,
                p.Type, p.Method, p.Status,
                p.Reference, p.Notes, p.CreatedAt)),
            ExpiringLeasesList: expiringLeases.Select(l => new LeaseDto(
                l.Id, l.TenantId, l.Tenant?.FullName ?? "",
                l.UnitId, l.Unit?.Name ?? "", l.Unit?.Property?.Name ?? "",
                l.StartDate, l.EndDate,
                l.ColdRent, l.AdditionalCosts, l.TotalRent,
                l.DepositAmount, l.DepositPaid, l.DepositStatus,
                l.Status, l.NoticePeriodMonths, l.PaymentDayOfMonth,
                l.IsActive, l.Notes, l.CreatedAt))
        );
    }
}
