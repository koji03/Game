using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    GameObject _target;
    private Vector3 distance;
    /// <summary>
    /// カメラの初期化
    /// </summary>
    public void Init(GameObject player)
    {
        Vector3 position = transform.position;
        position.x = player.transform.position.x;
        transform.position = position;
        _target = player;
        distance = transform.position - _target.transform.position;
    }
    void LateUpdate()
    {
        if(_target !=null)
        {
            transform.position = _target.transform.position + distance;
        }
    }
}
