using Godot;

public partial class ScreenRouter : Control
{
    private const string MenuScenePath = "res://scenes/screens/MenuScreen.tscn";
    private const string GameplayScenePath = "res://scenes/screens/GameplayScreen.tscn";
    private const string ResultsScenePath = "res://scenes/screens/ResultsScreen.tscn";
    private const string MenuMusicPath = "res://assets/pongtime-menu.mp3";
    private const string GameplayMusicPath = "res://assets/pongtime-main.mp3";
    private const string ResultsMusicPath = "res://assets/pongtime-review.mp3";

    private Control? _currentScreen;
    private AudioStreamPlayer? _menuMusic;
    private AudioStreamPlayer? _gameplayMusic;
    private AudioStreamPlayer? _resultsMusic;

    public override void _Ready()
    {
        BuildMusicPlayers();
        ShowMenu();
    }

    private void ShowMenu()
    {
        var menu = LoadScreen<MenuScreen>(MenuScenePath);
        menu.StartRequested += ShowGameplay;
        SetScreen(menu);
        PlayMenuMusic();
    }

    private void ShowGameplay()
    {
        var gameplay = LoadScreen<GameplayScreen>(GameplayScenePath);
        gameplay.MatchFinished += ShowResults;
        SetScreen(gameplay);
        PlayGameplayMusic();
    }

    private void ShowResults(MatchResult result)
    {
        var results = LoadScreen<ResultsScreen>(ResultsScenePath);
        results.Initialize(result);
        results.ReplayRequested += ShowGameplay;
        results.MenuRequested += ShowMenu;
        SetScreen(results);
        PlayResultsMusic();
    }

    private T LoadScreen<T>(string path) where T : Control
    {
        var scene = GD.Load<PackedScene>(path);
        return scene.Instantiate<T>();
    }

    private void SetScreen(Control screen)
    {
        _currentScreen?.QueueFree();
        _currentScreen = screen;
        screen.SetAnchorsPreset(LayoutPreset.FullRect);
        AddChild(screen);
    }

    private void BuildMusicPlayers()
    {
        _menuMusic = CreateMusicPlayer(MenuMusicPath);
        _gameplayMusic = CreateMusicPlayer(GameplayMusicPath);
        _resultsMusic = CreateMusicPlayer(ResultsMusicPath);
        AddChild(_menuMusic);
        AddChild(_gameplayMusic);
        AddChild(_resultsMusic);
    }

    private static AudioStreamPlayer CreateMusicPlayer(string path)
    {
        var stream = GD.Load<AudioStreamMP3>(path);
        if (stream is null)
        {
            GD.PushWarning($"Music stream could not be loaded: {path}");
        }
        else
        {
            stream.Loop = true;
        }

        return new AudioStreamPlayer
        {
            Stream = stream,
            VolumeDb = -8f
        };
    }

    private void PlayMenuMusic()
    {
        PlayOnly(_menuMusic, _gameplayMusic, _resultsMusic);
    }

    private void PlayGameplayMusic()
    {
        PlayOnly(_gameplayMusic, _menuMusic, _resultsMusic);
    }

    private void PlayResultsMusic()
    {
        PlayOnly(_resultsMusic, _menuMusic, _gameplayMusic);
    }

    private static void PlayOnly(AudioStreamPlayer? active, params AudioStreamPlayer?[] inactivePlayers)
    {
        foreach (var inactive in inactivePlayers)
        {
            inactive?.Stop();
        }

        if (active is not null && !active.Playing)
        {
            active.Play();
        }
    }
}
