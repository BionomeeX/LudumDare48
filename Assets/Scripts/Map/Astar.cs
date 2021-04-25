using Scripts.Map.Room;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scripts.Map
{

    public class Astar
    {

        public class Node
        {

            public Node(ARoom node)
            {
                this.currentNode = node;
            }
            public ARoom currentNode;

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

        public static List<ARoom> RetracePath(Node startPosition, Node endPosition)
        {
            List<ARoom> path = new List<ARoom>();
            Node currentNode = endPosition;

            while (currentNode != startPosition)
            {
                path.Add(currentNode.currentNode);
                currentNode = currentNode.parent;
            }

            path.Reverse();
            return path;
        }

        public static float ComputePathWeight(List<ARoom> path)
        {
            float result = 0;
            foreach (var room in path)
            {
                result += room.GetCost();
            }
            return result;
        }

        public static List<ARoom> FindPath(ARoom startPosition, ARoom endPosition)
        {
            if (startPosition == endPosition)
            {
                return new List<ARoom>();
            }

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

                foreach (ARoom neighbor in currentNode.currentNode.GetNeighborhood())
                {

                    Node neighbour = new Node(neighbor);

                    if (closedSet.Contains(neighbour))
                    {
                        continue;
                    }

                    float newMovementCostToNeighbour = currentNode.gCost + neighbour.currentNode.GetCost();

                    if (newMovementCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
                    {
                        neighbour.gCost = newMovementCostToNeighbour;
                        neighbour.hCost = ARoom.GetDistance(neighbor, endPosition);
                        neighbour.parent = currentNode;

                        if (!openSet.Contains(neighbour))
                        {
                            openSet.Add(neighbour);
                        }
                    }
                }
            }
            return new List<ARoom>();
        }

    }

}
