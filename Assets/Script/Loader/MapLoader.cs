using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
#if UNITY_EDI
using static UnityEditor.PlayerSettings;
#endif

public class MapLoader : MonoBehaviour
{
    [SerializeField] private GridManager gridManager;     // 그리드 생성기
    [SerializeField] private Movement2D movement2D;

    [SerializeField] GameObject[] editPrefabs;
    [SerializeField] Transform[] editParents;
    [SerializeField] Transform wallTopParent;
    [SerializeField] GameObject wallTopPrefab;

    private void Start()
    {
        string mapName = TextDataManager.Instance.mapName;
        StartCoroutine(LoadMapFromServer(mapName));
    }

    //서버에서 불러오기
    private IEnumerator LoadMapFromServer(string mapName)
    {
        string baseUrl = ConfigLoader.GetBaseUrl();
        string userId = TextDataManager.Instance.userId;
        string url = $"{baseUrl}/maps/user/{userId}";
        Debug.Log($"🌐 요청 URL: {url}");

        UnityWebRequest request = UnityWebRequest.Get(url);
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError($"❌ 맵 로딩 실패: {request.error}");
            yield break;
        }

        string rawJson = request.downloadHandler.text;
        MapDataList dataList = JsonUtility.FromJson<MapDataList>(rawJson);

        MapData targetMap = dataList.maps.Find(map => map.map_name == mapName);

        if (targetMap == null)
        {
            Debug.LogWarning($"🟡 해당 이름의 맵이 존재하지 않습니다. 새 맵으로 간주합니다: {mapName}");
            TextDataManager.Instance.mapId = null;

            // 초기 위치를 (0, 0)으로 설정
            Vector2Int initialPosition = new Vector2Int(0, 0);
            movement2D.SetInitialPosition(initialPosition);

            yield break;
        }

        Vector2Int loadedPosition = new Vector2Int(targetMap.agent_pos[0], targetMap.agent_pos[1]);
        movement2D.SetInitialPosition(loadedPosition);

        LoadGridFromData(targetMap);
    }

    public void LoadGridFromData(MapData data)
    {
        Debug.Log($"📦 맵 크기: {data.map_size[0]}x{data.map_size[1]}");

        // 기존 오브젝트 삭제
        foreach (Transform child in editParents[0]) Destroy(child.gameObject);
        foreach (Transform child in editParents[1]) Destroy(child.gameObject);
        foreach (Transform child in editParents[2]) Destroy(child.gameObject);

        GameObject oldExit = GameObject.FindGameObjectWithTag("Exit");
        if (oldExit != null) Destroy(oldExit);

        // 맵 크기 설정
        gridManager.columns = data.map_size[0];
        gridManager.rows = data.map_size[1];
        gridManager.GenerateGrid();

        // 오브젝트 생성
        foreach (Vector2Int pos in data.wall_list)
        {
            Vector3 wallPos = new Vector3(pos.x, pos.y, 0f);
            Vector3 wallTopPos = new Vector3(pos.x, pos.y + 0.4f, 0f);
            Instantiate(editPrefabs[0], wallPos, Quaternion.identity, editParents[0]);
            Instantiate(wallTopPrefab, wallTopPos, Quaternion.identity, wallTopParent);

        }

        foreach (Vector2Int pos in data.bit_list)
            Instantiate(editPrefabs[1], new Vector3(pos.x, pos.y, 0), Quaternion.identity, editParents[1]);

        foreach (Vector2Int pos in data.trap_list)
            Instantiate(editPrefabs[2], new Vector3(pos.x, pos.y, 0), Quaternion.identity, editParents[2]);

        Instantiate(editPrefabs[3], new Vector3(data.exit_pos.x, data.exit_pos.y, 0), Quaternion.identity);

        Debug.Log("✅ 맵 서버에서 불러오기 완료!");
    }

    //public void LoadGridFromJson(string mapName)
    //{
    //    // 경로 설정
    //    string path = Path.Combine(Application.dataPath, mapName+".json");

    //    if (!File.Exists(path))
    //    {
    //        Debug.LogError("⛔ 저장된 grid_data.json 파일이 없습니다!");
    //        return;
    //    }

    //    string json = File.ReadAllText(path);
    //    MapData data = JsonUtility.FromJson<MapData>(json);
    //    GameObject exit = GameObject.FindGameObjectWithTag("Exit");

    //    Debug.Log($"📦 불러온 맵 크기: {data.map_size[0]}x{data.map_size[1]}");
    //    Debug.Log($"📦 불러온 벽 개수: {data.wall_list.Count}");

    //    // 기존 벽 제거
    //    foreach (Transform child in editParents[0])
    //    {
    //        Destroy(child.gameObject);
    //    }
    //    foreach (Transform child in editParents[1])
    //    {
    //        Destroy(child.gameObject);
    //    }
    //    foreach (Transform child in editParents[2])
    //    {
    //        Destroy(child.gameObject);
    //    }
    //    Destroy(exit);


    //    // 맵 크기 설정
    //    gridManager.columns = data.map_size[0];
    //    gridManager.rows = data.map_size[1];

    //    // 그리드 재생성
    //    gridManager.GenerateGrid(); // 이 메서드는 GridManager에서 그리드 생성하는 함수

    //    // 벽 다시 생성
    //    foreach (Vector2Int pos in data.wall_list)
    //    {
    //        Vector3 wallPos = new Vector3(pos.x, pos.y, 0f);
    //        Vector3 wallTopPos = new Vector3(pos.x, pos.y+0.4f, 0f);
    //        Instantiate(editPrefabs[0], wallPos, Quaternion.identity, editParents[0]);
    //        Instantiate(wallTopPrefab, wallTopPos, Quaternion.identity, wallTopParent);
    //    }
    //    // 코인 다시 생성
    //    foreach (Vector2Int pos in data.bit_list)
    //    {
    //        Vector3 bitPos = new Vector3(pos.x, pos.y, 0f);
    //        Instantiate(editPrefabs[1], bitPos, Quaternion.identity, editParents[1]);
    //    }
    //    // 함정 다시 생성
    //    foreach (Vector2Int pos in data.trap_list)
    //    {
    //        Vector3 trapPos = new Vector3(pos.x, pos.y, 0f);
    //        Instantiate(editPrefabs[2], trapPos, Quaternion.identity, editParents[2]);
    //    }
    //    // 출구 다시 생성
    //    Vector3 exitPos = new Vector3(data.exit_pos.x, data.exit_pos.y, 0f);
    //    Instantiate(editPrefabs[3], exitPos, Quaternion.identity);

    //    Debug.Log("✅ 맵 불러오기 완료!");
    //}
}