using UnityEngine;

public class TitleSceneChange : MonoBehaviour
{
    
    [SerializeField] private string nextSceneName = "MainScene"; // 遷移先のシーン名（インスペクターで設定）

    private void Update()
    {
        // Enterキーが押されたらシーン遷移
        if (Input.GetKeyDown(KeyCode.Return)) // ReturnはEnterキー
        {
            SceneChanger.ChangeScene(nextSceneName);
        }
    }
    
}
