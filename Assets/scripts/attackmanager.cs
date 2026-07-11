using UnityEngine;

public class attackmanager : MonoBehaviour
{
    public playermovement playerMovement;
   void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            StartCoroutine(follower.ins.Shake(0.05f, 0.15f));
            playerMovement.recieved(other);
        }
    }
}
