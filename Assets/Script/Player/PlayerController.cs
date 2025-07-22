using System.Collections;
using UnityEngine;
using System;

public class PlayerController : MonoBehaviour
{
    private Movement2D movement2D;
    private Vector3 startPosition;

    private void Awake()
    {
        movement2D = GetComponent<Movement2D>();
    }

    private void Start()
    {
        startPosition = transform.position;
    }

    public bool IsMoving => movement2D.IsMove;

    public void ResetPosition()
    {
        movement2D.ResetToStart();
    }

    // ✅ 외부에서 절대 위치 이동 요청
    public void MoveToPosition(Vector2 target, Action onComplete)
    {
        StartCoroutine(MoveRoutine(target, onComplete));
    }

    private IEnumerator MoveRoutine(Vector2 target, Action onComplete)
    {
        movement2D.MoveTo(target);  // Movement2D에 정의된 함수

        while (movement2D.IsMove)
            yield return null;

        onComplete?.Invoke();
    }

    // ✅ 수동 테스트용 방향키 입력 (필요 없으면 제거해도 됨)
    private void Update()
    {
        if (movement2D.IsMove) return;

        Vector3 dir = Vector3.zero;

        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
            dir = Vector3.up;
        else if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
            dir = Vector3.down;
        else if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
            dir = Vector3.left;
        else if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
            dir = Vector3.right;

        if (dir != Vector3.zero)
        {
            Vector2 nextPos = transform.position + dir;
            MoveToPosition(nextPos, null);
        }
    }
}