using UnityEngine;

public class TransparencyObject : MonoBehaviour
{
    [Header("マテリアル設定")]
    [SerializeField] private Material opaqueMaterial;
    [SerializeField] private Material transparentMaterial;

    private MeshRenderer meshRenderer;
    private Material workingMaterial;

    private float currentAlpha = 1f;
    private float fadeSpeed = 1f;
    private float goalAlpha = 1f;

    private bool isFading = false;
    private bool isTransparent = false;

    private void Awake()
    {
        meshRenderer = GetComponentInChildren<MeshRenderer>();

        if (!meshRenderer)
        {
            Debug.LogError("TransparencyObject: MeshRenderer が見つかりません");
            enabled = false;
            return;
        }

        // 最初はOpaqueマテリアルで表示
        workingMaterial = new Material(opaqueMaterial);
        meshRenderer.material = workingMaterial;
        currentAlpha = 1f;
    }

    private void Update()
    {
        if (!isFading) return;

        currentAlpha = Mathf.MoveTowards(currentAlpha, goalAlpha, fadeSpeed * Time.deltaTime);

        // マテリアルの切替が必要なら行う
        if (!isTransparent && goalAlpha < 1f)
        {
            workingMaterial = new Material(transparentMaterial);
            meshRenderer.material = workingMaterial;
            isTransparent = true;
        }

        Color color = workingMaterial.color;
        color.a = currentAlpha;
        workingMaterial.color = color;
        meshRenderer.material = workingMaterial;

        // フェード完了チェック
        if (Mathf.Approximately(currentAlpha, goalAlpha))
        {
            isFading = false;

            // 完全不透明に戻ったら Opaque に戻す
            if (Mathf.Approximately(goalAlpha, 1f))
            {
                workingMaterial = new Material(opaqueMaterial);
                meshRenderer.material = workingMaterial;
                isTransparent = false;
            }
        }
    }

    /// <summary>
    /// フェードを開始（透明にする）
    /// </summary>
    public void StartFade(float toAlpha, float speed)
    {
        //Debug.Log($"StartFade: {toAlpha}");
        goalAlpha = Mathf.Clamp01(toAlpha);
        fadeSpeed = Mathf.Max(speed, 0.01f);
        isFading = true;
    }

    /// <summary>
    /// 不透明に戻す
    /// </summary>
    public void RemoveAlpha(float speed)
    {
        StartFade(1f, speed);
    }
}
