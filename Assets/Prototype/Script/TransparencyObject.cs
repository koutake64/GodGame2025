using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine.Rendering.Universal.Internal;

public class TransparencyObject : MonoBehaviour
{
    private MeshRenderer    mesh;           // MeshRenderer
    private Material        material;       // Material
    private float           currentAlpha;   // 現在の透明度
    private float           targetAlpha;    // 目標透明度
    private bool            isFade;         // フェードするか
    private float           fadeSpeed;      // フェード速度
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        mesh = transform.GetChild(0).GetComponent<MeshRenderer>();
        material = mesh.material;
        currentAlpha = material.color.a;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isFade) return;

        // 現在の透明度を目標透明度に向かって補間
        currentAlpha = Mathf.MoveTowards(currentAlpha, targetAlpha, fadeSpeed * Time.deltaTime);

        // 色を更新
        Color color = material.color;
        color.a = currentAlpha;
        material.color = color;

        // フェード完了したらフラグを下げる
        if (Mathf.Approximately(currentAlpha, targetAlpha))
        {
            isFade = false;
        }
    }

    public void StartFade(float alpha, float speed)
    {
        // フラグを立てる
        isFade = true;

        // 目標α値を0～1の間にクランプ
        targetAlpha = Mathf.Clamp01(alpha);

        // フェード速度を設定
        fadeSpeed = speed;
    }

    public void RemoveAlpha(float speed)
    {
        StartFade(1.0f, speed);
    }
}