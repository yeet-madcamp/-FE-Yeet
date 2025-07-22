using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditController : MonoBehaviour
{
    public enum EditMode { None, Wall, Bit, Exit,Trap }
    public enum EditState { Placing, Deleting}
    private EditMode currentMode = EditMode.None;
    private EditState currentState = EditState.Placing;

    [SerializeField] GameObject[] editPrefabs;
    [SerializeField] Transform[] editParents;

    [SerializeField] GameObject wallTopPrefab;       // 벽 꼭대기 프리팹
    [SerializeField] Transform wallTopParent;

    [SerializeField] GridManager gridManager;
    
    [HideInInspector] public int minX = -5, maxX = 5;
    [HideInInspector] public int minY = -5, maxY = 5;

    int offsetC;
    int offsetR;

    private void Start()
    {
        offsetC = gridManager.columns / 2;
        offsetR = gridManager.rows / 2;
        maxX = offsetC;
        maxY = offsetR;
        minX = -offsetC;
        minY = -offsetR;
    }

    private void Update()
    {
        if (currentMode == EditMode.None) return;

        if (Input.GetMouseButton(0))
        {
            switch (currentMode)
            {
                case EditMode.Wall:
                    WallControl();
                    break;
                case EditMode.Bit:
                    BitcoinControl();
                    break;
                case EditMode.Trap:
                    TrapControl();
                    break;

            } 
        }
        if (Input.GetMouseButtonDown(0) && currentMode == EditMode.Exit)
        {
            Debug.Log("exit 클릭됨.");
            ExitControl();
        }
    }
    void WallControl()
    {
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0;

        int gridX = Mathf.RoundToInt(mouseWorldPos.x);
        int gridY = Mathf.RoundToInt(mouseWorldPos.y);

        // ✅ 바운더리 검사
        if (gridX < minX || gridX > maxX || gridY < minY || gridY > maxY)
        {
            Debug.Log("⛔ 바운더리 밖: " + gridX + "," + gridY);
            return;
        }

        Vector3 gridPos = new Vector3(gridX, gridY, 0);
        Vector3 topPos = new Vector3(gridX, gridY + 0.4f, 0);

        // 이미 벽이 있는지 확인
        Collider2D hit = Physics2D.OverlapPoint(gridPos);
        if (currentState == EditState.Placing)
        {
            if (hit == null)
            {
                Instantiate(editPrefabs[0], gridPos, Quaternion.identity, editParents[0]);
                Instantiate(wallTopPrefab, topPos, Quaternion.identity, wallTopParent);
            }
        }
        else if (currentState == EditState.Deleting)
        {
            if (hit != null && hit.CompareTag("Wall"))
            {
                Destroy(hit.gameObject);
                // 꼭대기도 삭제
                Collider2D topHit = Physics2D.OverlapPoint(topPos);
                if (topHit != null && topHit.CompareTag("WallTop"))
                    Destroy(topHit.gameObject);
            }
        }
    }

    void BitcoinControl()
    {
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0;

        int gridX = Mathf.RoundToInt(mouseWorldPos.x);
        int gridY = Mathf.RoundToInt(mouseWorldPos.y);

        // ✅ 바운더리 검사
        if (gridX < minX || gridX > maxX || gridY < minY || gridY > maxY)
        {
            Debug.Log("⛔ 바운더리 밖: " + gridX + "," + gridY);
            return;
        }

        Vector3 gridPos = new Vector3(gridX, gridY, 0);

        // 이미 오브젝트가 있는지 확인
        Collider2D hit = Physics2D.OverlapPoint(gridPos);

        if (currentState == EditState.Placing)
        {
            // 현재 맵에 존재하는 코인 개수 확인
            int currentCoinCount = GameObject.FindGameObjectsWithTag("BitCoin").Length;

            if (currentCoinCount >= 3)
            {
                Debug.Log("⚠️ 최대 코인 개수(3개)에 도달했습니다.");
                return;
            }

            if (hit == null)
            {
                Instantiate(editPrefabs[1], gridPos, Quaternion.identity, editParents[1]);
            }
        }
        else if (currentState == EditState.Deleting)
        {
            if (hit != null && hit.CompareTag("BitCoin"))
            {
                Destroy(hit.gameObject);
            }
        }
    }
    void TrapControl()
    {
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0;

        int gridX = Mathf.RoundToInt(mouseWorldPos.x);
        int gridY = Mathf.RoundToInt(mouseWorldPos.y);

        // ✅ 바운더리 검사
        if (gridX < minX || gridX > maxX || gridY < minY || gridY > maxY)
        {
            Debug.Log("⛔ 바운더리 밖: " + gridX + "," + gridY);
            return;
        }

        Vector3 gridPos = new Vector3(gridX, gridY, 0);

        // 이미 벽이 있는지 확인
        Collider2D hit = Physics2D.OverlapPoint(gridPos);
        if (currentState == EditState.Placing)
        {
            if (hit == null)
                Instantiate(editPrefabs[2], gridPos, Quaternion.identity, editParents[2]);
        }
        else if (currentState == EditState.Deleting)
        {
            if (hit != null && hit.CompareTag("Trap"))
                Destroy(hit.gameObject);
        }
    }
    void ExitControl()
    {
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0;

        int gridX = Mathf.RoundToInt(mouseWorldPos.x);
        int gridY = Mathf.RoundToInt(mouseWorldPos.y);

        // ✅ 바운더리 검사
        if (gridX < minX || gridX > maxX || gridY < minY || gridY > maxY)
        {
            Debug.Log("⛔ 바운더리 밖: " + gridX + "," + gridY);
            return;
        }

        Vector3 gridPos = new Vector3(gridX, gridY, 0);
        GameObject exitObj = GameObject.FindGameObjectWithTag("Exit");

        // 이미 벽이 있는지 확인
        Collider2D hit = Physics2D.OverlapPoint(gridPos);
        if (currentState == EditState.Placing)
        {
            if(exitObj != null)
            {
                Destroy(exitObj);
            }
            if (hit == null)
                Instantiate(editPrefabs[3], gridPos, Quaternion.identity);
        }
        else if (currentState == EditState.Deleting)
        {
            if (hit != null && hit.CompareTag("Exit"))
                Destroy(hit.gameObject);
        }
    }
    // UI 버튼에서 호출
    public void SetMode(string editMode)
    {
        System.Enum.TryParse(editMode, out EditMode mode);
        currentMode = mode;
        
    }
    public void setState(string editState)
    {
        System.Enum.TryParse(editState, out EditState state);
        currentState = state;
    }
    public void StopAllModes() => currentMode = EditMode.None;
}
