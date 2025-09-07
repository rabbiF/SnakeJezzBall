using System.Numerics;
using SnakeJezzBall.Utils;

namespace SnakeJezzBall.Services.Interfaces
{
    public interface IGridManager
    {
        // === PROPRIÉTÉS PRINCIPALES ===
        int columns { get; }
        int rows { get; }

        // === MÉTHODES DE CONVERSION ===
        Vector2 CoordinatesToWorld(Coordinates coords);
        Coordinates WorldToCoordinates(Vector2 worldPos);

        // === MÉTHODES DE VALIDATION ===
        bool IsValidPosition(Coordinates coords);
        bool IsInBounds(Coordinates coords);
        bool IsAccessible(Coordinates coords);

        // === GESTION DES TYPES DE CELLULES ===
        CellType GetCellType(Coordinates coords);
        void SetCellType(Coordinates coords, CellType cellType);

        // === MÉTHODES DE RECHERCHE ===
        List<Coordinates> GetEmptyPositions();
        List<Coordinates> GetPositionsOfType(CellType cellType);
        List<Coordinates> GetAccessibleNeighbors(Coordinates position);

        // === MISE À JOUR DES OBJETS ===
        void UpdateSnakePosition(IEnumerable<Coordinates> snakeBody);
        void UpdateApplePosition(Coordinates applePos);
        void AddWall(Coordinates wallPos);

        // === GESTION DE LA GRILLE ===
        void ClearGrid();
        void FillArea(Coordinates topLeft, Coordinates bottomRight, CellType cellType);
        void GenerateRandomWalls(int wallCount, Random? randomGenerator = null);

        // === GESTION DES MURS ===
        void ClearTemporaryWalls();
        void ConvertTemporaryWalls();

        // === MÉTHODES UTILITAIRES ===
        int CountCellsOfType(CellType cellType);
        float GetFillPercentage();
        bool HasPathBetween(Coordinates start, Coordinates end);

        // === AFFICHAGE ===
        void Draw();
    }
}