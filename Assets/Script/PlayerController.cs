using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
	private	Movement2D	movement2D;

    private void Awake()
    {
        movement2D = GetComponent<Movement2D>();
    }

    private void Start()
    {
        //StartCoroutine(ChangeDirectionRoutine());
    }

    private void Update()
    {
        if (movement2D.IsMove) return; // 이동 중이면 입력 무시

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
            movement2D.MoveDirection = dir;
        }
    }

    private IEnumerator ChangeDirectionRoutine()
    {
        while (true)
        {
            Vector3 dir = Vector3.zero;

            int axis = Random.Range(0, 2); // 0이면 x축, 1이면 y축 (임시 랜덤)

            if (axis == 0)
            {
                dir = new Vector3(Random.Range(0, 2) * 2 - 1, 0, 0); // (-1, 0) 또는 (1, 0)
            }
            else
            {
                dir = new Vector3(0, Random.Range(0, 2) * 2 - 1, 0); // (0, -1) 또는 (0, 1)
            }

            movement2D.MoveDirection = dir; // dir을 backend에서 가져온 값을 넣으면 될듯?

            yield return new WaitForSeconds(1f);
        }
    }
}

