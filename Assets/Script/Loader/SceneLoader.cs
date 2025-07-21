using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    [SerializeField] TextEditor textEditor;
    public void LoadSandboxScene()
    {
        string currentInput = textEditor.inputField.text;
        textEditor.OnEditComplete(currentInput);
        Debug.Log("mapName scene change: "+ TextDataManager.Instance.mapName);
        SceneManager.LoadScene("SandBoxScene");
    }
    public void LoadSavedScene()
    {
        // mapName은 MapListLoader.cs에서 설정됨 
        SceneManager.LoadScene("SandBoxScene");
    }
    public void LoadMainScene()
    {
        SceneManager.LoadScene("MainScene");
    }
}
