using SnakeJezzBall.Utils;

namespace SnakeJezzBall.Services.Interfaces
{
    /// <summary>
    /// Gestionnaire des territoires conquis par le joueur via la création de murs
    /// </summary>
    public interface ITerritoryManager
    {

        // Liste des coordonnées de toutes les zones conquises (fermées par des murs)
        List<Coordinates> ConqueredZones { get; }

        // Pourcentage de la grille totale qui a été conquise (0.0 à 1.0)
        float ConqueredPercentage { get; }


        // Nombre total de cases conquises
        int ConqueredCellCount { get; }


        // Liste de tous les murs placés
        List<Coordinates> WallPositions { get; }

        // Ajoute un mur à la position spécifiée et recalcule les territoires
        void AddWall(Coordinates position);

        // Retire un mur de la position spécifiée et recalcule les territoires    
        bool RemoveWall(Coordinates position);

  
        // Vérifie si une position donnée fait partie d'une zone conquise
        bool IsPositionConquered(Coordinates position);


        // Vérifie si une position donnée contient un mur
        bool IsWallAt(Coordinates position);

        // Détecte et retourne les nouvelles zones qui viennent d'être fermées
        List<Coordinates> GetNewlyClosedZones();

        // Remet à zéro tous les murs et territoires conquis
        void ClearAllTerritory();

         // Calcule le score bonus basé sur le pourcentage de territoire conquis
        int CalculateTerritoryBonus(int baseMultiplier = 1000);
  
        // Vérifie si l'objectif de territoire pour le niveau actuel est atteint
        bool IsLevelObjectiveReached(float targetPercentage = 0.75f);


        // Obtient toutes les zones libres (non conquises et sans murs)
        // Utile pour placer des pommes
        List<Coordinates> GetFreePositions();
    }
}