
namespace SnakeJezzBall.Utils
{
    public static class FloodFill
    {

        // Remplit une zone connectée en partant d'un point
        public static List<Coordinates> Fill(
            Coordinates start,
            int gridWidth,
            int gridHeight,
            Func<Coordinates, bool> isBlocked)
        {
            List<Coordinates> filled = new List<Coordinates>();
            Queue<Coordinates> toVisit = new Queue<Coordinates>();
            HashSet<Coordinates> visited = new HashSet<Coordinates>();

            toVisit.Enqueue(start);
            visited.Add(start);

            while (toVisit.Count > 0)
            {
                Coordinates current = toVisit.Dequeue();
                filled.Add(current);

                // Vérifier les 4 directions
                Coordinates[] directions = {
                    Coordinates.up, Coordinates.down,
                    Coordinates.left, Coordinates.right
                };

                foreach (var direction in directions)
                {
                    Coordinates neighbor = current + direction;

                    // Vérifier les limites de la grille
                    if (neighbor.column < 0 || neighbor.column >= gridWidth ||
                        neighbor.row < 0 || neighbor.row >= gridHeight)
                        continue;

                    // Si déjà visité ou bloqué, ignorer
                    if (visited.Contains(neighbor) || isBlocked(neighbor))
                        continue;

                    visited.Add(neighbor);
                    toVisit.Enqueue(neighbor);
                }
            }

            return filled;
        }
   
        // Détecte si une zone est fermée (entourée de murs)

        public static bool IsZoneClosed(
            Coordinates start,
            int gridWidth,
            int gridHeight,
            HashSet<Coordinates> walls)
        {
            var zone = Fill(start, gridWidth, gridHeight, pos => walls.Contains(pos));

            // Si la zone touche les bords, elle n'est pas fermée
            foreach (var coord in zone)
            {
                if (coord.column == 0 || coord.column == gridWidth - 1 ||
                    coord.row == 0 || coord.row == gridHeight - 1)
                {
                    return false; // Zone ouverte (touche les bords)
                }
            }

            return true; // Zone fermée !
        }
    }
}