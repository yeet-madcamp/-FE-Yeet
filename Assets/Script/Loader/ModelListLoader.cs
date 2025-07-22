using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public enum MainMode { edit, train};

public class ModelListLoader : MonoBehaviour
{
    [SerializeField] private Transform[] contentParents;       // 생성 위치
    [SerializeField] private GameObject itemPrefab;         // 버튼 프리팹
    [SerializeField] private GameObject modelInfoPanel;     // 선택한 모델 정보 표시용 패널

    [SerializeField] private TextEditor nameEditor;
    [SerializeField] private TextEditor typeEditor;
    [SerializeField] private TextEditor rateEditor;
    [SerializeField] private TextEditor batchEditor;

    [SerializeField] MainMode mainMode;

    private ModelData selectedModel;
    void Start()
    {
        StartCoroutine(LoadModelListFromServer());
    }

    public IEnumerator LoadModelListFromServer()
    {
        string baseUrl = ConfigLoader.GetBaseUrl();  // 예: http://yeetai.duckdns.org/api/backend
        string userId = TextDataManager.Instance.userId;
        string url = $"{baseUrl}/models/user/{userId}";
        Debug.Log("📡 모델 요청 주소: " + url);

        UnityWebRequest request = UnityWebRequest.Get(url);
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("❌ 모델 목록 로딩 실패: " + request.error);
            yield break;
        }

        string json = request.downloadHandler.text;
        Debug.Log("📦 받아온 모델 JSON: " + json);

        ModelDataList dataList = JsonUtility.FromJson<ModelDataList>(json);
        Debug.Log($"✅ 모델 개수: {dataList.models.Count}");

        PopulateModelButtons(dataList.models);
    }

    void PopulateModelButtons(List<ModelData> models)
    {
        ClearExistingMapItems("NewMapBtn");

        foreach (var model in models)
        {
            foreach(Transform parent in contentParents)
            {
                GameObject item = Instantiate(itemPrefab, parent);
                var textComp = item.GetComponentInChildren<TMPro.TextMeshProUGUI>();
                if (textComp != null)
                    textComp.text = model.model_name;

                var button = item.GetComponent<Button>();
                if (button != null)
                {
                    if (mainMode == MainMode.edit)
                    {
                        var capturedModel = model; // 🔒 캡처 중요
                        button.onClick.AddListener(() =>
                        {
                            Debug.Log($"🧠 선택된 모델: {capturedModel.model_name} (ID: {capturedModel.model_id})");
                            OnModelSelected(capturedModel);
                        });
                    }
                    else if (mainMode == MainMode.train)
                    {
                        var capturedModel = model;
                        button.onClick.AddListener(() =>
                        {
                            TextDataManager.Instance.modelId = model.model_id;
                            OnModelSelectedColor(button);
                        });

                    }
                }
            }
        }
    }
    private void ClearExistingMapItems(string excludeName)
    {
        foreach(Transform parent in contentParents)
        {
            foreach (Transform child in parent)
            {

                if (child.name == excludeName)
                {
                    continue; // 삭제 제외
                }

                Destroy(child.gameObject); // 삭제
            }
        }
        
    }
    void OnModelSelected(ModelData model)
    {
        selectedModel = model;

        nameEditor.displayText.text = model.model_name;
        typeEditor.displayText.text = model.model_type;
        rateEditor.displayText.text = model.learning_rate.ToString();
        batchEditor.displayText.text = model.batch_size.ToString();

        modelInfoPanel.SetActive(true);
    }
    public void SaveModelChanges()
    {
        if (selectedModel == null) return;

        selectedModel.model_name = nameEditor.displayText.text;
        selectedModel.model_type = typeEditor.displayText.text;
        selectedModel.learning_rate = float.Parse(rateEditor.displayText.text);
        selectedModel.batch_size = int.Parse(batchEditor.displayText.text);

        StartCoroutine(SaveModelToServer(selectedModel));
    }
    IEnumerator SaveModelToServer(ModelData model)
    {
        string url = ConfigLoader.GetBaseUrl() + "/models/" + model.model_id;
        Debug.Log("모델 저장 URL : " + url);
        string json = JsonUtility.ToJson(model);
        byte[] jsonBytes = System.Text.Encoding.UTF8.GetBytes(json);

        UnityWebRequest request = new UnityWebRequest(url, "POST"); // ✅ PUT 사용
        request.uploadHandler = new UploadHandlerRaw(jsonBytes);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("❌ 모델 저장 실패: " + request.error);
        }
        else
        {
            Debug.Log("✅ 모델 저장 성공!");
            StartCoroutine(LoadModelListFromServer());
        }
    }
    private Button selectedModelButton = null;
    private Image selectedModelImage = null;
    // 📌 Model 버튼 클릭 시
    void OnModelSelectedColor(Button currentButton)
    {
        if (selectedModelImage != null)
        {
            selectedModelImage.color = Color.gray;
        }

        // 현재 버튼 색 설정
        Image img = currentButton.GetComponent<Image>();
        if (img != null)
        {
            img.color = Color.yellow;
            selectedModelImage = img;
        }

        selectedModelButton = currentButton;
    }

}