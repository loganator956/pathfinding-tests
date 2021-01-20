using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathfindingAgent : MonoBehaviour
{
    Color agentLRColourStart;
    Color agentLRColourEnd;

    [Header("Agent Properties")]
    public float MoveSpeed = 0.1f;
    public float NodeSuccessDistance = 0.1f;
    public PathfindingGrid grid;
    // The pathfinding agent is what is attached to the actual game characters within the game world and contains the A* pathfinding algorithm. 
    public bool SelfRegisterOnStart = false;
    public enum Status
    {
        Idle, WaitingForTarget, CalculatingPath, WalkingAlongPath
    }
    public Status AgentStatus { private set; get; }

    Vector2 target;
    public Vector2 Target
    {
        get { return target; }
        set
        {
            target = IntifyVector(value);
            CalculatePath(IntifyVector(transform.position), target);
        }
    }

    public Vector2 IntifyVector(Vector2 input)
    {
        return new Vector2(Convert.ToInt32(input.x), Convert.ToInt32(input.y));
    }

    private void CalculatePath(Vector2 start, Vector2 end)
    {
        List<Node> OpenNodes = new List<Node>();
        List<Node> ClosedNodes = new List<Node>();

        // adding the starting node
        OpenNodes.Add(grid.nodeGrid[(int)start.x, (int)start.y]);
        Node startNode = grid.nodeGrid[(int)start.x, (int)start.y];
        startNode.GCost = 0;
        startNode.HCost = GetDistance(start, end);
        Node targetNode = grid.nodeGrid[(int)end.x, (int)end.y];

        while (OpenNodes.Count > 0)
        {
            Node currentNode = GetLowestFCostNode(OpenNodes);
            OpenNodes.Remove(currentNode);
            ClosedNodes.Add(currentNode);

            if (currentNode == targetNode)
            {
                RetracePath(startNode, targetNode);
                return;
            }

            foreach (Node neighbour in grid.GetNeighbours(currentNode))
            {
                if (!neighbour.Walkable || ClosedNodes.Contains(neighbour)) { continue; }

                float newCostToNeighbour = currentNode.GCost + GetDistance(currentNode.Position, neighbour.Position);
                if (newCostToNeighbour < neighbour.GCost || !OpenNodes.Contains(neighbour))
                {
                    neighbour.GCost = newCostToNeighbour;
                    neighbour.HCost = GetDistance(neighbour.Position, targetNode.Position);
                    neighbour.ParentNode = currentNode;

                    if (!OpenNodes.Contains(neighbour))
                    {
                        OpenNodes.Add(neighbour);
                    }
                }
            }
        }
    }

    List<Node> path;

    ///<summary>Iterates through each node, following their parent's node until we reach the start node.true</summary>
    ///<param name="startNode">The original start of the path</param>
    ///<param name="targetNode">The original end (target) of the path</param>
    private void RetracePath(Node startNode, Node targetNode)
    {
        path = new List<Node>();
        Node currentNode = targetNode;
        // path.Add(currentNode);
        while (currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.ParentNode;
        }
        path.Reverse();
        AgentStatus = Status.WalkingAlongPath;
        RefreshPathLine();
    }

    float GetDistance(Vector2 start, Vector2 end)
    {
        return (end - start).magnitude;
    }

    private Node GetLowestFCostNode(List<Node> list)
    {
        int lowestIndex = 0;
        float lowestFCost = list[0].FCost;

        int iterationInt = 0;
        foreach (Node node in list)
        {
            if (node.FCost < lowestFCost)
            {
                lowestIndex = iterationInt;
                lowestFCost = node.FCost;
            }
            iterationInt++;
        }
        return list[lowestIndex];
    }

    // Start is called before the first frame update
    void Start()
    {
        agentLRColourStart = GetRandomColour();
        agentLRColourEnd = GetRandomColour();
        if (GetComponent<PathfindingGrid>() == null) { grid = gameObject.AddComponent<PathfindingGrid>(); }
        else { grid = GetComponent<PathfindingGrid>(); };
        AgentStatus = Status.WaitingForTarget;
        if (SelfRegisterOnStart)
        {
            PathfindingHost.RegisterAgent(this, true);
        }
    }
    
    void Update()
    {
        if (AgentStatus == Status.WalkingAlongPath)
        {
            Vector3 Difference = nodeToWaypoint(path[0].Position) - transform.position;
            Difference.Normalize();
            Vector3 move = Difference * Time.deltaTime * MoveSpeed;
            transform.position += move;
            if (Vector3.Distance(transform.position, nodeToWaypoint(path[0].Position)) < move.magnitude)
            {
                path.RemoveAt(0);
                if (path.Count == 0)
                {
                    AgentStatus = Status.Idle;
                    Debug.Log("Reached Destination!");
                }
                RefreshPathLine();
            }
        }
    }

    Vector3 nodeToWaypoint(Vector2 nodePos)
    {
        return new Vector3(nodePos.x, nodePos.y, transform.position.z);
    }

    void OnDrawGizmos()
    {
        // if (path != null)
        // {
        //     Gizmos.color = Color.blue;
        //     foreach (Node node in path)
        //     {
        //         Gizmos.DrawSphere(node.Position, 0.1f);
        //     }
        // }
    }

    void RefreshPathLine()
    {
        LineRenderer line = GetComponentInChildren<LineRenderer>();
        // if (line == null)
        // {
        //     line = gameObject.GetCom.gameObject.AddComponent<LineRenderer>();
        // }
        line.material = new Material(Shader.Find("Sprites/Default"));
        line.startColor = agentLRColourStart;
        line.endColor = agentLRColourEnd;
        line.positionCount = path.Count;
        line.startWidth = 0.4f;
        line.endWidth = 0.1f;
        line.SetPositions(NodeToV3Array(path.ToArray(), Vector3.back));
    }

    public Vector3[] NodeToV3Array(Node[] input, Vector3 offset)
    {
        Vector3[] v3 = new Vector3[input.Length];
        for (int i = 0; i < input.Length; i++)
        {
            v3[i] = (Vector3)input[i].Position + offset;
        }
        return v3;
    }

    public Color GetRandomColour()
    {
        Color colour = new Color(UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f));
        return colour;
    }
}


public class Node
{
    ///<summary>Total of G and H Costs</summary>
    public float FCost
    {
        get
        {
            return GCost + HCost;
        }
    }
    ///<summary>Distance from start node</summary>
    public float GCost;
    ///<summary>Distance from end node</summary>
    public float HCost;
    ///<summary>Parent node</summary>
    public Node ParentNode;
    public Vector2 Position;
    public bool Walkable;
}