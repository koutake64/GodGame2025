using UnityEngine;
using System.Collections.Generic;

public class TransparencyObject : MonoBehaviour
{
    [Header("透明度")]
    [SerializeField, Range(0.0f, 1.0f)] private float alpha;

    private Transform cameraTransform;
    private Transform cameraTargetTransform;
    private List<GameObject> transparentList = new List<GameObject>();  // 透明オブジェクトリスト

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cameraTransform = this.GetComponent<Transform>();
        if(!cameraTransform)
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
        if (!cameraTargetTransform || !cameraTransform)
        {
            return;
        }

        // このフレームで衝突したオブジェクト
        List<GameObject> currentHits = new List<GameObject>();

        // 方向と距離を計算
        Vector3 direction = (cameraTransform.position - cameraTargetTransform.position).normalized;
        float distance = Vector3.Distance(cameraTransform.position, cameraTargetTransform.position);

        // レイを作成し衝突オブジェクトを検知
        RaycastHit[] hits = Physics.RaycastAll(cameraTransform.position, direction, distance);
        foreach (var hit in hits)
        {
            // 透明化しないオブジェクト確認
            GameObject hitObject = hit.collider.gameObject;

            if (hitObject.tag == "Player" || hitObject.tag == "Princess" || hitObject.tag == "Security")
            {
                continue;
            }

            // 重複していないか確認
            if (!transparentList.Contains(hitObject))
            {
                // リストに追加
                transparentList.Add(hitObject);

                // 透明化
                MeshRenderer mesh = hitObject.GetComponent<MeshRenderer>();
                if (mesh)
                {
                    Color color = mesh.material.color;
                    color.a = alpha;
                    mesh.material.color = color;
                }
            }

            // このフレームで衝突したリストに追加
            currentHits.Add(hitObject);
        }

        // 透明解除オブジェクトを確認
        List<GameObject> restore = new List<GameObject>();
        foreach(var obj in transparentList)
        {
            if(!currentHits.Contains(obj))
            {
                restore.Add(obj);
            }
        }

        // α値を元に戻す
        foreach(var obj in restore)
        {
            // 透明化リストから排除
            transparentList.Remove(obj);

            // 透明化解除
            MeshRenderer mesh = obj.GetComponent<MeshRenderer>();
            if (mesh)
            {
                Color color = mesh.material.color;
                color.a = 1.0f;
                mesh.material.color = color;
            }
        }
    }
    public void SetTargetTransform(Transform target)
    {
        cameraTargetTransform = target;
    }
}
