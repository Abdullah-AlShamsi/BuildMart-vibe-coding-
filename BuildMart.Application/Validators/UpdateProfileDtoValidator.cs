using BuildMart.Application.DTOs.Auth;
using FluentValidation;

namespace BuildMart.Application.Validators;

public class UpdateProfileDtoValidator : AbstractValidator<UpdateProfileDto>
{
    public UpdateProfileDtoValidator()
    {
        RuleFor(x => x.FullName).NotEmpty().MaximumLength(150);
        RuleFor(x => x.PhoneNumber).MaximumLength(20).When(x => x.PhoneNumber != null);
        RuleFor(x => x.Address).MaximumLength(500).When(x => x.Address != null);
        RuleFor(x => x.City).MaximumLength(100).When(x => x.City != null);
    }
}
