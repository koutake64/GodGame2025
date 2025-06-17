using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;


public class UIAnimator : MonoBehaviour
{
    public enum E_AnimationType
    {
        Move,
        Custom,
    }

    [Header("共通設定")]
    [SerializeField, Header("アニメーションの種類")] private E_AnimationType animationType;
    [SerializeField, Header("アニメーション全体の再生時間")] private float duaration = 1.0f;
    [SerializeField, Header("再生までの遅延")] private float delay = 0.0f;
    [SerializeField, Header("イージング曲線")] private AnimationCurve easing = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [Header("移動設定")]
    [SerializeField, Header("移動開始位置")] private Vector3 startPos;
    [SerializeField, Header("移動終了位置")] private Vector3 endPos;
    [SerializeField, Header("エディタ上で設定されている位置を開始地点とする")] private bool isCurrentPos = false;

    [Header("カスタム設定")]
    [SerializeField, Header("自作アニメーション用のコールバック")] private UnityEvent<float> onCustomAnimationUpdate;

    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();

        switch(animationType)
        {
            case E_AnimationType.Move:
                rectTransform.anchoredPosition = startPos;
                break;
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// 外部用　アニメーション再生
    /// </summary>
    public void Play()
    {
        StopAllCoroutines();
        StartCoroutine(Animate());
    }

    /// <summary>
    /// アニメーション処理コルーチン
    /// </summary>
    /// <returns></returns>
    private IEnumerator Animate()
    {
        yield return new WaitForSeconds(delay);

        float timer = 0.0f;

        while(timer < duaration)
        {
            float t = timer / duaration;
            float easeT = easing.Evaluate(t);

            // アニメーションごとの処理
            switch (animationType)
            {
                case E_AnimationType.Move:
                    // UIの位置を補間移動
                    rectTransform.anchoredPosition=Vector3.Lerp(startPos, endPos, easeT); 
                    break;
                case E_AnimationType.Custom:
                    // カスタムコールバックを呼び出す
                    onCustomAnimationUpdate?.Invoke(easeT);
                    break;
            }

            // 次フレームへ
            timer += Time.deltaTime;
            yield return null;
        }

        // アニメーション終了時に最終状態を適用
        switch (animationType)
        {
            case E_AnimationType.Move:
                rectTransform.anchoredPosition = endPos;
                break;
            case E_AnimationType.Custom:
                onCustomAnimationUpdate?.Invoke(1f);
                break;
        }
    }
}
