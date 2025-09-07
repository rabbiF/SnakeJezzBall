namespace SnakeJezzBall.Utils
{
    public enum AppleType
    {
        Normal,    // +1 segment, points normaux
        Golden,    // +2 segment, bonus points  
        Shrink     // 0 segment, points
    }

    public enum CollisionType
    {
        None,
        Boundary,
        Wall,
        Self,
        Apple
    }

    public enum SnakeState
    {
        Normal,        // Mouvement classique
        WallBuilding,  // Création de murs
        Stunned,       // Temporairement immobilisé (si pomme spéciale)
        Invincible     // Mode temporaire d'invincibilité
    }

    public enum CellType
    {
        Empty = 0,
        Snake = 1,
        Apple = 2,
        Wall = 3,
        TemporaryWall = 4  // Murs en construction
    }

    public enum GameState
    {
        Menu,
        Playing,
        Paused,
        GameOver,
        Victory
    }

    public enum Direction
    {
        Up,
        Down,
        Left,
        Right
    }

    public enum TerritoryState
    {
        Free,           // Zone libre
        Walled,         // Zone avec murs
        Conquered,      // Zone conquise/fermée
        Contested       // Zone en cours de conquête
    }

    public enum WallType
    {
        Permanent,      // Mur permanent (bord de grille)
        PlayerMade,     // Mur créé par le joueur
        Temporary       // Mur temporaire (en construction)
    }
}
