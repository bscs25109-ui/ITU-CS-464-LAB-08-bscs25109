using UnityEngine;

public class ZombieChase : MonoBehaviour
{
    [SerializeField] Transform target;
    [SerializeField] float speed = 1.5f;

    void Update()                                  // Time.deltaTime = 0 while paused, so zombies stop by themselves
    {
        if (target == null) return;
        Vector3 dir = target.position - transform.position;
        dir.y = 0f;
        if (dir.magnitude < 1.2f) return;
        transform.position += dir.normalized * speed * Time.deltaTime;
    }
}
