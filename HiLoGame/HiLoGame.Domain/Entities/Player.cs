namespace HiLoGame.Domain.Entities;

public class Player
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public int Attempts { get; set; } = 0;
    public bool HasWon { get; set; } = false;
}