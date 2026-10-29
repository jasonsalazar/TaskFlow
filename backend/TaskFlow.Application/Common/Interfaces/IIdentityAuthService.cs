using TaskFlow.Application.Authentication;

namespace TaskFlow.Application.Common.Interfaces;

public interface IIdentityAuthService
{
    Task<IdentityAuthResult> RegisterAsync(
        string firstName,
        string lastName,
        string email,
        string password,
        CancellationToken cancellationToken);

    Task<IdentityAuthResult> LoginAsync(
        string email,
        string password,
        CancellationToken cancellationToken);
}