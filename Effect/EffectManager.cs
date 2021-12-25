using Cysharp.Threading.Tasks;
using System;
using UnityEngine;


[System.Serializable]
public struct EffectData
{
    public GameObject _effect;
    public AudioClip _SE;
    public float time;
}

public class EffectManager : MonoBehaviour,IEffectManager
{
    [SerializeField]AilmentsEffectData _ailmentsEffectData;

    private void Awake()
    {
        Locator<IEffectManager>.Bind(this);
    }

    private void OnDestroy()
    {
        Locator<IEffectManager>.Unbind(this);
    }

    /// <summary>
    /// エフェクトデータを返す
    /// </summary>
    EffectData GetAilmentEffectData(Ailment ailment)
    {
        return _ailmentsEffectData.GetAilmentEffect(ailment);
    }
    /// <summary>
    /// 状態異常のエフェクトを再生
    /// </summary>
    /// <returns></returns>
    async public virtual UniTask PlayAilmentEffect(Vector3 position, Ailment ailment, Quaternion rotation)
    {
        var data = GetAilmentEffectData(ailment);
        await PlayEffect(position, data, rotation);
    }
    /// <summary>
    /// エフェクトを再生
    /// </summary>
    async public virtual UniTask PlayEffect(Vector3 effectPosition, EffectData data, Quaternion effectRotation)
    {
        //エフェクトを作成
        var obj= InstantiateEffect(data._effect,effectPosition,effectRotation);
        SoundManager.Instance.PlaySE(data._SE);

        await UniTask.Delay(TimeSpan.FromSeconds(data.time));
        Destroy(obj);
    }
    /// <summary>
    /// エフェクトを再生
    /// </summary>
    async public virtual UniTask PlayEffect(Vector3 position, EffectData data)
    {
        var obj = Instantiate(data._effect);
        position.y += .1f;

        SoundManager.Instance.PlaySE(data._SE);
        await UniTask.Delay(TimeSpan.FromSeconds(data.time));
        Destroy(obj);
    }

    /// <summary>
    /// エフェクトを作成
    /// </summary>
    GameObject InstantiateEffect(GameObject effect, Vector3 effectPosition, Quaternion effectRotation)
    {
        var obj = Instantiate(effect);
        effectPosition.y += .1f;

        var rX = obj.transform.rotation.x;
        var rY = obj.transform.rotation.y;
        var rZ = obj.transform.rotation.z;

        if (rX == 0 && rY == 0 && rZ == 0)
        {
            obj.transform.rotation = effectRotation;
        }
        obj.transform.position = effectPosition;
        return obj;
    }
}
