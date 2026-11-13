using UnityEngine;


public interface IMovementStrategy
{
    void Move(Rigidbody rb, PlayerStats stats, Vector2 input);
}

public class SmoothMovement : IMovementStrategy
{
    public void Move(Rigidbody rb, PlayerStats stats, Vector2 input)
    {
        Vector3 target = new Vector3(input.x, 0f, input.y);
        if (target.sqrMagnitude > 1f) target.Normalize();
        target *= stats.Velocity;

        Vector3 current = rb.linearVelocity;
        rb.linearVelocity = new Vector3(target.x, current.y, target.z);
    }
}


public class AccelerateMovement : IMovementStrategy
{
    public void Move(Rigidbody rb, PlayerStats stats, Vector2 input)
    {
        Vector3 target = new Vector3(input.x, 0f, input.y);
        if (target.sqrMagnitude > 1f) target.Normalize();
        target *= stats.Velocity;

        Vector3 current = rb.linearVelocity;
        Vector3 horizontal = new Vector3(current.x, 0f, current.z);

        horizontal = Vector3.MoveTowards(horizontal, target, stats.Acceleration * Time.fixedDeltaTime);

        // Se conserva la velocidad vertical (gravedad / salto)
        rb.linearVelocity = new Vector3(horizontal.x, current.y, horizontal.z);
    }
}

public class PlayerStats
{
    private readonly float baseVelocity;
    private float velocity;
    private float acceleration;
    private bool invisible;

    public PlayerStats(float velocity, float acceleration)
    {
        baseVelocity = velocity;
        this.velocity = velocity;
        this.acceleration = acceleration;
        invisible = false;
    }

    public float BaseVelocity => baseVelocity;
    public float Velocity { get => velocity; set => velocity = value; }
    public float Acceleration { get => acceleration; set => acceleration = value; }
    public bool Invisible { get => invisible; set => invisible = value; }

    public void ResetVelocity()
    {
        velocity = baseVelocity;
    }
}