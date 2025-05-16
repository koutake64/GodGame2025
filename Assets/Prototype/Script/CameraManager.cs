using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [SerializeField, SelectTag, Header("追従対象のタグ")] private string playerTag;
    [SerializeField, SelectTag, Header("追従対象のタグ")] private string princessTag;
    [SerializeField, Header("プレイヤーとの相対位置")] private Vector3 offsetPosition;
    [SerializeField, Header("カメラの固定角度")] private Vector3 fixedRotation;
    [SerializeField, Header("透明化スクリプト")] private TransparencyObject transparencyObject;

    private Transform playerTransform;
    private Transform princessTransform;
    private TimeManager timeManager;

    void Start()
    {
        transform.eulerAngles = fixedRotation;
        timeManager = FindFirstObjectByType<TimeManager>();
    }

    void LateUpdate()
    {
        // 朝、昼、夜でカメラ切り替え
        switch (timeManager.CurrentState)
        {
            case CommonSE_Proto.E_TIMEOFDAY.night:
                if (princessTransform == null)
                {
                    GameObject princessObj = GameObject.FindWithTag(princessTag);
                    if (princessObj != null)
                    {
                        princessTransform = princessObj.transform;
                        transparencyObject.AddTargetTransform(princessTransform);
                    }
                    else
                        Debug.LogWarning("princessが見つかりませんでした。タグを確認してください。");
                }

                if (princessTransform != null)
                    transform.position = princessTransform.position + offsetPosition;

                break;

            case CommonSE_Proto.E_TIMEOFDAY.morning:
            case CommonSE_Proto.E_TIMEOFDAY.noon:
                if (playerTransform == null)
                {
                    GameObject playerObj = GameObject.FindWithTag(playerTag);
                    if (playerObj != null)
                    {
                        playerTransform = playerObj.transform;
                    }
                    else
                        Debug.LogWarning("プレイヤーが見つかりませんでした。タグを確認してください。");
                }

                if (playerTransform != null)
                    transform.position = playerTransform.position + offsetPosition;

                break;
            case CommonSE_Proto.E_TIMEOFDAY.afternoon:
                if (playerTransform == null)
                {
                    GameObject playerObj = GameObject.FindWithTag(playerTag);
                    if (playerObj != null)
                    {
                        playerTransform = playerObj.transform;
                    }
                    else
                        Debug.LogWarning("プレイヤーが見つかりませんでした。タグを確認してください。");
                }

                if (playerTransform != null)
                    transform.position = playerTransform.position + offsetPosition;

                break;
        }
    }
}
