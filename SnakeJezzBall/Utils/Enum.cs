namespace SnakeJezzBall.Utils
{
    public enum AppleType
    {
        Normal,    // +1 segment, points normaux
        Golden,    // +2 segment, bonus points  
        Shrink     // 0 segment, points
    }

    public enum CollisionType
    {
        None,
        Boundary,
        Wall,
        Self,
        Apple
    }
}
