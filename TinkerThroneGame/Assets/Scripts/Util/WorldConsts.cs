using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldConsts : MonoBehaviour
{
    public static readonly Vector2Int[] simpleDirections = { Vector2Int.up, Vector2Int.right, Vector2Int.down, Vector2Int.left };
    public const float BUILDING_HIGHT = 2.25f;
    public const float GRID_SIZE = 0.5f;
    public const float GRID_SIZE_RECIPROCAL = 2f;
    public static Vector3 STANDARD_ROTATION = new Vector3(0, 45, 0);
    public static Vector3 SPACECRAFT_MODULE_ALLIGN_DIFF = new Vector3(0, 0, 0);
    public static Color[] BUILDING_SPACE_COLORS = new Color[4] {Color.green, Color.yellow, new Color(1,0.6f,0), Color.red };

    public static Vector2Int screenResolution =  new Vector2Int(1920, 1080);
}
public enum Dir
{
    front,
    right,
    back,
    left,
    all
}