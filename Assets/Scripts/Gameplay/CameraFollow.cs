using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] Transform target;
    [SerializeField] float smooth = 8f;

    void LateUpdate()
    {
        if (target == null) return;
        transform.position = Vector3.Lerp(transform.position, target.position, 1f - Mathf.Exp(-smooth * Time.unscaledDeltaTime));
    }
}