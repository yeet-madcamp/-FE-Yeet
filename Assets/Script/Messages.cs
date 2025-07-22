using Newtonsoft.Json;

[System.Serializable]
public class BaseMessage
{
    [JsonProperty("event")]
    public string eventType;
}

[System.Serializable]
public class StepMessage : BaseMessage
{
    public int episode;
    public int step;
    public float[] state;
    public int action;
    public float reward;
    public float total_reward;
    public float? loss;
    public float epsilon;
    public bool terminated;
    public bool truncated;
    public int success;
}

[System.Serializable]
public class ModelPathMessage : BaseMessage
{
    public string model_url;
}

[System.Serializable]
public class EpisodeSuccessMessage : BaseMessage
{
    public int episode;
    public float total_reward;
}