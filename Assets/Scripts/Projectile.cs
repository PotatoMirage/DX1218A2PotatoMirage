using UnityEngine;
using UnityEngine.Pool;

public class Projectile : MonoBehaviour
{
    private IObjectPool<GameObject> pool;
    private float speed = 100f;
    [SerializeField] private float lifeTime = 5f;
    [SerializeField] private LayerMask hitMask;

    [SerializeField] private GameObject hitEffectPrefab;
    [SerializeField] private AudioClip hitSound;
    private float timer;
    public void SetPool(IObjectPool<GameObject> pool) => this.pool = pool;
    public void ResetProjectile()
    {
        timer = 0f;
    }
    private void Update()
    {
        timer += Time.deltaTime;
        if (timer >= lifeTime)
        {
            Deactivate();
            return;
        }

        float moveDistance = speed * Time.deltaTime;

        if (Physics.Raycast(transform.position, transform.right, out RaycastHit hit, moveDistance, hitMask))
        {
            HandleHit(hit);
        }
        else
        {
            transform.Translate(Vector3.right * moveDistance);
        }
    }

    private void HandleHit(RaycastHit hit)
    {
        transform.position = hit.point;

        if (hitEffectPrefab != null)
        {
            Instantiate(hitEffectPrefab, transform.position, Quaternion.LookRotation(hit.normal));
        }

        if (hitSound != null)
        {
            AudioSource.PlayClipAtPoint(hitSound, transform.position);
        }

        IDamageable target = hit.collider.GetComponentInParent<IDamageable>();

        target?.TakeDamage(10);

        Deactivate();
    }

    private void OnEnable() => Invoke(nameof(Deactivate), 7f);

    private void Deactivate()
    {
        if (gameObject.activeInHierarchy)
            pool?.Release(this.gameObject);
    }
}