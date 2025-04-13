using UnityEngine;

public class Silver : MonoBehaviour
{
    public AudioClip pickupSound;
    public float volume = 0.7f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 6)
        {
            Blue_Inventory blue_inven = collision.GetComponent<Blue_Inventory>();
            blue_inven.add_silver();
        }
        else if (collision.gameObject.layer == 7)
        {
            RedInventory red_inven = collision.GetComponent<RedInventory>();
            red_inven.add_silver();
        }

        AudioSource.PlayClipAtPoint(pickupSound, transform.position, volume);
        Destroy(gameObject);
    }
}
