using Raylib_cs;
using SnakeJezzBall.GameObjects;
using SnakeJezzBall.Services;
using SnakeJezzBall.Services.Interfaces;
using SnakeJezzBall.Utils;
using static GameConfig;
using static Raylib_cs.Raylib;

namespace SnakeJezzBall.Scenes
{
    public class GameScene : Scene
    {
        private IGridManager gridManager = null!;
        private CollisionManager collisionManager = null!;
        private Snake snake = null!;
        private Apple apple = null!;
        private List<Wall> walls = new List<Wall>();

        private bool gameOverLoaded = false;
        private int score = 0;
        private float gameTimeSeconds = 0f;
        private Coordinates startPosition;

        public GameScene()
        {
            InitializeGame();
        }


        private void InitializeGame()
        {
            gridManager = ServiceLocator.Get<IGridManager>();
            collisionManager = new CollisionManager(gridManager, walls);

            startPosition = new Coordinates(GRID_WIDTH / 2, GRID_HEIGHT / 2);
            snake = new Snake(startPosition, collisionManager, INITIAL_SNAKE_LENGTH);
            apple = new Apple(AppleType.Normal);

            // Reset game state
            score = 0;
            gameTimeSeconds = 0f;
            gameOverLoaded = false;
            walls.Clear();
        }

        public override void Load()
        {
            // Logique de chargement si nécessaire
        }

        public override void Update(float dt)
        {
            gameTimeSeconds += dt;

            // Gérer l'expiration des pommes
            if (Apple.HasExpired((int)gameTimeSeconds))
            {
                apple.Respawn();
                gameTimeSeconds = 0f;
            }

            // Vérifier game over
            if (snake.IsGameOver && !gameOverLoaded)
            {
                GameOverScene.GameOverReason = snake.GameOverReason;
                ScenesManager.Load<GameOverScene>();
                gameOverLoaded = true;
                return;
            }

            if (!snake.IsGameOver)
            {
                snake.Update(dt);
                HandleInput();
                HandleAppleCollision();
            }
        }

        private void HandleAppleCollision()
        {
            if (snake.IsCollidingWithApple(apple))
            {
                // Appliquer les effets selon le type de pomme
                ApplyAppleEffect(apple.type);

                score += apple.points;
                apple.Respawn();
                gameTimeSeconds = 0f;
            }
        }

        private void ApplyAppleEffect(AppleType appleType)
        {
            switch (appleType)
            {
                case AppleType.Normal:
                    snake.Grow();
                    break;
                case AppleType.Golden:
                    snake.Grow();
                    snake.Grow(); // Double croissance
                    break;
                case AppleType.Shrink:
                    // Pas de croissance mais on gagne quand même des points
                    break;
            }
        }

        public override void Draw()
        {
            gridManager.Draw();

            // Dessiner les murs
            foreach (Wall wall in walls)
            {
                wall.Draw();
            }

            snake.Draw();
            apple.Draw();

            DrawUI();
        }

        private void DrawUI()
        {
            // Score
            string scoreText = $"Score: {score}";
            int scorePositionX = PositionTextX(scoreText, SIZE_FONT_H3);
            DrawText(scoreText, scorePositionX, 5, SIZE_FONT_H3, DARK_GREEN);

            // Contrôles
            DrawText("Déplacements : Z/S/Q/D", 5, 5, SIZE_FONT_H3, DARK_GREEN);
            DrawText("Pommes : Rouge-10pts / Or-50pts / Bleue-5pts", 5, 25, SIZE_FONT_H3, DARK_GREEN);

            // Mode mur
            if (snake.IsInWallMode)
            {
                DrawText("MODE MUR - ESPACE pour sortir", 5, SCREEN_HEIGHT - 50, 20, Color.Yellow);
            }
            else
            {
                DrawText("ESPACE pour mode mur", 5, SCREEN_HEIGHT - 50, 20, Color.Gray);
            }

            DrawText("R pour recommencer", 5, SCREEN_HEIGHT - 25, 16, Color.Gray);
        }

        private void HandleInput()
        {
            // Mouvements
            if (IsKeyPressed(KeyboardKey.W) || IsKeyPressed(KeyboardKey.Z))
                snake.ChangeDirection(Coordinates.up);
            else if (IsKeyPressed(KeyboardKey.S))
                snake.ChangeDirection(Coordinates.down);
            else if (IsKeyPressed(KeyboardKey.A) || IsKeyPressed(KeyboardKey.Q))
                snake.ChangeDirection(Coordinates.left);
            else if (IsKeyPressed(KeyboardKey.D))
                snake.ChangeDirection(Coordinates.right);

            // Mode mur
            else if (IsKeyPressed(KeyboardKey.Space))
            {
                if (snake.IsInWallMode)
                    snake.ExitWallMode();
                else
                    snake.EnterWallMode();
            }

            // Restart
            else if (IsKeyPressed(KeyboardKey.R))
            {
                RestartGame();
            }
        }

        private void RestartGame()
        {
            InitializeGame();
        }

        public override void Unload()
        {
        }

        public override int PositionTextX(string text, int fontSize)
        {
            int textWidth = MeasureText(text, fontSize);
            return (SCREEN_WIDTH - textWidth) - 5;
        }
    }
}