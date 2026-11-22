using UnityEngine;

public class ParticleFeedback : MonoBehaviour
{
    [SerializeField] Health health;
    [SerializeField] GameManager game;
    [SerializeField] ParticleSystem hitSparks, pickupSparkle, healPuff;

    void OnEnable()
    {
        health.Damaged += OnDamaged;
        health.Healed += OnHealed;
        game.Collected += OnCollected;
    }

    void OnDisable()
    {
        health.Damaged -= OnDamaged;
        health.Healed -= OnHealed;
        game.Collected -= OnCollected;
    }

    void OnDamaged(int current, int max) => hitSparks.Play();       // each call fires the burst again
    void OnHealed(int current, int max) => healPuff.Play();
    void OnCollected(int score, int target) => pickupSparkle.Play();
}