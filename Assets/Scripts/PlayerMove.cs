using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    [SerializeField] float speed = 5f;

    void Update()
    {
        if (GameUI.IsPaused) return;               // no input while paused
        float x = Input.GetAxis("Horizontal");     // A / D / arrow keys
        float z = Input.GetAxis("Vertical");       // W / S / arrow keys
        transform.Translate(new Vector3(x, 0f, z) * speed * Time.deltaTime, Space.World);
    }
}
