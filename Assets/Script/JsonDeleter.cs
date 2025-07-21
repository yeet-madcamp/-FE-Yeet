using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public class JsonDeleter : MonoBehaviour
{
    MapListLoader mapList;

    private void Awake()
    {
        mapList = GetComponent<MapListLoader>();
    }

    public void DeleteCurrentMap()
    {
        string mapName = TextDataManager.Instance.mapName;

        // 로컬 파일 삭제
        DeleteMapJson(mapName);

        // 서버에서도 삭제 요청
        string mapId = TextDataManager.Instance.mapId;
        if (!string.IsNullOrEmpty(mapId))
        {
            StartCoroutine(DeleteMapFromServer(mapId));
        }
        else
        {
            Debug.LogWarning("🟡 mapId가 비어 있어서 서버 삭제는 생략합니다.");
        }

        // 리스트 새로고침 등 후처리 가능
        StartCoroutine(LoadMapListFromServer());
    }
    IEnumerator LoadMapListFromServer()
    {
        string url = ConfigLoader.GetBaseUrl() + "/maps";
        Debug.Log("요청 주소: " + url);

        UnityWebRequest request = UnityWebRequest.Get(url);
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("❌ 맵 목록 로딩 실패: " + request.error);
            yield break;
        }

        string rawJson = request.downloadHandler.text;
        Debug.Log("📦 받아온 JSON: " + rawJson);

        MapDataList dataList = JsonUtility.FromJson<MapDataList>(rawJson);
        Debug.Log($"✅ 맵 개수: {dataList.maps.Count}");

        mapList.PopulateMapButtons(dataList.maps);
    }

    private void DeleteMapJson(string mapName)
    {
        string path = Path.Combine(Application.persistentDataPath, mapName + ".json");

        if (File.Exists(path))
        {
            File.Delete(path);
            Debug.Log($"🗑️ 로컬 파일 삭제 완료: {path}");
        }
        else
        {
            Debug.LogWarning($"❌ 삭제 실패 - 파일 없음: {path}");
        }
    }

    private IEnumerator DeleteMapFromServer(string mapId)
    {
        string baseUrl = ConfigLoader.GetBaseUrl();
        string url = $"{baseUrl}/maps/jsewo9119/{mapId}";
        Debug.Log($"🌐 서버 삭제 요청 URL: {url}");

        UnityWebRequest request = UnityWebRequest.Delete(url);
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError($"❌ 서버 삭제 실패: {request.error}");
        }
        else
        {
            Debug.Log("✅ 서버에서 맵 삭제 완료");
        }
    }
}