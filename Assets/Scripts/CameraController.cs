using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class CameraController : MonoBehaviour
{
    [Header("GUI")]
    public Image toolImage;
    public Sprite toolSpawn;
    public Sprite toolTarget;
    public Sprite toolBuild;
    public Sprite toolDemolish;
    [Header("Movement Properties")]
    public float moveSpeed = 10f;

    [Header("References")]
    public WorldGenerator worldGenerator;

    public enum Tool
    {
        Spawn, Target, BuildObstacle, Demolish
    }
    private Tool currentTool;
    public Tool CurrentTool
    {
        private set
        {
            currentTool = value;
            switch (currentTool)
            {
                case Tool.Spawn:
                    toolImage.sprite = toolSpawn;
                    break;
                case Tool.Target:
                    toolImage.sprite = toolTarget;
                    break;
                case Tool.BuildObstacle:
                    toolImage.sprite = toolBuild;
                    break;
                case Tool.Demolish:
                    toolImage.sprite = toolDemolish;
                    break;
            }
        }
        get
        {
            return currentTool;
        }
    }

    void Awake()
    {

    }

    void Start()
    {
        CurrentTool = Tool.Target;
    }

    // Update is called once per frame
    void Update()
    {
        float multiplier = 1f;
        if (Input.GetKey(KeyCode.LeftShift)) { multiplier = 3f; };
        if (worldGenerator == null) { Debug.LogError("CameraControler WorldGenerator not set"); };
        if (Input.GetKey(KeyCode.D)) { transform.position += Vector3.right * moveSpeed * multiplier * Time.deltaTime; }
        else if (Input.GetKey(KeyCode.A)) { transform.position += Vector3.left * moveSpeed * multiplier * Time.deltaTime; };
        if (Input.GetKey(KeyCode.W)) { transform.position += Vector3.up * moveSpeed * multiplier * Time.deltaTime; }
        else if (Input.GetKey(KeyCode.S)) { transform.position += Vector3.down * moveSpeed * multiplier * Time.deltaTime; };

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            CurrentTool = Tool.Spawn;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            CurrentTool = Tool.Target;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            CurrentTool = Tool.BuildObstacle;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            CurrentTool = Tool.Demolish;
        }



        if (Input.GetKey(KeyCode.Q))
        {
            Camera.main.orthographicSize -= Time.deltaTime * multiplier;
        }
        else if (Input.GetKey(KeyCode.E))
        {
            Camera.main.orthographicSize += Time.deltaTime * multiplier;
        }

        if (Input.GetKey(KeyCode.F))
        {
            // zoom and move to the selected agent
            Camera.main.orthographicSize = 6f;
            Vector3 agentPos = (PathfindingHost.isAstar ? PathfindingHost.GetSelectedAgent().transform.position : PathfindingHost.GetSelectedSimpleAgent().transform.position);
            transform.position = new Vector3(agentPos.x, agentPos.y, transform.position.z);
        }
        if (Input.GetKeyDown(KeyCode.M))
        {
            Vector3 centrePoint = new Vector3(WorldGenerator.tileCountX / 2f, WorldGenerator.tileCountY / 2f, transform.position.z);
            Camera.main.orthographicSize = WorldGenerator.tileCountY / 2f;
            transform.position = centrePoint;
        }

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            Vector3 mousePositionV3 = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 mousePosition = new Vector2(mousePositionV3.x, mousePositionV3.y);
            if (worldGenerator.CheckTileInBounds(mousePosition))
            {
                // clicked on a tile
                int selectedTileX = Convert.ToInt32(mousePosition.x);
                int selectedTileY = Convert.ToInt32(mousePosition.y);

                switch (CurrentTool)
                {
                    case Tool.Target:
                        if (PathfindingHost.isAstar) { PathfindingHost.GetSelectedAgent().Target = new Vector2(selectedTileX, selectedTileY); }
                        else { PathfindingHost.GetSelectedSimpleAgent().Target = new Vector2(selectedTileX, selectedTileY); };
                        break;
                    case Tool.Spawn:
                        throw new NotImplementedException("This feature has not been implemented yet");
                    case Tool.BuildObstacle:
                        PathfindingHost.Obstacles[selectedTileX, selectedTileY] = true;
                        PathfindingHost.ChangeObstacles();
                        WorldGenerator.tileGameObjects[selectedTileX, selectedTileY].GetComponent<SpriteRenderer>().color = Color.black;
                        break;
                    case Tool.Demolish:
                        PathfindingHost.Obstacles[selectedTileX, selectedTileY] = false;
                        PathfindingHost.ChangeObstacles();
                        WorldGenerator.tileGameObjects[selectedTileX, selectedTileY].GetComponent<SpriteRenderer>().color = Color.white;
                        break;
                }
            }
        }
        if (Input.GetKey(KeyCode.Mouse0))
        {
            Vector3 mousePositionV3 = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 mousePosition = new Vector2(mousePositionV3.x, mousePositionV3.y);
            if (worldGenerator.CheckTileInBounds(mousePosition))
            {
                // clicked on a tile
                int selectedTileX = Convert.ToInt32(mousePosition.x);
                int selectedTileY = Convert.ToInt32(mousePosition.y);
                switch (CurrentTool)
                {
                    case Tool.BuildObstacle:
                        PathfindingHost.Obstacles[selectedTileX, selectedTileY] = true;
                        PathfindingHost.ChangeObstacles();
                        WorldGenerator.tileGameObjects[selectedTileX, selectedTileY].GetComponent<SpriteRenderer>().color = Color.black;
                        break;
                    case Tool.Demolish:
                        PathfindingHost.Obstacles[selectedTileX, selectedTileY] = false;
                        PathfindingHost.ChangeObstacles();
                        WorldGenerator.tileGameObjects[selectedTileX, selectedTileY].GetComponent<SpriteRenderer>().color = Color.white;
                        break;
                }
            }
        }
    }
}
