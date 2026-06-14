using UnityEngine;
using System.Collections;

public class playermovement : MonoBehaviour
{
    public static playermovement ins;
    public float speed = 5f; // Speed of the player movement
    public float range = 5f; // Range
    public Weapon weapon;
    public LayerMask dashLayer;
    private Rigidbody2D rb;
    private bool cooldown = false;
    private float speedmulti = 1f;
    public enum PlayerState
    {
        Idle,
        Moving,
        Dashing
    }
    public PlayerState currentState = PlayerState.Idle;
    private void Awake()
	{
		if (ins == null)
		{
			ins = this;
			DontDestroyOnLoad(this);
        }
        else
		{
			Destroy(gameObject);
        }
	}
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (inputhandler.ins.inputs["Sprint"] && currentState != PlayerState.Dashing) {
            speedmulti = save.ins.activesave.speedmulti;
        } else {
            speedmulti = 1f;
        }
        if (currentState != PlayerState.Dashing){
            rb.linearVelocity = new Vector2(inputhandler.ins.move.normalized.x * save.ins.activesave.speed * speedmulti, rb.linearVelocity.y);
        if(inputhandler.ins.move.magnitude > 0) {
            currentState = PlayerState.Moving;
        } else  {
            currentState = PlayerState.Idle;
        }}
        if(inputhandler.ins.inputs["jump"] && isGrounded()) {
            rb.AddForce(Vector2.up * save.ins.activesave.jumpstrength, ForceMode2D.Impulse);
        }
        if (inputhandler.ins.inputs["shoot"] && weapon != null && currentState != PlayerState.Dashing  && UIStuff.ins.energybar.value >= save.ins.activesave.staminause && !cooldown) {
            Debug.DrawRay(transform.position, Vector2.right * transform.localScale.x * weapon.range * range, Color.red);
                RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.right * transform.localScale.x, weapon.range * range, dashLayer);
                if (hit.collider != null) {
                    StartCoroutine(DashTowards(hit.point));
                    
                    UIStuff.ins.energybar.value -= save.ins.activesave.staminause;
                    cooldown = true;
                }
        }
        if (inputhandler.ins.mousepos.x > transform.position.x) {
            transform.localScale = new Vector3(1, 1, 1);
        } else {
            transform.localScale = new Vector3(-1, 1, 1);
        }
    }
    public bool isGrounded() {
        Debug.DrawRay(transform.position, Vector2.down * 1.1f, Color.red);
        return Physics2D.Raycast(transform.position, Vector2.down, 1.1f, LayerMask.GetMask("Ground"));
    }
    
IEnumerator DashTowards(Vector2 target)
{
    currentState = PlayerState.Dashing;
    Vector2 direction = (target - (Vector2)transform.position).normalized;

    while (Vector2.Distance(transform.position, target) > weapon.range/2f)
    {
        rb.linearVelocity = direction * save.ins.activesave.dashSpeed;
        yield return null;
    }

    rb.linearVelocity = Vector2.zero;
    yield return follower.ins.Shake(0.1f, 0.25f);
    currentState = PlayerState.Idle;
    yield return UIStuff.ins.Cooldown(save.ins.activesave.dashCooldown);
    cooldown = false;
}

}
