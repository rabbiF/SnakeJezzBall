using Raylib_cs;
using static Raylib_cs.Raylib;
using static GameConfig;


namespace SnakeJezzBall.Scenes
{
    public class MenuScene : Scene
    {
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
            int menuTitle = PositionTextX(TITLE, SIZE_FONT_H1);
            int menuText = PositionTextX(MENU_START_TEXT, SIZE_FONT_H2);

            DrawText(TITLE, menuTitle, POSITION_TEXT_H1, SIZE_FONT_H1, DARK_GREEN);
            DrawText(MENU_START_TEXT, menuText, POSITION_TEXT_H2, SIZE_FONT_H2, WHITE);
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
