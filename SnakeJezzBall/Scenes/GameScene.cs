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
        // === VARIABLES PRINCIPALES ===
        private IGridManager gridManager;
        private Snake snake;
        private Apple apple;
        private bool gameOverLoaded = false;
        private int score = 0;
        private float gameTimeSeconds = 0f;
        private List<Wall> walls = new List<Wall>();

        // === CONSTRUCTEUR OPTIMISÉ ===
        public GameScene()
        {
            // Initialisation directe pour éviter les avertissements nullable
            gridManager = ServiceLocator.Get<IGridManager>();
            gridManager.ClearGrid();

            // Initialisation des objets de jeu
            Coordinates startPosition = new Coordinates(GRID_WIDTH / 2, GRID_HEIGHT / 2);
            snake = new Snake(startPosition, INITIAL_SNAKE_LENGTH);
            apple = new Apple(AppleType.Normal);

            // Reset des variables de jeu
            ResetGameState();
        }

        // === MÉTHODES SCENE OBLIGATOIRES ===
        public override void Load()
        {
            // Logique de chargement si nécessaire
        }

        public override void Update(float dt)
        {
            gameTimeSeconds += dt;

            // Mettre à jour la pomme avec sa nouvelle méthode Update
            apple.Update(dt);

            if (Apple.HasExpired((int)gameTimeSeconds))
            {
                apple.Respawn();
                gameTimeSeconds = 0f;
            }

            if (snake.isGameOver && !gameOverLoaded)
            {
                GameOverScene.gameOverReason = snake.gameOverReason;
                ScenesManager.Load<GameOverScene>();
                gameOverLoaded = true;
                return;
            }
            else if (!snake.isGameOver)
            {
                snake.Update(dt);
                HandleInput();

                // Gérer les murs créés par le serpent (avec machine à états)
                if (snake.lastWallCreated.HasValue)
                {
                    walls.Add(new Wall(snake.lastWallCreated.Value));
                }

                if (snake.IsCollidingWithApple(apple))
                {
                    // Utiliser la nouvelle méthode ApplyEffect pour gérer automatiquement les effets
                    apple.ApplyEffect(snake);
                    score += apple.points;

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

            // Dessiner les murs
            foreach (Wall wall in walls)
            {
                wall.Draw();
            }

            // Interface utilisateur améliorée
            DrawUI();
        }

        public override void Unload()
        {
            // Nettoyage si nécessaire
        }

        public override int PositionTextX(string text, int fontSize)
        {
            int textWidth = MeasureText(text, fontSize);
            return (SCREEN_WIDTH - textWidth) - 5;
        }

        // === INTERFACE UTILISATEUR ===
        private void DrawUI()
        {
            // Score en haut à droite
            string scoreText = $"Score: {score}";
            int scorePositionX = PositionTextX(scoreText, SIZE_FONT_H3);
            DrawText(scoreText, scorePositionX, 5, SIZE_FONT_H3, DARK_GREEN);

            // Contrôles en haut à gauche
            DrawText("Déplacements : Z/S/Q/D", 5, 5, SIZE_FONT_H3, DARK_GREEN);
            DrawText("Pommes : Rouge-10pts / Or-50pts / Bleue-5pts", 5, 25, SIZE_FONT_H3, DARK_GREEN);

            // Indicateur de mode selon l'état du serpent (machine à états)
            switch (snake.CurrentState)
            {
                case SnakeState.WallBuilding:
                    DrawText("MODE MUR - ESPACE pour sortir", 5, SCREEN_HEIGHT - 50, 20, Color.Yellow);
                    break;
                case SnakeState.Stunned:
                    DrawText("ÉTOURDI - Attendez...", 5, SCREEN_HEIGHT - 50, 20, Color.Red);
                    break;
                case SnakeState.Invincible:
                    DrawText("INVINCIBLE !", 5, SCREEN_HEIGHT - 50, 20, Color.Gold);
                    break;
                default:
                    DrawText("ESPACE pour mode mur", 5, SCREEN_HEIGHT - 50, 20, Color.Gray);
                    break;
            }

            // Contrôles de restart simplifiés
            DrawText("R pour recommencer", 5, SCREEN_HEIGHT - 25, 16, Color.Gray);
        }

        // === GESTION DES ENTRÉES AMÉLIORÉE ===
        private void HandleInput()
        {
            // Touches de déplacement (support Z/Q/S/D français + W/A/S/D anglais)
            if (IsKeyPressed(KeyboardKey.W) || IsKeyPressed(KeyboardKey.Z))
                snake.ChangeDirection(Coordinates.up);
            else if (IsKeyPressed(KeyboardKey.S))
                snake.ChangeDirection(Coordinates.down);
            else if (IsKeyPressed(KeyboardKey.A) || IsKeyPressed(KeyboardKey.Q))
                snake.ChangeDirection(Coordinates.left);
            else if (IsKeyPressed(KeyboardKey.D))
                snake.ChangeDirection(Coordinates.right);

            // Mode mur avec machine à états
            else if (IsKeyPressed(KeyboardKey.Space))
            {
                if (snake.isInWallMode)
                    snake.ExitWallMode();
                else
                    snake.EnterWallMode();
            }

            // Restart simple mais optimisé
            else if (IsKeyPressed(KeyboardKey.R))
            {
                RestartGame();
            }
        }

        // === FONCTIONS DE GESTION DU JEU  ===
        // Reset seulement les variables d'état du jeu (score, timer, etc.)
        // Garde les objets existants
        private void ResetGameState()
        {
            score = 0;
            gameTimeSeconds = 0f;
            gameOverLoaded = false;
            walls.Clear();
        }

        // Restart du jeu optimisé - utilise Snake.Reset() au lieu de recréer
        private void RestartGame()
        {
            // Nettoyer la grille
            gridManager.ClearGrid();

            // Reset optimisé du serpent (plus efficace que new Snake())
            Coordinates startPosition = new Coordinates(GRID_WIDTH / 2, GRID_HEIGHT / 2);
            snake.Reset(startPosition, INITIAL_SNAKE_LENGTH);

            // Nouvelle pomme (légère à recréer)
            apple = new Apple(AppleType.Normal);

            // Reset l'état du jeu
            ResetGameState();
        }


        public void InitializeGameObjects()
        {
            RestartGame();
        }
    }
}