using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class SelectButton : MonoBehaviour
{
    [SerializeField] int id;

    [SerializeField, Header("遷移先のシーン名"), SceneSelector]
    private string[] scenes;

    [SerializeField, Header("ボタン")]
    private Button[] buttons;

    [SerializeField, Header("終了ボタンの可否")]
    private bool isEndButton = true;

    private int currentIndex = 0;


	[SerializeField] private InputActionAsset inputActions;
	private InputAction upAction;
	private InputAction downAction;
	private InputAction leftAction;
	private InputAction rightAction;
	private InputAction nextAction;


	private void Start()
    {
        ChangeColor();

		var map = inputActions.FindActionMap("Menu");
		upAction = map.FindAction("UP");
		downAction = map.FindAction("Down");
		leftAction = map.FindAction("Left");
		rightAction = map.FindAction("Right");
		nextAction = map.FindAction("Next");

		// 有効化
		upAction.Enable();
		downAction.Enable();
		leftAction.Enable();
		rightAction.Enable();
		nextAction.Enable();
	}

    private void Update()
    {
        // Todo InputSystemに置換
        if (rightAction.WasPressedThisFrame())
        {
            currentIndex = (currentIndex - 1 + buttons.Length) % buttons.Length;
            ChangeColor();
        }
        else if (leftAction.WasPressedThisFrame())
        {
            currentIndex = (currentIndex + 1) % buttons.Length;
            ChangeColor();
        }
        else if (nextAction.WasPressedThisFrame())
        {
            SelectedAction(currentIndex);
        }
    }

    private void ChangeColor()
    {
        for (int i = 0; i < buttons.Length; i++)
        {
            var cb = buttons[i].colors;
            cb.normalColor = (i == currentIndex) ? Color.yellow : Color.white;
            buttons[i].colors = cb;
        }
    }

    private void SelectedAction(int index)
    {
        // 終了ボタンがある場合、最後のボタンはEndGame
        if (isEndButton && index == buttons.Length - 1)
        {
            EndGame();
            return;
        }

        if (index >= 0 && index < scenes.Length && !string.IsNullOrEmpty(scenes[index]))
        {
            if (index == 0 && id == 0)
            {
                int nextStageNum = StageNummber.Get();
                nextStageNum++;
                StageNummber.Set(nextStageNum);
            }

            SceneChanger.ChangeScene(scenes[index]);
        }
    }


    // 終了処理
    public void EndGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
