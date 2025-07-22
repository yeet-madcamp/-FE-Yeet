using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

[System.Serializable]
public class MapData
{
    public string map_id;
    public string map_name;
    public string map_type;
    public string map_owner_id;
    public string map_owner_name;
    public int[] map_size;
    public int max_steps;
    public Vector2Int agent_pos;
    public Vector2Int exit_pos;
    public List<Vector2Int> wall_list = new List<Vector2Int>();
    public List<Vector2Int> bit_list = new List<Vector2Int>();
    public List<Vector2Int> trap_list = new List<Vector2Int>();
}
[System.Serializable]
public class MapSaveResponse
{
    public string map_id;
    public string map_name;
    public string map_type;
    public string map_owner_id;
    public string map_owner_name;
    public int[] map_size;
    public Vector2Int agent_pos;
    public Vector2Int exit_pos;
    public List<Vector2Int> wall_list;
    public List<Vector2Int> bit_list;
    public List<Vector2Int> trap_list;
    public int max_steps;
    public string map_url;
    public int type;
}

public class MapSaver : MonoBehaviour
{
    [SerializeField] private GridManager gridManager;
    [SerializeField] private GameObject ERRORPanel;

    [SerializeField] private Camera mapCamera;

    public void SaveGridToJson()
    {
        MapData data = GenerateMapData();
        if (data == null)
        {
            Debug.LogWarning("❌ 맵 저장 실패: 필수 요소 누락");
            ERRORPanel.SetActive(true);
            return;
        }

        // 기존 map_id가 있다면 포함 (중요!)
        data.map_id = TextDataManager.Instance.mapId;  // 여기에 저장된 map_id 필요

        // 서버에도 저장 시도
        StartCoroutine(SaveGridToServer(data));
    }

    private MapData GenerateMapData()
    {
        MapData data = new MapData();
        data.map_size = new int[] { gridManager.columns, gridManager.rows };

        GameObject[] walls = GameObject.FindGameObjectsWithTag("Wall");
        GameObject[] coins = GameObject.FindGameObjectsWithTag("BitCoin");
        GameObject[] traps = GameObject.FindGameObjectsWithTag("Trap");
        GameObject exit = GameObject.FindGameObjectWithTag("Exit");
        GameObject agent = GameObject.FindGameObjectWithTag("Player");

        if (exit == null)
        {
            Debug.LogWarning("⚠️ 출구(Exit)가 맵에 없습니다! 저장을 중단합니다.");
            return null;
        }

        foreach (var wall in walls)
            data.wall_list.Add(Vector2Int.RoundToInt(wall.transform.position));
        foreach (var coin in coins)
            data.bit_list.Add(Vector2Int.RoundToInt(coin.transform.position));
        foreach (var trap in traps)
            data.trap_list.Add(Vector2Int.RoundToInt(trap.transform.position));

        data.map_name = TextDataManager.Instance.mapName;
        data.map_type = "grid";
        data.map_owner_id = TextDataManager.Instance.userId;
        data.map_owner_name = TextDataManager.Instance.enteredUsername;
        data.max_steps = TextDataManager.Instance.mapStep;
        data.agent_pos = Vector2Int.RoundToInt(agent.transform.position);
        data.exit_pos = Vector2Int.RoundToInt(exit.transform.position);

        return data;
    }

    public IEnumerator SaveGridToServer(MapData data)
    {
        //if (string.IsNullOrEmpty(data.map_id))
        //{
        //    Debug.LogError("❌ map_id가 없습니다. 서버 저장을 중단합니다.");
        //    yield break;
        //}

        string baseUrl = ConfigLoader.GetBaseUrl();
        string url = $"{baseUrl}/maps/{data.map_id}";
        Debug.Log("🌐 저장 요청 URL: " + url);


        string json = JsonUtility.ToJson(data);
        byte[] jsonBytes = System.Text.Encoding.UTF8.GetBytes(json);
        Debug.Log("🔍 요청 JSON:\n" + json);

        UnityWebRequest request = new UnityWebRequest(url, "POST");
        request.uploadHandler = new UploadHandlerRaw(jsonBytes);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("❌ 서버 저장 실패: " + request.error);
        }
        else
        {
            Debug.Log("✅ 서버 저장 성공!");
            // 🔽 응답 파싱
            string responseText = request.downloadHandler.text;
            MapSaveResponse response = JsonUtility.FromJson<MapSaveResponse>(responseText);

            Debug.Log("📥 받은 map_id: " + response.map_id);

            // 🔽 이미지 업로드
            SaveMapAndUploadImage(response);
        }
    }
    public Texture2D CaptureMapImage(Camera targetCamera, int width, int height, float centerRatio = 0.5f)
    {
        RenderTexture rt = new RenderTexture(width, height, 24);
        targetCamera.targetTexture = rt;
        targetCamera.Render();

        RenderTexture.active = rt;

        // 중심 기준 영역 계산
        int cropWidth = Mathf.RoundToInt(width * centerRatio);
        int cropHeight = Mathf.RoundToInt(height * centerRatio);
        int x = (width - cropWidth) / 2;
        int y = (height - cropHeight) / 2;

        // 중앙 부분만 읽기
        Texture2D cropped = new Texture2D(cropWidth, cropHeight, TextureFormat.RGB24, false);
        cropped.ReadPixels(new Rect(x, y, cropWidth, cropHeight), 0, 0);
        cropped.Apply();

        // 정리
        targetCamera.targetTexture = null;
        RenderTexture.active = null;
        Destroy(rt);

        return cropped;
    }
    public string SaveTextureToPNG(Texture2D texture, string fileName)
    {
        string path = Path.Combine(Application.persistentDataPath, fileName);
        byte[] bytes = texture.EncodeToPNG();
        File.WriteAllBytes(path, bytes);
        Debug.Log("📸 이미지 저장 경로: " + path);
        return path;
    }
    void SaveMapAndUploadImage(MapSaveResponse response)
    {
        // width=512, height=512에서 중앙 70% 영역만 캡처하고 싶을 때
        Texture2D mapImage = CaptureMapImage(mapCamera, 512, 512, 0.5f);
        string imagePath = SaveTextureToPNG(mapImage, $"{TextDataManager.Instance.mapId}.png");

        string mapId = response.map_id;
        StartCoroutine(FindObjectOfType<MapImageUploader>().UploadMapImage(mapId, imagePath));
    }
}