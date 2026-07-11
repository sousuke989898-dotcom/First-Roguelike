
public enum TileType
{
    None,
    Wall,
    Floor,
    Road,

    Stairs_Up,
    Stairs_Down,
    Door_Closed,
    Door_Opend,
}

public static class TileTypeExtensions
{

    public static bool CanMove(this TileType tile)
    {
        return tile switch
        {
            TileType.Wall or TileType.None => false,
            _ => true,
        };
    }

    public static bool CanSpawn(this TileType tile)
    {
        return tile switch
        {
            TileType.Floor => true,
            _ => false,
        };
    }
}
