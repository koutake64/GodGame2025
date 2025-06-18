using UnityEngine;

public class CardDealMotion : MonoBehaviour
{
    [Header("移動設定")]
    [SerializeField, Header("開始位置")] private Vector2 start;
    [SerializeField, Header("終了位置")] private Vector2 end;
    [SerializeField, Header("放物線の高さ")] private float arcHeight;

    [Header("回転設定")]
    [SerializeField, Header("最終回転角")] private float rotationAmount = 360f;
    [SerializeField, Header("時間に対する回転を制御するカーブ")] private AnimationCurve rotationSpeedCurve = AnimationCurve.Linear(0, 0, 1, 1);

    private RectTransform rectTransform;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        rectTransform.anchoredPosition = start;
    }

    public void AnimateCardMotion(float t)
    {
        // 移動位置(放物線軌道)
        Vector2 pos = Vector2.Lerp(start, end, t);
        pos.y += Mathf.Sin(t * Mathf.PI) * arcHeight;

        // 回転角(カーブで制御)
        float rotationT = rotationSpeedCurve.Evaluate(t);
        float angle = Mathf.Lerp(0f, rotationAmount, rotationT);

        // 更新
        rectTransform.anchoredPosition = pos;
        rectTransform.localRotation = Quaternion.Euler(0, 0, angle);
    }
}
