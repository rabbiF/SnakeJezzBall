
namespace SnakeJezzBall.GameObjects
{
    public abstract class GameObject
    {
        public bool isActive { get; set; } = true;
        public abstract void Update(float deltaTime);
        public abstract void Draw();
    }
}