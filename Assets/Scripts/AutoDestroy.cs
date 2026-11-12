using UnityEngine;

// Deletes this object after a set time (e.g. once a one-shot animation has finished)
public class AutoDestroy : MonoBehaviour
{
    [SerializeField] private float lifetime = 0.35f;

    void Start()
    {
        Destroy(gameObject, lifetime);
    }
}