using System.Collections;
using UnityEngine;

public interface IProjectile
{
    GameObject Target { get; }
    Rigidbody Rigidbody { get; }
    Transform Transform { get; }
    void Initialize(GameObject target, IProjectileBehaviour behaviour, ProjectileSpawner origin);
}

public interface IProjectileBehaviour
{
    IEnumerator StartSequence(IProjectile projectile, Vector3 targetPosition);
    void Move(Rigidbody projectile, Vector3 targetPosition);
    void OnHit(GameObject target);
}
