using UnityEngine;

public class Rotation : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 10f;
    [SerializeField] private Vector3 rotationAxis = Vector3.up;

    void FixedUpdate()
    {
        transform.Rotate(rotationAxis, rotationSpeed * Time.fixedDeltaTime);
    }
}
