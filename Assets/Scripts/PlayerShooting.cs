using UnityEngine;
using UnityEngine.Pool;

public class PlayerShooting : MonoBehaviour
{
    [SerializeField] private InputReader inputReader;
    [SerializeField] private PlayerController playerController;
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float fireRate = 0.5f;

    private float nextFireTime;
    private IObjectPool<GameObject> projectilePool;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip shootClip;
    private void Awake()
    {
        projectilePool = new ObjectPool<GameObject>(
            createFunc: () => Instantiate(projectilePrefab),
            actionOnGet: (obj) => {
                obj.SetActive(true);
                obj.transform.SetPositionAndRotation(firePoint.position, firePoint.rotation);
                Projectile p = obj.GetComponent<Projectile>();
                if (p != null) p.ResetProjectile();
            },
            actionOnRelease: (obj) => obj.SetActive(false),
            actionOnDestroy: (obj) => Destroy(obj),
            collectionCheck: false,
            defaultCapacity: 10,
            maxSize: 50
        );
    }

    private void OnEnable()
    {
        inputReader.AttackEvent += HandleShooting;
    }

    private void OnDisable()
    {
        inputReader.AttackEvent -= HandleShooting;
    }

    private void HandleShooting()
    {
        if (!playerController.IsRangedMode) return;

        if (Time.time < nextFireTime) return;

        nextFireTime = Time.time + fireRate;

        GameObject projectile = projectilePool.Get();

        Projectile projScript = projectile.GetComponent<Projectile>();
        if (projScript != null) projScript.SetPool(projectilePool);
    }
}