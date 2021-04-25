using Scripts.Map;
using Scripts.Map.Room;
using Scripts.Map.Room.ModulableRoom;
using System.Linq;
using UnityEngine;

namespace Scripts.UI
{
    public class OpenGameMenu : MonoBehaviour
    {
        [SerializeField]
        private GameObject _buildPanel;

        [SerializeField]
        private Material[] _materials;
        private Material _debugMaterial = null;

        [SerializeField]
        private Texture2D[] _textures;

        [SerializeField]
        private ExplanationPanel _explanationPanelScript;

        [SerializeField]
        private ExplanationPanel _roomInfo;

        private void Start()
        {
            _buildPanel.SetActive(false);
            _explanationPanelScript.gameObject.SetActive(false);
            _roomInfo.gameObject.SetActive(false);
        }

        public void ToggleBuildPanel()
        {
            _buildPanel.SetActive(!_buildPanel.activeInHierarchy);
        }

        private RoomType? _currentSelection;
        public void SetCurrentBuild(int type)
        {
            var rType = (RoomType)type;
            _currentSelection = rType == _currentSelection || type == -1 ? (RoomType?)null : rType;
            if (_currentSelection == null)
            {
                Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
            }
            else
            {
                Cursor.SetCursor(type == 0 ? _textures[0] : _textures[type - 2], Vector2.zero, CursorMode.Auto);
            }
        }

        public void OnBuildHoverEnter(int id)
        {
            var elem = ModularRoomFactory.BuildModularRoom((RoomType)id, null);
            _explanationPanelScript.gameObject.SetActive(true);
            _explanationPanelScript.Name.text = elem.GetName();
            _explanationPanelScript.Description.text = elem.GetDescription();
        }

        public void OnBuildHoverExit()
        {
            _explanationPanelScript.gameObject.SetActive(false);
        }

        private GenericRoom clicked = null;
        private void Update()
        {
            // We want the user to click and release his mouse on the same element
            if (Input.GetMouseButtonDown(0))
            {
                var mousePos = Input.mousePosition;
                mousePos.z = -Camera.main.transform.position.z;
                var pos = Camera.main.ScreenToWorldPoint(mousePos);
                var pos2d = (Vector2)pos;
                var room = MapManager.S.MapRooms.FirstOrDefault((x) =>
                {
                    var xSize = x.Size.x / 2f;
                    return pos2d.x > x.GameObject.transform.position.x - xSize && pos2d.x < x.GameObject.transform.position.x + xSize
                    && pos2d.y > x.GameObject.transform.position.y && pos2d.y < x.GameObject.transform.position.y + x.Size.y;
                });
                if (room != null)
                {
                    if (_currentSelection != null && room.IsBuilt) // We want to change the type of a room
                    {
                        clicked = room as GenericRoom;
                        if (clicked != null && !clicked.RoomType.IsEmpty())
                        {
                            clicked = null;
                        }
                    }
                    else // Display info about a room
                    {
                        _roomInfo.Name.text = room.GetName();
                        _roomInfo.Description.text = room.GetDescription();
                        if (_roomInfo.DetailPanel.transform.childCount > 0)
                        {
                            Destroy(_roomInfo.DetailPanel.transform.GetChild(0).gameObject);
                        }
                        var gRoom = room as GenericRoom;
                        if (gRoom != null)
                        {
                            var p = gRoom.RoomType.GetDescriptionPanel();
                            if (p != null)
                            {
                                var go = Instantiate(p, _roomInfo.DetailPanel.transform);
                                var t = (RectTransform)go.transform;
                                var pT = (RectTransform)_roomInfo.DetailPanel.transform;
                                t.sizeDelta = Vector2.zero;
                                t.position = pT.position;
                            }
                        }
                        _roomInfo.gameObject.SetActive(true);
                    }
                }
                else
                {
                    _roomInfo.gameObject.SetActive(false);
                }
            }
            if (Input.GetMouseButtonUp(0) && clicked != null && clicked.IsBuilt)
            {
                clicked.RoomType = ModularRoomFactory.BuildModularRoom(_currentSelection.Value, clicked);
                foreach (var renderer in clicked.GameObject.GetComponentsInChildren<MeshRenderer>())
                {
                    if (_debugMaterial == null)
                    {
                        _debugMaterial = renderer.material;
                    }
                    renderer.material = _materials[(int)_currentSelection.Value - 3];
                }
                clicked = null;
                SetCurrentBuild(-1);
            }
            if (Input.GetMouseButtonDown(1) && _currentSelection != null) // Reset selection
            {
                SetCurrentBuild(-1);
            }
        }
    }
}
