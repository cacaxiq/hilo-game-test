namespace HiLoGame.Api;

public record GuessRequest(Guid PlayerId, int Guess);