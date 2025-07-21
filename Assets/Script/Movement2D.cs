using System.Collections;
using UnityEngine;

public class Movement2D : MonoBehaviour
{
	[SerializeField]
	private	float	moveTime = 0.5f;
    private int minX = 0;
    private int maxX = 6;
    private int minY = 0;
    private int maxY = 6;

    public	Vector3	MoveDirection	{ set; get; } = Vector3.zero;	// �̵� ����
	public	bool	IsMove			{ set; get; } = false;          // ���� �̵�������
    private GridManager gridManager;

    private Vector2Int gridPosition;

    private void Awake()
    {
        gridManager = GameObject.Find("GameManager").GetComponent<GridManager>();
    }

    private void Start()
    {
        // 현재 위치를 가장 가까운 정수 그리드로 스냅
        gridPosition = Vector2Int.RoundToInt(transform.position);
        transform.position = new Vector3(gridPosition.x, gridPosition.y, 0);
        int offsetC = gridManager.columns / 2;
        int offsetR = gridManager.rows / 2;
        maxX = offsetC;
        maxY = offsetR;
        minX = -offsetC;
        minY = -offsetR;

        StartCoroutine(MoveLoop());
    }

    private IEnumerator MoveLoop()
	{
		while ( true )
		{
			if ( MoveDirection != Vector3.zero && IsMove == false )
			{
                Vector2Int delta = new Vector2Int((int)MoveDirection.x, (int)MoveDirection.y);
                Vector2Int targetGrid = gridPosition + delta;

                if (targetGrid.x < minX || targetGrid.x > maxX || targetGrid.y < minY || targetGrid.y > maxY)
                {
                    // 바운더리 바깥이면 이동하지 않음
                    yield return null;
                    continue;
                }

                
                // 벽이 있으면 움직이지 벽쪽으로 움직이지 못
                Vector3 worldPos = new Vector3(targetGrid.x, targetGrid.y, 0);
                Collider2D hit = Physics2D.OverlapPoint(worldPos);
                if (hit != null && hit.CompareTag("Wall"))
                {
                    yield return null;
                    continue;
                }
                else if (hit != null && hit.CompareTag("BitCoin"))
                {
                    // 코인 수집
                    Debug.Log("🪙 비트코인 획득!");
                    Destroy(hit.gameObject);
                    // 필요시 점수 증가 등 추가 처리
                }

                yield return StartCoroutine(GridSmoothMovement(targetGrid));
            }

			yield return null;
		}
	}

	private IEnumerator GridSmoothMovement(Vector2Int targetGrid)
	{
		Vector3 start = transform.position;
        Vector3 end = new Vector3(targetGrid.x, targetGrid.y, 0);

        float	current = 0;
		float	percent = 0;

		IsMove = true;

		while ( percent < 1 )
		{
			current += Time.deltaTime;
			percent = current / moveTime;

			transform.position = Vector3.Lerp(start, end, percent);

			yield return null;
		}

        // 이동 완료 후 위치 고정
        gridPosition = targetGrid;
        transform.position = end;

        IsMove = false;

        // 원하면 멈춤 시간도 여기서 추가
        yield return new WaitForSeconds(0.5f);
    }
}

