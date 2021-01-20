using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathfindingGrid : MonoBehaviour
{
    public Node[,] nodeGrid;
    // Start is called before the first frame update
    void Start()
    {
        CreateGrid();
    }

    void CreateGrid()
    {
        nodeGrid = new Node[WorldGenerator.tileCountX, WorldGenerator.tileCountY];
        for (int x = 0; x < WorldGenerator.tileCountX; x++)
        {
            for (int y = 0; y < WorldGenerator.tileCountY; y++)
            {
                Node node = new Node();
                node.Position = new Vector2(x, y);
                node.Walkable = !PathfindingHost.Obstacles[x, y];
                nodeGrid[x, y] = node;
            }
        }
    }

    public void RefreshWalkableTiles()
    {
        Debug.Log("RefreshWalkableTiles()");
        for (int x = 0; x < nodeGrid.GetLength(0); x++)
        {
            for (int y = 0; y < nodeGrid.GetLength(1); y++)
            {
                nodeGrid[x, y].Walkable = !PathfindingHost.Obstacles[x, y];
            }
        }
    }

    public List<Node> GetNeighbours(Node node)
    {
        List<Node> neighbours = new List<Node>();
        int indexX = (int)node.Position.x;
        int indexY = (int)node.Position.y;

        if (PathfindingHost.AllNeighbours)
        {
            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    if (x == 0 && y == 0)
                    {
                        continue;
                    }
                    int cx = indexX + x;
                    int cy = indexY + y;
                    if (ValidateIndex(cx, cy))
                    {
                        neighbours.Add(nodeGrid[cx, cy]);
                    }
                }
            }
        }
        else
        {
            // only 4 adjacent tiles
            if (ValidateIndex(indexX - 1, indexY)) { neighbours.Add(nodeGrid[indexX - 1, indexY]); }; // left
            if (ValidateIndex(indexX + 1, indexY)) { neighbours.Add(nodeGrid[indexX + 1, indexY]); }; // right
            if (ValidateIndex(indexX, indexY - 1)) { neighbours.Add(nodeGrid[indexX, indexY - 1]); }; // down
            if (ValidateIndex(indexX, indexY + 1)) { neighbours.Add(nodeGrid[indexX, indexY + 1]); }; // up
        }
        return neighbours;
    }

    private bool ValidateIndex(int cx, int cy)
    {
        if (cx >= 0 && cx < WorldGenerator.tileCountX && cy >= 0 && cy < WorldGenerator.tileCountY) { return true; } else { return false; };
    }
}
