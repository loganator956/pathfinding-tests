/* This script has been rewritten to work with the same things as the A* script. So it now uses the PathfindingGrid system
 * with nodes rather than just arrays within this class. Should you, for some reason, want to use this script as part of
 * a different project, you will likely need to change various parts of it. Should probably just use the A* pathfinding
 * stuff though. 
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

public class SimplePathfindingAgent : MonoBehaviour
{
    public PathfindingAgent.Status AgentStatus;
    public bool SelfRegisterOnStart = false;

    private Vector2 target;
    public Vector2 Target
    {
        get { return target; }
        set
        {
            target = IntifyVector(value);
            CalculatePath(IntifyVector(transform.position), target);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        if (SelfRegisterOnStart) { PathfindingHost.RegisterAgent(this, true); };
        AgentStatus = PathfindingAgent.Status.WaitingForTarget;
    }

    // Update is called once per frame
    void Update()
    {

    }

    float[,] tileScores;
    private void CalculatePath(Vector2 start, Vector2 end)
    {
        Vector2 iStart = IntifyVector(start);
        Vector2 iEnd = IntifyVector(end);

        tileScores = new float[PathfindingHost.Obstacles.GetLength(0), PathfindingHost.Obstacles.GetLength(1)];
        for (int x = 0; x < tileScores.GetLength(0); x++) { for (int z = 0; z < tileScores.GetLength(1); z++) { tileScores[x, z] = -1f; }; }; // resetting the values
        tileScores[(int)iStart.x, (int)iStart.y] = 0;
        AssignTileVals(iStart);
        // now got values, need to retrace the steps
        // SaveTilesToFile();
        RetracePath(iStart, iEnd);
    }

    private void SaveTilesToFile()
    {
        List<string> lines = new List<string>();
        for(int y = 0; y< tileScores.GetLength(1);y++)
        {
            string line = "";
            for (int x = 0; x < tileScores.GetLength(0);x++)
            {
                line += $"{tileScores[x, y]},";
            }
            lines.Add(line);
        }
        lines.Reverse();
        File.WriteAllLines("tileScores.csv", lines);
    }

    List<Vector2> path;

    private void RetracePath(Vector2 startTile, Vector2 targetTile)
    {
        path = new List<Vector2>();
        Vector2 currentTile = targetTile;
        int count = 0;
        while (IntifyVector(currentTile) != IntifyVector(startTile))
        {
            if (count > 2000)
            {
                throw new Exception("Pathfinding timed out. Reached 2000 iterations");
            }
            count++;
            path.Add(currentTile);
            // work out lowest value around neighbours that ISN'T -1f
            // on A* script we just go through the parent nodes but we don't assign that here. 
            Vector2 lowestNeighbour = targetTile;
            // float lowestNeighbourValue = tileScores[(int)lowestNeighbour.x, (int)lowestNeighbour.y];
            foreach (Vector2 neighbour in GetNeighbours(IntifyVector(currentTile)))
            {
                Vector2 v2 = IntifyVector(neighbour);
                int x = (int)v2.x;
                int y = (int)v2.y;
                if (tileScores[x, y] < 0f)
                {
                    continue;
                }
                else
                {
                    if (tileScores[x, y] <= tileScores[(int)lowestNeighbour.x, (int)lowestNeighbour.y])
                    {
                        lowestNeighbour = neighbour;
                        // lowestNeighbourValue = tileScores[(int)lowestNeighbour.x, (int)lowestNeighbour.y];
                    }
                }
            }
            currentTile = lowestNeighbour;
            path.Add(lowestNeighbour);
        }
    }

    private void AssignTileVals(Vector2 centreTile)
    {
        float centreScore = tileScores[(int)centreTile.x, (int)centreTile.y];
        // get neighbours 
        foreach (Vector2 neighbour in GetNeighbours(centreTile))
        {
            if (!PathfindingHost.Obstacles[(int)neighbour.x, (int)neighbour.y])
            {
                // can walk on neighbour, calculate tilescore. 
                float newTileScore = centreScore + 1; // +1 could be replaced by the walk cost of a tile.
                int x = (int)IntifyVector(neighbour).x;
                int y = (int)IntifyVector(neighbour).y;
                if (newTileScore < tileScores[x, y] || tileScores[x, y] == -1f)
                {
                    tileScores[x, y] = newTileScore;
                    AssignTileVals(new Vector2(x, y));
                }
            }
            // else { Debug.LogError("Non-walkable"); };
        }
    }

    private Vector2[] GetNeighbours(Vector2 CentreTile)
    {
        List<Vector2> list = new List<Vector2>();
        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0) { continue; }
                else
                {
                    Vector2 newTile = new Vector2(CentreTile.x + x, CentreTile.y + y);
                    if (ValidateIndex(newTile)) { list.Add(newTile); };
                }
            }
        }
        return list.ToArray();
    }

    private bool ValidateIndex(Vector2 v2)
    {
        if (v2.x >= 0 && v2.x < PathfindingHost.Obstacles.GetLength(0) && v2.y >= 0 && v2.y < PathfindingHost.Obstacles.GetLength(1))
        {
            return true;
        }
        return false;
    }

    public Vector2 IntifyVector(Vector2 input)
    {
        return new Vector2(Convert.ToInt32(input.x), Convert.ToInt32(input.y));
    }

    void OnDrawGizmos()
    {
        if (path != null)
        {
            foreach (Vector2 n in path)
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawSphere(n, 0.2f);
            }
        }
    }
}
