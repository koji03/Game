using UnityEngine;

[CreateAssetMenu(fileName = "Enemy", menuName = "ScriptableObjects/CharactorData/Enemy")]
public class EnemyModel : CharactorModel
{
    public int _fixedAtk = 10;
    public int _fixedDef = 10;
    [Tooltip("索敵範囲")]
    public int _scanningrange = 4;
    [Tooltip("trueなら貫通する")]
    public bool _isPenetrating;
    public AttackDirection _attackDirection;
    public AttackAilmentData _ailmentData;
    public EffectData _effectData;

    [System.Serializable]
    public class AttackAilmentData
    {
        public Ailment _ailment = Ailment.Null;
        [Range(0,100)]
        public int _chance = 0;
    }
}
