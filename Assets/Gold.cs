using UnityEngine;

public class Gold : MonoBehaviour
{
    [Header("Sound Effect")]
    public AudioClip goldPickupClip;
    [Range(0f, 1f)]
    public float volume = 1.0f;   

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 6) 
        {
            Blue_Inventory blue_inven = Blue_Inventory.Instance;
            if (blue_inven != null)
            {
                blue_inven.add_gold();
                blue_inven.write_gold();
            }
            else
            {
                Debug.LogWarning("Blue Inventory singleton not found!");
            }
        }
        else if (collision.gameObject.layer == 7) 
        {
            RedInventory red_inven = RedInventory.Instance;
            if (red_inven != null)
            {
                red_inven.add_gold();
                red_inven.write_gold();
            }
            else
            {
                Debug.LogWarning("Red Inventory singleton not found!");
            }
        }

        
        if (goldPickupClip != null)
        {
            AudioSource.PlayClipAtPoint(goldPickupClip, transform.position, volume);
        }
        else
        {
            Debug.LogWarning("Gold pickup clip is not assigned in the Inspector!");
        }

        
        Destroy(gameObject);
    }
}
