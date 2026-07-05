using UnityEngine;

public class grappler : MonoBehaviour
{
    public LineRenderer line; // Line renderer for grappling hook
    public float radialSpeed = 5f; // Speed at which the player is pulled towards the grapple point
    public float limit = 5f; // Speed at which the player is pulled towards the grapple point
    private Rigidbody2D rb;
    private Vector2 ropeDir; // Direction of the rope from the player to the grapple point
    void OnEnable()
    {
        rb = playermovement.ins.GetComponent<Rigidbody2D>();
        Vector2 mousepos = inputhandler.ins.mousepos;
        RaycastHit2D hit = Physics2D.Raycast(rb.position, mousepos - rb.position, Mathf.Infinity, playermovement.ins.grappleLayer);
        if (hit.collider == null)
        {
            enabled = false; // Disable the grappler script if no valid grapple point is found
            return;
        }
        Vector2 grapplePoint = hit.point;
        Vector2 rope = rb.position - grapplePoint;
        ropeDir = rope.normalized;
        rb.linearVelocity -= ropeDir;
        
        line.enabled = true;
        line.SetPosition(1, grapplePoint);
    }
    void Update()
    {
        if (line.enabled)
        {
            if (rb.linearVelocity.magnitude < limit || !ropeDir.Equals(rb.linearVelocity.normalized))
            {
                rb.linearVelocity -= ropeDir * radialSpeed;
            }
            line.SetPosition(0, playermovement.ins.transform.position);
            if(!inputhandler.ins.inputs["Grappling"] || playermovement.ins.isGrounded())
            {
                line.enabled = false;
                enabled = false;
            }
        }
    }
}
