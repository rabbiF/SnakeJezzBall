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
        private readonly IGridManager gridManager;

        public Wall(Coordinates position)
        {
           gridPosition = position;
            gridManager = ServiceLocator.Get<IGridManager>();

            // Mettre à jour la grille pour marquer cette position comme mur
            gridManager.SetCellType(gridPosition, CellType.Wall);
        }

        public override void Update(float dt)
        {
            if (!isActive) return;          
        }

        public override void Draw()
        {
            Vector2 worldPos = gridManager.CoordinatesToWorld(gridPosition);


            Color wallColor = Color.Brown;     
            DrawRectangle(
                (int)worldPos.X, (int)worldPos.Y,
                CELL_SIZE, CELL_SIZE,
                wallColor
            );

            DrawRectangleLines(
                (int)worldPos.X, (int)worldPos.Y,
                CELL_SIZE, CELL_SIZE,
                Color.Black
            );
        }
      
        public Coordinates coordinates => gridPosition;
    }
}