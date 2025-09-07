using Raylib_cs;
using static Raylib_cs.Raylib;
using SnakeJezzBall.Services.Interfaces;
using SnakeJezzBall.Utils;
using System.Numerics;

namespace SnakeJezzBall.Services
{
    public class GridManager : IGridManager
    {
        public int columns { get; private set; }
        public int rows { get; private set; }
        public readonly int cellSize;
        public Vector2 origin { get; private set; }

        private CellType[,] cells; // Remplace l'ancien TCell par CellType

        public GridManager(int columns, int rows, int cellSize)
        {
            this.columns = columns;
            this.rows = rows;
            this.cellSize = cellSize;

            int gridPixelWidth = columns * cellSize;
            int gridPixelHeight = rows * cellSize;

            origin = new Vector2(
                (GameConfig.SCREEN_WIDTH - gridPixelWidth) / 2,
                (GameConfig.SCREEN_HEIGHT - gridPixelHeight) / 2
            );

            cells = new CellType[columns, rows];
            ClearGrid();
        }

        public Vector2 CoordinatesToWorld(Coordinates coords)
        {
            return origin + coords.ToVector() * cellSize;
        }

        public Coordinates WorldToCoordinates(Vector2 worldPos)
        {
            Vector2 gridPos = (worldPos - origin) / cellSize;
            return new Coordinates((int)gridPos.X, (int)gridPos.Y);
        }

        public bool IsValidPosition(Coordinates coords)
        {
            if (!IsInBounds(coords))
                return false;

            // Une position est valide si elle est vide ou contient une pomme
            CellType cellType = GetCellType(coords);
            return cellType == CellType.Empty || cellType == CellType.Apple;
        }

        public bool IsInBounds(Coordinates coords)
        {
            return coords.column >= 0 && coords.column < columns &&
                   coords.row >= 0 && coords.row < rows;
        }

        public CellType GetCellType(Coordinates coords)
        {
            if (!IsInBounds(coords))
                throw new ArgumentOutOfRangeException(nameof(coords), "Coordinates are out of range");

            return cells[coords.column, coords.row];
        }

        public void SetCellType(Coordinates coords, CellType cellType)
        {
            if (!IsInBounds(coords))
                throw new ArgumentOutOfRangeException(nameof(coords), "Coordinates are out of range");

            cells[coords.column, coords.row] = cellType;
        }

        public void ClearGrid()
        {
            for (int col = 0; col < columns; col++)
            {
                for (int row = 0; row < rows; row++)
                {
                    cells[col, row] = CellType.Empty;
                }
            }
        }

        public List<Coordinates> GetEmptyPositions()
        {
            var emptyPositions = new List<Coordinates>();

            for (int col = 0; col < columns; col++)
            {
                for (int row = 0; row < rows; row++)
                {
                    var coords = new Coordinates(col, row);
                    if (GetCellType(coords) == CellType.Empty)
                    {
                        emptyPositions.Add(coords);
                    }
                }
            }

            return emptyPositions;
        }

        public void UpdateSnakePosition(IEnumerable<Coordinates> snakeBody)
        {
            // Nettoyer les anciennes positions du serpent
            ClearCellsOfType(CellType.Snake);

            // Marquer les nouvelles positions
            foreach (var segment in snakeBody)
            {
                if (IsInBounds(segment))
                {
                    SetCellType(segment, CellType.Snake);
                }
            }
        }

        public void UpdateApplePosition(Coordinates applePos)
        {
            // Nettoyer les anciennes pommes
            ClearCellsOfType(CellType.Apple);

            // Placer la nouvelle pomme
            if (IsInBounds(applePos))
            {
                SetCellType(applePos, CellType.Apple);
            }
        }

        public void AddWall(Coordinates wallPos)
        {
            if (IsInBounds(wallPos) && GetCellType(wallPos) == CellType.Empty)
            {
                SetCellType(wallPos, CellType.Wall);
            }
        }

        private void ClearCellsOfType(CellType cellType)
        {
            for (int col = 0; col < columns; col++)
            {
                for (int row = 0; row < rows; row++)
                {
                    if (cells[col, row] == cellType)
                    {
                        cells[col, row] = CellType.Empty;
                    }
                }
            }
        }

        public void Draw()
        {
            for (int column = 0; column < columns; column++)
            {
                for (int row = 0; row < rows; row++)
                {
                    Coordinates cellCoords = new Coordinates(column, row);
                    Vector2 cellPos = CoordinatesToWorld(cellCoords);

                    // Dessiner les lignes de la grille
                    Raylib.DrawRectangleLines(
                        (int)cellPos.X, (int)cellPos.Y,
                        cellSize, cellSize,
                        Color.DarkGray
                    );
                }
            }
        }

        // === MÉTHODES UTILITAIRES SUPPLÉMENTAIRES ===


        // Compte le nombre de cellules d'un type donné
        public int CountCellsOfType(CellType cellType)
        {
            int count = 0;
            for (int col = 0; col < columns; col++)
            {
                for (int row = 0; row < rows; row++)
                {
                    if (cells[col, row] == cellType)
                        count++;
                }
            }
            return count;
        }

  
        // Calcule le pourcentage de remplissage de la grille
        public float GetFillPercentage()
        {
            int totalCells = columns * rows;
            int filledCells = totalCells - CountCellsOfType(CellType.Empty);
            return (float)filledCells / totalCells * 100f;
        }


        // Vérifie si une position est accessible (pas de mur)
        public bool IsAccessible(Coordinates coords)
        {
            if (!IsInBounds(coords))
                return false;

            CellType cellType = GetCellType(coords);
            return cellType != CellType.Wall;
        }


        // Obtient toutes les positions d'un type spécifique
        public List<Coordinates> GetPositionsOfType(CellType cellType)
        {
            var positions = new List<Coordinates>();

            for (int col = 0; col < columns; col++)
            {
                for (int row = 0; row < rows; row++)
                {
                    var coords = new Coordinates(col, row);
                    if (GetCellType(coords) == cellType)
                    {
                        positions.Add(coords);
                    }
                }
            }

            return positions;
        }


        // Nettoie tous les murs temporaires
        public void ClearTemporaryWalls()
        {
            ClearCellsOfType(CellType.TemporaryWall);
        }

     
        // Convertit tous les murs temporaires en murs permanents
        public void ConvertTemporaryWalls()
        {
            for (int col = 0; col < columns; col++)
            {
                for (int row = 0; row < rows; row++)
                {
                    if (cells[col, row] == CellType.TemporaryWall)
                    {
                        cells[col, row] = CellType.Wall;
                    }
                }
            }
        }

        // Vérifie s'il y a un chemin libre entre deux points
        // (Algorithme simple, peut être amélioré avec A* si nécessaire)  
        public bool HasPathBetween(Coordinates start, Coordinates end)
        {
            // Implémentation simple - vérifie juste si les deux points sont accessibles
            // Pour une vraie pathfinding, il faudrait implémenter A* ou BFS
            return IsAccessible(start) && IsAccessible(end);
        }

 
        // Obtient les voisins accessibles d'une position
        public List<Coordinates> GetAccessibleNeighbors(Coordinates position)
        {
            var neighbors = new List<Coordinates>();
            var directions = new[]
            {
                Coordinates.up,
                Coordinates.down,
                Coordinates.left,
                Coordinates.right
            };

            foreach (var direction in directions)
            {
                var neighbor = position + direction;
                if (IsAccessible(neighbor))
                {
                    neighbors.Add(neighbor);
                }
            }

            return neighbors;
        }

        // Remplit une zone avec un type de cellule
        public void FillArea(Coordinates topLeft, Coordinates bottomRight, CellType cellType)
        {
            int startCol = Math.Max(0, Math.Min(topLeft.column, bottomRight.column));
            int endCol = Math.Min(columns - 1, Math.Max(topLeft.column, bottomRight.column));
            int startRow = Math.Max(0, Math.Min(topLeft.row, bottomRight.row));
            int endRow = Math.Min(rows - 1, Math.Max(topLeft.row, bottomRight.row));

            for (int col = startCol; col <= endCol; col++)
            {
                for (int row = startRow; row <= endRow; row++)
                {
                    cells[col, row] = cellType;
                }
            }
        }

 
        // Crée un motif de murs aléatoires
        public void GenerateRandomWalls(int wallCount, Random? randomGenerator = null)
        {
            var random = randomGenerator ?? new Random();
            var emptyPositions = GetEmptyPositions();

            int wallsToCreate = Math.Min(wallCount, emptyPositions.Count);

            for (int i = 0; i < wallsToCreate; i++)
            {
                int randomIndex = random.Next(emptyPositions.Count);
                var wallPosition = emptyPositions[randomIndex];

                SetCellType(wallPosition, CellType.Wall);
                emptyPositions.RemoveAt(randomIndex);
            }
        }
    }
}