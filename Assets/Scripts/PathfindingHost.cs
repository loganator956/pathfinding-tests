using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PathfindingHost
{
    // pathfinding host is here to provide an easy way of interacting with pathfinding agents. 
    static bool[,] obstacles;
    public static bool isAstar;
    public static bool[,] Obstacles;
    public static void ChangeObstacles()
    {
        foreach (PathfindingAgent agent in Agents)
        {
            Debug.Log("obstacles changed");
            agent.grid.RefreshWalkableTiles();
        }
    }
    public static List<PathfindingAgent> Agents = new List<PathfindingAgent>();
    public static List<SimplePathfindingAgent> SimpleAgents = new List<SimplePathfindingAgent>();
    public static void RegisterAgent(PathfindingAgent agent, bool selectNewAgent)
    {
        Debug.Log($"Registering agent: {agent.gameObject.name}. {(selectNewAgent ? "Selecting new agent" : "Not selecting new agent")}");
        Agents.Add(agent);
        SelectAgent(agent);
    }
    public static void RegisterAgent(SimplePathfindingAgent agent, bool selectNewAgent)
    {
        Debug.Log($"Registering simple agent: {agent.gameObject.name}. {(selectNewAgent ? "Selecting new simple agent" : "Not selecting new simple agent")}");
        SimpleAgents.Add(agent);
        SelectAgent(agent);
    }
    private static PathfindingAgent selectedAgent;
    private static SimplePathfindingAgent selectedSimpleAgent;
    public static void SelectAgent(PathfindingAgent agent)
    {
        selectedAgent = agent;
    }
    public static void SelectAgent(SimplePathfindingAgent agent)
    {
        selectedSimpleAgent = agent;
    }
    public static PathfindingAgent GetSelectedAgent()
    {
        return selectedAgent;
    }
    public static SimplePathfindingAgent GetSelectedSimpleAgent()
    {
        return selectedSimpleAgent;
    }

    public static bool AllNeighbours = true;
}
