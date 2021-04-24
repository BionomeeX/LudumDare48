using System.Collections.Generic;
namespace Scripts.Map
{
    public class PathNode
    {
        public int x, y;
        public List<PathNode> neighbours;
        public PathNode(int x, int y)
        {
            this.x = x;
            this.y = y;
            neighbours = new List<PathNode>();
        }

        public static float GetDistance(PathNode begin, PathNode end) {
            return (begin.x - end.x) * (begin.x - end.x) + (begin.y - end.y) * (begin.y - end.y);
        }

        public float traverseCost;

    }
}
