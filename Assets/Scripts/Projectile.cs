using UnityEngine;
using UnityEngine.Pool;

public class Projectile : MonoBehaviour
{
    private IObjectPool<GameObject> _pool;
    private float _speed = 100f;

    [Header("Effects")]
    [SerializeField] private GameObject hitEffectPrefab;
    [SerializeField] private AudioClip hitSound;

    public void SetPool(IObjectPool<GameObject> pool) => _pool = pool;

    private void Update()
    {
        transform.Translate(_speed * Time.deltaTime * Vector3.right);
    }

    private void OnTriggerEnter(Collider other)
    {
        // --- 1. Spawn Hit Effect ---
        if (hitEffectPrefab != null)
        {
            // Spawn effect at projectile's current position
            Instantiate(hitEffectPrefab, transform.position, Quaternion.identity);
        }

        // --- 2. Play Sound ---
        if (hitSound != null)
        {
            // Use PlayClipAtPoint so the sound continues even if this projectile object is disabled
            AudioSource.PlayClipAtPoint(hitSound, transform.position);
        }

        // Return to pool
        _pool?.Release(this.gameObject);
    }

    private void OnEnable() => Invoke(nameof(Deactivate), 7f);

    private void Deactivate()
    {
        if (gameObject.activeInHierarchy)
            _pool?.Release(this.gameObject);
    }
}