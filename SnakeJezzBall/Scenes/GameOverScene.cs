using Raylib_cs;
using SnakeJezzBall.GameObjects;
using static GameConfig;
using static Raylib_cs.Raylib;
namespace SnakeJezzBall.Scenes
{
    internal class GameOverScene : Scene
    {
        public static string gameOverReason { get; set; } = GAME_OVER_START_TEXT;
        public override void Load()
        {
        }

        public override void Update(float dt)
        {
            if (IsKeyPressed(KeyboardKey.Space))
            {
                ScenesManager.Load<GameScene>();
            }
        }

        public override void Draw()
        {
            int gameOverX = PositionTextX(GAME_OVER_START_TEXT, SIZE_FONT_H1);
            int gameOverReasonX = PositionTextX(gameOverReason, SIZE_FONT_H2);
            int menuStartTextX = PositionTextX(MENU_START_TEXT, SIZE_FONT_H3);

            DrawText(GAME_OVER_START_TEXT, gameOverX, POSITION_TEXT_H1, SIZE_FONT_H1, RED);  
            DrawText(gameOverReason, gameOverReasonX, POSITION_TEXT_H2, SIZE_FONT_H2, WHITE);
            DrawText(MENU_START_TEXT, menuStartTextX, POSITION_TEXT_H3, SIZE_FONT_H3, DARK_GREEN);
        }

        public override void Unload()
        {
        }

        public override int PositionTextX(string text, int fontSize)
        {
            int gameOverWidth = MeasureText(text, fontSize);
            int gameOverX = (SCREEN_WIDTH - gameOverWidth) / 2;
            return gameOverX;
        }
    }
}
