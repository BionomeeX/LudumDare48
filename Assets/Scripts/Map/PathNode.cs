using System.Collections.Generic;
namespace Scripts.Map
{
    public class ARoom
    {
        public int x, y;
        public List<ARoom> neighbours;
        public ARoom(int x, int y)
        {
            this.x = x;
            this.y = y;
            neighbours = new List<ARoom>();
        }

        public static float GetDistance(ARoom begin, ARoom end) {
            return (begin.x - end.x) * (begin.x - end.x) + (begin.y - end.y) * (begin.y - end.y);
        }

        public float traverseCost;

    }
}
