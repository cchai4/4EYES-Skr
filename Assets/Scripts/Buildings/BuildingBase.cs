using UnityEngine;
using System.Collections;

public enum Team { Red, Blue }

public abstract class BuildingBase : MonoBehaviour
{
    public Team team;         
    [SerializeField] private int maxHP = 10;
    protected int currentHP;

    [Header("Destruction Sound")]
    public AudioClip destructionSfx;
    [Range(0f, 1f)]
    public float destructionVolume = 1f;

    
    private bool destructionHandled = false;

    protected virtual void Awake()
    {
        currentHP = maxHP;
        Debug.Log($"{name}: Awake, currentHP = {currentHP}");
    }

    
    public virtual void TakeDamage(int dmg)
    {
        currentHP -= dmg;
        Debug.Log($"{name}: Took damage {dmg}. CurrentHP now = {currentHP}");
        if (currentHP <= 0)
        {
            Debug.Log($"{name}: HP reached 0 or below, calling DestroyBuilding()");
            DestroyBuilding();
        }
    }

   
    protected virtual void DestroyBuilding()
    {
        destructionHandled = true;
        Debug.Log($"BuildingBase: Destroying {name} (Team {team}).");

        
        if (destructionSfx != null)
        {
            AudioSource.PlayClipAtPoint(destructionSfx, transform.position, destructionVolume);
        }

        
        if (transform.parent != null)
        {
            var slot = transform.parent.GetComponent<BuildingSlot>();
            if (slot != null)
            {
                Debug.Log($"{name}: Clearing building reference from parent BuildingSlot '{slot.name}'.");
                slot.ClearBuilding();
            }
            else
            {
                Debug.LogWarning($"{name}: Parent does not have a BuildingSlot component.");
            }
        }
        else
        {
            Debug.LogWarning($"{name}: No parent found to clear building reference from.");
        }

        
        if (GridManager.Instance != null)
        {
            Debug.Log($"{name}: Immediately refreshing territory via GridManager.");
            GridManager.Instance.RefreshAllTerritories();
        }
        else
        {
            Debug.LogWarning($"{name}: GridManager.Instance is null, cannot refresh territory.");
        }

        
        Destroy(gameObject);
    }

    private IEnumerator DelayedTerritoryRefresh()
    {
        Debug.Log($"{name}: Waiting one frame for territory refresh.");
        yield return null; 
        if (GridManager.Instance != null)
        {
            Debug.Log($"{name}: Refreshing territory via GridManager.");
            GridManager.Instance.RefreshAllTerritories();
        }
        else
        {
            Debug.LogWarning($"{name}: GridManager.Instance is null, cannot refresh territory.");
        }
    }

    
    private void OnDestroy()
    {
        if (!destructionHandled)
        {
            Debug.LogWarning($"{name}: OnDestroy() called without controlled destruction. Refreshing territory.");
            if (GridManager.Instance != null)
                GridManager.Instance.RefreshAllTerritories();
        }
    }
}
