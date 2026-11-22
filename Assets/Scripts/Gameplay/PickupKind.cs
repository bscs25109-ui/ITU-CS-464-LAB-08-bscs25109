using UnityEngine;

public enum PickupKind { Scrap, Medkit }

public class Pickup : MonoBehaviour
{
    [SerializeField] PickupKind kind = PickupKind.Scrap;
    [SerializeField] int amount = 1;

    void Update() => transform.Rotate(0f, 90f * Time.deltaTime, 0f);   // spins so it is easy to see

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        if (kind == PickupKind.Scrap) GameManager.Instance.Collect(amount);
        else other.GetComponent<Health>().Heal(amount);
        Destroy(gameObject);                       // the event listeners already did their job
    }
}