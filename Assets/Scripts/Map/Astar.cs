using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scripts.Map
{

    public class Astar
    {

        public static List<PathNode> RetracePath(PathNode startPosition, PathNode endPosition)
        {
            List<PathNode> path = new List<PathNode>();
            PathNode currentNode = endPosition;

            while (currentNode != startPosition)
            {
                path.Add(currentNode);
                currentNode = currentNode.parent;
            }

            path.Reverse();
            return path;
        }

        public static List<PathNode> FindPath(PathNode startPosition, PathNode endPosition)
        {
            List<PathNode> openSet = new List<PathNode>();
            HashSet<PathNode> closedSet = new HashSet<PathNode>();

            openSet.Add(startPosition);

            while (openSet.Count > 0)
            {

                PathNode currentNode = openSet[0];

                for (int i = 1; i < openSet.Count; ++i)
                {
                    PathNode runningNode = openSet[i];
                    if (runningNode.fCost < currentNode.fCost || runningNode.fCost == currentNode.fCost && runningNode.hCost < currentNode.hCost)
                    {
                        currentNode = openSet[i];
                    }
                }

                openSet.Remove(currentNode);
                closedSet.Add(currentNode);

                if (currentNode == endPosition)
                {
                    return RetracePath(startPosition, endPosition);
                }

                foreach (var neighbour in currentNode.neighbours)
                {
                    if (closedSet.Contains(neighbour.node))
                    {
                        continue;
                    }

                    float newMovementCostToNeighbour = currentNode.gCost + neighbour.cost;

                    if (newMovementCostToNeighbour < neighbour.node.gCost || !openSet.Contains(neighbour.node))
                    {
                        neighbour.node.gCost = newMovementCostToNeighbour;
                        neighbour.node.hCost = PathNode.GetDistance(neighbour.node, endPosition);
                        neighbour.node.parent = currentNode;

                        if (!openSet.Contains(neighbour.node))
                        {
                            openSet.Add(neighbour.node);
                        }
                    }
                }
            }
            return new List<PathNode>();
        }

    }

}
