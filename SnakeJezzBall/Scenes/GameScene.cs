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

        private IGridManager gridManager;
        private Snake snake;
        private Apple apple;
        private bool gameOverLoaded = false;
        private int score = 0;
        private float gameTimeSeconds = 0f;
        private List<Wall> walls = new List<Wall>();
        public GameScene()
        {
            gridManager = ServiceLocator.Get<IGridManager>();

            Coordinates startPosition = new Coordinates(GRID_WIDTH / 2, GRID_HEIGHT / 2);
            snake = new Snake(startPosition, 3);
            apple = new Apple(AppleType.Normal); 
        }
        public override void Load()
        {  

        }


        public override void Update(float dt)
        {
            gameTimeSeconds += dt;
            if(Apple.HasExpired((int)gameTimeSeconds))
            {
                apple.Respawn();
                gameTimeSeconds = 0f;
            }
           
            if (snake.isGameOver && !gameOverLoaded)
            {
                GameOverScene.GameOverReason = snake.gameOverReason;
                ScenesManager.Load<GameOverScene>();
                gameOverLoaded = true;               
                return;
            }
            else if (!snake.isGameOver)
            {
                snake.Update(dt);
                HandleInput();

                if (snake.lastWallCreated.HasValue)
                {
                    walls.Add(new Wall(snake.lastWallCreated.Value));
                }

                if (snake.IsCollidingWithApple(apple))
                {
                    snake.Grow();
                    score += apple.points;
                    switch (apple.type)
                    {
                        case AppleType.Normal:
                            snake.Grow();
                            break;
                        case AppleType.Golden:
                            snake.Grow();
                            snake.Grow();
                            break;
                        case AppleType.Shrink:
                            break;                  
                    }
            
                    apple.Respawn();
                    gameTimeSeconds = 0f;
                }
            }                
        }

        public override void Draw()
        {
           
            gridManager.Draw();
            snake.Draw();
            apple.Draw();
            string scoreText = $"Score: {score}";
            int scorePositionX = PositionTextX(scoreText, SIZE_FONT_H3);
            DrawText(scoreText, scorePositionX, 0, SIZE_FONT_H3, DARK_GREEN);
            string gamePad = "Déplacements : Z/S/Q/D";
            DrawText(gamePad, 5, 0, SIZE_FONT_H3, DARK_GREEN);

            string appleText = "Pommes : Rouge - 10 pts  / Or  - 50 pts / Bleue - 5 pts";
            DrawText(appleText, 5, 20, SIZE_FONT_H3, DARK_GREEN);

            foreach (Wall wall in walls)
            {
                wall.Draw();
            }

            if (snake.isInWallMode)
            {
                DrawText("MODE MUR - ESPACE pour sortir", 5, SCREEN_HEIGHT - 30, 20, Color.Yellow);
            }
            else
            {
                DrawText("ESPACE pour mode mur", 5, SCREEN_HEIGHT - 30, 20, Color.Gray);
            }

        } 

        public override void Unload()
        {     
        }

        public void InitializeGameObjects()
        {
            gridManager = ServiceLocator.Get<IGridManager>();

            Coordinates startPosition = new Coordinates(GRID_WIDTH / 2, GRID_HEIGHT / 2);
            snake = new Snake(startPosition, 3);
            apple = new Apple(AppleType.Normal);
        }


        private void HandleInput()
        {
            if (IsKeyPressed(KeyboardKey.W))
                snake.ChangeDirection(Coordinates.up);
            else if (IsKeyPressed(KeyboardKey.S))
                snake.ChangeDirection(Coordinates.down);
            else if (IsKeyPressed(KeyboardKey.A))
                snake.ChangeDirection(Coordinates.left);
            else if (IsKeyPressed(KeyboardKey.D))
                snake.ChangeDirection(Coordinates.right);
            else if (IsKeyPressed(KeyboardKey.R) && snake.isGameOver)
                InitializeGameObjects();
            else if (IsKeyPressed(KeyboardKey.Space))            
                if (snake.isInWallMode)
                    snake.ExitWallMode();
                else
                    snake.EnterWallMode();            
        }

        public override int PositionTextX(string text, int fontSize)
        {
            int gameOverWidth = MeasureText(text, fontSize);
            int gameOverX = (SCREEN_WIDTH - gameOverWidth) - 5;
            return gameOverX;
        }

    }
}
