using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;

public class Pathfinding : MonoBehaviour
{
    // public Transform seeker, target;

    PathRequestManager requestManager;

    Grid grid;

    void Awake()
    {
        requestManager = GetComponent<PathRequestManager>();
        grid = GetComponent<Grid>();
    }

    /* void Update()
     {
         if (Input.GetButtonDown("Jump"))
         {
             FindPath(seeker.position, target.position);
         }

     }
     */
    public void StartFindPath(Vector3 startPos, Vector3 targetPos)
    {
        StartCoroutine(FindPath(startPos, targetPos));
    }

    IEnumerator FindPath(Vector3 startPos, Vector3 targetPos)
    {
        Stopwatch sw = new Stopwatch();
        sw.Start();

        Vector3[] waypoints = new Vector3[0];
        bool pathSuccess = false;

        Node startNode = grid.NodeFromWorldPoint(startPos);
        Node targetNode = grid.NodeFromWorldPoint(targetPos);
        startNode.parent = startNode;

        if (startNode.walkable && targetNode.walkable)
        {
            //OPEN the set of nodes to be evaluated
            Heap<Node> openSet = new Heap<Node>(grid.MaxSize);
            //CLOSED the set of nodes already evaluated
            HashSet<Node> closedSet = new HashSet<Node>();
            openSet.Add(startNode);

            while (openSet.Count > 0)
            {
                //using Heap to optimize the pathfinding algorithm
                Node currentNode = openSet.RemoveFirst();

                /*Node node = openSet[0];
            
                for (int i = 1; i < openSet.Count; i++)
                {
                    //current = node in OPEN with the lowest f_cost
                    if (openSet[i].fCost < node.fCost || openSet[i].fCost == node.fCost)
                    {
                        if (openSet[i].hCost < node.hCost)
                            node = openSet[i];
                    }
                }

                //remove current from OPEN set
                openSet.Remove(node);*/
                //add to CLOSED set
                closedSet.Add(currentNode);

                //if current is the target node
                //path has been found
                if (currentNode == targetNode)
                {
                    sw.Stop();
                    //print ("Path found: " + sw.ElapsedMilliseconds + " ms");
                    pathSuccess = true;
                    break;
                }

                //foreach neighbour of the current node
                foreach (Node neighbour in grid.GetNeighbours(currentNode))
                {
                    {
                        //if neighbour is not traversable or neighbour is in CLOSED
                        if (!neighbour.walkable || closedSet.Contains(neighbour))
                        {
                            //skip to the next neighbour
                            continue;
                        }

                        //if new path to neighbour is shorter OR neighbour is not in OPEN
                        int newMovementCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbour);
                        if (newMovementCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
                        {
                            //calculating f_cost by sum of g_cost and h_cost
                            neighbour.gCost = newMovementCostToNeighbour;
                            neighbour.hCost = GetDistance(neighbour, targetNode);
                            //set parent of neighbour to current
                            neighbour.parent = currentNode;

                            //if neighbour is not in OPEN
                            //add neighbour to OPEN
                            if (!openSet.Contains(neighbour))
                                openSet.Add(neighbour);
                            else
                                openSet.UpdateItem(neighbour);
                        }
                    }
                }
            }
            //wait for 1 frame before returning
            yield return null;
            if (pathSuccess)
            {
                waypoints = RetracePath(startNode, targetNode);
            }
            requestManager.FinishedProcessingPath(waypoints, pathSuccess);

        }
    }
        //when the targetNode is found, need to retrace to startPos by using the parent to get the path from the startNode to the endNode
        Vector3[] RetracePath(Node startNode, Node endNode)
        {
            List<Node> path = new List<Node>();
            Node currentNode = endNode;

            while (currentNode != startNode)
            {
                path.Add(currentNode);
                currentNode = currentNode.parent;
            }
            Vector3[] waypoints = SimplifyPath(path);
            Array.Reverse(waypoints);
            return waypoints;

        }
        // path.Reverse();
        // grid.path = path;

    

        Vector3[] SimplifyPath(List<Node> path)
        {
            List<Vector3> waypoints = new List<Vector3>();
            //creating Vector2 to store direction of the last two nodes
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
            int dstX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
            int dstY = Mathf.Abs(nodeA.gridY - nodeB.gridY);

            //count on x-axis of the start position to see how many nodes are away from the target postion
            //count on y-axis of the start position to see how many nodes are away from the target postion
            //14 is how much 1 diagnal move cost
            //in the case distance x > distance y
            if (dstX > dstY)
                return 14 * dstY + 10 * (dstX - dstY);

            //in the case distance y > distance x
            return 14 * dstX + 10 * (dstY - dstX);
        }
    }

