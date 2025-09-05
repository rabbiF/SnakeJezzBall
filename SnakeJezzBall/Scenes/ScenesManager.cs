namespace SnakeJezzBall.Scenes
{
    public class ScenesManager
    {
        private static Scene? currentScene;

        public static void Load<T>() where T : Scene, new()
        {
            currentScene?.Unload();
            currentScene = new T();
            currentScene.Load();
        }

        public static void Update(float dt)
        {
            currentScene?.Update(dt);
        }

        public static void Draw()
        {
            currentScene?.Draw();
        }
    }
}
