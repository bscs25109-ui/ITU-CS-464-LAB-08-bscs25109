using System.Collections.Generic;
using UnityEngine;

public class ZombieChase : MonoBehaviour
{
    public static readonly List<ZombieChase> All = new List<ZombieChase>();

    [SerializeField] Transform target;
    [SerializeField] Health targetHealth;
    [SerializeField] float speed = 1.5f;
    [SerializeField] int damage = 10;

    float stunnedUntil;
    float nextHit;

    void OnEnable() => All.Add(this);
    void OnDisable() => All.Remove(this);

    public void Knock(Vector3 push)
    {
        transform.position += push;
        stunnedUntil = Time.time + 0.6f;
    }

    void Update()                                   // Time.deltaTime is 0 while paused, so zombies stop by themselves
    {
        if (target == null || GameManager.Instance.IsOver || Time.time < stunnedUntil) return;
        Vector3 dir = target.position - transform.position;
        dir.y = 0f;
        float dist = dir.magnitude;
        if (dist > 1.2f) transform.position += dir.normalized * speed * Time.deltaTime;
        if (dist < 1.5f && Time.time >= nextHit)
        {
            nextHit = Time.time + 1f;
            targetHealth.TakeDamage(damage);
        }
    }
}