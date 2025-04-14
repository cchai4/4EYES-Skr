using System.Collections.Generic;
using UnityEngine;

public class MovementWASD : MonoBehaviour
{
    [Header("Key Assignments")]
    public PlayerControlsSO controls;

    // Reference to the RedStun component.
    private RedStun redStun;
    public float acceleration = 10f;
    public float maxSpeed = 5f;
    private Rigidbody2D rb2;

    // Reference to the spawner with the list of spawned resource GameObjects.
    public Spawner spawner;

    // Public property to expose the last movement direction.
    public Vector2 LastMovementDirection { get; private set; } = Vector2.up;
    // Public property to expose the current target velocity.
    public Vector2 CurrentTargetVelocity { get; private set; } = Vector2.zero;
    // NEW: Public property to expose the current target position.
    public Vector2 CurrentTargetPosition { get; private set; } = Vector2.zero;
    // NEW: Public property to store whether the target resource is a gold item.
    public bool CurrentTargetIsGold { get; private set; } = false;

    // Dampening distance within which the movement will slow down.
    public float dampeningDistance = 1.5f;

    [Header("Special Movement Destination Requirements")]
    // Required amounts for special destination override.
    public int requiredGoldForDestination = 5;
    public int requiredDiamondForDestination = 3;

    void Awake()
    {
        rb2 = GetComponent<Rigidbody2D>();
        redStun = GetComponent<RedStun>();
    }

    void Update()
    {
        // Skip movement if stunned.
        if (redStun != null && redStun.isStunned)
            return;

        Vector2 targetVelocity;
        Vector2 currentPosition = transform.position;

        // Check if inventory meets the resource requirements.
        if (RedInventory.Instance != null &&
            RedInventory.Instance.HasEnoughResources(requiredGoldForDestination, requiredDiamondForDestination))
        {
            // Override target: move toward (0,4)
            Vector2 targetPosition = new Vector2(0f, 4f);
            CurrentTargetPosition = targetPosition;
            // For inventory override we mark the target as not specifically gold.
            CurrentTargetIsGold = false;

            Vector2 desiredDirection = (targetPosition - currentPosition).normalized;
            targetVelocity = desiredDirection * maxSpeed;

            // Dampening when approaching (0,4)
            float distance = Vector2.Distance(currentPosition, targetPosition);
            if (distance < dampeningDistance)
            {
                float dampFactor = Mathf.Clamp01(distance / dampeningDistance);
                targetVelocity *= dampFactor;
            }

            if (desiredDirection.sqrMagnitude > 0.01f)
                LastMovementDirection = desiredDirection;
        }
        else // Regular behavior using spawned resource targets.
        {
            List<GameObject> resources = spawner.spawnedResources;
            if (resources != null && resources.Count > 0)
            {
                GameObject closestResource = null;
                float closestDistance = Mathf.Infinity;
                foreach (GameObject resource in resources)
                {
                    if (resource != null && (resource.CompareTag("Gold") || resource.CompareTag("Diamond")))
                    {
                        float distance = Vector2.Distance(currentPosition, resource.transform.position);
                        if (distance < closestDistance)
                        {
                            closestDistance = distance;
                            closestResource = resource;
                        }
                    }
                }

                if (closestResource != null)
                {
                    Vector2 targetPosition = closestResource.transform.position;
                    CurrentTargetPosition = targetPosition;
                    // Set flag depending on whether the target is Gold.
                    CurrentTargetIsGold = closestResource.CompareTag("Gold");

                    Vector2 desiredDirection = (targetPosition - currentPosition).normalized;
                    targetVelocity = desiredDirection * maxSpeed;

                    // Only dampen for Diamond targets.
                    if (!CurrentTargetIsGold && closestResource.CompareTag("Diamond"))
                    {
                        if (closestDistance < dampeningDistance)
                        {
                            float dampFactor = Mathf.Clamp01(closestDistance / dampeningDistance);
                            targetVelocity *= dampFactor;
                        }
                    }

                    if (desiredDirection.sqrMagnitude > 0.01f)
                        LastMovementDirection = desiredDirection;
                }
                else
                {
                    targetVelocity = Vector2.zero;
                }
            }
            else
            {
                targetVelocity = Vector2.zero;
            }
        }

        CurrentTargetVelocity = targetVelocity;
        rb2.linearVelocity = Vector2.MoveTowards(rb2.linearVelocity, targetVelocity, acceleration * Time.deltaTime);
    }
}
