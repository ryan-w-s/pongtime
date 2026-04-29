using Godot;

public partial class ScreenRouter : Control
{
    private const string MenuScenePath = "res://scenes/screens/MenuScreen.tscn";
    private const string GameplayScenePath = "res://scenes/screens/GameplayScreen.tscn";
    private const string ResultsScenePath = "res://scenes/screens/ResultsScreen.tscn";

    private Control? _currentScreen;

    public override void _Ready()
    {
        ShowMenu();
    }

    private void ShowMenu()
    {
        var menu = LoadScreen<MenuScreen>(MenuScenePath);
        menu.StartRequested += ShowGameplay;
        SetScreen(menu);
    }

    private void ShowGameplay()
    {
        var gameplay = LoadScreen<GameplayScreen>(GameplayScenePath);
        gameplay.MatchFinished += ShowResults;
        SetScreen(gameplay);
    }

    private void ShowResults(MatchResult result)
    {
        var results = LoadScreen<ResultsScreen>(ResultsScenePath);
        results.Initialize(result);
        results.ReplayRequested += ShowGameplay;
        results.MenuRequested += ShowMenu;
        SetScreen(results);
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
}
