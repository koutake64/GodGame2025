using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance; // シングルトンインスタンス

    /// <summary>
    /// シングルトンの初期化とマネージャーオブジェクトの生成
    /// </summary>
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        Cursor.visible = false;             // カーソル
        Application.targetFrameRate = 60;   // FPS
        QualitySettings.vSyncCount = 0;     // Sync(モニター依存にならないようにする処理
    }

    private void Update()
    {
        if(Input.GetKey(KeyCode.Escape))
        {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;//ゲームプレイ終了
#else
			Application.Quit();//ゲームプレイ終了
#endif
		}

		if (Input.GetMouseButton(0))
        {
            Cursor.visible = false;
        }


    }
}
