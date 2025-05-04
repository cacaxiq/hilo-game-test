namespace HiLoGame.Api.Validators;

using FluentValidation;

public class JoinGameRequestValidator : AbstractValidator<JoinGameRequest>
{
    public JoinGameRequestValidator()
    {
        RuleFor(x => x.PlayerName)
            .NotEmpty().WithMessage("Player name is required")
            .MaximumLength(100);
    }
}
