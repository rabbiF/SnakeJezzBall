using SnakeJezzBall.Services.Interfaces;
using SnakeJezzBall.Utils;

namespace SnakeJezzBall.Services
{
    // Gestionnaire des territoires conquis par le joueur via la création de murs
    public class TerritoryManager : ITerritoryManager
    {
        private readonly IGridManager gridManager;
        private readonly HashSet<Coordinates> walls = new HashSet<Coordinates>();
        private readonly List<Coordinates> conqueredZones = new List<Coordinates>();
        private List<Coordinates> previousConqueredZones = new List<Coordinates>();

        // Événements optionnels
        public event Action<List<Coordinates>>? OnZoneConquered;
        public event Action<Coordinates>? OnWallAdded;
        public event Action<float>? OnTerritoryPercentageChanged;

        public TerritoryManager()
        {
            gridManager = ServiceLocator.Get<IGridManager>();
        }

        #region Propriétés publiques

        public List<Coordinates> ConqueredZones => new List<Coordinates>(conqueredZones);

        public float ConqueredPercentage
        {
            get
            {
                int totalCells = gridManager.Width * gridManager.Height;
                return totalCells > 0 ? conqueredZones.Count / (float)totalCells : 0f;
            }
        }

        public int ConqueredCellCount => conqueredZones.Count;

        public List<Coordinates> WallPositions => new List<Coordinates>(walls);

        #endregion

        #region Gestion des murs

        public void AddWall(Coordinates position)
        {
            if (!gridManager.IsValidPosition(position))
                return;

            bool wasAdded = walls.Add(position);
            if (wasAdded)
            {
                float oldPercentage = ConqueredPercentage;
                RecalculateTerritory();

                // Déclencher les événements
                OnWallAdded?.Invoke(position);

                float newPercentage = ConqueredPercentage;
                if (Math.Abs(newPercentage - oldPercentage) > 0.001f)
                {
                    OnTerritoryPercentageChanged?.Invoke(newPercentage);
                }
            }
        }

        public bool RemoveWall(Coordinates position)
        {
            bool wasRemoved = walls.Remove(position);
            if (wasRemoved)
            {
                float oldPercentage = ConqueredPercentage;
                RecalculateTerritory();

                float newPercentage = ConqueredPercentage;
                if (Math.Abs(newPercentage - oldPercentage) > 0.001f)
                {
                    OnTerritoryPercentageChanged?.Invoke(newPercentage);
                }
            }
            return wasRemoved;
        }

        public bool IsWallAt(Coordinates position)
        {
            return walls.Contains(position);
        }

        #endregion

        #region Gestion des territoires

        public bool IsPositionConquered(Coordinates position)
        {
            return conqueredZones.Contains(position);
        }

        public List<Coordinates> GetNewlyClosedZones()
        {
            var newZones = conqueredZones.Except(previousConqueredZones).ToList();
            return newZones;
        }

        public List<Coordinates> GetFreePositions()
        {
            var freePositions = new List<Coordinates>();

            for (int x = 0; x < gridManager.Width; x++)
            {
                for (int y = 0; y < gridManager.Height; y++)
                {
                    var pos = new Coordinates(x, y);

                    // Position libre = pas de mur ET pas conquise
                    if (!walls.Contains(pos) && !conqueredZones.Contains(pos))
                    {
                        freePositions.Add(pos);
                    }
                }
            }

            return freePositions;
        }

        #endregion

        #region Calculs et objectifs

        public int CalculateTerritoryBonus(int baseMultiplier = 1000)
        {
            return (int)(ConqueredPercentage * baseMultiplier);
        }

        public bool IsLevelObjectiveReached(float targetPercentage = 0.75f)
        {
            return ConqueredPercentage >= targetPercentage;
        }

        #endregion

        #region Gestion globale

        public void ClearAllTerritory()
        {
            walls.Clear();
            conqueredZones.Clear();
            previousConqueredZones.Clear();

            OnTerritoryPercentageChanged?.Invoke(0f);
        }

        // Recalcule tous les territoires conquis en utilisant l'algorithme FloodFill     
        private void RecalculateTerritory()
        {
            // Sauvegarder l'état précédent pour détecter les nouvelles zones
            previousConqueredZones = new List<Coordinates>(conqueredZones);
            conqueredZones.Clear();

            // Créer un HashSet des positions déjà vérifiées pour optimiser
            var alreadyChecked = new HashSet<Coordinates>();

            // Parcourir toute la grille pour trouver les zones fermées
            for (int x = 0; x < gridManager.Width; x++)
            {
                for (int y = 0; y < gridManager.Height; y++)
                {
                    var startPos = new Coordinates(x, y);

                    // Ignorer si c'est un mur ou déjà vérifié
                    if (walls.Contains(startPos) || alreadyChecked.Contains(startPos))
                        continue;

                    // Vérifier si cette zone est fermée
                    if (IsZoneClosed(startPos))
                    {
                        // Récupérer toute la zone fermée
                        var zone = FloodFill.Fill(
                            startPos,
                            gridManager.Width,
                            gridManager.Height,
                            pos => walls.Contains(pos)
                        );

                        // Ajouter toutes les positions de cette zone
                        conqueredZones.AddRange(zone);

                        // Marquer ces positions comme vérifiées
                        foreach (var pos in zone)
                        {
                            alreadyChecked.Add(pos);
                        }
                    }
                    else
                    {
                        // Zone ouverte, marquer comme vérifiée quand même
                        var openZone = FloodFill.Fill(
                            startPos,
                            gridManager.Width,
                            gridManager.Height,
                            pos => walls.Contains(pos)
                        );

                        foreach (var pos in openZone)
                        {
                            alreadyChecked.Add(pos);
                        }
                    }
                }
            }

            // Détecter et signaler les nouvelles zones conquises
            var newlyConquered = GetNewlyClosedZones();
            if (newlyConquered.Count > 0)
            {
                OnZoneConquered?.Invoke(newlyConquered);
            }
        }

        // Vérifie si une zone est fermée (entourée de murs)
        private bool IsZoneClosed(Coordinates startPosition)
        {
            // Utiliser FloodFill pour obtenir toute la zone connectée
            var zone = FloodFill.Fill(
                startPosition,
                gridManager.Width,
                gridManager.Height,
                pos => walls.Contains(pos)
            );

            // Vérifier si aucune position de la zone ne touche les bords de la grille
            foreach (var pos in zone)
            {
                if (pos.column == 0 || pos.column == gridManager.Width - 1 ||
                    pos.row == 0 || pos.row == gridManager.Height - 1)
                {
                    return false; // Zone ouverte (touche les bords)
                }
            }

            return true; // Zone fermée
        }

        #endregion
    }
}