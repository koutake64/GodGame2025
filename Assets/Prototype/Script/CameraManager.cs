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
	private bool initNightPos = false;

	void Start()
    {
        transform.eulerAngles = fixedRotation;
        timeManager = FindFirstObjectByType<TimeManager>();
		fieldDataManager = FindFirstObjectByType<_FieldDataManager>();
	}

	void LateUpdate()
	{
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
					{
						if (!initNightPos)
						{
							transform.position = new Vector3(5, 2, -9);
							initNightPos = true;
						}
						Debug.LogWarning("princessが見つかりません。タグを確認してください。");
						return;
					}
				}

				// princess が見つかればフラグをリセット
				initNightPos = false;

				Vector2Int fieldSize = fieldDataManager.GetFieldSize();
				float princessX = princessTransform.position.x;
				Vector3 targetPos = princessTransform.position + offsetPosition;

				if (princessX < 5 || princessX > fieldSize.x - 6)
				{
					targetPos.x = transform.position.x;
				}

				transform.position = targetPos;
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
					{
						Debug.LogWarning("プレイヤーが見つかりません。タグを確認してください。");
						return;
					}
				}

				Vector2Int fieldSizeDay = fieldDataManager.GetFieldSize();
				float playerX = playerTransform.position.x;
				Vector3 targetPosDay = playerTransform.position + offsetPosition;

				if (playerX < 5 || playerX > fieldSizeDay.x - 6)
				{
					targetPosDay.x = transform.position.x;
				}

				transform.position = targetPosDay;
				break;
		}
	}
}

