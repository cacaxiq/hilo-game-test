namespace HiLoGame.Domain.Entities;

public class Game
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public int Min { get; private set; }
    public int Max { get; private set; }
    public int MysteryNumber { get; private set; }
    public List<Player> Players { get; private set; } = [];
    public GameStatus Status { get; private set; } = GameStatus.WaitingForPlayers;

    public Game(int min, int max)
    {
        Min = min;
        Max = max;
        MysteryNumber = new Random().Next(min, max + 1);
    }

    public void JoinPlayer(Player player)
    {
        if (Status != GameStatus.WaitingForPlayers)
            throw new InvalidOperationException("Cannot join a game already started.");
        Players.Add(player);
    }

    public GuessResult MakeGuess(Guid playerId, int guess)
    {
        if (Status == GameStatus.Finished)
            throw new InvalidOperationException("Game is already finished.");

        if (guess < Min || guess > Max)
            throw new ArgumentOutOfRangeException(nameof(guess));

        var player = Players.FirstOrDefault(p => p.Id == playerId)
                     ?? throw new ArgumentException("Player not found");

        player.Attempts++;

        if (guess != MysteryNumber) return guess < MysteryNumber ? GuessResult.HI : GuessResult.LO;
       
        Status = GameStatus.Finished;
        player.HasWon = true;
        return GuessResult.CORRECT;
    }
}