using Scripts.Events;
using Scripts.Exploration;
using Scripts.Map;
using Scripts.Map.Room;
using Scripts.Map.Room.ModulableRoom;
using System.Linq;
using UnityEngine;

namespace Scripts.UI
{
    public class OpenGameMenu : MonoBehaviour
    {
        public static OpenGameMenu S;

        private void Awake()
        {
            S = this;
        }

        [SerializeField]
        private GameObject _buildPanel, _explorationPanel, _recruitementPanel;

        private ExplorationPanel _explorationPanelScript;
        private RecruitementPanel _recruitementPanelScript;

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
            _explorationPanel.SetActive(false);
            _recruitementPanel.SetActive(false);
            _explanationPanelScript.gameObject.SetActive(false);
            _roomInfo.gameObject.SetActive(false);

            _explorationPanelScript = _explorationPanel.GetComponent<ExplorationPanel>();
            _recruitementPanelScript = _recruitementPanel.GetComponent<RecruitementPanel>();
        }

        public void ToggleBuildPanel()
        {
            _explorationPanel.SetActive(false);
            _recruitementPanel.SetActive(false);
            _buildPanel.SetActive(!_buildPanel.activeInHierarchy);
        }

        public void ToggleExplorationPanel()
        {
            _buildPanel.SetActive(false);
            _recruitementPanel.SetActive(false);
            _explorationPanel.SetActive(!_explorationPanel.activeInHierarchy);
            if (_explorationPanel.activeInHierarchy)
            {
                _explorationPanelScript.Enable();
            }
        }

        public void ToggleRecruitementPanel()
        {
            _buildPanel.SetActive(false);
            _explorationPanel.SetActive(false);
            _recruitementPanel.SetActive(!_recruitementPanel.activeInHierarchy);
            if (_recruitementPanel.activeInHierarchy)
            {
                _recruitementPanelScript.Enable();
            }
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

        public void OnFlagSetHover()
        {
            _explanationPanelScript.gameObject.SetActive(true);
            _explanationPanelScript.Name.text = "Set destination";
            _explanationPanelScript.Description.text = "Set where your submarines need to go";
        }

        public void OnFlagUnsetHover()
        {
            _explanationPanelScript.gameObject.SetActive(true);
            _explanationPanelScript.Name.text = "Retreat";
            _explanationPanelScript.Description.text = "Send your submarines back to the base";
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

        public void UpdateRoomInfo()
        {
            if (currInfoD != null)
            {
                var room = currInfoD;
                _roomInfo.Name.text = room.GetName();
                _roomInfo.Description.text = room.GetDescription();
                if (_roomInfo.DetailPanel.transform.childCount > 0)
                {
                    for (int i = 0; i < _roomInfo.DetailPanel.transform.childCount; i++)
                    {
                        Destroy(_roomInfo.DetailPanel.transform.GetChild(i).gameObject);
                    }
                }
                var gRoom = room as GenericRoom;
                if (gRoom != null || room.IsBuilt)
                {
                    var p = room.IsBuilt
                        ? gRoom.RoomType.GetDescriptionPanel()
                        : room.GetDescriptionPanel();
                    if (p != null)
                    {
                        var go = Instantiate(p, _roomInfo.DetailPanel.transform);
                        var t = (RectTransform)go.transform;
                        var pT = (RectTransform)_roomInfo.DetailPanel.transform;
                        t.sizeDelta = Vector2.zero;
                        t.position = pT.position;
                        if (room.IsBuilt) gRoom.RoomType.SetupConfigPanel(go);
                        else room.SetupConfigPanel(go);
                    }
                }
                _roomInfo.gameObject.SetActive(true);
            }
        }
        private ARoom currInfoD;

        private GenericRoom clicked = null;
        private void Update()
        {
            // We want the user to click and release his mouse on the same element
            if (Input.GetMouseButtonDown(0))
            {
                currInfoD = null;
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
                        if (gRoom != null || room.IsBuilt)
                        {
                            var p = room.IsBuilt
                                ? gRoom.RoomType.GetDescriptionPanel()
                                : room.GetDescriptionPanel();
                            if (p != null)
                            {
                                var go = Instantiate(p, _roomInfo.DetailPanel.transform);
                                var t = (RectTransform)go.transform;
                                var pT = (RectTransform)_roomInfo.DetailPanel.transform;
                                t.sizeDelta = Vector2.zero;
                                t.position = pT.position;
                                if (room.IsBuilt) gRoom.RoomType.SetupConfigPanel(go);
                                else room.SetupConfigPanel(go);
                            }
                        }
                        _roomInfo.gameObject.SetActive(true);
                        currInfoD = gRoom;
                    }
                }
                else if (Input.mousePosition.x < Screen.width - 200
                    || Input.mousePosition.y < Screen.height - 300)
                {
                    _roomInfo.gameObject.SetActive(false);
                }
            }
            if (Input.GetMouseButtonUp(0) && clicked != null && clicked.IsBuilt)
            {
                clicked.RoomType = ModularRoomFactory.BuildModularRoom(_currentSelection.Value, clicked);
                int index;
                if (clicked.Size.x == 2)
                {
                    if (clicked.Size.y == 1)
                        index = 0;
                    else
                        index = 1;
                }
                else
                {
                    if (clicked.Size.y == 1)
                        index = 2;
                    else
                        index = 3;
                }
                GameObject go;
                if (_currentSelection.Value == RoomType.AIRLOCK) go = PAirlock[index];
                else if (_currentSelection.Value == RoomType.DEFENSE) go = PDefense[index];
                else if (_currentSelection.Value == RoomType.FACTORY) go = PFactory[index];
                else if (_currentSelection.Value == RoomType.MINING) go = PMining[index];
                else if (_currentSelection.Value == RoomType.STORAGE) go = PStorage[index];
                else go = null;

                if (go != null)
                {
                    var n = Instantiate(go, clicked.GameObject.transform.position, Quaternion.Euler(0f, 180f, 0f));
                    Destroy(clicked.GameObject);
                    clicked.GameObject = n;
                }
                EventManager.S.NotifyManager(Events.Event.RoomSetType, clicked);
                clicked = null;
                SetCurrentBuild(-1);
            }
            if (Input.GetMouseButtonDown(1)) // Reset selection
            {
                currInfoD = null;
                if (_currentSelection != null)
                {
                    SetCurrentBuild(-1);
                }
                SubmarineManager.S.RemoveSubmarinePlacementMode();
            }
        }

        public GameObject[] PStorage, PFactory, PMining, PDefense, PAirlock;
    }
}
