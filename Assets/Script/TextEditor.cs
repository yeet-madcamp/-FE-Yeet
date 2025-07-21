using UnityEngine;
using UnityEngine.UI; // TMP 쓰는 경우: using TMPro;
using TMPro;

public class TextEditor : MonoBehaviour
{
    public TextMeshProUGUI displayText;          // 현재 텍스트 보여주는 Text
    public TMP_InputField inputField;     // 수정할 수 있는 InputField

    void Start()
    {
        inputField.gameObject.SetActive(false); // 처음엔 숨겨둠
        inputField.onSubmit.AddListener(OnEditComplete);
    }

    // 버튼 클릭 시 호출
    public void OnEditButtonClick()
    {
        inputField.text = displayText.text;
        displayText.gameObject.SetActive(false);
        inputField.gameObject.SetActive(true);
        inputField.Select();
        inputField.ActivateInputField();
    }

    // 저장하거나 포커스 잃었을 때 호출
    // 엔터 입력 후 자동 호출됨 (OnEndEdit 이벤트로 연결)
    public void OnEditComplete(string newText)
    {
        displayText.text = newText;
        displayText.gameObject.SetActive(true);
        inputField.gameObject.SetActive(false);

        ProcessText(newText);
    }

    // 텍스트 처리용 메서드 맵에만 적용
    void ProcessText(string newText)
    {
        Debug.Log("newText1: " + newText);
        TextDataManager.Instance.mapName = newText;
        Debug.Log("newText2: " + TextDataManager.Instance.mapName);
    }
}