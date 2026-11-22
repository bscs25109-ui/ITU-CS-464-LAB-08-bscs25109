using UnityEngine;

public class ShakeOnHit : MonoBehaviour
{
    [SerializeField] Health health;
    [SerializeField] CameraShake shake;

    void OnEnable() => health.Damaged += Shake;
    void OnDisable() => health.Damaged -= Shake;

    void Shake(int current, int max) => shake.AddTrauma(0.55f);
}