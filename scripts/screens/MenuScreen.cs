using System;
using Godot;

public partial class MenuScreen : Control
{
    public event Action? StartRequested;

    public override void _Ready()
    {
        BuildInterface();
    }

    private void BuildInterface()
    {
        var background = new ColorRect
        {
            Color = NeonTheme.Background
        };
        background.SetAnchorsPreset(LayoutPreset.FullRect);
        AddChild(background);

        var column = new VBoxContainer
        {
            Alignment = BoxContainer.AlignmentMode.Center
        };
        column.SetAnchorsPreset(LayoutPreset.FullRect);
        column.OffsetLeft = 48;
        column.OffsetRight = -48;
        column.OffsetTop = 90;
        column.OffsetBottom = -90;
        AddChild(column);

        var title = new Label { Text = "PONGTIME" };
        NeonTheme.StyleLabel(title, 64, NeonTheme.Cyan);
        column.AddChild(title);

        var subtitle = new Label
        {
            Text = "NEON PONG WITH TACTICAL SLOW TIME",
            AutowrapMode = TextServer.AutowrapMode.WordSmart
        };
        NeonTheme.StyleLabel(subtitle, 18, NeonTheme.Muted);
        column.AddChild(subtitle);

        AddSpacer(column, 56);

        var startButton = new Button
        {
            Text = "START"
        };
        NeonTheme.StyleButton(startButton);
        startButton.CustomMinimumSize = new Vector2(240, 68);
        startButton.Pressed += () => StartRequested?.Invoke();
        column.AddChild(startButton);

        AddSpacer(column, 36);

        var hint = new Label
        {
            Text = "ARROWS OR DRAG TO MOVE  |  SPACE TO SLOW TIME",
            AutowrapMode = TextServer.AutowrapMode.WordSmart
        };
        NeonTheme.StyleLabel(hint, 16, NeonTheme.Muted);
        column.AddChild(hint);
    }

    private static void AddSpacer(BoxContainer container, float height)
    {
        container.AddChild(new Control { CustomMinimumSize = new Vector2(1, height) });
    }
}
