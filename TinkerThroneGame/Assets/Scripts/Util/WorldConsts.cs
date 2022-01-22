using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldConsts : MonoBehaviour
{
    public static readonly Vector2Int[] simpleDirections = { Vector2Int.up, Vector2Int.right, Vector2Int.down, Vector2Int.left };
    public const float BUILDING_HIGHT = 2.25f;
    public const float GRID_SIZE = 0.5f;
    public const float GRID_SIZE_RECIPROCAL = 2f;
    public static Vector3 SPACECRAFT_MODULE_ALLIGN_DIFF = new Vector3(0, 0, 0);

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