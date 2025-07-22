using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System;

[System.Serializable]
public class UserRequest
{
    public string username;
    public string id;
}

[System.Serializable]
public class UserResponse
{
    public string id;
    public string username;
    public string user_id;
}

public class LoginLogic : MonoBehaviour
{
    public static LoginLogic Instance;



    SceneLoader sceneLoader;

    private void Awake()
    {
        sceneLoader = GetComponent<SceneLoader>();

        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }
    public void OnClickLogin()
    {
        StartCoroutine(TryLoginOrCreate());
    }

    private IEnumerator TryLoginOrCreate()
    {
        string id = TextDataManager.Instance.enteredId;
        string username = TextDataManager.Instance.enteredUsername;

        // 1단계: GET으로 유저 조회
        yield return GetUser(id,
            onSuccess: (user) =>
            {
                if (user.username != username)
                {
                    Debug.LogWarning($"❌ ID는 존재하지만 이름이 다릅니다. 입력된 이름: {username}, 등록된 이름: {user.username}");
                    // 여기서 UI 피드백도 가능
                    return;
                }

                Debug.Log($"✅ 기존 유저 로그인 성공: {user.username}");
                PlayerPrefs.SetString("user_id", user.id); ;
                TextDataManager.Instance.userId = user.user_id;
                sceneLoader.LoadMainSceneInLogin();
                Debug.Log("userId = " + TextDataManager.Instance.userId);
            },
            onError: (err) =>
            {
                Debug.Log("❌ 기존 유저 없음, 새로 생성 시도");

                // 2단계: POST로 새 유저 생성
                StartCoroutine(Login(id, username,
                    onSuccess: (newUser) =>
                    {
                        Debug.Log($"🆕 새 유저 생성 완료: {newUser.username}");
                        PlayerPrefs.SetString("user_id", newUser.id);
                        TextDataManager.Instance.userId = newUser.user_id;
                        sceneLoader.LoadMainSceneInLogin();
                        Debug.Log("userId = " + TextDataManager.Instance.userId);
                    },
                    onError: (postErr) =>
                    {
                        Debug.LogWarning("❌ 다시시도 하세요. " + postErr);
                    }));
            });
    }
    public IEnumerator GetUser(string userId, System.Action<UserResponse> onSuccess, System.Action<string> onError)
    {
        string baseUrl = ConfigLoader.GetBaseUrl();
        string url = $"{baseUrl}/user/users/id/{userId}";

        UnityWebRequest request = UnityWebRequest.Get(url);
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            var user = JsonUtility.FromJson<UserResponse>(request.downloadHandler.text);
            onSuccess?.Invoke(user);
        }
        else
        {
            Debug.LogWarning($"유저 조회 실패: {request.responseCode} - {request.downloadHandler.text}");
            onError?.Invoke(request.downloadHandler.text);
        }
    }
    private IEnumerator Login(string id, string username, Action<UserResponse> onSuccess, Action<string> onError)
    {
        string baseUrl = ConfigLoader.GetBaseUrl();
        string url = $"{baseUrl}/user/users";

        UserRequest requestData = new UserRequest { id = id, username = username };
        string jsonData = JsonUtility.ToJson(requestData);

        UnityWebRequest request = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            var user = JsonUtility.FromJson<UserResponse>(request.downloadHandler.text);
            onSuccess?.Invoke(user);
        }
        else
        {
            onError?.Invoke(request.downloadHandler.text);
        }
    }
}