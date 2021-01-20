using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public static class BasicConfigManager
{
    static string confFileName = "a-star-paths.conf";
    #region settings names
    static string s_WorldSizeX = "WorldSizeX = ";
    static string s_WorldSizeY = "WorldSizeY = ";
    static string s_NeighbourBehaviour = "NeighbourBehaviour = ";
    #endregion
    public static void LoadConfig()
    {
        if (!File.Exists(confFileName))
        {
            string[] lines = new string[] { "WorldSizeX = 10", "WorldSizeY = 10", "NeighbourBehaviour = all" };
            File.WriteAllLines(confFileName, lines);
        }
        // read and interpret conf file
        string[] readLines = File.ReadAllLines(confFileName);
        foreach (string s in readLines)
        {
            if (s.Contains(s_WorldSizeX)) { WorldGenerator.tileCountX = int.Parse(s.Replace(s_WorldSizeX, "")); }
            else if (s.Contains(s_WorldSizeY)) { WorldGenerator.tileCountY = int.Parse(s.Replace(s_WorldSizeY, "")); }
            else if (s.Contains(s_NeighbourBehaviour)) { PathfindingHost.AllNeighbours = (s.Replace(s_NeighbourBehaviour, "") == "all" ? true : false); };
        }
    }
}
