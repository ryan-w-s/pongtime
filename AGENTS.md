# AGENTS.md

## Project Overview

Pongtime is a Godot 4.6 Mono C# project for a simple vertical Pong game.

The game is classic Pong rotated into a portrait-first 9:16 playfield:

- Virtual resolution: `540x960`.
- Actual window resolution: flexible and scalable across common desktop and mobile-sized windows.
- Theme: neon blue.
- Screens: menu, gameplay, and results.
- Player paddle: bottom.
- CPU paddle: top.
- Player ability: briefly slow down time for the ball and CPU, while the player paddle stays at normal responsiveness.

Keep the experience focused, responsive, and arcade-clean. Prefer a polished small game over a broad feature set.

## Godot And Tooling

- Use Godot `4.6` with Mono/C#.
- Use C# scripts targeting the existing `.csproj` setup.
- Prefer the Godot MCP tools when inspecting, running, debugging, or validating the project:
  - `run_project` for normal launch checks.
  - `run_with_debug` for console output, errors, and warnings.
  - `remote_tree_dump` for runtime scene-tree inspection.
  - `list_missing_assets` before finalizing asset-heavy changes.
  - `launch_editor` when editor-side setup is genuinely needed.
- It is acceptable to edit scene, script, and project files directly when MCP cannot perform the needed operation, but validate through Godot afterward whenever possible.

## Game Requirements

### Resolution And Scaling

- Design the game around a `540x960` virtual coordinate space.
- Maintain a 9:16 vertical composition.
- Use Godot stretch/window settings that allow the game to run at varied actual resolutions without breaking layout.
- Gameplay-critical positions should be derived from viewport or configured playfield dimensions rather than hard-coded physical window size.
- Keep UI readable on both small and large windows.

### Screens

Implement the game as three clear states:

- `Menu`: title, start action, and any minimal options needed.
- `Gameplay`: active Pong match with score display, ball, player paddle, and CPU paddle.
- `Results`: final score/result with actions to replay or return to menu.

Prefer one central state owner for screen transitions so logic does not get scattered across unrelated nodes.

### Gameplay

- Bottom paddle is player-controlled.
- Top paddle is CPU-controlled.
- Keep controls simple and responsive.
- Ball behavior should be deterministic enough to feel fair, with enough variation to avoid stale rallies.
- CPU should be competent but beatable.
- Define match-ending conditions clearly, such as a target score or timed round.
- Include a brief player-triggered slow-time ability:
  - Slow the ball and CPU paddle, but do not slow the player paddle or player input.
  - Keep the duration short and readable.
  - Use a cooldown, meter, or limited charges so the ability is tactical rather than always-on.
  - Communicate availability and active state clearly in the gameplay UI.
  - Add subtle neon-blue visual/audio feedback when time is slowed, while preserving ball readability.

### Visual Direction

- Theme is neon blue: dark background, bright cyan/blue gameplay elements, glow where practical.
- Avoid overcomplicating visuals. Strong silhouettes, crisp motion, and readable contrast matter most.
- Effects should support play clarity. Do not let glow, particles, or animation obscure the ball or paddles.

## Suggested Project Structure

Use this structure unless an existing implementation already establishes a better pattern:

```text
res://
  scenes/
    Main.tscn
    screens/
      MenuScreen.tscn
      GameplayScreen.tscn
      ResultsScreen.tscn
    gameplay/
      Ball.tscn
      Paddle.tscn
  scripts/
    core/
      GameState.cs
      ScreenRouter.cs
    screens/
      MenuScreen.cs
      GameplayScreen.cs
      ResultsScreen.cs
    gameplay/
      Ball.cs
      Paddle.cs
      CpuPaddleController.cs
      PlayerPaddleController.cs
  assets/
    fonts/
    audio/
    materials/
```

Keep scene names and script class names aligned. For example, `GameplayScreen.tscn` should usually have `GameplayScreen.cs` attached to its root.

## C# Style

- Use idiomatic Godot C# with partial classes inheriting from Godot node types.
- Use `[Export]` for tuning values that designers may adjust in the editor.
- Cache required child nodes in `_Ready()` using `%UniqueName` paths or exported `NodePath` references.
- Keep `_Process()` and `_PhysicsProcess()` small. Move gameplay calculations into named methods.
- Prefer clear constants for virtual dimensions, scoring values, speeds, and bounds.
- Avoid static global state unless it is a deliberate autoload or immutable configuration.
- Use Godot signals/events for screen-level coordination where they make ownership cleaner.

## Scene And Node Guidance

- Use `Control`-based roots for UI screens.
- Use `Node2D` or a suitable 2D root for gameplay.
- Keep gameplay objects independent enough to test and retune:
  - Ball owns its velocity and collision response.
  - Paddle owns movement limits and dimensions.
  - Controllers own player input or CPU tracking behavior.
  - Gameplay screen owns score, round reset, and win/loss decisions.
- Prefer named child nodes and Godot unique-name access for important UI labels and buttons.

## Input

- Support mouse/touch horizontal dragging for the player paddle if practical.
- Support keyboard fallback for desktop play, such as left/right or A/D.
- Keep input handling centralized in the player paddle controller or gameplay screen.

## Validation Checklist

Before considering a change complete:

- Build succeeds for the Godot C# project.
- The project runs through Godot MCP without script errors.
- Menu can start gameplay.
- Gameplay can reach a result.
- Results can replay or return to menu.
- The layout remains portrait and playable at multiple window sizes.
- Ball, paddles, score, and buttons are readable against the neon-blue theme.
- No missing assets are reported for files referenced by scenes.

Useful checks:

```powershell
dotnet build Pongtime.sln
```

Use Godot MCP for runtime checks whenever available.

## Git And Change Hygiene

- Keep changes focused on the requested feature or fix.
- Do not rewrite unrelated generated Godot metadata unless the editor or importer changes it as part of the work.
- Do not remove user-created files or uncommitted changes without explicit permission.
- Prefer small, meaningful commits when asked to commit.

## Definition Of Done For The Initial Game

The first complete version should include:

- `540x960` portrait virtual layout.
- Main scene wired as the project entry point.
- Menu screen.
- Gameplay screen with working player paddle, CPU paddle, ball, scoring, and round reset.
- Results screen with replay/menu flow.
- Neon-blue visual styling.
- Successful build and Godot run/debug validation.
