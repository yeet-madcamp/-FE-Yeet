using System.Collections;
using UnityEngine;

public class Movement2D : MonoBehaviour
{
    [SerializeField]
    private float moveTime = 0.000001f;

    public int minX = 0;
    public int maxX = 6;
    public int minY = 0;
    public int maxY = 6;

    

    private GridManager gridManager;

    private Vector2Int initialGridPosition;
    private Vector3 initialWorldPosition;

    private Vector2Int gridPosition;

    public bool IsMove { get; private set; } = false;

    private void Awake()
    {
        gridManager = GameObject.Find("GameManager").GetComponent<GridManager>();
    }

    private void Start()
    {
        //gridPosition = Vector2Int.RoundToInt(transform.position);
        //transform.position = new Vector3(gridPosition.x, gridPosition.y, 0);

        //initialGridPosition = gridPosition;
        //initialWorldPosition = transform.position;

        //int offsetC = gridManager.columns / 2;
        //int offsetR = gridManager.rows / 2;
        //maxX = offsetC;
        //maxY = offsetR;
        //minX = -offsetC;
        //minY = -offsetR;
        Vector2Int pos = new Vector2Int(0, 0);
        SetInitialPosition(pos);
    }

    public void SetInitialPosition(Vector2Int gridPos)
    {
        gridManager = GameObject.Find("GameManager").GetComponent<GridManager>();

        int offsetC = gridManager.columns / 2;
        int offsetR = gridManager.rows / 2;
        maxX = offsetC;
        maxY = offsetR;
        minX = -offsetC;
        minY = -offsetR;

        gridPosition = gridPos;
        initialGridPosition = gridPos;

        initialWorldPosition = new Vector3(gridPos.x, gridPos.y, 0);
        transform.position = initialWorldPosition;
    }

    public void MoveTo(Vector2 targetGrid)
    {
        moveStraitGrid(targetGrid);
    }
    public void MoveToSmooth(Vector2 targetGrid)
    {
        StopAllCoroutines(); // 이전 이동 취소
        StartCoroutine(GridSmoothMovement(Vector2Int.RoundToInt(targetGrid)));
    }

    void moveStraitGrid(Vector2 targetGrid)
    {
        Vector2Int target = Vector2Int.RoundToInt(targetGrid);

        // 바운더리 검사
        if (target.x < minX || target.x > maxX || target.y < minY || target.y > maxY)
            return;

        Vector3 end = new Vector3(target.x, target.y, 0);

        // 충돌 검사
        Collider2D hit = Physics2D.OverlapPoint(end);
        if (hit != null && hit.CompareTag("Wall"))
        {
            return;
        }
        else if (hit != null && hit.CompareTag("BitCoin"))
        {
            Debug.Log("🪙 비트코인 획득!");
            hit.gameObject.SetActive(false);
        }

        IsMove = true;

        gridPosition = target;
        transform.position = end;

        IsMove = false;
    }

    private IEnumerator GridSmoothMovement(Vector2Int targetGrid)
    {
        Vector3 start = transform.position;
        Vector3 end = new Vector3(targetGrid.x, targetGrid.y, 0);

        // 충돌 검사
        Collider2D hit = Physics2D.OverlapPoint(end);
        if (hit != null && hit.CompareTag("Wall"))
        {
            yield break;
        }
        else if (hit != null && hit.CompareTag("BitCoin"))
        {
            Debug.Log("🪙 비트코인 획득!");
            hit.gameObject.SetActive(false);  // destroy 대신
        }

        IsMove = true;

        //// 너무 짧으면 그냥 순간 이동
        //if (moveTime <= 0.05f)
        //{
        //    transform.position = end;
        //    gridPosition = targetGrid;
        //    IsMove = false;
        //    yield break;
        //}

        float current = 0f;
        float percent = 0f;

        while (percent < 1f)
        {
            current += Time.deltaTime;
            percent = current / moveTime;
            transform.position = Vector3.Lerp(start, end, percent);
            yield return null;
        }

        gridPosition = targetGrid;
        transform.position = end;
        IsMove = false;

        yield return new WaitForSeconds(0.0001f); // 약간의 텀 (선택)
    }

    public void ResetToStart()
    {
        Debug.Log("🔁 플레이어 위치 초기화!");
        StopAllCoroutines();
        IsMove = false;

        gridPosition = initialGridPosition;
        transform.position = initialWorldPosition;
    }
}