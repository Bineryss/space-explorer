using UnityEngine;

public interface ISpaceShipControler
{
    Vector2 MovementInput { get; }
    Vector2 LookInput { get; }
    Transform Transform { get; }
}
