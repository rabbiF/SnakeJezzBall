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
        private ITerritoryManager territoryManager;
        private Snake snake;
        private Apple apple;
        private bool gameOverLoaded = false;
        private int score = 0;
        private float gameTimeSeconds = 0f;
        private List<Wall> walls = new List<Wall>();
        private List<Territory> territories = new List<Territory>();

        // === OBJECTIF JEZZBALL ===
        private const float TARGET_PERCENTAGE = 0.75f; // 75% de territoire à capturer
        private bool levelCompleted = false;

        // === CONSTRUCTEUR ===
        public GameScene()
        {
            // Initialisation directe pour éviter les avertissements nullable
            gridManager = ServiceLocator.Get<IGridManager>();
            territoryManager = ServiceLocator.Get<ITerritoryManager>();
            gridManager.ClearGrid();

            // Initialisation des objets de jeu
            Coordinates startPosition = new Coordinates(GRID_WIDTH / 2, GRID_HEIGHT / 2);
            snake = new Snake(startPosition, INITIAL_SNAKE_LENGTH);
            apple = new Apple(AppleType.Normal);

            // S'abonner aux événements de territoire
            territoryManager.OnZoneConquered += OnTerritoryConquered;
            territoryManager.OnTerritoryPercentageChanged += OnTerritoryPercentageChanged;

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

            // Mettre à jour la pomme
            apple.Update(dt);

            if (Apple.HasExpired((int)gameTimeSeconds))
            {
                apple.Respawn();
                gameTimeSeconds = 0f;
            }

            // Vérifier victoire (75% de territoire capturé)
            if (!levelCompleted && territoryManager.IsLevelObjectiveReached(TARGET_PERCENTAGE))
            {
                levelCompleted = true;
                OnLevelCompleted();
                return;
            }

            if (snake.isGameOver && !gameOverLoaded)
            {
                GameOverScene.gameOverReason = snake.gameOverReason;
                ScenesManager.Load<GameOverScene>();
                gameOverLoaded = true;
                return;
            }
            else if (!snake.isGameOver && !levelCompleted)
            {
                snake.Update(dt);
                HandleInput();

                // Gérer les murs créés par le serpent ET les territoires
                if (snake.lastWallCreated.HasValue)
                {
                    var wallPos = snake.lastWallCreated.Value;
                    walls.Add(new Wall(wallPos));
                    territoryManager.AddWall(wallPos); // Ajouter au gestionnaire de territoire
                }

                if (snake.IsCollidingWithApple(apple))
                {
                    apple.ApplyEffect(snake);
                    score += apple.points;

                    // Bonus territoire si applicable
                    score += territoryManager.CalculateTerritoryBonus();

                    apple.Respawn();
                    gameTimeSeconds = 0f;
                }

                // Mettre à jour les territoires
                UpdateTerritories(dt);
            }
        }

        public override void Draw()
        {
            gridManager.Draw();

            // Dessiner les territoires en premier (fond)
            foreach (var territory in territories)
            {
                territory.Draw();
            }

            // Dessiner les murs
            foreach (Wall wall in walls)
            {
                wall.Draw();
            }

            snake.Draw();
            apple.Draw();

            // Interface utilisateur avec info territoire
            DrawUI();
        }

        public override void Unload()
        {
            // Se désabonner des événements
            territoryManager.OnZoneConquered -= OnTerritoryConquered;
            territoryManager.OnTerritoryPercentageChanged -= OnTerritoryPercentageChanged;
        }

        public override int PositionTextX(string text, int fontSize)
        {
            int textWidth = MeasureText(text, fontSize);
            return (SCREEN_WIDTH - textWidth) - 5;
        }

        // === INTERFACE UTILISATEUR AVEC TERRITOIRES ===
        private void DrawUI()
        {
            // Score en haut à droite
            string scoreText = $"Score: {score}";
            int scorePositionX = PositionTextX(scoreText, SIZE_FONT_H3);
            DrawText(scoreText, scorePositionX, 5, SIZE_FONT_H3, DARK_GREEN);

            // Contrôles en haut à gauche
            DrawText("Déplacements : Z/S/Q/D", 5, 5, SIZE_FONT_H3, DARK_GREEN);
            DrawText("Pommes : Rouge-10pts / Or-50pts / Bleue-5pts", 5, 25, SIZE_FONT_H3, DARK_GREEN);

            // === INFORMATIONS TERRITOIRE (JEZZBALL) ===
            float percentage = territoryManager.ConqueredPercentage * 100f;
            string territoryText = $"Territoire: {percentage:F1}% / {TARGET_PERCENTAGE * 100}%";
            DrawText(territoryText, 5, 45, SIZE_FONT_H3, Color.Blue);

            // Barre de progression du territoire
            DrawTerritoryProgressBar();

            // Indicateur de mode selon l'état du serpent
            switch (snake.CurrentState)
            {
                case SnakeState.WallBuilding:
                    DrawText("MODE CONSTRUCTION - ESPACE pour sortir", 5, SCREEN_HEIGHT - 70, 18, Color.Yellow);
                    DrawText("Créez des murs pour enfermer des zones !", 5, SCREEN_HEIGHT - 50, 16, Color.Orange);
                    break;
                case SnakeState.Stunned:
                    DrawText("ÉTOURDI - Attendez...", 5, SCREEN_HEIGHT - 50, 20, Color.Red);
                    break;
                case SnakeState.Invincible:
                    DrawText("INVINCIBLE !", 5, SCREEN_HEIGHT - 50, 20, Color.Gold);
                    break;
                default:
                    DrawText("ESPACE pour mode construction", 5, SCREEN_HEIGHT - 50, 18, Color.Gray);
                    break;
            }

            // Objectif du niveau
            if (!levelCompleted)
            {
                DrawText("Objectif: Capturez 75% du territoire !", 5, SCREEN_HEIGHT - 25, 16, Color.Green);
            }
            else
            {
                DrawText("NIVEAU TERMINÉ ! Appuyez sur N pour continuer", 5, SCREEN_HEIGHT - 25, 16, Color.Lime);
            }
        }

        private void DrawTerritoryProgressBar()
        {
            // Barre de progression pour le territoire
            int barX = 200;
            int barY = 45;
            int barWidth = 200;
            int barHeight = 16;

            float percentage = territoryManager.ConqueredPercentage;
            float targetPercentage = TARGET_PERCENTAGE;

            // Fond de la barre
            DrawRectangle(barX, barY, barWidth, barHeight, Color.DarkGray);

            // Progression actuelle
            int progressWidth = (int)(barWidth * percentage);
            Color progressColor = percentage >= targetPercentage ? Color.Green : Color.Blue;
            DrawRectangle(barX, barY, progressWidth, barHeight, progressColor);

            // Ligne d'objectif
            int targetX = barX + (int)(barWidth * targetPercentage);
            DrawLine(targetX, barY, targetX, barY + barHeight, Color.Red);

            // Contour
            DrawRectangleLines(barX, barY, barWidth, barHeight, Color.Black);
        }

        // === GESTION DES ENTRÉES ===
        private void HandleInput()
        {
            // Touches de déplacement
            if (IsKeyPressed(KeyboardKey.W) || IsKeyPressed(KeyboardKey.Z))
                snake.ChangeDirection(Coordinates.up);
            else if (IsKeyPressed(KeyboardKey.S))
                snake.ChangeDirection(Coordinates.down);
            else if (IsKeyPressed(KeyboardKey.A) || IsKeyPressed(KeyboardKey.Q))
                snake.ChangeDirection(Coordinates.left);
            else if (IsKeyPressed(KeyboardKey.D))
                snake.ChangeDirection(Coordinates.right);

            // Mode construction de murs (JezzBall)
            else if (IsKeyPressed(KeyboardKey.Space))
            {
                if (snake.isInWallMode)
                    snake.ExitWallMode();
                else
                    snake.EnterWallMode();
            }

            // Restart
            else if (IsKeyPressed(KeyboardKey.R))
            {
                RestartGame();
            }

            // Niveau suivant (si terminé)
            else if (IsKeyPressed(KeyboardKey.N) && levelCompleted)
            {
                NextLevel();
            }
        }

        // === GESTION DES TERRITOIRES ===
        private void UpdateTerritories(float dt)
        {
            // Mettre à jour l'animation des territoires
            foreach (var territory in territories)
            {
                territory.Update(dt);
            }
        }

        private void OnTerritoryConquered(List<Coordinates> newZone)
        {
            // Créer un nouveau territoire visuel pour la zone conquise
            var territory = new Territory(newZone, TerritoryState.Conquered);
            territories.Add(territory);

            // Bonus de score pour territoire conquis
            int bonus = newZone.Count * 10; // 10 points par cellule conquise
            score += bonus;

        }

        private void OnTerritoryPercentageChanged(float newPercentage)
        {  
            // Déclencher des effets visuels ou sonores
        }

        private void OnLevelCompleted()
        {
            // Bonus de fin de niveau
            int levelBonus = (int)(territoryManager.ConqueredPercentage * 2000);
            score += levelBonus;

            // Arrêter le serpent
            snake.ChangeState(SnakeState.Stunned);
        }

        // === FONCTIONS DE GESTION DU JEU ===
        private void ResetGameState()
        {
            score = 0;
            gameTimeSeconds = 0f;
            gameOverLoaded = false;
            levelCompleted = false;
            walls.Clear();
            territories.Clear();
            territoryManager.ClearAll();
        }

        private void RestartGame()
        {
            gridManager.ClearGrid();

            Coordinates startPosition = new Coordinates(GRID_WIDTH / 2, GRID_HEIGHT / 2);
            snake.Reset(startPosition, INITIAL_SNAKE_LENGTH);
            apple = new Apple(AppleType.Normal);

            ResetGameState();
        }

        private void NextLevel()
        {
            // Garder le score, recommencer avec plus de difficulté
            gridManager.ClearGrid();

            Coordinates startPosition = new Coordinates(GRID_WIDTH / 2, GRID_HEIGHT / 2);
            snake.Reset(startPosition, INITIAL_SNAKE_LENGTH);
            apple = new Apple(AppleType.Normal);

            // Reset seulement le territoire et l'état du niveau
            gameTimeSeconds = 0f;
            gameOverLoaded = false;
            levelCompleted = false;
            walls.Clear();
            territories.Clear();
            territoryManager.ClearAll();

        }

        public void InitializeGameObjects()
        {
            RestartGame();
        }
    }
}