using System;
using Godot;

public partial class ResultsScreen : Control
{
    public event Action? ReplayRequested;
    public event Action? MenuRequested;

    private Label? _winnerLabel;
    private Label? _scoreLabel;
    private MatchResult _result;
    private bool _hasResult;

    public override void _Ready()
    {
        BuildInterface();
        ApplyResult();
    }

    public void Initialize(MatchResult result)
    {
        _result = result;
        _hasResult = true;
        ApplyResult();
    }

    private void BuildInterface()
    {
        var background = new ColorRect { Color = NeonTheme.Background };
        background.SetAnchorsPreset(LayoutPreset.FullRect);
        AddChild(background);

        var column = new VBoxContainer
        {
            Alignment = BoxContainer.AlignmentMode.Center
        };
        column.SetAnchorsPreset(LayoutPreset.FullRect);
        column.OffsetLeft = 46;
        column.OffsetRight = -46;
        column.OffsetTop = 90;
        column.OffsetBottom = -90;
        AddChild(column);

        _winnerLabel = new Label();
        NeonTheme.StyleLabel(_winnerLabel, 46, NeonTheme.Cyan);
        column.AddChild(_winnerLabel);

        _scoreLabel = new Label();
        NeonTheme.StyleLabel(_scoreLabel, 72, NeonTheme.Text);
        column.AddChild(_scoreLabel);

        AddSpacer(column, 44);

        var replay = new Button { Text = "REPLAY" };
        NeonTheme.StyleButton(replay);
        replay.CustomMinimumSize = new Vector2(240, 64);
        replay.Pressed += () => ReplayRequested?.Invoke();
        column.AddChild(replay);

        AddSpacer(column, 18);

        var menu = new Button { Text = "MENU" };
        NeonTheme.StyleButton(menu);
        menu.CustomMinimumSize = new Vector2(240, 64);
        menu.Pressed += () => MenuRequested?.Invoke();
        column.AddChild(menu);
    }

    private void ApplyResult()
    {
        if (!_hasResult || _winnerLabel is null || _scoreLabel is null)
        {
            return;
        }

        _winnerLabel.Text = _result.WinnerText;
        _scoreLabel.Text = _result.ScoreText;
    }

    private static void AddSpacer(BoxContainer container, float height)
    {
        container.AddChild(new Control { CustomMinimumSize = new Vector2(1, height) });
    }
}
