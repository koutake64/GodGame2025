using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class MeshBounds : MonoBehaviour
{
    private TimeManager timeManager; // TimeManagerの参照をInspectorから設定

    private Material material;

    // AwakeかStartでMesh.boundsを拡張する
    void Start()
    {
        timeManager = FindAnyObjectByType<TimeManager>();

        // マテリアル取得
        material = GetComponent<MeshRenderer>().material;

        var meshFilter = GetComponent<MeshFilter>();
        if (meshFilter != null)
        {
            var mesh = meshFilter.mesh;
            mesh.bounds = new Bounds(Vector3.zero, Vector3.one * 100f); // 大きめに！
        }
    }

    void Update()
    {
        // 時間帯に応じた色設定
        Color targetColor = GetColorByTimeState(timeManager.GetCurState());
        material.SetColor("_Color", targetColor);
    }

    private Color GetColorByTimeState(CommonSE_Proto.E_TIMEOFDAY state)
    {
        switch (state)
        {
            case CommonSE_Proto.E_TIMEOFDAY.morning: return new Color(0.6f, 1.0f, 1.0f);  // やさしい青
            case CommonSE_Proto.E_TIMEOFDAY.noon: return Color.white;                 // 明るい白
            case CommonSE_Proto.E_TIMEOFDAY.afternoon: return new Color(1.0f, 0.6f, 0.3f); // 茜色
            case CommonSE_Proto.E_TIMEOFDAY.night: return new Color(0.2f, 0.3f, 1.0f);  // 深い青
            default: return Color.gray;
        }
    }
}
