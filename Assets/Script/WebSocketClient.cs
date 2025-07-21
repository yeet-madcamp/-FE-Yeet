using UnityEngine;
// ❗ NativeWebSocket만 사용 (System.Net.WebSockets 제거)
using NativeWebSocket;

public class WebSocketClient : MonoBehaviour
{
    // ❗ 명시적으로 NativeWebSocket 사용
    private NativeWebSocket.WebSocket websocket;

    async void Start()
    {
        websocket = new NativeWebSocket.WebSocket("ws://yeetai.duckdns.org/api/backend/ws");

        websocket.OnOpen += () =>
        {
            Debug.Log("✅ Connection open!");
        };

        websocket.OnError += (e) =>
        {
            Debug.Log("❌ Error: " + e);
        };

        websocket.OnClose += (e) =>
        {
            Debug.Log("❌ Connection closed!");
        };

        websocket.OnMessage += (bytes) =>
        {
            string message = System.Text.Encoding.UTF8.GetString(bytes);
            Debug.Log("📩 Received message: " + message);
        };

        await websocket.Connect();
    }

    void Update()
    {
#if !UNITY_WEBGL || UNITY_EDITOR
        websocket?.DispatchMessageQueue();
#endif
    }

    private async void OnApplicationQuit()
    {
        await websocket.Close();
    }

    // ❗ 함수 이름 변경
    public async void SendText(string msg)
    {
        if (websocket.State == WebSocketState.Open)
        {
            await websocket.SendText(msg);
        }
    }
}