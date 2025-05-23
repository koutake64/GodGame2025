using System.Collections.Generic;
using UnityEngine;

public class RayToTarget : MonoBehaviour
{
    [Header("透明度")]
    [SerializeField, Range(0.0f, 1.0f)] private float alpha;

    [Header("透明度フェード速度")]
    [SerializeField] private float fadeSpeed = 1.0f;

    private List<Transform> cameraTargetTransform = new List<Transform>();                                  // レイを生成するターゲット
    private List<GameObject> transparentList = new List<GameObject>();                                      // 透明オブジェクトリスト
    private TimeManager timeManager;                                                                        // TimeManager
    private CommonSE_Proto.E_TIMEOFDAY timeZone;                                                            // 現在の時間

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
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
    void LateUpdate()
    {
        if (timeManager)
        {
            SetTargetList();
        }

        if (cameraTargetTransform.Count == 0)
        {
            return;
        }

        // このフレームで衝突したオブジェクト
        List<GameObject> currentHits = new List<GameObject>();

        foreach (var target in cameraTargetTransform)
        {
            // 方向と距離を計算
            Vector3 direction = (target.position - transform.position).normalized;
            float distance = Vector3.Distance(transform.position, target.position);

            // レイを作成し衝突オブジェクトを検知
            RaycastHit[] hits = Physics.RaycastAll(transform.position, direction, distance);

            // シーンビュー上にRayを描画
            // Ray作成
            Ray ray = new Ray(transform.position, direction);
            Debug.DrawRay(ray.origin, ray.direction * distance, Color.blue);

            foreach (var hit in hits)
            {
                // 透明化しないオブジェクト確認
                GameObject hitObject = hit.collider.gameObject;
                TransparencyObject transparency = hitObject.GetComponent<TransparencyObject>();
                if(!transparency)
                {
                    continue;
                }

                // 重複していないか確認
                if (!transparentList.Contains(hitObject))
                {
                    // 透明化処理を呼び出す
                    transparency.StartFade(alpha, fadeSpeed);

                    // リストに追加
                    transparentList.Add(hitObject);
                    
                }

                // このフレームで衝突したリストに追加
                currentHits.Add(hitObject);
            }
        }

        // 透明解除オブジェクトを確認
        List<GameObject> restore = new List<GameObject>();
        foreach (var obj in transparentList)
        {
            if (!currentHits.Contains(obj))
            {
                restore.Add(obj);
            }
        }

        // α値を元に戻す
        foreach (var obj in restore)
        {
            TransparencyObject transparency = obj.GetComponent<TransparencyObject>();
            if (!transparency)
            {
                continue;
            }

            // 透明度を戻す
            transparency.RemoveAlpha(fadeSpeed);

            transparentList.Remove(obj);
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
            case CommonSE_Proto.E_TIMEOFDAY.afternoon:
                {
                    Transform player = GameObject.FindWithTag("Player").transform;
                    if (player) cameraTargetTransform.Add(player);
                }
                break;
            case CommonSE_Proto.E_TIMEOFDAY.night:
                {
                    GameObject princess = GameObject.FindWithTag("Princess");
                    if (princess)
                    {
                        cameraTargetTransform.Add(princess.transform);
                    }
                }
                break;
        }
    }
}