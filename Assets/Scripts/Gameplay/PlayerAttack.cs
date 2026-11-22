using System;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [SerializeField] float range = 3.5f;
    [SerializeField] float push = 5f;
    [SerializeField] float cooldown = 0.5f;

    public event Action Attacked;
    float nextAttack;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) Attack();
    }

    public void Attack()                            // also called by the HIT button on the HUD
    {
        if (GameUI.IsPaused || GameManager.Instance.IsOver) return;
        if (Time.time < nextAttack) return;
        nextAttack = Time.time + cooldown;

        foreach (ZombieChase z in ZombieChase.All)
        {
            Vector3 d = z.transform.position - transform.position;
            d.y = 0f;
            if (d.magnitude <= range) z.Knock(d.normalized * push);
        }
        Attacked?.Invoke();
    }
}