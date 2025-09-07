using Raylib_cs;
using SnakeJezzBall.Services;
using SnakeJezzBall.Services.Interfaces;
using SnakeJezzBall.Utils;
using System.Numerics;
using static GameConfig;
using static Raylib_cs.Raylib;

namespace SnakeJezzBall.GameObjects
{
    public class Apple : GridObject
    {
        private readonly IGridManager gridManager;
        private static readonly Random random = new Random(); // Instance statique

        public AppleType type { get; private set; }
        public int points { get; private set; }
        private float lifetime = 0f;
        private float pulseAnimation = 0f;

        public Apple(AppleType type)
        {
            gridManager = ServiceLocator.Get<IGridManager>();
            this.type = type;
            this.points = GetPointsForType(type);
            Respawn();
        }

        public override void Update(float deltaTime)
        {
            if (!IsActive) return;

            lifetime += deltaTime;
            pulseAnimation += deltaTime * 3f; // Animation de pulsation

            // Vérifier si la pomme a expiré
            if (HasExpired((int)lifetime))
            {
                Respawn();
            }
        }

        public void Respawn()
        {
            // Générer position aléatoire valide et nouveau type
            do
            {
                GridPosition = Coordinates.Random(gridManager.columns, gridManager.rows);
                var chooseApple = random.Next(0, 3);
                switch (chooseApple)
                {
                    case 0:
                        type = AppleType.Normal;
                        break;
                    case 1:
                        type = AppleType.Golden;
                        break;
                    case 2:
                        type = AppleType.Shrink;
                        break;
                }
                points = GetPointsForType(type);
            }
            while (!IsValidSpawnPosition());

            // Mettre à jour la grille
            gridManager.UpdateApplePosition(GridPosition);

            // Reset du timer
            lifetime = 0f;
            pulseAnimation = 0f;
            IsActive = true;
        }

        // Éviter de spawn sur le serpent ou les murs
        private bool IsValidSpawnPosition()
        {
            return gridManager.IsValidPosition(GridPosition);
        }

        public override void Draw()
        {
            Vector2 worldPosition = gridManager.CoordinatesToWorld(GridPosition);
            Vector2 center = worldPosition + new Vector2(CELL_SIZE * 0.5f, CELL_SIZE * 0.5f);

            Color appleColor = GetColorForType(type);

            // Animation de pulsation pour pomme dorée
            float pulseScale = 1f;
            if (type == AppleType.Golden)
            {
                pulseScale = 1f + MathF.Sin(pulseAnimation) * 0.1f;
            }

            float radius = CELL_SIZE * 0.4f * pulseScale;

            // Ombre légère
            DrawCircle(
                (int)center.X + 2,
                (int)center.Y + 2,
                radius,
                new Color(0, 0, 0, 50)
            );

            // Corps de la pomme
            DrawCircle(
                (int)center.X,
                (int)center.Y,
                radius,
                appleColor
            );

            // Contour pour plus de visibilité
            DrawCircleLines(
                (int)center.X,
                (int)center.Y,
                radius,
                Color.Black
            );

            // Effet spécial pour pomme dorée
            if (type == AppleType.Golden)
            {
                DrawCircleLines(
                    (int)center.X,
                    (int)center.Y,
                    radius + 2,
                    new Color(255, 215, 0, 100)
                );
            }

            // Indicateur de temps restant
            if (lifetime > APPLE_TIMER_LIMIT * 0.7f) // Clignotement dans les derniers 30%
            {
                float blinkSpeed = 10f;
                if (MathF.Sin(lifetime * blinkSpeed) > 0)
                {
                    DrawCircleLines(
                        (int)center.X,
                        (int)center.Y,
                        radius + 4,
                        Color.Red
                    );
                }
            }
        }

        private Color GetColorForType(AppleType type)
        {
            return type switch
            {
                AppleType.Normal => RED,
                AppleType.Golden => Color.Gold,
                AppleType.Shrink => Color.Blue,
                _ => RED
            };
        }

        private int GetPointsForType(AppleType type)
        {
            return type switch
            {
                AppleType.Normal => 10,
                AppleType.Golden => 50,
                AppleType.Shrink => 5,
                _ => 10
            };
        }

        public static bool HasExpired(int gameTimeSeconds)
        {
            return gameTimeSeconds > APPLE_TIMER_LIMIT;
        }

        public Coordinates coordinates => GridPosition;

        public void ApplyEffect(Snake snake)
        {
            switch (type)
            {
                case AppleType.Normal:
                    snake.Grow();
                    break;

                case AppleType.Golden:
                    snake.Grow();
                    snake.Grow(); // Double croissance
                    snake.MakeInvincible(2f); // Bonus : courte invincibilité
                    break;

                case AppleType.Shrink:
                    // Pas de croissance mais ne réduit pas non plus
                    break;
            }
        }
    }
}