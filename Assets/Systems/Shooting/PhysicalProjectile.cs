using System.Collections;
using UnityEngine;

public class PhysicalProjectile : IProjectileBehaviour
{

    public void Move(Rigidbody rigidbody, Vector3 targetPosition)
    {
        rigidbody.AddForce((targetPosition - rigidbody.position).normalized * 10f, ForceMode.VelocityChange);
    }

    public void OnHit(GameObject target)
    {
        IDamageable damageable = target.GetComponent<IDamageable>();
        if (damageable == null) return;
        damageable.TakeDamage(10);
        Debug.Log("Physical projectile hit: " + target.name);
    }

    public IEnumerator StartSequence(IProjectile projectile, Vector3 targetPosition)
    {
        Debug.Log("Starting physical projectile sequence");
        projectile.Rigidbody.AddRelativeForce(projectile.Transform.forward * 50f, ForceMode.Impulse);
        yield return new WaitForSeconds(1f);
    }
}
