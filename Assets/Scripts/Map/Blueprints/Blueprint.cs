using Scripts.Map.Room;


namespace Scripts.Map.Blueprints
{

    public class Blueprint
    {
        public ARoom RoomRef;


        public Blueprint(ARoom room) {
            RoomRef = room;
        }
    }

}
