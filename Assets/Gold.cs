using UnityEngine;

public class Gold : MonoBehaviour
{
    public AudioClip pickupSound;
    public float volume = 0.7f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 6)
        {
            Blue_Inventory blue_inven = collision.GetComponent<Blue_Inventory>();
            if (blue_inven != null) blue_inven.add_gold();
        }
        else if (collision.gameObject.layer == 7)
        {
            RedInventory red_inven = collision.GetComponent<RedInventory>();
            if (red_inven != null) red_inven.add_gold();
        }

        if (pickupSound != null)
            AudioSource.PlayClipAtPoint(pickupSound, transform.position, volume);

        Destroy(gameObject);
    }
}
