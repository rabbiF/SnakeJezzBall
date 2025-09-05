using System.Numerics;
using SnakeJezzBall.Utils;

namespace SnakeJezzBall.Services.Interfaces
{
    public interface IGridManager
    {
        int columns { get; }
        int rows { get; }

        Vector2 CoordinatesToWorld(Coordinates coords);
        Coordinates WorldToCoordinates(Vector2 worldPos);
        bool IsValidPosition(Coordinates coords);

        void Draw();
    }
}