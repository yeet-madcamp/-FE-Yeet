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
    public string model_color;
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

    public void LoadModelsFormServerCall()
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
        Debug.Log($"📥 서버 응답 이전 ");
        yield return request.SendWebRequest();
        string rawJson = request.downloadHandler.text;
        Debug.Log($"📥 서버 응답 원문:\n{rawJson}");

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("❌ 모델 목록 로딩 실패: " + request.error);
        }
        else
        {
            

            ModelDataList modelList = JsonUtility.FromJson<ModelDataList>(rawJson);

            if (modelList == null)
            {
                Debug.LogError("❌ modelList 파싱 실패");
            }
            else if (modelList.models == null)
            {
                Debug.LogError("❌ modelList.models == null (파싱 실패)");
            }
            else
            {
                Debug.Log($"✅ 모델 개수: {modelList.models.Count}");
            }
            

            foreach (ModelData model in modelList.models)
            {
                Debug.Log($"📦 모델 이름: {model.model_name}, ID: {model.model_id}, 타입: {model.model_type}");
                if(model.model_id == TextDataManager.Instance.modelId)
                {
                    if (model.model_color != null)
                    {
                        Color agentColor = HexToColor(model.model_color);

                        // Agent 오브젝트 찾아서 이미지 색상 적용
                        GameObject agent = GameObject.FindGameObjectWithTag("Player");
                        Debug.Log(agent ? $"🔍 찾은 오브젝트 이름: {agent.name}" : "❌ Player 태그 오브젝트 없음");
                        Debug.Log("들어보기 전");
                        if (agent != null)
                        {
                            Debug.Log("에이전트 있음 ");
                            var spriteRenderer = agent.GetComponent<SpriteRenderer>();

                            Debug.Log(spriteRenderer ? "✅ SpriteRenderer 있음" : "❌ SpriteRenderer 없음");
                            if (spriteRenderer != null)
                            {
                                spriteRenderer.color = agentColor;
                                foreach (var childRenderer in agent.GetComponentsInChildren<SpriteRenderer>())
                                {
                                    if (childRenderer != spriteRenderer) // 부모 본인은 제외
                                    {
                                        childRenderer.color = agentColor;
                                        Debug.Log($"🎨 자식 SpriteRenderer 색상 변경: {childRenderer.gameObject.name}");
                                    }
                                }
                            }

                        }
                    }
                }
            }
        }

    }
    Color HexToColor(string hex)
    {
        Debug.Log("컬러 전 ");
        if (!string.IsNullOrEmpty(hex))
        {
            Debug.Log($"컬러 값 원본: {hex}");
            Color color;
            if (ColorUtility.TryParseHtmlString(hex.StartsWith("#") ? hex : "#" + hex, out color))
            {
                Debug.Log($"✅ 파싱된 색상: {color}");
                return color;
            }
            else
            {
                Debug.LogError("❌ Color 파싱 실패");
            }
        }
        return Color.white;
    }
}