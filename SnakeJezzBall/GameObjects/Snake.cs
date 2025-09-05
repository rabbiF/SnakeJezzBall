using Raylib_cs;
using SnakeJezzBall.Services;
using SnakeJezzBall.Services.Interfaces;
using SnakeJezzBall.Utils;
using System.Numerics;
using static GameConfig;
using static Raylib_cs.Raylib;

namespace SnakeJezzBall.GameObjects
{
    public class Snake
    {
        private readonly IGridManager gridManager;
        private Queue<Coordinates> body = new Queue<Coordinates>();
        private Coordinates direction = Coordinates.right;
        private Coordinates nextDirection = Coordinates.right;        
        private float moveTimer = 0f;
        public bool isGameOver { get; private set; } = false;
        public string gameOverReason { get; private set; } = "";
        public Coordinates GetCurrentDirection() => direction;
        public Coordinates head => body.Last();
        public bool isInWallMode = false;
        public Coordinates? lastWallCreated { get; private set; } = null;



        // Pour les collisions et debug
        //public IEnumerable<Coordinates> Body => body;
        //public int Length => body.Count;

        public Snake(Coordinates start, int startSize = 3)
        {
            gridManager = ServiceLocator.Get<IGridManager>();
            // Construction de la queue vers la tête
            for (int i = startSize - 1; i >= 0; i--)
            {
                body.Enqueue(start - direction * i);
            }
        }

        public void Update(float deltaTime)
        {
            moveTimer += deltaTime;

            if (moveTimer >= SNAKE_MOVE_INTERVAL) 
            {
                Move();
                moveTimer = 0f;
            }
        }
        public void Move()
        {
            direction = nextDirection;
            Coordinates newHead = head + direction;

            // Vérifier collision avec les murs
            if (!gridManager.IsValidPosition(newHead))
            {
                isGameOver = true;
                gameOverReason = "Collision avec un mur !";
                return;
            }

            // Vérifier collision avec soi-même AVANT de bouger
            if (body.Contains(newHead))
            {
                isGameOver = true;
                gameOverReason = "Collision avec soi-même !";
                return;
            }

            if (isInWallMode && body.Count > 0)
            {
                lastWallCreated = body.Last(); // Position qu'on va quitter
            }
            else
            {
                lastWallCreated = null; // Pas de mur créé
            }


            // Mouvement normal
            body.Enqueue(newHead);
            body.Dequeue();
        }

        public void Draw()
        {
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
            if (newDirection == -direction) return;
            if (newDirection == Coordinates.zero) return;

            nextDirection = newDirection;
        }

        public bool IsCollidingWithSelf()
        {
            return body.Count != body.Distinct().Count();
        }

        public bool IsCollidingWithApple(Apple apple)
        {
            return head == apple.coordinates;
        }

        public void Grow()
        {
            // Ajouter un segment à la position actuelle de la tête
            // (sera déplacé au prochain Move())
            body.Enqueue(head);
        }

        /*public bool IsPositionOccupied(Coordinates position)
        {
            return body.Contains(position);
        }*/
        public void EnterWallMode()
        {
            isInWallMode = true;
        }                    
        

        public void ExitWallMode()
        {
            isInWallMode = false;
        }
    }
}