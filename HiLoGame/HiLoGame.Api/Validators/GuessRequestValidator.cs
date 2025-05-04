namespace HiLoGame.Api.Validators;

using FluentValidation;

public class GuessRequestValidator : AbstractValidator<GuessRequest>
{
    public GuessRequestValidator()
    {
        RuleFor(x => x.PlayerId).NotEmpty();
        RuleFor(x => x.Guess).InclusiveBetween(0, 1000);
    }
}
