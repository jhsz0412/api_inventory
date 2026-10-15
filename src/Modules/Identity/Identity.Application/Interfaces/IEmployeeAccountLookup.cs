namespace Identity.Application.Interfaces;

public interface IEmployeeAccountLookup
{
    Task<bool> IsActiveEmployeeAsync(
        Guid employeeId,
        CancellationToken cancellationToken = default);
}