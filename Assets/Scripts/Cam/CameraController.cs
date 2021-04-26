using Scripts.ScriptableObjects;
using UnityEngine;

namespace Scripts.Cam
{
    public class CameraController : MonoBehaviour
    {
        private void FixedUpdate()
        {
            transform.Translate(new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized
                * ConfigManager.S.Config.CameraSpeed * (Input.GetKey(KeyCode.LeftShift) ? 3f : 1f));
            if (transform.position.z > -2f)
            {
                transform.position = new Vector3(transform.position.x, transform.position.y, -2f);
            }
        }

        private void Update()
        {
            transform.Translate(new Vector3(0f, 0f, Input.mouseScrollDelta.y));
            if (Input.GetKeyDown(KeyCode.R))
            {
                transform.position = new Vector3(6f, -5f, -11f);
            }
        }
    }
}