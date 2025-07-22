using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public class MapImageUploader : MonoBehaviour
{
    public IEnumerator UploadMapImage(string mapId, string imagePath)
    {
        if (!File.Exists(imagePath))
        {
            Debug.LogError("❌ 이미지 파일이 존재하지 않습니다: " + imagePath);
            yield break;
        }

        byte[] imageData = File.ReadAllBytes(imagePath);
        WWWForm form = new WWWForm();
        form.AddBinaryData("file", imageData, Path.GetFileName(imagePath), "image/png");

        string baseUrl = ConfigLoader.GetBaseUrl();
        string url = $"{baseUrl}/maps/upload-image/{mapId}";
        Debug.Log("🌐 이미지 업로드 URL: " + url);

        UnityWebRequest request = UnityWebRequest.Post(url, form);
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("❌ 이미지 업로드 실패: " + request.error);
        }
        else
        {
            Debug.Log("✅ 이미지 업로드 성공!");
        }
    }
}