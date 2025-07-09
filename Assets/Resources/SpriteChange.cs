using UnityEngine;
using UnityEngine.UI;

public class SpriteChange : MonoBehaviour
{
    [SerializeField,Header("テクスチャリスト")] private Sprite[] spriteList; // スプライトのリスト

    private Image image; // 画像コンポーネント
    private int StageNum;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        image = this.GetComponent<Image>();
        StageNum = StageNummber.Get();

        image.sprite = spriteList[StageNum];
    }
}
