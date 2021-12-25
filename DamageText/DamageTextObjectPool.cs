using UnityEngine;
using UniRx;
public class DamageTextObjectPool : Singleton<DamageTextObjectPool>
{
    [SerializeField]
    private DamageText _original = null;
    //オブジェクトプール
    private DamageTextPool _pool;
    private void Start()
    {
        //プール作成
        _pool = new DamageTextPool(_original);
    }
    public void ShowDamageText(int damage,Vector3 position)
    {
       _pool.RentAsync().Subscribe(damageText => {
           damageText.ShowDamageText(_pool.Return,position, damage);
       });
    }
}
