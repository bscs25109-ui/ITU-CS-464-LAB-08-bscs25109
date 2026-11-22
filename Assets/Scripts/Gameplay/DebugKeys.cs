using UnityEngine;

public class DebugKeys : MonoBehaviour
{
    [SerializeField] Health health;

    void Update()
    {
        if (GameUI.IsPaused || GameManager.Instance.IsOver) return;
        if (Input.GetKeyDown(KeyCode.H)) health.TakeDamage(25);          // H = take damage
        if (Input.GetKeyDown(KeyCode.J)) health.Heal(25);                // J = heal
        if (Input.GetKeyDown(KeyCode.K)) GameManager.Instance.Collect(1);// K = collect scrap
    }
}