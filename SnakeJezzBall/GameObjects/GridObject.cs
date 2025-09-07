using SnakeJezzBall.Services;
using SnakeJezzBall.Services.Interfaces;
using SnakeJezzBall.Utils;
using System.Numerics;

namespace SnakeJezzBall.GameObjects
{
    public abstract class GridObject : GameObject
    {
        public Coordinates gridPosition { get; set; }

        protected Vector2 worldPosition =>
            ServiceLocator.Get<IGridManager>().CoordinatesToWorld(gridPosition);

        public abstract override void Update(float dt);
        public abstract override void Draw();
    }
}