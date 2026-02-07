using UnityEngine;
using UnityEngine.Pool;

public class Projectile : MonoBehaviour
{
    private IObjectPool<GameObject> _pool;
    private float _speed = 20f;

    public void SetPool(IObjectPool<GameObject> pool) => _pool = pool;

    private void Update()
    {
        transform.Translate(_speed * Time.deltaTime * Vector3.right);
    }

    private void OnTriggerEnter(Collider other)
    {
        _pool?.Release(this.gameObject);
    }

    private void OnEnable() => Invoke(nameof(Deactivate), 3f);
    private void Deactivate() { if (gameObject.activeInHierarchy) _pool?.Release(this.gameObject); }
}