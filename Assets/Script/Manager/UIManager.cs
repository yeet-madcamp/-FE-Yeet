using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    bool isPopupOn = false;
    public void OpenPopup(GameObject popup)
    {
        Debug.Log("✅ 버튼 클릭됨!");
        popup.SetActive(true);
    }
    public void ClosePopup(GameObject popup)
    {
        popup.SetActive(false);
    }
    public void OnOffPopup(GameObject popup)
    {
        if (!isPopupOn)
        {
            popup.SetActive(true);
            isPopupOn = true;
        }
        else
        {
            popup.SetActive(false);
            isPopupOn = false;
        }
    }


}
