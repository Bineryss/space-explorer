using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Projectile : MonoBehaviour, IProjectile
{
    public GameObject Target => target;

    public Rigidbody Rigidbody => GetComponent<Rigidbody>();

    public Transform Transform => transform;

    private GameObject target;
    private IProjectileBehaviour behaviour;
    private ProjectileSpawner origin;

    private bool isInitialized = false;

    public void Initialize(GameObject target, IProjectileBehaviour behaviour, ProjectileSpawner origin)
    {
        this.target = target;
        this.behaviour = behaviour;
        this.origin = origin;
        StartCoroutine(StartSequence());
    }

    void FixedUpdate()
    {
        if (!isInitialized) return;
        behaviour.Move(Rigidbody, target.transform.position);
    }

    void OnTriggerEnter(Collider other)
    {
        if(!other.CompareTag(target.tag)) return;
        behaviour.OnHit(other.gameObject);
        origin.ClearProjectile(this);
    }

    private IEnumerator StartSequence()
    {
        yield return behaviour.StartSequence(this, target.transform.position);
        isInitialized = true;
    }
}
