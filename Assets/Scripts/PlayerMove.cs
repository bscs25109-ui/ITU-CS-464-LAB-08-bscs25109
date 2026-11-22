using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    [SerializeField] float speed = 5f;
    [SerializeField] float limit = 13.5f;           // stay on the 30 x 30 ground

    void Update()
    {
        if (GameUI.IsPaused || GameManager.Instance.IsOver) return;   // no input while paused or finished
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        Vector3 p = transform.position + new Vector3(x, 0f, z) * speed * Time.deltaTime;
        p.x = Mathf.Clamp(p.x, -limit, limit);
        p.z = Mathf.Clamp(p.z, -limit, limit);
        transform.position = p;
    }
}