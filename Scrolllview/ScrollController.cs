using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScrollController : SerializedMonoBehaviour
{
    [SerializeField]
    ScrollRect _scrollRect = null;
    [SerializeField] float _paddingTop = 0;
    [SerializeField] float _paddingBottom = 0;
    [SerializeField] float _space = 0;
    [SerializeField] RectTransform _contentRectTransform;
    [SerializeField]  RectTransform _scrollRectTransform = null;

    public RectTransform _cellRectTransform;
    float GetSpace() { return _space; }
    float GetCellSize() { return _cellRectTransform.sizeDelta.y; }
    float GetContentAnchoredPos() { return _contentRectTransform.anchoredPosition.y; }

    [NonSerialized]public List<ICellData> _cellDataList = new List<ICellData>();
    LinkedList<GameObject> _cellLinkList = new LinkedList<GameObject>();
    List<float> _cellPositionList = new List<float>();

    int _cellNum = 0;
    float _diffMove;
    int _cellDataIndex = 0;

    public void Initialize(List<ICellData> dataList)
    {
        _cellDataList.AddRange(dataList);
        CalcContentSize();
        _cellNum = CalcCellNum();
        CalcCellPosFromDataList();
        for (int i = 0; i < _cellNum; i++)
        {
            InstantiateCell(i);
        }
        AlignCell();
    }
    /// <summary>
    /// 新しいデータリストでセルを更新する
    /// </summary>
    /// <param name="dataList"></param>
    public void Refresh(List<ICellData> dataList)
    {
        _cellDataList = dataList;
        CalcContentSize();
        _cellNum = CalcCellNum();
        CalcCellPosFromDataList();
        ResetNormalizePosition();
        RefreshCell();
        AlignCell();

    }
    void ResetNormalizePosition()
    {
        var pos = _contentRectTransform.anchoredPosition;
        pos.y = 0.0f;
        _scrollRect.verticalNormalizedPosition = 1.0f;
        _diffMove = 0.0f;
        _cellDataIndex = 0;
    }
    void RefreshCell()
    {
        //多ければ生成、少なければ破棄
        if (_cellNum > _contentRectTransform.childCount)
        {
            var count = _cellNum - _contentRectTransform.childCount;
            for (int i = 0; i < count; i++)
            {
                Instantiate(_cellRectTransform.gameObject, _contentRectTransform);
            }
        }
        else if (_cellNum < _contentRectTransform.childCount)
        {
            int count = _contentRectTransform.childCount;
            for (int i = _cellNum; i < count; i++)
            {
                Destroy(_contentRectTransform.GetChild(i).gameObject);
            }
        }

        //リンクリスト初期化
        _cellLinkList.Clear();
        for (int i = 0; i < _cellNum; i++)
        {
            _cellLinkList.AddLast(_contentRectTransform.GetChild(i).gameObject);
        }

        //表示内容を更新
        for (int i = 0; i < _cellNum; i++)
        {
            var view = _contentRectTransform.GetChild(i).GetComponent<ICellView>();
                view.UpdateView(_cellDataList[i]);
        }
    }
    void Update()
    {
        if (_cellDataList.Count == 0) { return; }

        ScrollDown();
        ScrollUp();

    }


    void ScrollDown()
    {
        while (GetContentAnchoredPos() + _diffMove > _paddingTop + GetCellSize())
        {
            if (_cellDataIndex + _cellNum >= _cellDataList.Count) { return; }
            _diffMove -= (GetCellSize()) + GetSpace();


            MoveLast();

            var first = _cellLinkList.First;
            _cellLinkList.RemoveFirst();
            _cellLinkList.AddLast(first);
            _cellDataIndex++;
        }
    }

    void ScrollUp()
    {
        while (GetContentAnchoredPos() + _diffMove < 0.0f)
        {
            if (GetContentAnchoredPos() < 0.0f) { return; }

            _cellDataIndex--;
            _diffMove += (GetCellSize()) + GetSpace();

            MoveFirst();

            var last = _cellLinkList.Last;
            _cellLinkList.RemoveLast();
            _cellLinkList.AddFirst(last);
        }
    }


    /// <summary>
    /// セルの生成処理
    /// </summary>
    void InstantiateCell(int i)
    {
        var cell = Instantiate(_cellRectTransform.gameObject, _contentRectTransform);
        var view = cell.GetComponent<ICellView>();
        view.UpdateView(_cellDataList[i]);
        _cellLinkList.AddLast(cell);
    }
    //コンテンツのサイズを計算する
    void CalcContentSize()
    {
        float size = 0.0f;
        size += _paddingTop;
        for(int i=0; i< _cellDataList.Count; i++)
        {
            size += GetCellSize();
        }
        size += GetSpace() * (_cellDataList.Count - 1);
        size += _paddingBottom;
        Vector2 sizeDelta = _contentRectTransform.sizeDelta;
        sizeDelta.y = size;
        _contentRectTransform.sizeDelta = sizeDelta;

    }

    //cellの数を計算
    int CalcCellNum()
    {
        int num = 0;
        float scrollSize = _scrollRectTransform.sizeDelta.y;
        float minCellSize = int.MaxValue;
        minCellSize = GetCellSize();

        scrollSize -= _paddingTop;
        scrollSize -= _paddingBottom;//original
        for (int i = 0; i < _cellDataList.Count; i++)
        {
            scrollSize -= minCellSize;
            scrollSize -= i == 0 ? 0.0f : GetSpace();
            if (scrollSize < 0)
            {
                num++;
                break;
            }
            num++;
        }
        num++;
        if (num > _cellDataList.Count)
        {
            num = _cellDataList.Count;
        }
        return num;
    }
    /// <summary>
    /// データリストから各セル座標を計算してリストに保存
    /// </summary>
    void CalcCellPosFromDataList()
    {
        float p = 0.0f;
        _cellPositionList.Clear();
        for (int i = 0; i < _cellDataList.Count; i++)
        {
            p += i == 0 ? _paddingTop : GetCellSize() + GetSpace();
            _cellPositionList.Add(p);
        }
    }

    /// <summary>
    /// 座標リストからセルの座標を取得して設定
    /// </summary>
    void AlignCell()
    {
        for (int i = 0; i < _cellNum; i++)
        {
            var cell = _contentRectTransform.GetChild(i) as RectTransform;
            Vector3 pos = cell.anchoredPosition;
            pos.y = -_cellPositionList[i];
            cell.anchoredPosition = pos;
        }

    }


    /// <summary>
    /// 末尾の要素を戦闘に移動する計算
    /// </summary>
    void MoveFirst()
    {
        var last = _cellLinkList.Last;
        var lastTrans = last.Value.transform as RectTransform;

        var first = _cellLinkList.First;
        var firstTrans = first.Value.transform as RectTransform;

        var pos = lastTrans.anchoredPosition;
        pos.y = firstTrans.anchoredPosition.y + (GetCellSize() + GetSpace());

        lastTrans.anchoredPosition = pos;

        var view = last.Value.GetComponent<ICellView>();
        view.UpdateView(_cellDataList[_cellDataIndex]);
    }


    /// <summary>
    /// 先頭の要素を末尾に移動する計算
    /// </summary>
    void MoveLast()
    {
        var first = _cellLinkList.First;
        var firstTrans = first.Value.transform as RectTransform;
        var last = _cellLinkList.Last;
        var lastTrans = last.Value.transform as RectTransform;
        var pos = firstTrans.anchoredPosition;

        pos.y = lastTrans.anchoredPosition.y - (lastTrans.sizeDelta.y + GetSpace());

        firstTrans.anchoredPosition = pos;

        var view = first.Value.GetComponent<ICellView>();
        view.UpdateView(_cellDataList[_cellDataIndex + _cellNum]);

    }
}
