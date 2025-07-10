using UnityEngine;
using UnityEngine.UI;

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

    private void Start()
    {
        ChangeColor();
    }

    private void Update()
    {
        // Todo InputSystemに置換
        if (Input.GetKeyDown(KeyCode.A))
        {
            currentIndex = (currentIndex - 1 + buttons.Length) % buttons.Length;
            ChangeColor();
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            currentIndex = (currentIndex + 1) % buttons.Length;
            ChangeColor();
        }
        else if (Input.GetKeyDown(KeyCode.Return))
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
