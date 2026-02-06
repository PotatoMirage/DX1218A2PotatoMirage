using UnityEngine;
using UnityEngine.Pool; // Unity's built-in Object Pool

public class PlayerShooting : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private InputReader _inputReader;
    [SerializeField] private PlayerController _playerController;
    [SerializeField] private GameObject _projectilePrefab;
    [SerializeField] private Transform _firePoint;
    [SerializeField] private float _fireRate = 0.5f;

    private float _nextFireTime;
    private IObjectPool<GameObject> _projectilePool;

    private void Awake()
    {
        // Initialize Object Pool
        _projectilePool = new ObjectPool<GameObject>(
            createFunc: () => Instantiate(_projectilePrefab),
            actionOnGet: (obj) => {
                obj.SetActive(true);
                obj.transform.position = _firePoint.position;
                obj.transform.rotation = _firePoint.rotation;
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
        _inputReader.AttackEvent += HandleShooting;
    }

    private void OnDisable()
    {
        _inputReader.AttackEvent -= HandleShooting;
    }

    private void HandleShooting()
    {

        // 2. Rate Limit
        if (Time.time < _nextFireTime) return;

        // 3. Fire
        _nextFireTime = Time.time + _fireRate;

        // Trigger Animation
        _playerController.Animator.SetTrigger("Shoot");

        // Use Object Pool
        GameObject projectile = _projectilePool.Get();

        // Pass the pool back to the projectile so it can release itself later
        // (Assuming Projectile script has a reference to the pool or you handle release here)
        var projScript = projectile.GetComponent<Projectile>();
        if (projScript != null) projScript.SetPool(_projectilePool);
    }
}