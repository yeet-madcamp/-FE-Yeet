using UnityEngine;

public class TextDataManager : MonoBehaviour
{
    public static TextDataManager Instance; // ✅ 싱글톤 인스턴스

    [HideInInspector] public string mapName = "grid_data"; // 저장 또는 불러올 맵 이름
    [HideInInspector] public string mapId = "";             // ✅ 서버에서 받아온 map_id 저장용
    [HideInInspector] public string modelId = "";
    [HideInInspector] public bool isLoopOn = false;


    void Awake()
    {
        // 싱글톤 인스턴스 중복 방지
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 씬 전환 시 파괴되지 않게 유지
        }
        else
        {
            Destroy(gameObject); // 이미 존재하면 중복 제거
        }
    }
}