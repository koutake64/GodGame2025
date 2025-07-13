using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// α値を変化させて点滅するスクリプト
/// </summary>
public class Flicker : MonoBehaviour
{
    private float timer = 0;

    [SerializeField, Header("点滅の周期")] private float totalTime = 1.0f;

    private Image image;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        image = this.GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        float rate = timer / totalTime;

        float alpha = Mathf.Sin(2.0f * Mathf.PI * rate);
        alpha = (alpha + 1.0f) * 0.5f;
        alpha *= 255.0f;

        image.color = new Color(1.0f, 1.0f, 1.0f, alpha / 255.0f);

        timer += Time.deltaTime;
    } 
}
