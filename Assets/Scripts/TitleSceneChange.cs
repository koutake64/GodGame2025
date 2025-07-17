using UnityEngine;
using UnityEngine.InputSystem;

public class TitleSceneChange : MonoBehaviour
{
	[SerializeField] private InputActionAsset inputActions;
	private InputAction nextAction;
	[SerializeField] private string nextSceneName = "MainScene"; // 遷移先のシーン名（インスペクターで設定）

	private void Start()
	{
		var map = inputActions.FindActionMap("Menu");
		nextAction = map.FindAction("Next");
		nextAction.Enable();
	}

	private void Update()
    {
        // Enterキーが押されたらシーン遷移
        if (nextAction.WasPressedThisFrame()) // ReturnはEnterキー
        {
            SceneChanger.ChangeScene(nextSceneName);
        }
    }
    
}
