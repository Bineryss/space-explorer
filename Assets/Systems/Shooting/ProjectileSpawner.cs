using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
using UnityEngine.InputSystem;

public class ProjectileSpawner : SerializedMonoBehaviour
{
    [Header("Projectile Settings")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private GameObject target;
    [OdinSerialize] private IProjectileBehaviour projectileBehaviour;

    [Header("Debug")]
    [SerializeField] private List<IProjectile> projectiles = new();
    public void OnTrigger(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            SpawnProjectile();
        }
    }

    public void SpawnProjectile()
    {
        GameObject projectileObject = Instantiate(projectilePrefab, transform.position, GetComponentInParent<Transform>().rotation);
        IProjectile projectile = projectileObject.GetComponent<IProjectile>();
        projectile.Initialize(target, projectileBehaviour, this);
        projectiles.Add(projectile);
        // Logic to spawn a projectile
        Debug.Log("Projectile Spawned");
    }

    public void ClearProjectile(Projectile projectile)
    {
        if (projectile != null)
        {
            Destroy(projectile.gameObject);
            Debug.Log("Projectile Cleared");
        }
    }
}
