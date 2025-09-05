
namespace SnakeJezzBall.Scenes
{
    public abstract class Scene
    {
        public abstract void Load();
        public abstract void Update(float deltaTime);
        public abstract void Draw();
        public abstract void Unload();
        public abstract int PositionTextX(string text, int fontSize);
    }
}
