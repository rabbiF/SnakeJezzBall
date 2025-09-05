using Raylib_cs;
using SnakeJezzBall.Services;
using SnakeJezzBall.Services.Interfaces;
using SnakeJezzBall.Utils;
using System.Numerics;
using static GameConfig;
using static Raylib_cs.Raylib;

namespace SnakeJezzBall.GameObjects
{
    public class Wall : GridObject
    {
        public Coordinates coordinates { get; private set; }

        private readonly IGridManager gridManager;
        public Wall(Coordinates position)
        {
            GridPosition = position;
            gridManager = ServiceLocator.Get<IGridManager>();
        }

        public override void Update(float dt)
        {

        }
        public override void Draw()
        {
            Vector2 worldPos = gridManager.CoordinatesToWorld(GridPosition);

            DrawRectangle(
                (int)worldPos.X, (int)worldPos.Y,
                CELL_SIZE, CELL_SIZE,
                Color.Brown
            );

            DrawRectangleLines(
                (int)worldPos.X, (int)worldPos.Y,
                CELL_SIZE, CELL_SIZE,
                Color.Black
            );
        }
    }
}
