using SnakeJezzBall.GameObjects;
using SnakeJezzBall.Services.Interfaces;
using SnakeJezzBall.Utils;

namespace SnakeJezzBall.Services
{
    public class CollisionManager
    {
        private readonly IGridManager gridManager;
        private readonly List<Wall> walls;

        public CollisionManager(IGridManager gridManager, List<Wall> walls)
        {
            this.gridManager = gridManager;
            this.walls = walls;
        }

        public CollisionResult CheckSnakeMovement(Snake snake, Coordinates newPosition)
        {
            // Vérifier les limites de la grille
            if (!gridManager.IsValidPosition(newPosition))
            {
                return new CollisionResult
                {
                    HasCollision = true,
                    Type = CollisionType.Boundary,
                    Message = "Collision avec les limites !"
                };
            }

            // Vérifier collision avec les murs
            if (HasWallAt(newPosition))
            {
                return new CollisionResult
                {
                    HasCollision = true,
                    Type = CollisionType.Wall,
                    Message = "Collision avec un mur !"
                };
            }

            // Vérifier collision avec soi-même
            if (snake.Body.Contains(newPosition))
            {
                return new CollisionResult
                {
                    HasCollision = true,
                    Type = CollisionType.Self,
                    Message = "Collision avec soi-même !"
                };
            }

            return new CollisionResult { HasCollision = false };
        }

        public bool HasWallAt(Coordinates position)
        {
            return walls.Any(wall => wall.GridPosition == position);
        }

        public void AddWall(Coordinates position)
        {
            if (!HasWallAt(position))
            {
                walls.Add(new Wall(position));
            }
        }
    }

    public class CollisionResult
    {
        public bool HasCollision { get; set; }
        public CollisionType Type { get; set; }
        public string Message { get; set; } = "";
    }  
}