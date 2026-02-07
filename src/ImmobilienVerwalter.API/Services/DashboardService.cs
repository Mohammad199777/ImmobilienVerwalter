using ImmobilienVerwalter.API.DTOs;
using ImmobilienVerwalter.Core.Entities;
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
        var ownerPropertyIds = propertyList.Select(p => p.Id).ToHashSet();

        var totalUnits = propertyList.Sum(p => p.Units.Count);
        var occupiedUnits = propertyList.Sum(p =>
            p.Units.Count(u => u.Status == UnitStatus.Vermietet));
        var vacantUnits = totalUnits - occupiedUnits;
        var occupancyRate = totalUnits > 0 ? (decimal)occupiedUnits / totalUnits * 100 : 0;

        // Nur eigene Unit-IDs für Lease-Filterung
        var ownerUnitIds = propertyList.SelectMany(p => p.Units).Select(u => u.Id).ToHashSet();

        // Einnahmen: Nur Payments aus eigenen Leases
        var allMonthPayments = await _uow.Payments.GetByMonthAsync(now.Year, now.Month);
        var ownMonthPayments = allMonthPayments
            .Where(p => p.Lease != null && ownerUnitIds.Contains(p.Lease.UnitId))
            .ToList();
        var monthlyIncome = ownMonthPayments
            .Where(p => p.Status == PaymentStatus.Eingegangen)
            .Sum(p => p.Amount);

        // Ausgaben: Nur eigene (mit eigener Property oder ohne Property)
        var monthlyExpenses = await _uow.Expenses.GetTotalExpensesAsync(now.Year, now.Month);
        // TODO: Wenn Multi-User relevant, sollte GetTotalExpensesAsync auch nach PropertyIds filtern

        // Jährliche Werte berechnen
        var yearlyIncome = await CalculateYearlyIncomeAsync(ownerUnitIds, now.Year);
        var yearlyExpenses = await _uow.Expenses.GetTotalExpensesAsync(now.Year);

        // Überfällige Zahlungen nur für eigene Leases
        var allOverdue = await _uow.Payments.GetOverduePaymentsAsync();
        var ownOverdue = allOverdue
            .Where(p => p.Lease != null && ownerUnitIds.Contains(p.Lease.UnitId))
            .ToList();

        // Auslaufende Mietverträge nur für eigene Units
        var allExpiring = await _uow.Leases.GetExpiringLeasesAsync(90);
        var ownExpiring = allExpiring
            .Where(l => ownerUnitIds.Contains(l.UnitId))
            .ToList();

        var recentPayments = ownMonthPayments
            .OrderByDescending(p => p.PaymentDate)
            .Take(10);

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
            OverduePayments: ownOverdue.Count,
            ExpiringLeases: ownExpiring.Count,
            RecentPayments: recentPayments.Select(p => new PaymentDto(
                p.Id, p.LeaseId, p.Lease?.Tenant?.FullName ?? "",
                p.Amount, p.PaymentDate, p.DueDate,
                p.PaymentMonth, p.PaymentYear,
                p.Type, p.Method, p.Status,
                p.Reference, p.Notes, p.CreatedAt)),
            ExpiringLeasesList: ownExpiring.Select(l => new LeaseDto(
                l.Id, l.TenantId, l.Tenant?.FullName ?? "",
                l.UnitId, l.Unit?.Name ?? "", l.Unit?.Property?.Name ?? "",
                l.StartDate, l.EndDate,
                l.ColdRent, l.AdditionalCosts, l.TotalRent,
                l.DepositAmount, l.DepositPaid, l.DepositStatus,
                l.Status, l.NoticePeriodMonths, l.PaymentDayOfMonth,
                l.IsActive, l.Notes, l.CreatedAt))
        );
    }

    private async Task<decimal> CalculateYearlyIncomeAsync(HashSet<Guid> ownerUnitIds, int year)
    {
        decimal total = 0;
        for (int month = 1; month <= 12; month++)
        {
            var payments = await _uow.Payments.GetByMonthAsync(year, month);
            total += payments
                .Where(p => p.Status == PaymentStatus.Eingegangen &&
                            p.Lease != null &&
                            ownerUnitIds.Contains(p.Lease.UnitId))
                .Sum(p => p.Amount);
        }
        return total;
    }
}
