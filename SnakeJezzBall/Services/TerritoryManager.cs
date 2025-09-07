using SnakeJezzBall.Services.Interfaces;
using SnakeJezzBall.Utils;

namespace SnakeJezzBall.Services
{
    public class TerritoryManager : ITerritoryManager
    {
        private readonly IGridManager gridManager;
        private readonly HashSet<Coordinates> walls = new HashSet<Coordinates>();
        private readonly List<Coordinates> conqueredZones = new List<Coordinates>();
        private List<Coordinates> previousConqueredZones = new List<Coordinates>();

        // Événements
        public event Action<List<Coordinates>>? OnZoneConquered;
        public event Action<Coordinates>? OnWallAdded;
        public event Action<float>? OnTerritoryPercentageChanged;

        // Propriétés
        public List<Coordinates> ConqueredZones => new List<Coordinates>(conqueredZones);
        public float ConqueredPercentage => conqueredZones.Count / (float)(gridManager.columns * gridManager.rows);
        public int ConqueredCellCount => conqueredZones.Count;
        public List<Coordinates> WallPositions => walls.ToList();

        public TerritoryManager(IGridManager gridManager)
        {
            this.gridManager = gridManager;
        }

        public void AddWall(Coordinates position)
        {
            if (!gridManager.IsInBounds(position)) return;
            
            if (walls.Add(position))
            {
                // Utiliser SetCellType si disponible, sinon logique alternative
                try 
                {
                    gridManager.SetCellType(position, CellType.Wall);
                }
                catch (Exception)
                {
                    // Si SetCellType n'existe pas, utiliser une logique alternative
                    // ou simplement tracker les murs dans notre HashSet
                }
                
                OnWallAdded?.Invoke(position);
                RecalculateTerritories();
            }
        }

        public bool RemoveWall(Coordinates position)
        {
            if (walls.Remove(position))
            {
                gridManager.SetCellType(position, CellType.Empty);
                RecalculateTerritories();
                return true;
            }
            return false;
        }

        public void RecalculateTerritories()
        {
            previousConqueredZones = new List<Coordinates>(conqueredZones);
            conqueredZones.Clear();

            // Marquer toutes les cellules comme non visitées
            var visited = new bool[gridManager.columns, gridManager.rows];

            // Algorithme de flood-fill pour détecter les zones fermées
            for (int col = 0; col < gridManager.columns; col++)
            {
                for (int row = 0; row < gridManager.rows; row++)
                {
                    var coords = new Coordinates(col, row);

                    if (!visited[col, row] && CanBeConquered(coords))
                    {
                        var zone = FloodFillZone(coords, visited);
                        if (IsZoneClosed(zone))
                        {
                            foreach (var cell in zone)
                            {
                                conqueredZones.Add(cell);
                                gridManager.SetCellType(cell, CellType.Wall); // Marquer comme territoire
                            }
                        }
                    }
                }
            }

            // Déclencher événements si changements
            if (conqueredZones.Count != previousConqueredZones.Count)
            {
                var newZones = conqueredZones.Except(previousConqueredZones).ToList();
                if (newZones.Count > 0)
                {
                    OnZoneConquered?.Invoke(newZones);
                }
                OnTerritoryPercentageChanged?.Invoke(ConqueredPercentage);
            }
        }

        private bool CanBeConquered(Coordinates position)
        {
            // Ne peut pas conquérir les murs, le serpent, ou les zones déjà conquises
            var cellType = gridManager.GetCellType(position);
            return cellType == CellType.Empty || cellType == CellType.Apple;
        }

        private List<Coordinates> FloodFillZone(Coordinates start, bool[,] visited)
        {
            var zone = new List<Coordinates>();
            var queue = new Queue<Coordinates>();
            queue.Enqueue(start);

            while (queue.Count > 0)
            {
                var current = queue.Dequeue();

                if (!gridManager.IsInBounds(current) ||
                    visited[current.column, current.row] ||
                    !CanBeConquered(current))
                {
                    continue;
                }

                visited[current.column, current.row] = true;
                zone.Add(current);

                // Ajouter les voisins
                var neighbors = new[]
                {
                    current + Coordinates.up,
                    current + Coordinates.down,
                    current + Coordinates.left,
                    current + Coordinates.right
                };

                foreach (var neighbor in neighbors)
                {
                    queue.Enqueue(neighbor);
                }
            }

            return zone;
        }

        private bool IsZoneClosed(List<Coordinates> zone)
        {
            // Une zone est fermée si elle ne touche aucun bord de la grille
            foreach (var cell in zone)
            {
                if (cell.column == 0 || cell.column == gridManager.columns - 1 ||
                    cell.row == 0 || cell.row == gridManager.rows - 1)
                {
                    return false; // Zone touche un bord
                }
            }
            return true;
        }

        public void ClearAll()
        {
            walls.Clear();
            conqueredZones.Clear();

            // Nettoyer la grille
            for (int col = 0; col < gridManager.columns; col++)
            {
                for (int row = 0; row < gridManager.rows; row++)
                {
                    var coords = new Coordinates(col, row);
                    if (gridManager.GetCellType(coords) == CellType.Wall)
                    {
                        gridManager.SetCellType(coords, CellType.Empty);
                    }
                }
            }
        }

        public int CalculateTerritoryBonus()
        {
            // Bonus basé sur le pourcentage de territoire conquis
            return (int)(ConqueredPercentage * 1000);
        }

        public bool IsLevelObjectiveReached(float targetPercentage = 0.75f)
        {
            return ConqueredPercentage >= targetPercentage;
        }

        public List<Coordinates> GetFreePositions()
        {
            var freePositions = new List<Coordinates>();

            for (int col = 0; col < gridManager.columns; col++)
            {
                for (int row = 0; row < gridManager.rows; row++)
                {
                    var coords = new Coordinates(col, row);
                    var cellType = gridManager.GetCellType(coords);

                    if (cellType == CellType.Empty &&
                        !walls.Contains(coords) &&
                        !conqueredZones.Contains(coords))
                    {
                        freePositions.Add(coords);
                    }
                }
            }

            return freePositions;
        }
    }
}