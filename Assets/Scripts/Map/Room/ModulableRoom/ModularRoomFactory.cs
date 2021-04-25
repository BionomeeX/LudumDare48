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

                case RoomType.AIRLOCK:
                    return new AirlockRoom();

                case RoomType.DEFENSE:
                    return new DefenseRoom();

                case RoomType.FACTORY:
                    return new FactoryRoom();

                case RoomType.MINING:
                    return new MiningRoom();

                case RoomType.EMPTY:
                    return new EmptyRoom();

                default:
                    throw new System.ArgumentException("Can't build modular room of type " + rType, nameof(rType));
            }
        }

        private ModularRoomFactory()
        { }
    }
}
