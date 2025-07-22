using UnityEngine;
using UnityEngine.UI;

public class ToggleExample : MonoBehaviour
{
    [SerializeField] private Toggle myToggle;

    void Start()
    {
        // 토글 상태 변경 시 이벤트 등록
        myToggle.onValueChanged.AddListener(LoopMode);
    }

    public void LoopMode(bool isOn)
    {
        TextDataManager.Instance.isLoopOn = isOn;
    }
}