
using Cysharp.Threading.Tasks;
using UnityEngine;

public interface IEffectManager
{
    UniTask PlayEffect(Vector3 position, EffectData data, Quaternion rotation);
    UniTask PlayAilmentEffect(Vector3 position, Ailment ailment, Quaternion rotation);
}
