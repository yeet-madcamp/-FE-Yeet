using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    [SerializeField] TextEditor textEditor;
    [SerializeField] WebSocketClient ws;
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
        TextDataManager.Instance.isLoopOn = false;
        ws.DisconnectWebSocket();
        SceneManager.LoadScene("MainScene");
    }
    public void LoadTrainingScene()
    {
        SceneManager.LoadScene("TrainingScene");
    }
    public void LoadMainSceneInLogin()
    {
        SceneManager.LoadScene("MainScene");
    }
    public void LoadMainSceneInSB()
    {
        TextDataManager.Instance.isLoopOn = false;
        SceneManager.LoadScene("MainScene");
    }
}
