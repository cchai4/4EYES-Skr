using UnityEngine;
using System.Collections;
using static GridCellTint;

[RequireComponent(typeof(Rigidbody2D))]
public class RedAIController : MonoBehaviour
{
    public enum AIState { GatherDiamond, Idle }

    [Header("AI Settings")]
    public AIState currentState = AIState.GatherDiamond;
    public float acceleration = 10f, maxSpeed = 5f, retargetInterval = 1f;

    [Header("References")]
    public Transform spriteTransform;
    public GridCursor redCursor; // Assign the proper GridCursor in the Inspector

    private Rigidbody2D rb;
    private Transform target;
    private Vector2 moveDirection;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (spriteTransform == null)
            spriteTransform = transform;
    }

    void Start()
    {
        InvokeRepeating(nameof(UpdateTarget), 0f, retargetInterval);
        StartCoroutine(TestAllAIBehaviors());
    }

    void Update()
    {
        switch (currentState)
        {
            case AIState.GatherDiamond: HandleGatherMovement(); break;
            case AIState.Idle: moveDirection = Vector2.zero; rb.linearVelocity = Vector2.zero; break;
        }
    }

    void UpdateTarget()
    {
        if (currentState == AIState.GatherDiamond)
        {
            target = FindClosestWithTag("Diamond") ?? FindClosestWithTag("Gold");
        }
        else target = null;
    }

    void HandleGatherMovement()
    {
        Vector2 direction = target != null ? (target.position - transform.position).normalized : Vector2.zero;
        direction = SnapToEightDirections(direction);
        moveDirection = direction;
        rb.linearVelocity = Vector2.MoveTowards(rb.linearVelocity, direction * maxSpeed, acceleration * Time.deltaTime);
        if (direction.sqrMagnitude > 0.01f)
        {
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
            spriteTransform.rotation = Quaternion.Euler(0f, 0f, angle);
        }
    }

    Transform FindClosestWithTag(string tag)
    {
        GameObject[] objs = GameObject.FindGameObjectsWithTag(tag);
        Transform closest = null;
        float best = float.MaxValue;
        Vector3 pos = transform.position;
        foreach (var obj in objs)
        {
            float dist = (obj.transform.position - pos).sqrMagnitude;
            if (dist < best) { best = dist; closest = obj.transform; }
        }
        return closest;
    }

    Vector2 SnapToEightDirections(Vector2 dir)
    {
        if (dir == Vector2.zero) return Vector2.zero;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        angle = Mathf.Round(angle / 45f) * 45f;
        float rad = angle * Mathf.Deg2Rad;
        return new Vector2(Mathf.Cos(rad), Mathf.Sin(rad)).normalized;
    }

    public IEnumerator EnterGrid()
    {
        if (!gameObject.activeInHierarchy)
        {
            Debug.Log("[RedAI] Already inactive. Aborting EnterGrid.");
            yield break;
        }
        Debug.Log("[RedAI] EnterGrid: Moving right until grid collision.");
        float start = Time.time;
        while (gameObject.activeInHierarchy)
        {
            moveDirection = Vector2.right;
            yield return null;
            if (Time.time - start > 15f)
            {
                Debug.LogWarning("[RedAI] Timeout entering grid.");
                break;
            }
        }
        Debug.Log("[RedAI] Should now be inactive and in grid mode.");
    }

    public IEnumerator TravelToCell(int targetRow, int targetCol)
    {
        if (redCursor == null || !redCursor.hasJoined)
        {
            Debug.LogError("[RedAI] GridCursor not set or joined.");
            yield break;
        }
        Debug.Log($"[RedAI] Traveling to cell ({targetRow}, {targetCol}).");
        while (redCursor.CurrentRow != targetRow)
        {
            int dir = redCursor.CurrentRow < targetRow ? 1 : -1;
            if (!AttemptCursorMove(redCursor.CurrentRow + dir, redCursor.CurrentCol))
            {
                Debug.LogWarning("[RedAI] Row move blocked.");
                yield break;
            }
            yield return new WaitForSeconds(0.2f);
        }
        while (redCursor.CurrentCol != targetCol)
        {
            int dir = redCursor.CurrentCol < targetCol ? 1 : -1;
            if (!AttemptCursorMove(redCursor.CurrentRow, redCursor.CurrentCol + dir))
            {
                Debug.LogWarning("[RedAI] Column move blocked.");
                yield break;
            }
            yield return new WaitForSeconds(0.2f);
        }
        Debug.Log("[RedAI] Arrived at target cell.");
    }

    public IEnumerator ExitGrid()
    {
        if (redCursor == null || !redCursor.hasJoined)
        {
            Debug.LogError("[RedAI] GridCursor not set or not joined. Aborting ExitGrid.");
            yield break;
        }
        Debug.Log("[RedAI] Exiting grid...");
        while (redCursor.CurrentCol != 0)
        {
            int dir = redCursor.CurrentCol > 0 ? -1 : 1;
            if (!AttemptCursorMove(redCursor.CurrentRow, redCursor.CurrentCol + dir))
            {
                Debug.LogWarning("[RedAI] Move blocked while exiting grid.");
                yield break;
            }
            yield return new WaitForSeconds(0.2f);
        }
        Debug.Log("[RedAI] Triggering final left move to exit grid.");
        AttemptCursorMove(redCursor.CurrentRow, -1);
        yield return null;
        yield return new WaitForSeconds(1f);
        if (gameObject.activeInHierarchy)
            Debug.Log("[RedAI] Back on left side, reactivated.");
        else
            Debug.LogWarning("[RedAI] Still inactive. ExitGrid may have failed.");
    }

    private bool AttemptCursorMove(int newRow, int newCol)
    {
        if (!redCursor || !redCursor.hasJoined) return false;
        int r = Mathf.Clamp(newRow, 0, redCursor.TotalRows - 1);
        int c = Mathf.Clamp(newCol, 0, redCursor.TotalCols - 1);
        if (r == redCursor.CurrentRow && c == redCursor.CurrentCol) return false;
        if (!redCursor.CanMoveToCell(r, c)) return false;
        redCursor.DoMoveToCell(r, c);
        return true;
    }

    private IEnumerator TestAllAIBehaviors()
    {
        Debug.Log("[RedAI] Test: Gathering for 3 sec.");
        yield return new WaitForSeconds(3f);
        yield return StartCoroutine(EnterGrid());
        yield return new WaitForSeconds(2f);
        if (redCursor != null)
        {
            float timer = 0f;
            while (!redCursor.hasJoined && timer < 2f)
            {
                yield return null;
                timer += Time.deltaTime;
            }
            if (redCursor.hasJoined)
            {
                yield return StartCoroutine(TravelToCell(2, 2));
                yield return StartCoroutine(ExitGrid());
            }
            else Debug.LogWarning("[RedAI] Cursor never joined.");
        }
        else Debug.LogWarning("[RedAI] No GridCursor assigned.");
        if (gameObject.activeInHierarchy)
        {
            Debug.Log("[RedAI] Test routine complete. Resuming normal behavior.");
            currentState = AIState.GatherDiamond;
        }
    }
}
