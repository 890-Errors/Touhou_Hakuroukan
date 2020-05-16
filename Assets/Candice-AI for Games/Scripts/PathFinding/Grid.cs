using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{
    public bool displayGridGizmos;
    public LayerMask unwalkableMask;//The layer that the agent cannot walk on.
    public Vector2 gridWorldSize;//The size of the grid
    public float nodeRadius;//The size of each node
    public int obstacleProximityPenalty = 10;//Penalty added to nodes around obstacles
    Node[,] grid;//The nodes of the grid
    float nodeDiameter;
    int gridSizeX, gridSizeY;
    public TerrainType[] walkableRegions;
    LayerMask walkableMask;
    int penaltyMin = int.MaxValue;
    int penaltyMax = int.MinValue;
    Dictionary<int, int> walkableRegionsDictionary = new Dictionary<int, int>();
    private void Awake()
    {
        nodeDiameter = nodeRadius * 2;
        gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);
        gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);
        foreach(TerrainType region in walkableRegions)
        {
            walkableMask.value |= region.terrainMask.value;
            walkableRegionsDictionary.Add(Convert.ToInt32(Mathf.Log(region.terrainMask.value, 2)),region.terrainPenalty);
        }
        CreateGrid();
    }
    public int MaxSize
    {
        get { return gridSizeX * gridSizeY; }
    }

    void CreateGrid()
    {
        //
        //Method Name : void CharCreateGridacterDead()
        //Purpose     : This method creates the grid and assigns movement penalties to the nodes as required.
        //Re-use      : none
        //Input       : none
        //Output      : none
        //
        grid = new Node[gridSizeX, gridSizeY];
        Vector3 worldBottomLeft = transform.position - Vector3.right * gridWorldSize.x / 2 - Vector3.forward * gridWorldSize.y / 2;
        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.forward * (y * nodeDiameter + nodeRadius);
                bool walkable = !(Physics.CheckSphere(worldPoint, nodeRadius, unwalkableMask));
                int movementPenalty = 0;

                Ray ray = new Ray(worldPoint + Vector3.up * 50, Vector3.down);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, 100, walkableMask))
                {
                    walkableRegionsDictionary.TryGetValue(hit.collider.gameObject.layer, out movementPenalty);
                }
                if (!walkable)
                {
                    movementPenalty += obstacleProximityPenalty;
                }

                grid[x, y] = new Node(walkable, worldPoint, x, y, movementPenalty);
            }
        }
        BlurPenaltyMap(3);
    }

    void BlurPenaltyMap(int blurSize)
    {
        //
        //Method Name : void BlurPenaltyMap(int blurSize)
        //Purpose     : This method blurs the penalty map by spreading the values, enabling the agent to move more smoothly and naturally.
        //Re-use      : none
        //Input       : int blurSize
        //Output      : none
        //
        int kernelSize = blurSize * 2 + 1;
        int kernelExtense = (kernelSize-1)/ 2;
        int[,] penaltiesHorizontalPass = new int[gridSizeX, gridSizeY];
        int[,] penaltiesVerticalPass = new int[gridSizeX, gridSizeY];

        for (int y = 0; y < gridSizeY; y++)
        {
            for (int x = -kernelExtense; x <= kernelExtense; x++)
            {
                int sampleX = Mathf.Clamp(x, 0, kernelExtense);
                penaltiesHorizontalPass[0, y] += grid[sampleX, y].movementPenalty;
            }
            for (int x = 1; x < gridSizeX; x++)
            {
                int removeIndex = Mathf.Clamp(x - kernelExtense - 1, 0, gridSizeX);
                int addIndex = Mathf.Clamp(x + kernelExtense, 0, gridSizeX -1);

                penaltiesHorizontalPass[x, y] = penaltiesHorizontalPass[x - 1, y] - grid[removeIndex, y].movementPenalty + grid[addIndex, y].movementPenalty;


            }
        }

        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = -kernelExtense; y <= kernelExtense; y++)
            {
                int sampleY = Mathf.Clamp(y, 0, kernelExtense);
                penaltiesVerticalPass[x, 0] += penaltiesHorizontalPass[x, sampleY];
            }
            int blurredPenalty = Mathf.RoundToInt((float)penaltiesVerticalPass[x, 0] / (kernelSize * kernelSize));
            grid[x, 0].movementPenalty = blurredPenalty;

            for (int y = 1; y < gridSizeY; y++)
            {
                int removeIndex = Mathf.Clamp(y - kernelExtense - 1, 0, gridSizeY);
                int addIndex = Mathf.Clamp(y + kernelExtense, 0, gridSizeY - 1);

                penaltiesVerticalPass[x, y] = penaltiesVerticalPass[x, y-1] - penaltiesHorizontalPass[x, removeIndex] + penaltiesHorizontalPass[x, addIndex];
                blurredPenalty = Mathf.RoundToInt((float)penaltiesVerticalPass[x, y] / (kernelSize * kernelSize));
                grid[x, y].movementPenalty = blurredPenalty;

                if(blurredPenalty > penaltyMax)
                {
                    penaltyMax = blurredPenalty;
                }
                if(blurredPenalty < penaltyMin)
                {
                    penaltyMin = blurredPenalty;
                }
            }
        }

    }
    public List<Node> GetNeighbours(Node node)
    {
        //
        //Method Name : List<Node> GetNeighbours(Node node)
        //Purpose     : This method returns a list of all nodes that surround the given node.
        //Re-use      : none
        //Input       : Node node
        //Output      : List<Node>
        //
        List<Node> neighbours = new List<Node>();
        for(int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if(x == 0 && y == 0)
                {
                    continue;
                }
                int checkX = node.gridX + x;
                int checkY = node.gridY + y;

                if(checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY)
                {
                    neighbours.Add(grid[checkX, checkY]);
                }
            }
        }
        return neighbours;
    }
    public Node NodeFromWorldPoint(Vector3 worldPosition)
    {
        //
        //Method Name : Node NodeFromWorldPoint(Vector3 worldPosition)
        //Purpose     : This method returns the specific node on the grid, from an actual vector3 position on the game world.
        //Re-use      : none
        //Input       : Vector3 worldPosition
        //Output      : Node
        //
        float percentX = (worldPosition.x + gridWorldSize.x / 2) / gridWorldSize.x;
        float percentY = (worldPosition.z + gridWorldSize.y / 2) / gridWorldSize.y;
        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);

        int x = Mathf.RoundToInt((gridSizeX - 1) * percentX);
        int y = Mathf.RoundToInt((gridSizeY - 1) * percentY);
        return grid[x, y];
       
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, new Vector3(gridWorldSize.x, 1, gridWorldSize.y));
        if (grid != null && displayGridGizmos)
        {
            foreach (Node n in grid)
            {
                Gizmos.color = Color.Lerp(Color.white, Color.black, Mathf.InverseLerp(penaltyMin, penaltyMax, n.movementPenalty));
                Gizmos.color = (n.walkable ? Gizmos.color : Color.red);
                Gizmos.DrawCube(n.worldPosition, Vector3.one * (nodeDiameter));

            }
        }
    }
}
[System.Serializable]
public class TerrainType
{
    public LayerMask terrainMask;
    public int terrainPenalty;
}
