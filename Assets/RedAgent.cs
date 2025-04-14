using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class RedAgent : Agent
{
    [SerializeField] private Transform _RedAgentPos;

    [HideInInspector] public int CurrentEpisode = 0;
    [HideInInspector] public float CumulativeReward = 0f;
    private int _totalPoints = 0;
    // Local storage for diamond count so we can detect changes.
    private int lastDiamondCount = 0;

    public Spawner spawner;
    public MovementWASD movementWASD;
    public RedInventory redInventory;
    public TimeScript timeScript;

    // Transform representing the center of your environment.
    public Transform environmentCenter;
    // SpawnOffset: 4 units to the left.
    public Vector3 spawnOffset = new Vector3(-4f, 0f, 0f);

    private void Awake()
    {
        if (redInventory == null)
            redInventory = RedInventory.Instance;
    }

    public override void OnEpisodeBegin()
    {
        // Reset internal score.
        _totalPoints = 0;

        // Determine environment center; if not provided, default to the origin.
        Vector3 center = environmentCenter != null ? environmentCenter.position : Vector3.zero;
        // Set the agent's spawn position: center + spawnOffset (i.e., 4 units left of center).
        if (_RedAgentPos != null)
        {
            _RedAgentPos.position = center + spawnOffset;
        }
        else
        {
            transform.position = center + spawnOffset;
        }

        // Clear the board by destroying all spawned resources.
        if (spawner != null && spawner.spawnedResources != null)
        {
            foreach (GameObject resource in spawner.spawnedResources)
            {
                if (resource != null)
                    Destroy(resource);
            }
            spawner.spawnedResources.Clear();
        }

        // Reset the time.
        if (timeScript != null)
            timeScript.ResetTimer();

        // Reset stored diamond count.
        if (redInventory != null)
            lastDiamondCount = redInventory.GetDiamondCount();
        else
            lastDiamondCount = 0;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        float posXNormalized = _RedAgentPos.localPosition.x;
        float posYNormalized = _RedAgentPos.localPosition.y;
        sensor.AddObservation(posXNormalized);
        sensor.AddObservation(posYNormalized);

        List<Vector2> positions = spawner.GetSpawnedResourcePositions();
        foreach (Vector2 pos in positions)
            sensor.AddObservation(pos);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        Vector2 moveInput = new Vector2(actions.ContinuousActions[0], actions.ContinuousActions[1]);
        movementWASD.MoveAgent(moveInput);

        AddReward(-2f / MaxStep);
        CumulativeReward = GetCumulativeReward();
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var continuousActionsOut = actionsOut.ContinuousActions;
        float moveX = 0f, moveY = 0f;
        if (Input.GetKey(KeyCode.D)) moveX = 1f;
        if (Input.GetKey(KeyCode.A)) moveX = -1f;
        if (Input.GetKey(KeyCode.W)) moveY = 1f;
        if (Input.GetKey(KeyCode.S)) moveY = -1f;
        Vector2 input = new Vector2(moveX, moveY).normalized;
        continuousActionsOut[0] = input.x;
        continuousActionsOut[1] = input.y;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Boundary"))
            AddReward(-1f);
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Boundary"))
            AddReward(-0.01f * Time.fixedDeltaTime);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Gold"))
        {
            AddReward(0.04f);
            _totalPoints += 1;
        }
        if (_totalPoints >= 20)
            EndEpisode();
    }

    void OnTriggerStay2D(Collider2D collision)
    {
        int currentDiamondCount = redInventory.GetDiamondCount();
        if (collision.gameObject.CompareTag("Diamond") && currentDiamondCount != lastDiamondCount)
        {
            AddReward(32f);
            lastDiamondCount = currentDiamondCount;
        }
    }
}
