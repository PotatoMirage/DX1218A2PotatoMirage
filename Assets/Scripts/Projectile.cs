using UnityEngine;
using UnityEngine.Pool;

public class Projectile : MonoBehaviour
{
    private IObjectPool<GameObject> _pool;
    private float _speed = 20f;

    public void SetPool(IObjectPool<GameObject> pool) => _pool = pool;

    private void Update()
    {
        transform.Translate(Vector3.forward * _speed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        // Return to pool instead of Destroy()
        _pool?.Release(this.gameObject);
    }

    // Safety timer to release if it hits nothing
    private void OnEnable() => Invoke(nameof(Deactivate), 3f);
    private void Deactivate() { if (gameObject.activeInHierarchy) _pool?.Release(this.gameObject); }
}