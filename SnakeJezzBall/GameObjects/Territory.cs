using Raylib_cs;
using SnakeJezzBall.Services;
using SnakeJezzBall.Services.Interfaces;
using SnakeJezzBall.Utils;
using System.Numerics;
using static GameConfig;
using static Raylib_cs.Raylib;

namespace SnakeJezzBall.GameObjects
{
    public class Territory : GridObject
    {
        private readonly IGridManager gridManager;
        public TerritoryState State { get; private set; }
        public List<Coordinates> Cells { get; private set; } = new List<Coordinates>();
        private float animationTimer = 0f;
        private Color baseColor;

        public Territory(List<Coordinates> cells, TerritoryState state)
        {
            gridManager = ServiceLocator.Get<IGridManager>();
            Cells = new List<Coordinates>(cells);
            State = state;

            if (cells.Count > 0)
            {
                gridPosition = cells[0]; // Position de référence
            }

            baseColor = GetColorForState(state);
        }

        public override void Update(float deltaTime)
        {
            if (!isActive) return;
            animationTimer += deltaTime;
        }

        public override void Draw()
        {
            if (!isActive || Cells.Count == 0) return;

            // Animation de conquête
            float alpha = State == TerritoryState.Conquered ?
                0.3f + 0.2f * MathF.Sin(animationTimer * 2f) : 0.2f;

            Color territoryColor = new Color(
                baseColor.R, baseColor.G, baseColor.B,
                (int)(255 * alpha)
            );

            // Dessiner toutes les cellules du territoire
            foreach (var cell in Cells)
            {
                Vector2 worldPos = gridManager.CoordinatesToWorld(cell);

                DrawRectangle(
                    (int)worldPos.X, (int)worldPos.Y,
                    CELL_SIZE, CELL_SIZE,
                    territoryColor
                );

                // Contour pour les territoires conquis
                if (State == TerritoryState.Conquered)
                {
                    DrawRectangleLines(
                        (int)worldPos.X, (int)worldPos.Y,
                        CELL_SIZE, CELL_SIZE,
                        baseColor
                    );
                }
            }
        }

        private Color GetColorForState(TerritoryState state)
        {
            return state switch
            {
                TerritoryState.Free => Color.Blank,
                TerritoryState.Walled => Color.Brown,
                TerritoryState.Conquered => Color.Green,
                TerritoryState.Contested => Color.Yellow,
                _ => Color.Gray
            };
        }

        public void ChangeState(TerritoryState newState)
        {
            State = newState;
            baseColor = GetColorForState(newState);
            animationTimer = 0f; // Reset animation
        }

        public bool Contains(Coordinates position)
        {
            return Cells.Contains(position);
        }

        public int GetArea()
        {
            return Cells.Count;
        }
    }
}