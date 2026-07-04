using UnityEngine;
using System.Collections;

public class playermovement : MonoBehaviour
{
    public static playermovement ins;
    public float speed = 5f; // Speed of the player movement
    public float range = 5f; // Range
    public float combo = 0; // Combo
    public CircleCollider2D col; // Collider of the player
    public Weapon weapon;
    public LayerMask dashLayer;
    private Rigidbody2D rb;
    private bool[] cooldown = new bool[3]; // Assuming 3 different cooldowns
    private float speedmulti = 1f;
    public enum PlayerState
    {
        Idle,
        Moving,
        Dashing,
        Running
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
        SetWeapon();
    }

    public void SetWeapon()
    {
        col.radius = weapon.range;
        GetComponent<Animator>().SetFloat("AttackSpeed", weapon.attackSpeed);
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
            currentState = speedmulti == 1 ? PlayerState.Moving : PlayerState.Running;
        } else  {
            currentState = PlayerState.Idle;
        }}
        if(inputhandler.ins.inputs["jump"] && isGrounded()) {
            rb.AddForce(Vector2.up * save.ins.activesave.jumpstrength, ForceMode2D.Impulse);
        }
        if (inputhandler.ins.inputs["shoot"] && weapon != null ) {
            if(currentState == PlayerState.Running  && UIStuff.ins.energybar.value >= save.ins.activesave.staminause && !cooldown[1]){
            Debug.DrawRay(transform.position, Vector2.right * transform.localScale.x * weapon.range * range, Color.red);
                RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.right * transform.localScale.x, weapon.range * range, dashLayer);
                if (hit.collider != null) {
                    StartCoroutine(DashTowards(hit.point, true));
                }else {
                   StartCoroutine(DashTowards((Vector2)transform.position + Vector2.right * transform.localScale.x * weapon.range * range, false));
                }
                UIStuff.ins.energybar.value -= save.ins.activesave.staminause;
                    cooldown[1] = true;
            }
            else if (currentState != PlayerState.Dashing && !cooldown[0])
            {
                GetComponent<Animator>().SetTrigger("attack");
                cooldown[0] = true;
                StartCoroutine(Cooldown(0, 1/weapon.attackRate));
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
    public void recieved(Collider2D other) {
            Debug.Log("Hit Enemy");
            combo++;
    }
    
IEnumerator Cooldown(int index, float time) {
        yield return UIStuff.ins.Cooldown(time);
        cooldown[index] = false;
    }
IEnumerator DashTowards(Vector2 target, bool hit)
{
    currentState = PlayerState.Dashing;
    Vector2 direction = (target - (Vector2)transform.position).normalized;

    while (Vector2.Distance(transform.position, target) > weapon.range/2f)
    {
        rb.linearVelocity = direction * save.ins.activesave.dashSpeed;
        yield return null;
    }

    rb.linearVelocity = Vector2.zero;
    if (hit) {
        col.gameObject.SetActive(true);
        yield return follower.ins.Shake(0.1f, 0.25f);

    }
    currentState = PlayerState.Idle;
    yield return Cooldown(1, save.ins.activesave.dashCooldown);
}

}
