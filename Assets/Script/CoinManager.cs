using System.Collections.Generic;
using UnityEngine;

public class CoinManager : MonoBehaviour
{
    [SerializeField] private List<GameObject> coinList = new List<GameObject>();

    void Start()
    {
        Invoke(nameof(InitCoins), 0.5f); // 0.1초 후 초기화
    }
    void InitCoins()
    {
        foreach (Transform child in transform)  // 자기 자식들만 탐색
        {
            if (child.tag == "BitCoin")
            {
                coinList.Add(child.gameObject);
            }
        }
    }

    public void ResetCoins()
    {
        foreach (GameObject coin in coinList)
        {
            coin.SetActive(true);  // 다시 보이게
        }
    }
}