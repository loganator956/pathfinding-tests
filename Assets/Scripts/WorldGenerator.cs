using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldGenerator : MonoBehaviour
{
    public static GameObject[,] tileGameObjects;

    [Header("Sprites")]
    public Sprite TileSprite;
    public static int tileCountX = 10;
    public static int tileCountY = 10;
    public bool isAstar = true;

    // Start is called before the first frame update
    void Awake()
    {
        BasicConfigManager.LoadConfig();
        PathfindingHost.isAstar = isAstar;
        GenerateWorld();
    }

    void GenerateWorld()
    {
        if (tileGameObjects != null) { foreach (GameObject go in tileGameObjects) { Destroy(go, 0); }; }; // deleting objects already created, if any
        tileGameObjects = new GameObject[tileCountX, tileCountY]; // initialising array
        PathfindingHost.Obstacles = new bool[tileCountX, tileCountY]; // initialising array
        Debug.Log("Generating world");
        for (int x = 0; x < tileCountX; x++)
        {
            for (int y = 0; y < tileCountY; y++)
            {
                GameObject newGo = new GameObject($"Tile ({x}, {y})");
                newGo.transform.position = new Vector3(x, y);
                SpriteRenderer renderer = newGo.AddComponent<SpriteRenderer>();
                renderer.sprite = TileSprite;
                tileGameObjects[x, y] = newGo;

                PathfindingHost.Obstacles[x,y] = false;
            }
        }
    }

    public bool CheckTileInBounds(float x, float y)
    {
        return CheckTileInBounds(new Vector2(x, y));
    }

    public bool CheckTileInBounds(Vector2 point)
    {
        bool passX = false;
        bool passY = false;
        if (point.x > 0 && point.x < tileGameObjects.GetLength(0)) { passX = true; };
        if (point.y > 0 && point.y < tileGameObjects.GetLength(1)) { passY = true; };
        if (passX && passY) { return true; } else { return false; };
    }

    // Update is called once per frame
    void Update()
    {

    }
}

