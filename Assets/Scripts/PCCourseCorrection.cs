using UnityEngine;

public class PCCourseCorrection : MonoBehaviour
{
    public Vector3 driftVelocity;
    [Header("Corrective Values")]
    [Tooltip("What percentage of currentSpeed should be used to correct drifting?")]
    [Range(0.0f, 1.0f)]
    public float driftingCorrection;
    [Tooltip("What percentage of currentSpeed should be used to correct rolling backwards?")]
    [Range(0.0f, 1.0f)]
    public float backwardsCorrection;
    [Tooltip("What percentage of falling velocity should be used to conserved for forward momentum?")]
    [Range(0.0f, 1.0f)]
    public float fallingCorrection;

    private PlayerController pc_master;
    private SurfaceAlignment pc_gravity;
    private Rigidbody m_rigidbody;
    private bool isFalling;
    private float currentSpeed;
    private float lastFallingVelocity;

    private void Start()
    {
        m_rigidbody = GetComponent<Rigidbody>();
        pc_master = GetComponent<PlayerController>();
        pc_gravity = GetComponent<SurfaceAlignment>();
    }

    private void Update()
    {
        // Takes the world-space linear velocity and converts it
        // into a relative-space linear velocity.
        driftVelocity = transform.InverseTransformDirection(m_rigidbody.linearVelocity);
        currentSpeed = pc_master.GetCurrentSpeed();

        DriftCorrection();
        BackwardsCorrection();
        FallingCorrection();
    }

    // If the player is drifting, such as when performing a turn, apply a force
    // relative to the strength of the drifting. Redirects corrected force forward.
    public void DriftCorrection()
    {
        if (driftVelocity.x < 1f || driftVelocity.x > 1f)
        {
            m_rigidbody.AddForce(transform.right * (-driftVelocity.x * driftingCorrection), ForceMode.Force);
            m_rigidbody.AddForce(transform.forward * (Mathf.Sign(driftVelocity.x) * driftingCorrection), ForceMode.Force);
        }
    }

    // If the player is rolling backwards, apply a forward force relative to
    // the player's desired current speed.
    public void BackwardsCorrection()
    {
        if (driftVelocity.z < 0f)
        {
            m_rigidbody.AddForce(transform.forward * (currentSpeed * backwardsCorrection), ForceMode.Force);
        }
    }

    // If the player's falling velocity suddenly stops, a portion of their
    // previous falling velocity is redirected into a sudden forward momentum.
    public void FallingCorrection()
    {
        // Come back in here later once SurfaceAlignment.cs has been folded
        // into a isGrounded check
        if (driftVelocity.y < 0)
            isFalling = true;
        if (driftVelocity.y == 0 && isFalling)
        {
            isFalling = false;
            m_rigidbody.AddForce(transform.forward * (lastFallingVelocity * fallingCorrection), ForceMode.Impulse);
        }
        lastFallingVelocity = -driftVelocity.y;
    }
}
