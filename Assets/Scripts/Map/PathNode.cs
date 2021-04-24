using System.Collections.Generic;
namespace Scripts.Map
{
    public class PathNode
    {
        public int x, y;
        public List<(PathNode node, float cost)> neighbours;
        public PathNode(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
        public float gCost;
        public float hCost;
        public float fCost
        {
            get
            {
                return gCost + hCost;
            }
        }
        public PathNode parent;
        public static float GetDistance(PathNode begin, PathNode end) {
            return (begin.x - end.x) * (begin.x - end.x) + (begin.y - end.y) * (begin.y - end.y);
        }

        public float traverseCost;

    }
}
