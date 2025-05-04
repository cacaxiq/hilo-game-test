namespace HiLoGame.Api.Validators;

using FluentValidation;

public class CreateGameRequestValidator : AbstractValidator<CreateGameRequest>
{
    public CreateGameRequestValidator()
    {
        RuleFor(x => x.Min)
            .LessThan(x => x.Max)
            .WithMessage("Min must be less than Max");
        RuleFor(x => x.Min).GreaterThanOrEqualTo(1);
        RuleFor(x => x.Max).LessThanOrEqualTo(1000);
    }
}
