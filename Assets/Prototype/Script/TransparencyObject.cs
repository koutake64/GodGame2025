using NUnit.Framework;
using UnityEngine;

public class TransparencyObject : MonoBehaviour
{
    private Transform cameraTransform;
    private Transform cameraTargetTransform;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cameraTransform = GameObject.FindWithTag("MainCamera").GetComponent<Transform>();
        if(cameraTransform)
        {
            Debug.LogError(
               "Script:TransparencyObject.cs \n" +
               "cameraTranformがnullです"
            );
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!cameraTargetTransform || !cameraTargetTransform)
        {
            return;
        }

        // 方向と距離を計算
        Vector3 direction = (cameraTransform.position - cameraTargetTransform.position).normalized;
        float distance = Vector3.Distance(cameraTransform.position, cameraTargetTransform.position);

        // レイを作成し衝突オブジェクトを検知
        RaycastHit[] hits = Physics.RaycastAll(cameraTransform.position, direction, distance);
        foreach (var hit in hits)
        {
            
        }

    }
    public void SetTargetTransform(Transform target)
    {
        cameraTargetTransform = target;
    }
}
