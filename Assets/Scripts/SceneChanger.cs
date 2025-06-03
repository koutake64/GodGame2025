using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
	public static SceneChanger Instance { get; private set; }

	private void Awake()
	{
		// シングルトン化
		if (Instance == null)
		{
			Instance = this;
			DontDestroyOnLoad(gameObject); // シーン間で維持
		}
		else
		{
			Destroy(gameObject);
		}
	}

	/// <summary>
	/// 指定したシーンに切り替える
	/// </summary>
	public static void ChangeScene(string sceneName)
	{
		if (!Application.CanStreamedLevelBeLoaded(sceneName))	// ロード可能か
		{
			Debug.LogError($"Scene '{sceneName}' が存在しません");
			return;
		}

		SceneManager.LoadScene(sceneName);
	}

	// Todo 非同期処理
}
