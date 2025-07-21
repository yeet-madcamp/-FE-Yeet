using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    [SerializeField] private GameObject tilePrefab;
    [SerializeField] private Transform gridParent;
    [SerializeField] private EditController editController;

    public int rows = 5;
    public int columns = 5;

    private GameObject[,] gridTiles;
    
    private void Start()
    {
        GenerateGrid(); // 시작 시 자동 생성
    }

    public void GenerateGrid()
    {
        int minX = -columns / 2, maxX = columns / 2+1;
        int minY = -rows / 2, maxY = rows / 2+1;
        // 범위 밖의 벽 제거
        GameObject[] walls = GameObject.FindGameObjectsWithTag("Wall");
        foreach (GameObject wall in walls)
        {
            Vector3 pos = wall.transform.position;

            // 벽이 범위를 벗어났다면 제거
            if (pos.x <  minX || pos.x >= maxX || pos.y < minY || pos.y >= maxY)
            {
                Destroy(wall);
            }
        }
        //범위 밖의 코인 제거
        GameObject[] coins = GameObject.FindGameObjectsWithTag("BitCoin");
        foreach (GameObject coin in coins)
        {
            Vector3 pos = coin.transform.position;

            // 벽이 범위를 벗어났다면 제거
            if (pos.x < minX || pos.x >= maxX || pos.y < minY || pos.y >= maxY)
            {
                Destroy(coin);
            }
        }

        // 기존 타일 삭제
        if (gridTiles != null)
        {
            foreach (var tile in gridTiles)
            {
                if (tile != null) Destroy(tile);
            }
        }

        gridTiles = new GameObject[rows, columns];
        int offsetX = columns / 2;
        int offsetY = rows / 2;
        gridParent.position = new Vector3(-offsetX, -offsetY, 0);

        for (int y = 0; y < rows; y++)
        {
            for (int x = 0; x < columns; x++)
            {
                Vector3 localPos = new Vector3(x, y, 0);
                Vector3 worldPos = gridParent.TransformPoint(localPos);

                GameObject tile = Instantiate(tilePrefab, worldPos, Quaternion.identity, gridParent);
                gridTiles[y, x] = tile;
            }
        }
    }

    // UI 버튼에서 호출할 수 있는 메서드
    public void SetGridSize(int newRows, int newCols)
    {
        rows = newRows;
        columns = newCols;
        GenerateGrid();
    }

    public void LoadMap()
    {
        // 이전 타일 제거
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        int offsetX = columns / 2;
        int offsetY = rows / 2;
        transform.position = new Vector3(-offsetX, -offsetY, 0);

        for (int y = 0; y < rows; y++)
        {
            for (int x = 0; x < columns; x++)
            {
                Vector3 localPos = new Vector3(x, y, 0);
                Vector3 worldPos = transform.TransformPoint(localPos);
                Instantiate(tilePrefab, worldPos, Quaternion.identity, transform);
            }
        }
    }
    public void OnClickSizeSelect(int mapSize)
    {
        rows = mapSize;
        columns = mapSize;
        editController.minX = -columns / 2;
        editController.maxX = columns / 2;
        editController.minY = -rows / 2;
        editController.maxY = rows / 2;
        GenerateGrid();
    }

}