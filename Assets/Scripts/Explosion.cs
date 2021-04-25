using UnityEngine;

public class Explosion : MonoBehaviour
{
    private void Start()
    {
        Destroy(gameObject, 3f);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        CarrierDrone[] drones = other.gameObject.GetComponents<CarrierDrone>();

        for (int i = 0; i < drones.Length; i++)
        {
            Debug.Log("Damaged: " + drones[i].gameObject.name);
            drones[i].DamageDrone();
        }

        if (other.CompareTag("Player"))
        {
            Player player = other.GetComponent<Player>();
            if(player != null)
            {
                player.Damage(1);
            }
        }
    }
}
