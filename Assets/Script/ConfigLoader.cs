using UnityEngine;

public static class ConfigLoader
{
    private static Config _config;

    public static Config LoadConfig()
    {
        if (_config != null) return _config;

        TextAsset configText = Resources.Load<TextAsset>("config");
        if (configText == null)
        {
            Debug.LogError("❌ config.json을 Resources 폴더에서 찾을 수 없습니다!");
            return null;
        }

        _config = JsonUtility.FromJson<Config>(configText.text);
        return _config;
    }

    public static string GetBaseUrl()
    {
        return LoadConfig()?.API_BASE_URL;
    }
}