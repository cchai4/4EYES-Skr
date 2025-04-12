using UnityEngine;

public class DashHitboxBlue : MonoBehaviour
{
    public float recoilMultiplier = 0.2f;
    private BlueDash blueDash;

    void Awake() => blueDash = GetComponentInParent<BlueDash>();

    void OnTriggerEnter2D(Collider2D col)
    {
        if (!col.CompareTag("Red")) return;

        Rigidbody2D redRb = col.GetComponent<Rigidbody2D>();
        if (redRb)
            redRb.AddForce(blueDash.dashDirection * blueDash.knockbackForce,
                           ForceMode2D.Impulse);

        // cancel dash & apply recoil to Blue
        blueDash.CancelDash();
        Rigidbody2D blueRb = blueDash.GetComponent<Rigidbody2D>();
        if (blueRb)
        {
            blueRb.velocity = Vector2.zero;
            blueRb.AddForce(-blueDash.dashDirection *
                            blueDash.knockbackForce * recoilMultiplier,
                            ForceMode2D.Impulse);
        }
    }
}
