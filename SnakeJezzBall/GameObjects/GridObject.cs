using SnakeJezzBall.Services;
using SnakeJezzBall.Services.Interfaces;
using SnakeJezzBall.Utils;
using System.Numerics;

namespace SnakeJezzBall.GameObjects
{
    public abstract class GridObject : GameObject
    {
        public Coordinates GridPosition { get; set; }

        protected Vector2 WorldPosition =>
            ServiceLocator.Get<IGridManager>().CoordinatesToWorld(GridPosition);

        public abstract override void Update(float deltaTime);
        public abstract override void Draw();
    }
}