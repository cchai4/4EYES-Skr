// Cannon.cs
using UnityEngine;

public class Cannon : BuildingBase
{
    public GameObject projectilePrefab;
    public float shootInterval = 2f;
    public float projectileSpeed = 8f;
    private float timer;

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= shootInterval)
        {
            var proj = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
            proj.GetComponent<Rigidbody2D>().linearVelocity = Vector2.right * projectileSpeed;
            timer = 0f;
        }
    }
}
