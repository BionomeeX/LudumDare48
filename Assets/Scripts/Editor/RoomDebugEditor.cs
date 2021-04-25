using UnityEditor;
using Scripts.Map;
public class RoomDebugEditor : EditorWindow
{

    [MenuItem("Window/RoomDebug")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(RoomDebugEditor));
    }

    private void OnGUI()
    {
        //MapManager.S.MapRooms
    }

}
