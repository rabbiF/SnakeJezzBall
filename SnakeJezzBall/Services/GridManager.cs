using Raylib_cs;
using static Raylib_cs.Raylib;
using SnakeJezzBall.Services.Interfaces;
using SnakeJezzBall.Utils;
using System.Numerics;

public class GridManager <TCell> : IGridManager
{
    public int columns { get; private set; }
    public int rows { get; private set; }
    public readonly int cellSize;
    public Vector2 Origin { get; private set; }

    private TCell[,] cells; // 0 = vide, 1 = snake, 2 = pomme, 3 = mur, etc.

    public GridManager(int columns, int rows, int cellSize)
    {
        this.columns = columns;
        this.rows = rows;
        this.cellSize = cellSize;

        int gridPixelWidth = columns * cellSize;
        int gridPixelHeight = rows * cellSize;

        Origin = new Vector2(
            (GameConfig.SCREEN_WIDTH - gridPixelWidth) / 2,
            (GameConfig.SCREEN_HEIGHT - gridPixelHeight) / 2
        );

        cells = new TCell[columns, rows]; 
    }

    public Vector2 CoordinatesToWorld(Coordinates coords)
    {
        return Origin + coords.ToVector() * cellSize;
    }

    public Coordinates WorldToCoordinates(Vector2 worldPos)
    {
        Vector2 gridPos = (worldPos - Origin) / cellSize;
        return new Coordinates((int)gridPos.X, (int)gridPos.Y);
    }

    public bool IsValidPosition(Coordinates coords)
    {
        return coords.column >= 0 && coords.column < columns &&
               coords.row >= 0 && coords.row < rows;
    }

    public TCell GetCell(Coordinates coords)
    {
        if (!IsValidPosition(coords))
            throw new ArgumentOutOfRangeException(nameof(coords), "Coordinates are out of range");

        return cells[coords.column, coords.row];
    }

    public void SetCell(Coordinates coords, TCell value)
    {
        if (!IsValidPosition(coords))
            throw new ArgumentOutOfRangeException(nameof(coords), "Coordinates are out of range");

        cells[coords.column, coords.row] = value;
    }

    public void Draw()
    {
        for (int column = 0; column < columns; column++)
        {
            for (int row = 0; row < rows; row++)
            {
                Coordinates cellCoords = new Coordinates(column, row);
                Vector2 cellPos = CoordinatesToWorld(cellCoords);

                Raylib.DrawRectangleLines(
                    (int)cellPos.X, (int)cellPos.Y,
                    cellSize, cellSize,
                    Color.DarkGray
                );
            }
        }
    }
}