using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Networking;
using System.Collections;

public class ModelSaver : MonoBehaviour
{
    public static ModelSaver Instance; // 싱글톤 (주의해서 사용)

    [Header("UI Elements")]
    [SerializeField] private TMP_InputField nameInput;
    [SerializeField] private TMP_InputField typeInput;
    [SerializeField] private TMP_InputField rateInput;
    [SerializeField] private TMP_InputField batchInput;
    [SerializeField] private GameObject newModelPanel;
    ModelListLoader modelListLoader;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);  // 중복 방지

        modelListLoader = GetComponent<ModelListLoader>();
    }

    public void OnClickSaveButton()
    {
        StartCoroutine(CreateModel());
    }

    public IEnumerator CreateModel()
    {
        // 입력값 받아오기
        string name = nameInput.text;
        string type = typeInput.text;
        float.TryParse(rateInput.text, out float rate);
        int.TryParse(batchInput.text, out int batch);

        // 유효성 검사
        if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(type))
        {
            Debug.LogWarning("❗ 이름/타입은 비워둘 수 없습니다.");
            yield break;
        }

        ModelData newModel = new ModelData
        {
            model_name = name,
            model_type = type,
            learning_rate = rate,
            batch_size = batch,
            model_owner_id = "jsewo9119",
            model_owner_name = "seowoo"
        };

        // 서버로 POST 전송
        string url = ConfigLoader.GetBaseUrl() + "/models";
        string json = JsonUtility.ToJson(newModel);
        byte[] jsonBytes = System.Text.Encoding.UTF8.GetBytes(json);

        UnityEngine.Networking.UnityWebRequest req = new UnityWebRequest(url, "POST");
        req.uploadHandler = new UnityEngine.Networking.UploadHandlerRaw(jsonBytes);
        req.downloadHandler = new UnityEngine.Networking.DownloadHandlerBuffer();
        req.SetRequestHeader("Content-Type", "application/json");

        yield return req.SendWebRequest();

        if (req.result == UnityEngine.Networking.UnityWebRequest.Result.Success)
        {
            Debug.Log("✅ 모델 생성 성공!");
            newModelPanel.SetActive(false);  // 패널 닫기
            modelListLoader.StartCoroutine(modelListLoader.LoadModelListFromServer());
        }
        else
        {
            Debug.LogError("❌ 모델 생성 실패: " + req.error);
        }
    }
}