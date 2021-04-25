namespace Scripts.Map.Room.ModulableRoom
{
    public class ModularRoomFactory
    {
        public static AModulableRoom BuildModularRoom(RoomType rType)
        {
            switch (rType)
            {
                case RoomType.STORAGE:
                    return new StorageRoom();

                default:
                    throw new System.ArgumentException("Can't build modular room of type " + rType, nameof(rType));
            }
        }

        private ModularRoomFactory()
        { }
    }
}
