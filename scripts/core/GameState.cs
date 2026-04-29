public enum GameScreen
{
    Menu,
    Gameplay,
    Results
}

public readonly record struct MatchResult(int PlayerScore, int CpuScore)
{
    public bool PlayerWon => PlayerScore > CpuScore;
    public string WinnerText => PlayerWon ? "PLAYER WINS" : "CPU WINS";
    public string ScoreText => $"{PlayerScore} - {CpuScore}";
}
