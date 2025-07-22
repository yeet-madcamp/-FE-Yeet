using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[System.Serializable]
public class ModelData
{
    public string model_owner_id;
    public string model_owner_name;
    public string model_name;
    public string model_type;
    public float learning_rate;
    public int batch_size;
    public float gamma;
    public float epsilon_start;
    public float epsilon_min;
    public float epsilon_decay;
    public int update_target_every;
    public string model_id;
    public string model_url;
}

[System.Serializable]
public class ModelDataList
{
    public int type; // response의 type 필드
    public List<ModelData> models;
}

public class ModelLoader : MonoBehaviour
{
    public void Start()
    {
        StartCoroutine(LoadModelsFromServer());
    }

    IEnumerator LoadModelsFromServer()
    {
        string baseUrl = ConfigLoader.GetBaseUrl();  // 예: http://yeetai.duckdns.org/api/backend
        string userId = TextDataManager.Instance.userId;
        string url = $"{baseUrl}/models/user/{userId}";

        UnityWebRequest request = UnityWebRequest.Get(url);
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("❌ 모델 목록 로딩 실패: " + request.error);
        }
        else
        {
            string rawJson = request.downloadHandler.text;

            // JsonUtility는 배열 파싱이 까다로워서 wrapper 필요
            ModelDataList modelList = JsonUtility.FromJson<ModelDataList>(rawJson);

            foreach (var model in modelList.models)
            {
                Debug.Log($"📦 모델 이름: {model.model_name}, ID: {model.model_id}, 타입: {model.model_type}");
            }
        }
    }
}