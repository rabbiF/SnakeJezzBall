using Raylib_cs;
public static class GameConfig
{
    public const int SCREEN_WIDTH = 800;
    public const int SCREEN_HEIGHT = 600;
    public const int GRID_WIDTH = 20;
    public const int GRID_HEIGHT = 15;
    public const int CELL_SIZE = 32;

    public const float SNAKE_MOVE_INTERVAL = 0.2f;
    public const int INITIAL_SNAKE_LENGTH = 3;
    public const float APPLE_TIMER_LIMIT = 10;

    // Couleurs
    public static readonly Color SNAKE_COLOR = Color.Green;
    public static readonly Color APPLE_COLOR = Color.Red;
    public static readonly Color GRID_COLOR = Color.Gray;
    public static readonly Color WHITE = Color.White;
    public static readonly Color GREEN = new Color { R = 173, G = 204, B = 96, A = 255 };
    public static readonly Color DARK_GREEN = new Color { R = 43, G = 51, B = 24, A = 255 };
    public static readonly Color RED = new Color { R = 122, G = 2, B = 2, A = 255 };

    //Chaines : titre et messages divers
    public static string TITLE = "SnakeJezzBall";
    public static string MENU_START_TEXT = "Appuyer sur Espace pour commencer";
    public static string GAME_OVER_START_TEXT = "GAME OVER";

    //Taille font
    public const int SIZE_FONT_H1 = 50;
    public const int SIZE_FONT_H2 = 20;
    public const int SIZE_FONT_H3 = 16;

    //Position messages
    public const int POSITION_TEXT_H1 = 200;
    public const int POSITION_TEXT_H2 = 300;
    public const int POSITION_TEXT_H3 = 350;
}
