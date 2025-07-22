using UnityEngine;
using NativeWebSocket;
using Newtonsoft.Json;
using System.Collections.Generic;
using TMPro;

public class WebSocketClient : MonoBehaviour
{
    [SerializeField] private PlayerController player;
    [SerializeField] private CoinManager coinManager;

    [SerializeField] private TextMeshProUGUI episodeText;
    [SerializeField] private TextMeshProUGUI stepText;
    [SerializeField] private TextMeshProUGUI successText;

    [SerializeField] private Queue<StepMessage> messageQueue = new Queue<StepMessage>();

    private WebSocket websocket;

    public bool isSpeedMode = false;

    string uri;

    async void Start()
    {
        string modelId = TextDataManager.Instance.modelId;
        string mapId = TextDataManager.Instance.mapId;
        try
        {
            if (!(TextDataManager.Instance.isLoopOn))
                uri = $"ws://yeetai.duckdns.org/api/backend/ws/train_dqn/{modelId}/{mapId}";
            else
                uri = $"ws://yeetai.duckdns.org/api/backend/ws/train_dqn/{modelId}/{mapId}/loop";

            Debug.Log("웹소켓 주소: " + uri);

            websocket = new WebSocket(uri);

            websocket.OnOpen += () =>
            {
                Debug.Log("✅ WebSocket 연결됨");
            };

            websocket.OnError += (e) =>
            {
                Debug.Log("❌ 에러: " + e);
            };

            websocket.OnClose += (WebSocketCloseCode closeCode) =>
            {
                Debug.Log($"❌ WebSocket 연결 종료됨 - 코드: {closeCode}");
            };

            websocket.OnMessage += (bytes) =>
            {
                string message = System.Text.Encoding.UTF8.GetString(bytes);
                Debug.Log("📩 메시지 수신: " + message);
                // 공통 메시지 파싱
                try
                {
                    var baseMsg = JsonConvert.DeserializeObject<BaseMessage>(message);

                    switch (baseMsg.eventType)
                    {
                        case "step":
                            var step = JsonConvert.DeserializeObject<StepMessage>(message);
                            HandleStep(step);
                            break;

                        case "model_loaded":
                            var loaded = JsonConvert.DeserializeObject<ModelPathMessage>(message);
                            Debug.Log("📦 모델 로드됨: " + loaded.model_url);
                            break;

                        case "model_saved":
                            var saved = JsonConvert.DeserializeObject<ModelPathMessage>(message);
                            Debug.Log("💾 모델 저장됨: " + saved.model_url);
                            break;

                        case "episode_success":
                            var success = JsonConvert.DeserializeObject<EpisodeSuccessMessage>(message);
                            Debug.Log($"🎉 에피소드 성공: {success.episode}, 총 보상: {success.total_reward}");
                            break;

                        default:
                            Debug.Log("⚠️ 알 수 없는 이벤트: " + baseMsg.eventType);
                            break;
                    }
                }
                catch (System.Exception ex)
                {
                    Debug.LogError("❌ 파싱 실패: " + ex.Message);
                }
            };

            await websocket.Connect();
        }
        catch (System.Exception ex)
        {
            Debug.LogError("❗ Start() 예외 발생: " + ex.Message);
        }
    }

    void Update()
    {
#if !UNITY_WEBGL || UNITY_EDITOR
        websocket?.DispatchMessageQueue();
#endif
        TryMoveNextStep();
    }

    private async void OnApplicationQuit()
    {
        try
        {
            if (websocket != null && websocket.State == WebSocketState.Open)
            {
                Debug.Log("🔌 앱 종료 시 WebSocket 닫기 시도 중...");
                await websocket.Close();
                Debug.Log("✅ WebSocket 정상 종료 완료");
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogWarning("⚠️ WebSocket 종료 중 예외 발생: " + ex.Message);
        }
    }

    public async void DisconnectWebSocket()
    {
        try
        {
            if (websocket != null && websocket.State == WebSocketState.Open)
            {
                await websocket.Close();
                Debug.Log("🔌 WebSocket 수동 종료 완료");
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogWarning("⚠️ WebSocket 수동 종료 중 예외 발생: " + ex.Message);
        }
    }

    public async void SendText(string msg)
    {
        if (websocket.State == WebSocketState.Open)
        {
            await websocket.SendText(msg);
        }
    }

    void HandleStep(StepMessage msg)
    {
        //Debug.Log($"[STEP] ep:{msg.episode} step:{msg.step} pos:[{msg.state[0]},{msg.state[1]}] reward:{msg.reward} loss:{msg.loss}");
        messageQueue.Enqueue(msg);

    }
    void TryMoveNextStep()
    {
        if (player.IsMoving) return;
        if (messageQueue.Count == 0) return;

        StepMessage msg = messageQueue.Dequeue();
        // UI 텍스트 업데이트
        if (episodeText != null)
            episodeText.text = $"Episode: {msg.episode}";
        if (stepText != null)
            stepText.text = $"Step: {msg.step}";
        if (TextDataManager.Instance.isLoopOn)
            successText.text = $"Success : {msg.success}";

        if (msg.terminated || msg.truncated)
        {
            player.ResetPosition();
            coinManager.ResetCoins();
            Debug.Log("재시작!");
            Debug.Log("로그!: " + msg.step);
            return;
        }

        Vector2 targetPos = new Vector2(msg.state[0], msg.state[1]);

        if (isSpeedMode || TextDataManager.Instance.isLoopOn)
            player.MoveToPosition(targetPos, TryMoveNextStep);  // 다음 이동 예약
        else if (!isSpeedMode)
            player.MoveToSmoothPosition(targetPos, TryMoveNextStep);
    }
    public void OnClickSpeedMode()
    {
        isSpeedMode = !isSpeedMode;
    }
}