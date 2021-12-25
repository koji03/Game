using TMPro;
using UnityEngine;

public class RecordPanel : MonoBehaviour
{
    [SerializeField]TextMeshProUGUI _currentRecord, _maxRecord;

    public void ShowPanel(int current, int max)
    {
        _currentRecord.text = current.ToString();
        _maxRecord.text = max.ToString();
        gameObject.SetActive(true);
    }

}
