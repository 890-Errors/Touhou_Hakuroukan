using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;
using System.Linq;
using System;
namespace ViridaxGameStudios.AI
{
    public class PathFinding
    {
        Grid grid;


        public PathFinding(Grid _grid)
        {
            grid = _grid;
            string[] className = (this.ToString()).Split('.');
            if (CandiceConfig.enableDebug)
                UnityEngine.Debug.Log(className[className.Length - 1] + ": Initialised.");
        }
        public void FindPath(PathRequest request, Action<PathResult> callback)
        {
            //
            //Method Name : void FindPath(PathRequest request, Action<PathResult> callback)
            //Purpose     : This method finds the path, using the PathRequest.
            //Re-use      : none
            //Input       : PathRequest request, Action<PathResult> callback
            //Output      : none
            //
            Stopwatch sw = new Stopwatch();
            sw.Start();

            Vector3[] waypoints = new Vector3[0];
            bool pathSuccess = false;
            Node startNode = grid.NodeFromWorldPoint(request.pathStart);
            Node targetNode = grid.NodeFromWorldPoint(request.pathEnd);

            if (startNode.walkable && targetNode.walkable)
            {
                Heap<Node> openSet = new Heap<Node>(grid.MaxSize);
                HashSet<Node> closedSet = new HashSet<Node>();
                openSet.Add(startNode);

                while (openSet.Count > 0)
                {
                    Node currentNode = openSet.RemoveFirst();
                    closedSet.Add(currentNode);
                    //If the path has been found
                    if (currentNode == targetNode)
                    {
                        sw.Stop();
                        pathSuccess = true;
                        break;
                    }
                    foreach (Node neighbour in grid.GetNeighbours(currentNode))
                    {
                        if (!neighbour.walkable || closedSet.Contains(neighbour))
                        {
                            continue;
                        }
                        int newMovementCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbour) + neighbour.movementPenalty;
                        if (newMovementCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
                        {
                            neighbour.gCost = newMovementCostToNeighbour;
                            neighbour.hCost = GetDistance(neighbour, targetNode);
                            neighbour.parent = currentNode;
                            if (!openSet.Contains(neighbour))
                            {
                                openSet.Add(neighbour);
                            }
                            else
                            {
                                openSet.UpdateItem(neighbour);
                            }
                        }
                    }
                }
            }
            if (pathSuccess)
            {
                waypoints = RetracePath(startNode, targetNode);
                pathSuccess = waypoints.Length > 0;
            }
            callback(new PathResult(waypoints, pathSuccess, request.callback));
        }
        Vector3[] RetracePath(Node startNode, Node endNode)
        {
            //
            //Method Name : Vector3[] RetracePath(Node startNode, Node endNode)
            //Purpose     : This method retraces the path from end to start, and reverses it to get the path from start to finish.
            //Re-use      : none
            //Input       : Node startNode, Node endNode
            //Output      : Vector3[]
            //
            List<Node> path = new List<Node>();
            Node currentNode = endNode;

            while (currentNode != startNode)
            {
                path.Add(currentNode);
                currentNode = currentNode.parent;
            }
            Vector3[] waypoints = ConvertAndSimplifyPath(path);
            Array.Reverse(waypoints);
            //waypoints.Reverse();

            return waypoints;

        }
        Vector3[] ConvertPath(List<Node> path)
        {
            //
            //Method Name : Vector3[] ConvertPath(List<Node> path)
            //Purpose     : This method  converts just converts the path into a Vector3[]
            //Re-use      : none
            //Input       : List<Node> path)
            //Output      : Vector3[]
            //
            List<Vector3> waypoints = new List<Vector3>();

            for (int i = 1; i < path.Count; i++)
            {
                waypoints.Add(path[i].worldPosition);
            }
            return waypoints.ToArray();
        }

        Vector3[] ConvertAndSimplifyPath(List<Node> path)
        {
            //
            //Method Name : Vector3[] ConvertAndSimplifyPath(List<Node> path)
            //Purpose     : This method simplifies the path by removing consecutive nodes where the direction is the same, and then converts it into a Vector3[]
            //Re-use      : none
            //Input       : List<Node> path)
            //Output      : Vector3[]
            //
            List<Vector3> waypoints = new List<Vector3>();
            Vector2 directionOld = Vector2.zero;

            for (int i = 1; i < path.Count; i++)
            {
                Vector2 directionNew = new Vector2(path[i - 1].gridX - path[i].gridX, path[i - 1].gridY - path[i].gridY);
                if (directionNew != directionOld)
                {
                    waypoints.Add(path[i].worldPosition);
                }
                directionOld = directionNew;
            }
            return waypoints.ToArray();
        }
        int GetDistance(Node nodeA, Node nodeB)
        {
            //
            //Method Name : int GetDistance(Node nodeA, Node nodeB)
            //Purpose     : This method finds the distance between 2 nodes.
            //Re-use      : none
            //Input       : Node nodeA, Node nodeB
            //Output      : int
            //
            int distX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
            int distY = Mathf.Abs(nodeA.gridY - nodeB.gridY);
            if (distX > distY)
            {
                return 14 * distY + 10 * (distX - distY);
            }
            else
            {
                return 14 * distX + 10 * (distY - distX);
            }
        }
    }

}
