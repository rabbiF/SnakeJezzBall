using Raylib_cs;
using SnakeJezzBall.Services;
using SnakeJezzBall.Services.Interfaces;
using SnakeJezzBall.Utils;
using System.Numerics;
using static GameConfig;
using static Raylib_cs.Raylib;

namespace SnakeJezzBall.GameObjects
{
    public class Snake : GridObject
    {
        private readonly CollisionManager collisionManager;
        private Queue<Coordinates> body = new Queue<Coordinates>();
        private Coordinates direction = Coordinates.right;
        private Coordinates nextDirection = Coordinates.right;
        private float moveTimer = 0f;

        public bool IsGameOver { get; private set; } = false;
        public string GameOverReason { get; private set; } = "";
        public Coordinates Head => body.Last();
        public IEnumerable<Coordinates> Body => body; // Expose pour les collisions
        public bool IsInWallMode { get; private set; } = false;
        public Coordinates? LastWallCreated { get; private set; } = null;

        public Snake(Coordinates start, CollisionManager collisionManager, int startSize = 3)
        {
            this.collisionManager = collisionManager;

            // Construction de la queue vers la tête
            for (int i = startSize - 1; i >= 0; i--)
            {
                body.Enqueue(start - direction * i);
            }
        }              

        public void Move()
        {
            direction = nextDirection;
            Coordinates newHead = Head + direction;

            // Utiliser le gestionnaire de collisions
            var collisionResult = collisionManager.CheckSnakeMovement(this, newHead);
            if (collisionResult.HasCollision)
            {
                IsGameOver = true;
                GameOverReason = collisionResult.Message;
                return;
            }

            // Gérer la création de murs
            HandleWallCreation();

            // Mouvement normal
            body.Enqueue(newHead);
            body.Dequeue();
        }

        private void HandleWallCreation()
        {
            if (IsInWallMode && body.Count > 0)
            {
                LastWallCreated = body.Last(); // Position qu'on va quitter
                collisionManager.AddWall(LastWallCreated.Value);
            }
            else
            {
                LastWallCreated = null;
            }
        }

        public override void Update(float dt)
        {
            if (IsGameOver) return;

            moveTimer += dt;

            if (moveTimer >= SNAKE_MOVE_INTERVAL)
            {
                Move();
                moveTimer = 0f;
            }
        }

        public override void Draw()
        {
            var gridManager = ServiceLocator.Get<IGridManager>();
            int segmentIndex = 0;

            foreach (Coordinates segment in body)
            {
                Vector2 worldPos = gridManager.CoordinatesToWorld(segment);

                // Dernière position = tête (plus claire)
                bool isHead = (segmentIndex == body.Count - 1);
                Color segmentColor = isHead ? Color.Lime : DARK_GREEN;

                DrawRectangle(
                    (int)worldPos.X, (int)worldPos.Y,
                    CELL_SIZE, CELL_SIZE,
                    segmentColor
                );

                // Contour pour plus de clarté
                DrawRectangleLines(
                    (int)worldPos.X, (int)worldPos.Y,
                    CELL_SIZE, CELL_SIZE,
                    Color.Black
                );

                segmentIndex++;
            }
        }

        public void ChangeDirection(Coordinates newDirection)
        {
            // Empêcher les demi-tours
            if (newDirection == -direction || newDirection == Coordinates.zero)
                return;

            nextDirection = newDirection;
        }

        public bool IsCollidingWithApple(Apple apple)
        {
            return Head == apple.coordinates;
        }

        public void Grow()
        {
            // Ajouter un segment à la position actuelle de la tête
            body.Enqueue(Head);
        }

        public void EnterWallMode()
        {
            IsInWallMode = true;
        }

        public void ExitWallMode()
        {
            IsInWallMode = false;
        }

        public void Reset(Coordinates startPosition, int startSize = 3)
        {
            body.Clear();
            direction = Coordinates.right;
            nextDirection = Coordinates.right;
            moveTimer = 0f;
            IsGameOver = false;
            GameOverReason = "";
            IsInWallMode = false;
            LastWallCreated = null;

            // Reconstruire le corps
            for (int i = startSize - 1; i >= 0; i--)
            {
                body.Enqueue(startPosition - direction * i);
            }
        }
    }
}