using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [SerializeField, SelectTag, Header("追従対象のタグ")] private string playerTag;
    [SerializeField, SelectTag, Header("追従対象のタグ")] private string princessTag;
    [SerializeField, Header("プレイヤーとの相対位置")] private Vector3 offsetPosition;
    [SerializeField, Header("カメラの固定角度")] private Vector3 fixedRotation;

    private Transform playerTransform;
    private Transform princessTransform;
    private TimeManager timeManager;
	private _FieldDataManager fieldDataManager;


	void Start()
    {
        transform.eulerAngles = fixedRotation;
        timeManager = FindFirstObjectByType<TimeManager>();
		fieldDataManager = FindFirstObjectByType<_FieldDataManager>();
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
					}
					else
						Debug.LogWarning("princessが見つかりませんでした。タグを確認してください。");
				}

				if (princessTransform != null)
				{
					Vector2Int fieldSize = fieldDataManager.GetFieldSize();
					float princessX = princessTransform.position.x;

					Vector3 targetPos = princessTransform.position + offsetPosition;

					// 左右端ではX座標を固定
					if (princessX < 5)
					{
						targetPos.x = transform.position.x;
					}
					else if (princessX > fieldSize.x - 6)
					{
						targetPos.x = transform.position.x;
					}

					transform.position = targetPos;
				}

				break;

			case CommonSE_Proto.E_TIMEOFDAY.morning:
            case CommonSE_Proto.E_TIMEOFDAY.noon:
			case CommonSE_Proto.E_TIMEOFDAY.afternoon:
                if (playerTransform == null)
                {
                    GameObject playerObj = GameObject.FindWithTag(playerTag);
                    if (playerObj != null)
                    {
                        playerTransform = playerObj.transform;
                    }
                    else
                        Debug.LogWarning("プレイヤーが見つかりません。タグを確認してください。");
                }

                if (playerTransform != null)
                {
					Vector2Int fieldSize = fieldDataManager.GetFieldSize();
					float playerX = playerTransform.position.x;

					Vector3 targetPos = playerTransform.position + offsetPosition;

					// X座標固定
                    // todo 変数化
					if (playerX < 5)
					{
						targetPos.x = transform.position.x;
					}
					else if (playerX > fieldSize.x - 6)
					{
						targetPos.x = transform.position.x;
					}

					transform.position = targetPos;
				}
                break;
            //case CommonSE_Proto.E_TIMEOFDAY.afternoon:
            //    if (playerTransform == null)
            //    {
            //        GameObject playerObj = GameObject.FindWithTag(playerTag);
            //        if (playerObj != null)
            //        {
            //            playerTransform = playerObj.transform;
            //        }
            //        else
            //            Debug.LogWarning("プレイヤーが見つかりません。タグを確認してください。");
            //    }

            //    if (playerTransform != null)
            //        transform.position = playerTransform.position + offsetPosition;

            //    break;
        }
    }
}
