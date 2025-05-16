using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;

public class TransparencyObject : MonoBehaviour
{
    [Header("透明度")]
    [SerializeField, Range(0.0f, 1.0f)] private float alpha;

    [Header("マテリアル")]
    [SerializeField] private Material transparentMaterial;
    [SerializeField] private Material opaqueMaterial;

    private Transform                           cameraTransform;                                            // カメラTransform
    private List<Transform>                     cameraTargetTransform = new List<Transform>();              // レイを生成するターゲット
    private List<GameObject>                    transparentList = new List<GameObject>();                   // 透明オブジェクトリスト
    private Dictionary<GameObject, Material>    originalMaterials = new Dictionary<GameObject, Material>(); // Matetial保存
    private TimeManager                         timeManager;                                                // TimeManager
    private CommonSE_Proto.E_TIMEOFDAY          timeZone;                                                   // 現在の時間

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cameraTransform = this.GetComponent<Transform>();
        if (!cameraTransform)
        {
            Debug.LogError(
               "Script:TransparencyObject.cs \n" +
               "cameraTransformがnullです"
            );
        }

        timeManager = GameObject.Find("Canvas").GetComponent<TimeManager>();
        if (!timeManager)
        {
            Debug.LogError(
               "Script:TransparencyObject.cs \n" +
               "timeManagerがnullです"
            );
        }
        else
        {
            // 一番最初に実行させたいので朝以外の時間で初期化
            timeZone = CommonSE_Proto.E_TIMEOFDAY.night;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (timeManager)
        {
            SetTargetList();
        }

        if (cameraTargetTransform.Count == 0 || !cameraTransform)
        {
            return;
        }

        // このフレームで衝突したオブジェクト
        List<GameObject> currentHits = new List<GameObject>();

        foreach (var target in cameraTargetTransform)
        {
            // 方向と距離を計算
            Vector3 direction = (target.position - cameraTransform.position).normalized;
            float distance = Vector3.Distance(cameraTransform.position, target.position);

            // レイを作成し衝突オブジェクトを検知
            RaycastHit[] hits = Physics.RaycastAll(cameraTransform.position, direction, distance);

            // シーンビュー上にRayを描画
            // Ray作成
            Ray ray = new Ray(cameraTransform.position, direction);
            Debug.DrawRay(ray.origin, ray.direction * distance, Color.blue);

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
                    // 透明化
                    MeshRenderer mesh = hitObject.GetComponent<MeshRenderer>();
                    if (!mesh)
                    {
                        mesh = hitObject.GetComponentInChildren<MeshRenderer>();
                    }
                    if (mesh)
                    {
                        // 元のマテリアルを保存
                        if (!originalMaterials.ContainsKey(hitObject))
                        {
                            originalMaterials[hitObject] = mesh.material;
                        }

                        mesh.material = new Material(transparentMaterial);
                        Color color = mesh.material.color;
                        color.a = alpha;
                        mesh.material.color = color;

                        // リストに追加
                        transparentList.Add(hitObject);
                    }
                }

                // このフレームで衝突したリストに追加
                currentHits.Add(hitObject);
            }
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
            MeshRenderer mesh = obj.transform.GetComponent<MeshRenderer>();
            if (!mesh)
            {
                mesh = obj.transform.GetComponentInChildren<MeshRenderer>();
            }
            if (mesh && originalMaterials.ContainsKey(obj))
            {
                mesh.material = originalMaterials[obj]; // 元の Opaque マテリアルに戻す
            }

            transparentList.Remove(obj);
            originalMaterials.Remove(obj);
        }
    }
    private void SetTargetList()
    {
        if (timeZone == timeManager.GetCurState()) return;

        // 最新の時間を取得
        timeZone = timeManager.GetCurState();

        // 一度リストをクリア
        cameraTargetTransform.Clear();

        switch (timeZone)
        {
            case CommonSE_Proto.E_TIMEOFDAY.morning:
                {
                    Transform princess = GameObject.FindWithTag("Princess").transform;
                    if (princess) cameraTargetTransform.Add(princess);
                    Transform player = GameObject.FindWithTag("Player").transform;
                    if (player) cameraTargetTransform.Add(player);
                }
                break;
            case CommonSE_Proto.E_TIMEOFDAY.noon:
                {
                    Transform player = GameObject.FindWithTag("Player").transform;
                    if (player) cameraTargetTransform.Add(player);
                }
                break;
            case CommonSE_Proto.E_TIMEOFDAY.afternoon:
                {
                    Transform player = GameObject.FindWithTag("Player").transform;
                    if (player) cameraTargetTransform.Add(player);
                }
                break;
            case CommonSE_Proto.E_TIMEOFDAY.night:
                {
                    Transform princess = GameObject.FindWithTag("Princess").transform;
                    if (princess) cameraTargetTransform.Add(princess);
                }
                break;
        }
    }
}