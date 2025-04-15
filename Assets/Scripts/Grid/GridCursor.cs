using UnityEngine;
using static GridCellTint;
using System.Collections;

public class GridCursor : MonoBehaviour
{
    [Header("Controls")]
    public PlayerControlsSO controls;

    [Header("Cursor Settings")]
    public ColorType tintOwner;
    public int startRow = 0, startCol = 0;

    private enum CursorState { Free, BuildingSelect }
    private CursorState currentState = CursorState.Free;
    [HideInInspector] public bool hasJoined = false;

    private static GameObject[,] cells;
    private static bool[,] occupied;
    private static int rows, cols;

    private int curRow, curCol;
    private BuildingSlot currentSlot;
    private int selectionIndex;
    private float debounce = 0f;

    void Start() => StartCoroutine(InitGrid());

    IEnumerator InitGrid()
    {
        Transform gp = null;
        while (gp == null || gp.childCount == 0)
        {
            gp = FindFirstObjectByType<GridManager>()?.gridParent;
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
                    yield break;
                }
            }
    }

    void Update()
    {
        if (cells == null) return;
        if (debounce > 0) debounce -= Time.deltaTime;

        if (currentState == CursorState.Free) HandleFree();
        else HandleBuildingSelect();
    }

    void HandleFree()
    {
        if (Input.GetKeyDown(controls.leftKey) && curCol == 0) { ReactivatePlayer(); return; }

        int nr = curRow, nc = curCol;
        if (Input.GetKeyDown(controls.upKey)) nr--;
        if (Input.GetKeyDown(controls.downKey)) nr++;
        if (Input.GetKeyDown(controls.leftKey)) nc--;
        if (Input.GetKeyDown(controls.rightKey)) nc++;
        nr = Mathf.Clamp(nr, 0, rows - 1);
        nc = Mathf.Clamp(nc, 0, cols - 1);

        if ((nr != curRow || nc != curCol))
        {
            var slot = cells[nr, nc].GetComponent<BuildingSlot>();
            bool blocked = slot && slot.IsBlocked(tintOwner);
            if (!blocked && !occupied[nr, nc])
            {
                ExitCell(curRow, curCol);
                EnterCell(nr, nc);
                curRow = nr;
                curCol = nc;
            }
        }

        if (Input.GetKeyDown(controls.dashDoubleTapKey) && hasJoined)
        {
            currentSlot = cells[curRow, curCol].GetComponent<BuildingSlot>();
            if (currentSlot == null) return;

            if (currentSlot.HasBuilding())
            {
                Debug.Log("Cannot build here — cell already contains a building.");
                return;
            }

            currentState = CursorState.BuildingSelect;
            selectionIndex = 0;
            BuildingSelectionUI.Instance.StartSelection(tintOwner, selectionIndex);
        }
    }

    void HandleBuildingSelect()
    {
        int Dir() => (tintOwner == ColorType.Red) ? 1 : -1;

        if (Input.GetKeyDown(controls.leftKey))
        {
            selectionIndex = Mathf.Clamp(selectionIndex - Dir(), 0, 3);
            BuildingSelectionUI.Instance.Highlight(tintOwner, selectionIndex);
        }

        if (Input.GetKeyDown(controls.rightKey))
        {
            selectionIndex = Mathf.Clamp(selectionIndex + Dir(), 0, 3);
            BuildingSelectionUI.Instance.Highlight(tintOwner, selectionIndex);
        }

        if (Input.GetKeyDown(controls.dashDoubleTapKey) && debounce <= 0f)
        {
            debounce = 0.25f;
            var chosen = (BuildingType)(selectionIndex + 1);
            currentSlot.PlaceBuilding(tintOwner, chosen);
            BuildingSelectionUI.Instance.EndSelection(tintOwner);
            currentState = CursorState.Free;
        }

        if (Input.GetKeyDown(controls.cancelKey))
        {
            BuildingSelectionUI.Instance.EndSelection(tintOwner);
            currentState = CursorState.Free;
        }
    }

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
        hasJoined = true;
        EnterCell(curRow, curCol);
        occupied[curRow, curCol] = false;
    }

    void ReactivatePlayer()
    {
        ExitCell(curRow, curCol);
        hasJoined = false;
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