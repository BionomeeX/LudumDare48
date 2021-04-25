using Scripts.Map;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class RoomDebugEditor : EditorWindow
{
    [MenuItem("Window/RoomDebug")]
    public static void ShowWindow()
    {
        GetWindow(typeof(RoomDebugEditor));
    }

    private void OnGUI()
    {
        EditorGUI.TextArea(new Rect(), string.Join("\n", MapManager.S.MapRooms.Select(x => x.ToString())));
    }

}
