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
        private readonly IGridManager gridManager;
        private Queue<Coordinates> body = new Queue<Coordinates>();
        private Coordinates direction = Coordinates.right;
        private Coordinates nextDirection = Coordinates.right;
        private float moveTimer = 0f;

        // Machine à états
        public SnakeState CurrentState { get; private set; } = SnakeState.Normal;
        private float stateTimer = 0f;

        public bool isGameOver { get; private set; } = false;
        public string gameOverReason { get; private set; } = "";
        public Coordinates head => body.Last();
        public IEnumerable<Coordinates> Body => body; // Pour les collisions
        public bool isInWallMode => CurrentState == SnakeState.WallBuilding;
        public Coordinates? lastWallCreated { get; private set; } = null;

        public Snake(Coordinates start, int startSize = 3)
        {
            gridManager = ServiceLocator.Get<IGridManager>();
            gridPosition = start;

            // Construction de la queue vers la tête
            for (int i = startSize - 1; i >= 0; i--)
            {
                body.Enqueue(start - direction * i);
            }
        }

        public override void Update(float deltaTime)
        {
            if (!isActive || isGameOver) return;

            UpdateState(deltaTime);

            moveTimer += deltaTime;
            if (moveTimer >= SNAKE_MOVE_INTERVAL)
            {
                Move();
                moveTimer = 0f;
            }
        }

        private void UpdateState(float deltaTime)
        {
            stateTimer += deltaTime;

            switch (CurrentState)
            {
                case SnakeState.Stunned:
                    if (stateTimer >= 2f) // 2 secondes de stun
                    {
                        ChangeState(SnakeState.Normal);
                    }
                    return; // Pas de mouvement en mode stunned

                case SnakeState.Invincible:
                    if (stateTimer >= 5f) // 5 secondes d'invincibilité
                    {
                        ChangeState(SnakeState.Normal);
                    }
                    break;

                case SnakeState.WallBuilding:
                case SnakeState.Normal:
                default:
                    break;
            }
        }

        public void ChangeState(SnakeState newState)
        {
            CurrentState = newState;
            stateTimer = 0f;
        }

        public void Move()
        {
            if (CurrentState == SnakeState.Stunned) return;

            direction = nextDirection;
            Coordinates newHead = head + direction;

            gridPosition = newHead; // Mise à jour position GridObject

            // Vérifier collision avec les murs ou limites
            if (!gridManager.IsValidPosition(newHead))
            {
                if (CurrentState != SnakeState.Invincible)
                {
                    isGameOver = true;
                    gameOverReason = "Collision avec un obstacle !";
                    return;
                }
            }

            // Vérifier collision avec soi-même
            if (body.Contains(newHead) && CurrentState != SnakeState.Invincible)
            {
                isGameOver = true;
                gameOverReason = "Collision avec soi-même !";
                return;
            }

            // Gérer la création de murs
            if (CurrentState == SnakeState.WallBuilding && body.Count > 0)
            {
                lastWallCreated = body.Last(); // Position qu'on va quitter
                gridManager.AddWall(lastWallCreated.Value);
            }
            else
            {
                lastWallCreated = null;
            }

            // Mouvement normal
            body.Enqueue(newHead);
            body.Dequeue();

            // Mettre à jour la grille
            gridManager.UpdateSnakePosition(body);
        }

        public override void Draw()
        {
            int segmentIndex = 0;
            Color baseColor = GetSnakeColor();

            foreach (Coordinates segment in body)
            {
                Vector2 worldPos = gridManager.CoordinatesToWorld(segment);

                // Dernière position = tête (plus claire)
                bool isHead = (segmentIndex == body.Count - 1);
                Color segmentColor = isHead ? GetHeadColor() : baseColor;

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

        private Color GetSnakeColor()
        {
            return CurrentState switch
            {
                SnakeState.WallBuilding => Color.Orange,
                SnakeState.Stunned => Color.Gray,
                SnakeState.Invincible => Color.Gold,
                _ => DARK_GREEN
            };
        }

        private Color GetHeadColor()
        {
            return CurrentState switch
            {
                SnakeState.WallBuilding => Color.Yellow,
                SnakeState.Stunned => Color.LightGray,
                SnakeState.Invincible => Color.White,
                _ => Color.Lime
            };
        }

        public Coordinates GetCurrentDirection() => direction;

        public void ChangeDirection(Coordinates newDirection)
        {
            if (CurrentState == SnakeState.Stunned) return;

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
            body.Enqueue(head);
        }

        public void EnterWallMode()
        {
            ChangeState(SnakeState.WallBuilding);
        }

        public void ExitWallMode()
        {
            ChangeState(SnakeState.Normal);
        }

        // Nouvelles méthodes pour les effets spéciaux
        public void Stun(float duration = 2f)
        {
            ChangeState(SnakeState.Stunned);
            stateTimer = 0f;
        }

        public void MakeInvincible(float duration = 5f)
        {
            ChangeState(SnakeState.Invincible);
            stateTimer = 0f;
        }

        // === FONCTIONS DE RESTART ===
        public void Reset(Coordinates startPosition, int startSize = 3)
        {
            // Nettoyer l'état actuel
            body.Clear();

            // Reset des variables d'état
            direction = Coordinates.right;
            nextDirection = Coordinates.right;
            moveTimer = 0f;
            isGameOver = false;
            gameOverReason = "";
            lastWallCreated = null;
            isActive = true;

            // Reset de l'état de la machine à états
            ChangeState(SnakeState.Normal);

            // Mise à jour de la position dans GridObject
            gridPosition = startPosition;

            // Reconstruire le corps du serpent
            for (int i = startSize - 1; i >= 0; i--)
            {
                body.Enqueue(startPosition - direction * i);
            }

            // Mettre à jour la grille si nécessaire
            if (gridManager != null)
            {
                gridManager.UpdateSnakePosition(body);
            }
        }    
   
    }
}