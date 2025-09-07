using SnakeJezzBall.Utils;

namespace SnakeJezzBall.Services.Interfaces
{
    public interface ITerritoryManager
    {
        List<Coordinates> ConqueredZones { get; }
        float ConqueredPercentage { get; }
        int ConqueredCellCount { get; }
        List<Coordinates> WallPositions { get; }

        void AddWall(Coordinates position);
        bool RemoveWall(Coordinates position);
        void RecalculateTerritories();
        void ClearAll();
        int CalculateTerritoryBonus();
        bool IsLevelObjectiveReached(float targetPercentage = 0.75f);
        List<Coordinates> GetFreePositions();

        // Événements
        event Action<List<Coordinates>>? OnZoneConquered;
        event Action<Coordinates>? OnWallAdded;
        event Action<float>? OnTerritoryPercentageChanged;
    }
}