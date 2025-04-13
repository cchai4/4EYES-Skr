using UnityEngine;

public class Gold : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 6) // Blue
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
        else if (collision.gameObject.layer == 7) // Red
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

        Destroy(gameObject); // always destroy self after pickup
    }
}
