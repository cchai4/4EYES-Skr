using UnityEngine;
using static GridCellTint;      // ColorType enum
using System.Collections;

public class GridCursor : MonoBehaviour
{
    /* ????? Inspector ????? */
    [Header("Movement Keys")]
    public KeyCode upKey;
    public KeyCode downKey;
    public KeyCode leftKey;
    public KeyCode rightKey;

    [Header("Action Keys")]
    public KeyCode buildKey;    // Red: Space  •  Blue: RightShift
    public KeyCode cancelKey;   // Red: LeftShift • Blue: Slash (/)

    [Header("Cursor Settings")]
    public ColorType tintOwner; // Red or Blue
    public int startRow = 0, startCol = 0;

    /* ????? state ????? */
    private enum CursorState { Free, BuildingSelect }
    private CursorState currentState = CursorState.Free;
    [HideInInspector] public bool hasJoined = false;

    /* grid data (shared) */
    private static GameObject[,] cells;
    private static bool[,] occupied;
    private static int rows, cols;

    /* runtime */
    private int curRow, curCol;
    private BuildingSlot currentSlot;
    private int selectionIndex;
    private float debounce = 0f;

    /* ????? init grid ????? */
    void Awake()
    {
        /* default keys if you forget to set them in Inspector */
        if (buildKey == KeyCode.None)
            buildKey = (tintOwner == ColorType.Red) ? KeyCode.Space : KeyCode.RightShift;
        if (cancelKey == KeyCode.None)
            cancelKey = (tintOwner == ColorType.Red) ? KeyCode.LeftShift : KeyCode.Slash;
    }

    void Start() => StartCoroutine(InitGrid());

    IEnumerator InitGrid()
    {
        Transform gp = null;
        while (gp == null || gp.childCount == 0)
        {
            gp = FindFirstObjectByType<GridManager>()?.gridParent; // Updated to use FindFirstObjectByType
            yield return null;
        }

        if (cells == null)
        {
            rows = cols = Mathf.RoundToInt(Mathf.Sqrt(gp.childCount));
            cells = new GameObject[rows, cols];
            occupied = new bool[rows, cols];

            foreach (Transform t in gp)
            {
                var p = t.name.Split('_');
                if (p.Length == 3 &&
                    int.TryParse(p[1], out int r) &&
                    int.TryParse(p[2], out int c))
                    cells[r, c] = t.gameObject;
            }
        }

        for (int r = 0; r < rows; r++)
            for (int c = 0; c < cols; c++)
            {
                int rr = (startRow + r) % rows;
                int cc = (startCol + c) % cols;
                if (!occupied[rr, cc])
                {
                    curRow = rr;
                    curCol = cc;
                    EnterCell(rr, cc);
                    yield break; // Updated to use yield break instead of return
                }
            }
    }

    /* ????? update ????? */
    void Update()
    {
        if (cells == null) return;
        if (debounce > 0) debounce -= Time.deltaTime;

        if (currentState == CursorState.Free) HandleFree();
        else HandleBuildingSelect();
    }

    /* ????? FREE state ????? */
    void HandleFree()
    {
        if (Input.GetKeyDown(leftKey) && curCol == 0) { ReactivatePlayer(); return; }

        int nr = curRow, nc = curCol;
        if (Input.GetKeyDown(upKey)) nr--;
        if (Input.GetKeyDown(downKey)) nr++;
        if (Input.GetKeyDown(leftKey)) nc--;
        if (Input.GetKeyDown(rightKey)) nc++;
        nr = Mathf.Clamp(nr, 0, rows - 1);
        nc = Mathf.Clamp(nc, 0, cols - 1);

        if ((nr != curRow || nc != curCol))
        {
            var slot = cells[nr, nc].GetComponent<BuildingSlot>();
            bool blocked = slot && slot.IsBlocked(tintOwner);
            if (!blocked && !occupied[nr, nc])
            { ExitCell(curRow, curCol); EnterCell(nr, nc); curRow = nr; curCol = nc; }
        }

        if (Input.GetKeyDown(buildKey) && hasJoined)
        {
            currentSlot = cells[curRow, curCol].GetComponent<BuildingSlot>();
            if (currentSlot == null) return;

            currentState = CursorState.BuildingSelect;
            selectionIndex = 0;
            BuildingSelectionUI.Instance.StartSelection(tintOwner, selectionIndex);
        }
    }

    /* ????? BUILDING?SELECT state ????? */
    void HandleBuildingSelect()
    {
        int Dir() => (tintOwner == ColorType.Red) ? 1 : -1;

        if (Input.GetKeyDown(leftKey))
        {
            selectionIndex = Mathf.Clamp(selectionIndex - Dir(), 0, 3);
            BuildingSelectionUI.Instance.Highlight(selectionIndex);
        }
        if (Input.GetKeyDown(rightKey))
        {
            selectionIndex = Mathf.Clamp(selectionIndex + Dir(), 0, 3);
            BuildingSelectionUI.Instance.Highlight(selectionIndex);
        }

        if (Input.GetKeyDown(buildKey) && debounce <= 0f)
        {
            debounce = 0.25f;
            var chosen = (BuildingType)(selectionIndex + 1);
            currentSlot.PlaceBuilding(tintOwner, chosen);

            BuildingSelectionUI.Instance.EndSelection();
            currentState = CursorState.Free;
        }

        /* NEW: cancel key */
        if (Input.GetKeyDown(cancelKey))
        {
            BuildingSelectionUI.Instance.EndSelection();
            currentState = CursorState.Free;
        }
    }

    /* ????? cell helpers ????? */
    void EnterCell(int r, int c)
    {
        occupied[r, c] = true;
        if (hasJoined) cells[r, c].GetComponent<GridCellTint>()?.Enter(tintOwner);
    }
    void ExitCell(int r, int c)
    {
        occupied[r, c] = false;
        if (hasJoined) cells[r, c].GetComponent<GridCellTint>()?.Exit(tintOwner);
    }

    public void ForcePlaceAt(int r, int c)
    {
        ExitCell(curRow, curCol);
        curRow = Mathf.Clamp(r, 0, rows - 1);
        curCol = Mathf.Clamp(c, 0, cols - 1);
        hasJoined = true; EnterCell(curRow, curCol);
        occupied[curRow, curCol] = false;
    }

    /* ????? re?activate entity ????? */
    void ReactivatePlayer()
    {
        ExitCell(curRow, curCol); hasJoined = false;
        string name = tintOwner == ColorType.Red ? "Red" : "Blue";
        GameObject ent = null;
        foreach (var o in Resources.FindObjectsOfTypeAll<GameObject>())
            if (o.name == name) { ent = o; break; }
        if (ent == null || ent.activeInHierarchy) return;

        ent.SetActive(true);
        float w = cells[curRow, curCol].transform.localScale.x;
        ent.transform.position = cells[curRow, curCol].transform.position + Vector3.left * w * 1.5f;
    }
}
