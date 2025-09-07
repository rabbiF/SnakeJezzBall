using Raylib_cs;
using SnakeJezzBall.GameObjects;
using SnakeJezzBall.Scenes;
using SnakeJezzBall.Services;
using SnakeJezzBall.Services.Interfaces;
using static GameConfig;
using static Raylib_cs.Raylib;

public class Game
{
    public void Load()
    {
        InitWindow(SCREEN_WIDTH, SCREEN_HEIGHT, TITLE);
        SetTargetFPS(2);

        RegisterServices();
        ScenesManager.Load<MenuScene>();

        while (!WindowShouldClose())
        {
            Update(GetFrameTime());
            Draw();
        }
        CloseWindow();
    }

    public void Update(float dt)
    {
        ScenesManager.Update(GetFrameTime());
    }

    public void Draw()
    {
        BeginDrawing();
        ClearBackground(GREEN);

        ScenesManager.Draw();

        EndDrawing();
    }

    private void RegisterServices()
    {
        // Enregistrement du GridManager amélioré
        ServiceLocator.Register<IGridManager>(
             new GridManager(GRID_WIDTH, GRID_HEIGHT, CELL_SIZE)
        );
    }
}