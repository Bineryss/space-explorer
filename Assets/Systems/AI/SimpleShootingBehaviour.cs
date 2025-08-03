using UnityEngine;

public class SimpleShootingBehaviour : MonoBehaviour
{
    [SerializeField] private ProjectileSpawner projectileSpawner;
    [SerializeField] private GameObject target;
    [SerializeField] private float spawnRate = 5f;

    private bool enemyDetected;
    private float nextSpawnTime;


    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(target.tag)) return;
        enemyDetected = true;
    }

    void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag(target.tag)) return;
        enemyDetected = false;
    }

    void Update()
    {
        if (!enemyDetected) return;
        if (nextSpawnTime < 1 / spawnRate)
        {
            nextSpawnTime += Time.deltaTime;
        }
        else
        {
            nextSpawnTime = 0f;
            projectileSpawner.SpawnProjectile();
        }

    }
}
