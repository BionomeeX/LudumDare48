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
        if (MapManager.S == null)
        {
            return;
        }
        // EditorGUI.TextArea(new Rect(), "AAAA");
        // // Debug.Log("AAAA");
        EditorGUI.TextArea(new Rect(0, 0, 500, 500), string.Join("\n", MapManager.S.MapRooms.Select(x => x.ToString())));
    }

}
