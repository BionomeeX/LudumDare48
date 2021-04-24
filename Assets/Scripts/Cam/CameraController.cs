using UnityEngine;

namespace Scripts.Cam
{
    public class CameraController : MonoBehaviour
    {
        private void Update()
        {
            transform.Translate(new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized * 0.05f);
            transform.Translate(new Vector3(0f, 0f, Input.mouseScrollDelta.y));
            if (transform.position.z > -2f)
            {
                transform.position = new Vector3(transform.position.x, transform.position.y, -2f);
            }
            if (Input.GetKeyDown(KeyCode.R))
            {
                transform.position = new Vector3(6f, -5f, 11f);
            }
        }
    }
}