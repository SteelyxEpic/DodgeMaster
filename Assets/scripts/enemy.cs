using UnityEngine;
using System.Collections;

public class enemy : MonoBehaviour
{
    public GameObject target;
    public float moveSpeed = 3f;
    public Weapon weapon;
    public bool stunned = false;

    private Rigidbody2D rb;
    private IEnumerator previousCoroutine;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        if (target == null || rb == null)
            return;
        if (stunned)
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            return;
        }

        Vector2 direction = target.transform.position - transform.position;
        if(Vector2.Distance(transform.position, target.transform.position) > weapon.range)
        {
            Debug.Log("Moving towards target");
            rb.linearVelocity = new Vector2(Mathf.Sign(direction.x) * moveSpeed, rb.linearVelocity.y);
        }else
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        }
    }
    public void TakeDamage(float damage)
    {
        // Handle taking damage here
        Debug.Log($"Enemy took {damage} damage!");
    }
    public void stunnedEnemy(float duration, bool knockback)
    {
        if (previousCoroutine != null)
        {
            StopCoroutine(previousCoroutine);
        }
        previousCoroutine = Stun(duration * (knockback ? 1.5f : 1f));
        StartCoroutine(previousCoroutine);
    }
    public IEnumerator Stun(float duration)
    {
        stunned = true;
        yield return new WaitForSeconds(duration);
        stunned = false;
    }
}
