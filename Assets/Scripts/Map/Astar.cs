using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scripts.Map
{

    public class Astar
    {

        public class Node {

            public int
            public float gCost;
            public float hCost;
            public float fCost {
                get {
                    return gCost + hCost;
                }
            }

        }

        public static float EuclidianDistance(Vector2Int startPosition, Vector2Int endPosition) {
            return (startPosition.x - endPosition.x) * (startPosition.x - endPosition.x) + (startPosition.y - endPosition.y) * (startPosition.y - endPosition.y);
        }

        public static (float gcost, float hcost, float fcost) PositionsValues(Vector2Int currentPosition, Vector2Int startPosition, Vector2Int endPosition){
            float gcost = EuclidianDistance(currentPosition, startPosition);
            float hcost = EuclidianDistance(currentPosition, endPosition);
            return (gcost, hcost, gcost + hcost);
        }

        public static List<Vector2Int> RetracePath

        public static List<Vector2Int> FindPath(Vector2Int startPosition, Vector2Int endPosition, List<List<int>> map)
        {
            List<Vector2Int> openSet = new List<Vector2Int>();
            HashSet<Vector2Int> closedSet = new HashSet<Vector2Int>();

            openSet.Add(startPosition);

            while(openSet.Count > 0)
            {

                Vector2Int currentPosition = openSet[0];

                (var currentGCost, var currentHCost, var currentFCost) = PositionsValues(currentPosition, startPosition, endPosition);

                for(int i = 1; i < openSet.Count; ++i){
                    (var runningGCost, var runningHCost, var runningFCost) = PositionsValues(openSet[i], startPosition, endPosition);

                    if(runningFCost < currentFCost || runningFCost == currentFCost && runningHCost < currentHCost){
                        currentPosition = openSet[i];
                    }
                }

                openSet.Remove(currentPosition);
                closedSet.Add(currentPosition);

                if(currentPosition == endPosition){
                    RetracePath(start)
                }

            }



        }




    }

}
