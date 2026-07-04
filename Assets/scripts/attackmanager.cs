using UnityEngine;

public class attackmanager : MonoBehaviour
{
    public playermovement playerMovement;
   void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            playerMovement.recieved(other);
        }
    }
}
