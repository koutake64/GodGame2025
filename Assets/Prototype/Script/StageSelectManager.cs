using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;

public class StageSelectManager : MonoBehaviour
{
    [SerializeField, Header("ボタン")] private Button[] buttons;

    private ColorBlock colorBlock;
    private string NormalColorHex = "#FFC539"; 
    private string PressedColorHex = "#B08B33";
    private string SelectedColorHex = "#C8C8C8"; 


    private float waitTime = 0.5f; // ボタンの押下後の待機時間（秒）
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // ボタンカラー設定
        colorBlock.normalColor = HexToRGB(NormalColorHex);
        colorBlock.pressedColor = HexToRGB(PressedColorHex);
        colorBlock.selectedColor = HexToRGB(PressedColorHex);
        colorBlock.highlightedColor = HexToRGB(NormalColorHex);
        colorBlock.disabledColor = HexToRGB("#808080"); // Disable color
        colorBlock.colorMultiplier = 1f;
        colorBlock.fadeDuration = 0.1f; // フェード時間設定

        for (int i = 0;i < buttons.Length; i++)
        {
            buttons[i].colors = colorBlock;
        }
    }

    // Update is called once per frame
    void Update()
    {
    }

    public Vector3 GetButtonPos(int num)
    {
        return buttons[num].transform.position;
    }

    public IEnumerator ActionButton(int index)
    {
        buttons[index].Select();

        yield return new WaitForSeconds(waitTime);
        switch (index)
        {
            case 0:
                StageNummber.Set(index);
                SceneChanger.ChangeScene("PrototypeScene");
                break;
            case 1:
                StageNummber.Set(index);
                SceneChanger.ChangeScene("PrototypeScene");
                break;
            case 2:
                StageNummber.Set(index);
                SceneChanger.ChangeScene("PrototypeScene");
                break;
            case 3:
                SceneChanger.ChangeScene("TitileScene");
                break;
            case 4:
                SceneChanger.EndGame();
                break;
        }

        yield break;
    }

    /// <summary>
    /// カラーコードからColor型に変換
    /// </summary>
    /// <param name="hex"></param>
    /// <returns></returns>
    private Color HexToRGB(string hex)
    {
        if (ColorUtility.TryParseHtmlString(hex, out Color color))
        {
            return color;
        }
        else
        {
            Debug.LogError("Invalid hex color string: " + hex);
            return Color.black;
        }
    }
}
