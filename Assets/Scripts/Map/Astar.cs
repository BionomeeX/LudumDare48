using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scripts.Map
{

    public class Astar
    {

        public class Node {

            public Node(PathNode node){
                this.currentNode = node;
            }
            public PathNode currentNode;

            public float gCost;
            public float hCost;
            public float fCost
            {
                get
                {
                    return gCost + hCost;
                }
            }
            public Node parent;

        }

        public static List<PathNode> RetracePath(Node startPosition, Node endPosition)
        {
            List<PathNode> path = new List<PathNode>();
            Node currentNode = endPosition;

            while (currentNode != startPosition)
            {
                path.Add(currentNode.currentNode);
                currentNode = currentNode.parent;
            }

            path.Reverse();
            return path;
        }

        public static List<PathNode> FindPath(PathNode startPosition, PathNode endPosition)
        {
            List<Node> openSet = new List<Node>();
            HashSet<Node> closedSet = new HashSet<Node>();

            Node startingNode = new Node(startPosition);
            openSet.Add(startingNode);

            while (openSet.Count > 0)
            {

                Node currentNode = openSet[0];

                for (int i = 1; i < openSet.Count; ++i)
                {
                    Node runningNode = openSet[i];
                    if (runningNode.fCost < currentNode.fCost || runningNode.fCost == currentNode.fCost && runningNode.hCost < currentNode.hCost)
                    {
                        currentNode = openSet[i];
                    }
                }

                openSet.Remove(currentNode);
                closedSet.Add(currentNode);

                if (currentNode.currentNode == endPosition)
                {
                    return RetracePath(startingNode, currentNode);
                }

                foreach (PathNode neighbor in currentNode.currentNode.neighbours)
                {

                    Node neighbour = new Node(neighbor);

                    if (closedSet.Contains(neighbour))
                    {
                        continue;
                    }

                    float newMovementCostToNeighbour = currentNode.gCost + neighbour.currentNode.traverseCost;

                    if (newMovementCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
                    {
                        neighbour.gCost = newMovementCostToNeighbour;
                        neighbour.hCost = PathNode.GetDistance(neighbor, endPosition);
                        neighbour.parent = currentNode;

                        if (!openSet.Contains(neighbour))
                        {
                            openSet.Add(neighbour);
                        }
                    }
                }
            }
            return new List<PathNode>();
        }

    }

}
