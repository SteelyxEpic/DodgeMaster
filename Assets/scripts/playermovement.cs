using UnityEngine;
using System.Collections;

public class playermovement : MonoBehaviour
{
    public static playermovement ins;
    public float speed = 5f; // Speed of the player movement
    public bool overheated = false; // Overheat status
    public float range = 5f; // Range
    public float combo = 0; // Combo
    public CircleCollider2D col; // Collider of the player
    public grappler grapple; // Grappler script reference
    public Weapon weapon;
    public LayerMask dashLayer;
    public LayerMask grappleLayer;
    private Rigidbody2D rb;
    private bool[] cooldown = new bool[20]; // Assuming 20 different cooldowns
    private float speedmulti = 1f;

    [Header("Grapple Handoff")]
    // How long after releasing the grapple that input movement eases back in
    // instead of instantly overwriting swing momentum.
    public float PGMT = 0.35f;
    private float postGrappleMomentumTime = 0.35f;
    // How quickly input regains control during that window (higher = snappier).
    public float momentumRegainRate = 6f;
    private float grappleReleaseTime = -999f;

    public enum PlayerState
    {
        Idle,
        Moving,
        Dashing,
        Running,
        Grappling,
        Smashing,
        cooldown
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

    // Called by grappler.OnDisable() so we know when to start easing input back in.
    public void NotifyGrappleReleased()
    {
        postGrappleMomentumTime = PGMT * Mathf.Sqrt(rb.linearVelocity.magnitude);
        grappleReleaseTime = Time.time;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if(currentState == PlayerState.Smashing){
        if (cooldown[3] && isGrounded())
{
    StartCoroutine(follower.ins.Shake(0.2f, 0.35f));
    col.enabled = false; // Disable the collider when the player lands
    currentState = PlayerState.cooldown;
    GetComponent<Animator>().SetFloat("smashspeed", GetComponent<Animator>().GetFloat("AttackSpeed"));

        // Start cooldown if desired
    StartCoroutine(Cooldown(3, 1/(2*weapon.attackRate)));
}else return;
        }
        if (currentState == PlayerState.Dashing || currentState == PlayerState.cooldown) {
            return; // Skip movement and other actions while dashing
        }
        if (inputhandler.ins.inputs["Grapple"] && !isGrounded() && !cooldown[2]) {
            grapple.enabled = true; // Enable the grappler script
            currentState = PlayerState.Grappling;
            cooldown[2] = true;
            StartCoroutine(Cooldown(2, 1/weapon.attackRate));
        }
        if (inputhandler.ins.inputs["Sprint"]) {
            speedmulti = save.ins.activesave.speedmulti;
        } else {
            speedmulti = 1f;
        }
        if (PlayerState.Running == currentState && UIStuff.ins.energybar.value > 0) {
            UIStuff.ins.energybar.value -= save.ins.activesave.staminause * Time.deltaTime;
        }

        if (grapple.enabled)
        {
            // The grappler fully owns rb.linearVelocity while active -- don't fight it.
            // Still let facing update below so aiming/looking works mid-swing.
            FaceTarget();
            return;
        }

        bool inMomentumWindow = Time.time - grappleReleaseTime < postGrappleMomentumTime;
        float targetX = inputhandler.ins.move.normalized.x * save.ins.activesave.speed * speedmulti;

        if (inMomentumWindow)
        {
            // Ease input back in instead of overwriting swing momentum instantly --
            // this is what lets a well-timed grapple release actually carry speed.
            float t = 1f - Mathf.Exp(-momentumRegainRate * Time.deltaTime);
            rb.linearVelocity = new Vector2(Mathf.Lerp(rb.linearVelocity.x, targetX, t), rb.linearVelocity.y);
        }
        else
        {
            rb.linearVelocity = new Vector2(targetX, rb.linearVelocity.y);
        }

        if(inputhandler.ins.move.magnitude > 0) {
            currentState = speedmulti == 1 ? PlayerState.Moving : PlayerState.Running;
            
        } else  {
            currentState = PlayerState.Idle;
        }
        if(inputhandler.ins.inputs["jump"]) if(isGrounded()) {
            rb.AddForce(Vector2.up * save.ins.activesave.jumpstrength, ForceMode2D.Impulse);
        }else if (!cooldown[1] && UIStuff.ins.energybar.value >= save.ins.activesave.staminause) {
            Debug.DrawRay(transform.position, Vector2.right * Mathf.Sign(rb.linearVelocity.x) * weapon.range * range, Color.red);
                RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.right  * Mathf.Sign(rb.linearVelocity.x), weapon.range * range, dashLayer);
                if (hit.collider != null) {
                    StartCoroutine(DashTowards(hit.point, true));
                }else {
                   StartCoroutine(DashTowards((Vector2)transform.position + Vector2.right * Mathf.Sign(rb.linearVelocity.x) * weapon.range * range, false));
                }
                UIStuff.ins.energybar.value -= save.ins.activesave.staminause;
                    cooldown[1] = true;
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
            }else if (!isGrounded(1.5f))
            {
                if(!cooldown[3])
                {
                    currentState = PlayerState.Smashing;
                    GetComponent<Animator>().SetTrigger("smash");
                    cooldown[3] = true;
                }
            }
            else if (!cooldown[0])
            {
                GetComponent<Animator>().SetTrigger("attack");
                cooldown[0] = true;
                StartCoroutine(Cooldown(0, 1/weapon.attackRate));
            }
        }
        FaceTarget();
        
        




            }

    private void FaceTarget()
    {
        if (inputhandler.ins.mousepos.x > transform.position.x) {
            transform.localScale = new Vector3(1, 1, 1);
        } else {
            transform.localScale = new Vector3(-1, 1, 1);
        }
    }

    public bool isGrounded(float range = 1.1f) {
        Debug.DrawRay(transform.position, Vector2.down * range, Color.red);
        return Physics2D.Raycast(transform.position, Vector2.down, range, LayerMask.GetMask("Ground"));
    }
    public void recieved(Collider2D other) {
            Debug.Log("Hit Enemy");
            combo++;
            UIStuff.ins.comboDisplayTrigger(combo.ToString());
    }
    
IEnumerator Cooldown(int index, float time) {
        yield return UIStuff.ins.Cooldown(time);
        cooldown[index] = false;
        if (currentState == PlayerState.cooldown)
        {
            currentState = PlayerState.Idle;
        }
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
        GetComponent<Animator>().SetTrigger("dash");
    yield return Cooldown(1, save.ins.activesave.dashCooldown);
}

}