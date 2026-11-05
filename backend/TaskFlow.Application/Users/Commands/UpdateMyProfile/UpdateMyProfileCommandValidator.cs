using FluentValidation;

namespace TaskFlow.Application.Users.Commands.UpdateMyProfile;

public sealed class UpdateMyProfileCommandValidator
    : AbstractValidator<UpdateMyProfileCommand>
{
    public UpdateMyProfileCommandValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.LastName)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.AvatarUrl)
            .Must(BeAValidUrl)
            .When(x => !string.IsNullOrWhiteSpace(x.AvatarUrl))
            .WithMessage("Avatar URL must be a valid HTTP or HTTPS URL.");
    }

    private static bool BeAValidUrl(string? value)
    {
        return Uri.TryCreate(value, UriKind.Absolute, out var uri)
            && (uri.Scheme == Uri.UriSchemeHttp
                || uri.Scheme == Uri.UriSchemeHttps);
    }
}