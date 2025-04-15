using UnityEngine;

public class diamond : MonoBehaviour
{
    public float Hover_duration = 3f;
    private float hover_time = 0f;
    private bool collected = false;

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 6 || collision.gameObject.layer == 7)
        {
            hover_time += Time.deltaTime;

            if (!collected && hover_time >= Hover_duration)
            {
                collected = true;

                if (collision.gameObject.layer == 6)
                {
                    Blue_Inventory blueInv = Blue_Inventory.Instance;
                    if (blueInv != null)
                    {
                        blueInv.add_diamond();
                        blueInv.write_diamond();
                    }
                    else
                    {
                        Debug.LogWarning("Blue Inventory singleton not found!");
                    }
                }
                else if (collision.gameObject.layer == 7) 
                {
                    RedInventory redInv = RedInventory.Instance;
                    if (redInv != null)
                    {
                        redInv.add_diamond();
                        redInv.write_diamond();
                    }
                    else
                    {
                        Debug.LogWarning("Red Inventory singleton not found!");
                    }
                }

                Destroy(gameObject);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 6 || collision.gameObject.layer == 7)
        {
            hover_time = 0f;
            collected = false;
        }
    }
}
