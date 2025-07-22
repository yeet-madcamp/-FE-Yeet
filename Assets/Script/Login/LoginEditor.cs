using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LoginEditor : MonoBehaviour
{
    public TextMeshProUGUI displayText;          // 현재 텍스트 보여주는 Text
    public TMP_InputField inputField;     // 수정할 수 있는 InputField

    string loginType;

    // Start is called before the first frame update
    void Start()
    {
        inputField.gameObject.SetActive(false); // 처음엔 숨겨둠
        inputField.onSubmit.AddListener(OnLoginComplete);
    }

    // 버튼 클릭 시 호출
    public void OnLoginButtonClick(string type)
    {
        inputField.text = displayText.text;
        displayText.gameObject.SetActive(false);
        inputField.gameObject.SetActive(true);
        inputField.Select();
        inputField.ActivateInputField();
        loginType = type;
    }

    // 저장하거나 포커스 잃었을 때 호출
    // 엔터 입력 후 자동 호출됨 (OnEndEdit 이벤트로 연결)
    public void OnLoginComplete(string newText)
    {
        displayText.text = newText;
        displayText.gameObject.SetActive(true);
        inputField.gameObject.SetActive(false);

        ProcessText(newText);
    }

    // 텍스트 처리용 메서드 맵에만 적용
    void ProcessText(string newText)
    {
        if(loginType == "id")
        {
            TextDataManager.Instance.enteredId = newText;
        }
        else if(loginType == "name")
        {
            TextDataManager.Instance.enteredUsername = newText;
        }
    }
}
