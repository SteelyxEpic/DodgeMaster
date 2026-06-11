using UnityEngine;

public class playermovement : MonoBehaviour
{
    public float speed = 5f; // Speed of the player movement
    private Rigidbody2D rb;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        rb.linearVelocity = new Vector2(inputhandler.ins.move.normalized.x * speed, rb.linearVelocity.y);
        if(inputhandler.ins.jump && isGrounded()) {
            rb.AddForce(Vector2.up * save.ins.activesave.jumpstrength, ForceMode2D.Impulse);
        }
        Debug.Log(isGrounded());}
    public bool isGrounded() {
        Debug.DrawRay(transform.position, Vector2.down * 1.1f, Color.red);
        return Physics2D.Raycast(transform.position, Vector2.down, 1.1f, LayerMask.GetMask("Ground"));
    }
}
