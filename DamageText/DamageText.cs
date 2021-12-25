using System;
using TMPro;
using UnityEngine;
using DG.Tweening;
public class DamageText : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _text;

    //プレイヤーやエネミーの上にダメージテキストを表示する
    public void ShowDamageText(Action<DamageText> callback,Vector3 objectPosition, int damage)
    {
        //ダメージを受けるのか、回復するのかで色を変える。
        ChangeTextColor(damage);
        //テキストを表示する時間
        float textDisplayTime = 1;
        _text.text = string.Format("{0}", damage.ToString("#;#;#;"));

        MoveText(textDisplayTime, objectPosition, callback);
    }
    //テキストを移動
    void MoveText(float textDisplayTime, Vector3 objectPosition, Action<DamageText> callback)
    {
        //透明にする。
        _text.DOFade(0, 0f);

        //テキストの位置を目的の場所に移動。
        var rectT = GetComponent<RectTransform>();
        var rectTransform = GetComponent<RectTransform>();
        rectTransform.position = RectTransformUtility.WorldToScreenPoint(Camera.main, objectPosition);

        //透明度を上げ、上に移動する。
        _text.DOFade(1, textDisplayTime).SetEase(Ease.OutQuart);
        rectTransform.DOAnchorPos(Vector2.up * 100, textDisplayTime).SetRelative(true).SetEase(Ease.OutQuart).OnComplete(() => callback(this));
    }
    /// <summary>
    /// ダメージか回復で色を分ける。
    /// </summary>
    /// <param name="damage"></param>
    void ChangeTextColor(int damage)
    {
        Color color;
        color = new Color(1, 0, 1);
        _text.color = (damage > 0) ? color : Color.cyan;
    }
}
