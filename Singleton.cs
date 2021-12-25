using Cysharp.Threading.Tasks;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : Singleton<T>
{
	protected virtual bool DestroyTargetGameObject => false;

	public static T Instance { get; private set; } = null;

	/// <summary>
	/// Singletonが有効か
	/// </summary>
	public static bool IsValid() => Instance != null;

	private void Awake()
	{
		if (Instance == null)
		{
			Instance = this as T;
			Instance.Init();
			return;
		}
		if (DestroyTargetGameObject)
		{
			Destroy(gameObject);
			gameObject.SetActive(false);
		}
		else
		{
			Destroy(this);
			gameObject.SetActive(false);
		}
	}

	/// <summary>
	/// 派生クラス用のAwake
	/// </summary>
	async protected virtual UniTask Init()
	{
	}

	private void OnDestroy()
	{
		if (Instance == this)
		{
			Instance = null;
		}
		OnRelease();
	}

	/// <summary>
	/// 派生クラス用のOnDestroy
	/// </summary>
	protected virtual void OnRelease()
	{
	}
}