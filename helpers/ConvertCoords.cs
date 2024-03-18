using Godot;

namespace OutofTheHole.Helpers;


public class CoordsHelper
{
    public static Vector2 ConvertCoords(Vector2 originRes, Vector2 destRes, Vector2 coords)
    {
        coords.Y = (coords.Y / originRes.Y) * destRes.Y;
        coords.X = (coords.X / originRes.X) * destRes.X;

        return coords;
    }
}