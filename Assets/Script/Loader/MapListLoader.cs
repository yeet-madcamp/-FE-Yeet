using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using static UnityEngine.ParticleSystem;

[System.Serializable]
public class MapDataList
{
    public int type;
    public string user_id;
    public List<MapData> maps;
}

public class MapListLoader : MonoBehaviour
{
    private enum ScenePosition { main, sandbox}

    [SerializeField] private ScenePosition currentPos;
    [SerializeField] private MainMode mainMode;
    [SerializeField] private Transform contentParent;        // Content 오브젝트
    [SerializeField] private GameObject itemPrefab;          // 생성할 버튼/패널 프리팹
    [SerializeField] private GameObject savedModelPanel;
    [SerializeField] private MapLoader mapLoader;

    void Start()
    {
        //ListMapItems();
        StartCoroutine(LoadMapListFromServer());
    }

    private void ClearExistingMapItems(string excludeName)
    {
        foreach (Transform child in contentParent)
        {
            
            if (child.name == excludeName)
            {
                continue; // 삭제 제외
            }

            Destroy(child.gameObject); // 삭제
        }
    }
    //서버 연결
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

        PopulateMapButtons(dataList.maps);
    }
    public void PopulateMapButtons(List<MapData> maps)
    {
        ClearExistingMapItems("NewMapBtn");

        foreach (var map in maps)
        {
            GameObject item = Instantiate(itemPrefab, contentParent);

            var textComp = item.GetComponentInChildren<TMPro.TextMeshProUGUI>();
            if (textComp != null)
                textComp.text = map.map_name;

            var button = item.GetComponent<Button>();
            if (button != null)
            {
                string selectedName = map.map_name;

                if (currentPos == ScenePosition.sandbox)
                {
                    button.onClick.AddListener(() =>
                    {
                        Debug.Log("🧲 버튼 클릭됨: " + selectedName);
                        mapLoader.LoadGridFromData(map); // 👈 서버에서 받은 데이터 직접 사용
                    });
                }
                else if (currentPos == ScenePosition.main)
                {
                    if(mainMode == MainMode.edit)
                    {
                        button.onClick.AddListener(() =>
                        {
                            TextDataManager.Instance.mapName = selectedName;
                            TextDataManager.Instance.mapId = map.map_id;
                            savedModelPanel.SetActive(true);
                            savedModelPanel.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = selectedName;
                        });
                    }
                    else if (mainMode == MainMode.train)
                    {
                        button.onClick.AddListener(() =>
                        {
                            TextDataManager.Instance.mapName = selectedName;
                            TextDataManager.Instance.mapId = map.map_id;
                            Debug.Log("mapName =" + TextDataManager.Instance.mapName);
                            Debug.Log("mapId =" + TextDataManager.Instance.mapId);
                            OnMapSelected(button);
                        });
                        
                    }
                }
            }
        }
    }
    private Button selectedMapButton = null;
    private Image selectedMapImage = null;

    // 📌 Map 버튼 클릭 시
    void OnMapSelected(Button currentButton)
    {
        if (selectedMapImage != null)
        {
            selectedMapImage.color = Color.gray;
        }

        // 현재 버튼 색 설정
        Image img = currentButton.GetComponent<Image>();
        if (img != null)
        {
            img.color = Color.yellow;
            selectedMapImage = img;
        }

        selectedMapButton = currentButton;
    }



    //public void ListMapItems()
    //{
    //    string folderPath = Application.persistentDataPath;
    //    string[] files = Directory.GetFiles(folderPath, "*.json");

    //    ClearExistingMapItems("NewMapBtn");

    //    foreach (string filePath in files)
    //    {
    //        string fileName = Path.GetFileNameWithoutExtension(filePath);

    //        // 프리팹 생성
    //        GameObject item = Instantiate(itemPrefab, contentParent);

    //        // 표시 텍스트가 있다면 설정 (예: TextMeshProUGUI 사용 시)
    //        var textComp = item.GetComponentInChildren<TMPro.TextMeshProUGUI>();
    //        if (textComp != null)
    //            textComp.text = fileName;

    //        // 클릭 이벤트 추가하고 싶다면 ↓
    //        var button = item.GetComponent<Button>();
    //        if (button != null)
    //        {
    //            if (currentPos == ScenePosition.sandbox)
    //            {
    //                string selectedName = fileName; // 클로저 보호
    //                button.onClick.AddListener(() =>
    //                {
    //                    Debug.Log("🧲 버튼 클릭됨: " + selectedName);
    //                    mapLoader.LoadGridFromJson(selectedName); // ✅ 여기서 호출
    //                });
    //            }
    //            else if (currentPos == ScenePosition.main)
    //            {
    //                button.onClick.AddListener(() =>
    //                {
    //                    TextDataManager.Instance.mapName = fileName;
    //                    savedModelPanel.SetActive(true);
    //                    savedModelPanel.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = fileName;

    //                });


    //            }

    //        }
    //    }
    //}

}