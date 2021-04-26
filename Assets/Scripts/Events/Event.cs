namespace Scripts.Events
{
    public enum Event
    {
        BlueprintDrawn,
        BlueprintFinished,
        MasterBlueprintFinished,
        RoomCreated,
        RoomDestroyed,
        EnemySpotted,
        Attacked,
        EnemyDead,
        MoveFinished,
        RoomSetType, // Set the type of a room (factory, storage...)
        ExplorationFlagSet,
        ExplorationFlagUnset,
        ExplorationNewZone,
    }
}
