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
        private float age = 0f; // Âge du mur pour des effets visuels

        public Wall(Coordinates position)
        {
            GridPosition = position;
            gridManager = ServiceLocator.Get<IGridManager>();

            // Mettre à jour la grille pour marquer cette position comme mur
            gridManager.SetCellType(GridPosition, CellType.Wall);
        }

        public override void Update(float dt)
        {
            if (!IsActive) return;          
        }

        public override void Draw()
        {
            Vector2 worldPos = gridManager.CoordinatesToWorld(GridPosition);

            // Couleur qui change légèrement avec l'âge pour un effet visuel
            Color wallColor = Color.Brown;
            if (age < 0.5f) // Animation lors de la création
            {
                float alpha = age / 0.5f;
                wallColor = new Color(
                    (int)(Color.Brown.R * alpha),
                    (int)(Color.Brown.G * alpha),
                    (int)(Color.Brown.B * alpha),
                    255
                );
            }

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
      
        public Coordinates coordinates => GridPosition;
    }
}