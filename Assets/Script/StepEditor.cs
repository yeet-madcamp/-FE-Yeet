using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StepEditor : MonoBehaviour
{
    public TextMeshProUGUI displayText;          // 현재 텍스트 보여주는 Text
    public TMP_InputField inputField;     // 수정할 수 있는 InputField

    void Start()
    {
        inputField.gameObject.SetActive(false); // 처음엔 숨겨둠
        inputField.onSubmit.AddListener(OnStepComplete);
    }

    public void OnSetpButtonClick()
    {
        inputField.text = displayText.text;
        displayText.gameObject.SetActive(false);
        inputField.gameObject.SetActive(true);
        inputField.Select();
        inputField.ActivateInputField();
    }
    public void OnStepComplete(string newText)
    {
        displayText.text = newText.ToString();
        displayText.gameObject.SetActive(true);
        inputField.gameObject.SetActive(false);
        ProcessStep(newText);
    }
    void ProcessStep(string newText)
    {
        Debug.Log("newText1: " + newText);
        TextDataManager.Instance.mapStep = int.Parse(newText);
        Debug.Log("newText2: " + TextDataManager.Instance.mapStep);
    }
}
