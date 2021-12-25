using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AilmentIcon : MonoBehaviour
{
    [SerializeField]TextMeshProUGUI _iconnameText;
    [SerializeField] Image _image;
    [System.NonSerialized]public string ailmentname;
    [System.NonSerialized]public bool _isDelete = false;

    //プレイヤーの状態異常を表示する
    public void WriteIconText(string text,Color color,bool isDelete)
    {
        _iconnameText.text = text;
        _image.color = color;
        ailmentname = text;
        _isDelete = isDelete;
    }

}
