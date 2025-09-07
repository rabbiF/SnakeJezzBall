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
        public IEnumerable<Coordinates> Body => body;
        public bool isInWallMode => CurrentState == SnakeState.WallBuilding;
        public Coordinates? lastWallCreated { get; private set; } = null;

        // === SYSTÈME DE CONSTRUCTION DE TERRITOIRES ===
        private List<Coordinates> currentWallPath = new List<Coordinates>();
        private bool isConstructing = false;
        private Coordinates constructionStartPosition;

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
                    if (stateTimer >= 2f)
                    {
                        ChangeState(SnakeState.Normal);
                    }
                    return;

                case SnakeState.Invincible:
                    if (stateTimer >= 5f)
                    {
                        ChangeState(SnakeState.Normal);
                    }
                    break;

                case SnakeState.WallBuilding:
                    // En mode construction, mouvement plus lent et stratégique
                    break;

                case SnakeState.Normal:
                default:
                    break;
            }
        }

        public void ChangeState(SnakeState newState)
        {
            // Transition spéciale pour le mode construction
            if (CurrentState == SnakeState.WallBuilding && newState != SnakeState.WallBuilding)
            {
                ExitConstructionMode();
            }
            else if (newState == SnakeState.WallBuilding && CurrentState != SnakeState.WallBuilding)
            {
                EnterConstructionMode();
            }

            CurrentState = newState;
            stateTimer = 0f;
        }

        private void EnterConstructionMode()
        {
            isConstructing = true;
            constructionStartPosition = head;
            currentWallPath.Clear();
            currentWallPath.Add(head); // Ajouter la position de départ
        }

        private void ExitConstructionMode()
        {
            if (isConstructing && currentWallPath.Count > 1)
            {
                // Finaliser la construction du mur
                CompleteTerritoryConstruction();
            }

            isConstructing = false;
            currentWallPath.Clear();
        }

        private void CompleteTerritoryConstruction()
        {
            // Vérifier si le chemin forme une ligne droite vers un bord
            // ou revient au point de départ (zone fermée)

            if (IsPathToEdge(currentWallPath) || IsClosedPath(currentWallPath))
            {
                // Succès : tous les murs du chemin deviennent permanents
                foreach (var wallPos in currentWallPath.Skip(1)) // Skip la position de départ
                {
                    lastWallCreated = wallPos; // Pour compatibility avec l'ancien système
                    // Le GameScene se chargera d'ajouter au TerritoryManager
                }
            }
            else
            {
                // Échec : pas de territoire créé
                // Possibilité d'ajouter une pénalité ici
            }
        }

        private bool IsPathToEdge(List<Coordinates> path)
        {
            if (path.Count < 2) return false;

            var lastPos = path.Last();

            // Vérifier si on a atteint un bord
            return lastPos.column == 0 ||
                   lastPos.column == gridManager.columns - 1 ||
                   lastPos.row == 0 ||
                   lastPos.row == gridManager.rows - 1;
        }

        private bool IsClosedPath(List<Coordinates> path)
        {
            if (path.Count < 4) return false; // Minimum pour une zone fermée

            var start = path.First();
            var end = path.Last();

            // Vérifier si on est revenu au point de départ ou près
            return Vector2.Distance(start.ToVector(), end.ToVector()) <= 1.5f;
        }

        public void Move()
        {
            if (CurrentState == SnakeState.Stunned) return;

            direction = nextDirection;
            Coordinates newHead = head + direction;

            gridPosition = newHead;

            // Vérifications de collision standard
            if (!gridManager.IsValidPosition(newHead))
            {
                if (CurrentState != SnakeState.Invincible)
                {
                    isGameOver = true;
                    gameOverReason = "Collision avec un obstacle !";
                    return;
                }
            }

            if (body.Contains(newHead) && CurrentState != SnakeState.Invincible)
            {
                isGameOver = true;
                gameOverReason = "Collision avec soi-même !";
                return;
            }

            // Gestion spéciale pour le mode construction
            if (CurrentState == SnakeState.WallBuilding && isConstructing)
            {
                // Ajouter la nouvelle position au chemin de construction
                currentWallPath.Add(newHead);

                // Vérifier si on a terminé la construction
                if (IsPathToEdge(currentWallPath) || IsClosedPath(currentWallPath))
                {
                    CompleteTerritoryConstruction();
                    ChangeState(SnakeState.Normal); // Sortir automatiquement du mode construction
                }
                else
                {
                    // Marquer temporairement cette position comme mur en construction
                    lastWallCreated = body.Last(); // Position qu'on va quitter
                }
            }
            else if (CurrentState == SnakeState.WallBuilding && !isConstructing)
            {
                // Mode mur simple (ancien système)
                lastWallCreated = body.Last();
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

                bool isHead = (segmentIndex == body.Count - 1);
                Color segmentColor = isHead ? GetHeadColor() : baseColor;

                DrawRectangle(
                    (int)worldPos.X, (int)worldPos.Y,
                    CELL_SIZE, CELL_SIZE,
                    segmentColor
                );

                DrawRectangleLines(
                    (int)worldPos.X, (int)worldPos.Y,
                    CELL_SIZE, CELL_SIZE,
                    Color.Black
                );

                segmentIndex++;
            }

            // Dessiner le chemin de construction en cours
            if (isConstructing && currentWallPath.Count > 1)
            {
                DrawConstructionPath();
            }
        }

        private void DrawConstructionPath()
        {
            for (int i = 1; i < currentWallPath.Count; i++) // Skip start position
            {
                Vector2 worldPos = gridManager.CoordinatesToWorld(currentWallPath[i]);

                // Effet visuel de construction
                Color constructionColor = new Color(255, 255, 0, 150); // Jaune transparente

                DrawRectangle(
                    (int)worldPos.X, (int)worldPos.Y,
                    CELL_SIZE, CELL_SIZE,
                    constructionColor
                );

                DrawRectangleLines(
                    (int)worldPos.X, (int)worldPos.Y,
                    CELL_SIZE, CELL_SIZE,
                    Color.Yellow
                );
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

        // === MÉTHODES PUBLIQUES (COMPATIBILITÉ) ===

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

        public void Reset(Coordinates startPosition, int startSize = 3)
        {
            body.Clear();
            direction = Coordinates.right;
            nextDirection = Coordinates.right;
            moveTimer = 0f;
            isGameOver = false;
            gameOverReason = "";
            lastWallCreated = null;
            isActive = true;

            // Reset système de construction
            isConstructing = false;
            currentWallPath.Clear();

            ChangeState(SnakeState.Normal);
            gridPosition = startPosition;

            for (int i = startSize - 1; i >= 0; i--)
            {
                body.Enqueue(startPosition - direction * i);
            }

            if (gridManager != null)
            {
                gridManager.UpdateSnakePosition(body);
            }
        }

        public void QuickReset()
        {
            Reset(head, body.Count);
        }

        public void ResetWithOptions(Coordinates startPosition, bool preserveSize = false, bool preserveState = false)
        {
            int currentSize = preserveSize ? body.Count : INITIAL_SNAKE_LENGTH;
            SnakeState currentState = preserveState ? CurrentState : SnakeState.Normal;

            Reset(startPosition, currentSize);

            if (preserveState)
            {
                ChangeState(currentState);
            }
        }

        // === INFORMATIONS DE DEBUG POUR TERRITOIRES ===
        public bool IsCurrentlyConstructing => isConstructing;
        public List<Coordinates> GetCurrentConstructionPath() => new List<Coordinates>(currentWallPath);
        public int GetConstructionProgress() => currentWallPath.Count;
    }
}