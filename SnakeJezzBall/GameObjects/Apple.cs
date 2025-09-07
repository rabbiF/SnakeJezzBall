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
        public Coordinates coordinates { get; private set; }
        public AppleType type { get; private set; }
        public int points { get; private set; }
        private readonly IGridManager gridManager;

        public Apple(AppleType type)
        {
            gridManager = ServiceLocator.Get<IGridManager>();
            this.type = type;
            this.points = GetPointsForType(type);
            Respawn();            
        }

        public void Respawn()
        {

            // Générer position aléatoire valide
            do
            {
                coordinates = Coordinates.Random(gridManager.columns, gridManager.rows);
                var chooseApple = new Random().Next(0, 3);
                switch (chooseApple) {
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
        }

        // Éviter de spawn sur le serpent
        private bool IsValidSpawnPosition()
        {
            return gridManager.IsValidPosition(coordinates);
        }
        public override void Update(float dt)
        {

        }

        public override void Draw()
        {

            Vector2 worldPosition = gridManager.CoordinatesToWorld(coordinates);
            Vector2 center = worldPosition + new Vector2(CELL_SIZE * 0.5f, CELL_SIZE * 0.5f);

            Color appleColor = GetColorForType(type);

            DrawCircle(
                (int)center.X,
                (int)center.Y,
                CELL_SIZE * 0.4f,
                appleColor
            );

            // Contour pour plus de visibilité
            DrawCircleLines(
                (int)center.X,
                (int)center.Y,
                CELL_SIZE * 0.4f,
                Color.Black
            );
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
    }
}