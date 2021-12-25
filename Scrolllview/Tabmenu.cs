using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Tabmenu : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] Color gold = new Color(0.96f, 0.88f, 0.6f);
    [SerializeField] Color gray = new Color(0.82f, 0.82f, 0.82f);
    [SerializeField] GameObject[] _focusTabs;
    [SerializeField] TextMeshProUGUI[] _texts;

    public int num = 0;
    public void OnClick(int clickNumber)
    {
        //タブの切り替え
        num = clickNumber;
        for (int i=0; i<_focusTabs.Length; i++)
        {
            //タブの番号と同じならアクティブにしてカラーを変える。
            _focusTabs[i].gameObject.SetActive(clickNumber == i);
            _texts[i].color = (clickNumber == i) ? gold : gray;
        }
    }
}
