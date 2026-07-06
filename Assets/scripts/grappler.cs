using UnityEngine;

public class grappler : MonoBehaviour
{
    public LineRenderer line;

    [Header("Rope Settings")]
    public float correctionStrength = 500f;   // How strongly the rope pulls back once stretched
    [Range(0f, 1f)]
    public float radialDamping = 0.6f;        // 0 = no damping, 1 = old hard-stop behaviour
    public float dampingForceMultiplier = 20f;

    [Header("Reel Settings")]
    public float reelSpeed = 5f;
    public float reelBoost = 1.2f;            // Extra swing speed granted per meter reeled in
    public float ropeSmoothTime = 0.08f;       // How fast ropeLength eases toward its target

    [Header("Feel")]
    public float attachEaseTime = 0.15f;      // Correction strength ramps in over this long on attach
    public float maxCorrectionForce = 60f;     // Clamp to stop launch-explosions on big errors
    public float releaseBoost = 1.15f;        // Speed multiplier applied to velocity on release

    private Rigidbody2D rb;

    private Vector2 grapplePoint;
    private Vector2 ropeDir;
    public float ropeLength;
    private float targetRopeLength;
    private float ropeLengthVelocity; // used by SmoothDamp

    private float attachTime;

    void OnEnable()
    {
        rb = playermovement.ins.GetComponent<Rigidbody2D>();

        Vector2 mousePos = inputhandler.ins.mousepos;

        RaycastHit2D hit = Physics2D.Raycast(
            rb.position,
            mousePos - rb.position,
            Mathf.Infinity,
            playermovement.ins.grappleLayer);

        if (!hit)
        {
            enabled = false;
            return;
        }

        grapplePoint = hit.point;

        targetRopeLength = Vector2.Distance(rb.position, grapplePoint);
        ropeLength = targetRopeLength;
        ropeLengthVelocity = 0f;
        attachTime = Time.time;

        line.enabled = true;
        line.positionCount = 2;
        line.SetPosition(1, grapplePoint);
    }

    void OnDisable()
    {
        if (rb == null) return;

        // Reward a clean release with a small speed kick in the direction you were already moving.
        rb.linearVelocity *= releaseBoost;
        playermovement.ins.NotifyGrappleReleased();
    }

    void FixedUpdate()
    {
        Vector2 rope = rb.position - grapplePoint;
        float currentLength = rope.magnitude;

        if (inputhandler.ins.inputs["Pull"])
        {
            float delta = reelSpeed * Time.fixedDeltaTime;
            targetRopeLength = Mathf.Max(1f, targetRopeLength - delta);

            // Reeling in converts rope shortening into extra swing speed instead of just
            // dragging the player closer -- this is what makes pulling feel powerful.
            if (currentLength > 0.001f)
            {
                Vector2 tangent = new Vector2(-rope.normalized.y, rope.normalized.x);
                float swingDir = Mathf.Sign(Vector2.Dot(rb.linearVelocity, tangent));
                if (swingDir == 0) swingDir = 1f;
                rb.linearVelocity += tangent * swingDir * (delta * reelBoost);
            }
        }

        // Smooth, frame-rate-independent approach to the target rope length (no more
        // fixed 10f-per-second lerp that behaves inconsistently at different physics rates).
        ropeLength = Mathf.SmoothDamp(ropeLength, targetRopeLength, ref ropeLengthVelocity, ropeSmoothTime);

        if (currentLength > 0.001f)
        {
            ropeDir = rope.normalized;

            float radialVelocity = Vector2.Dot(rb.linearVelocity, ropeDir);

            // Partial damping instead of a full cancel -- keeps some outward energy so the
            // catch feels springy rather than like hitting a wall.
            if (radialVelocity > 0)
            {
                rb.linearVelocity -= ropeDir * radialVelocity * radialDamping;
            }

            float error = currentLength - ropeLength;

            if (error > 0)
            {
                // Ease the rope's strength in right after attaching so a fast-moving player
                // doesn't get yanked to a stop in a single frame.
                float timeSinceAttach = Time.time - attachTime;
                float easeFactor = attachEaseTime > 0f
                    ? Mathf.Clamp01(timeSinceAttach / attachEaseTime)
                    : 1f;

                float springForce = error * correctionStrength * easeFactor;
                float dampingForce = radialVelocity * dampingForceMultiplier;

                float finalForce = Mathf.Clamp(springForce - dampingForce, -maxCorrectionForce, maxCorrectionForce);

                rb.AddForce(-ropeDir * finalForce);
            }
        }

        if (!inputhandler.ins.inputs["Grappling"] || playermovement.ins.isGrounded())
        {
            line.enabled = false;
            enabled = false;
        }
    }

    void LateUpdate()
    {
        // Decoupling the visual rope from FixedUpdate keeps it smooth on high refresh-rate
        // displays instead of visibly stepping between physics ticks.
        if (rb == null || !line.enabled) return;
        line.SetPosition(0, rb.position);
    }
}