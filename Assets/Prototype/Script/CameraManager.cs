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
				eee();
				break;
			case CommonSE_Proto.E_TIMEOFDAY.noon:
				eee();
				break;
			case CommonSE_Proto.E_TIMEOFDAY.afternoon:
				eee();
				break;
		}
	}

	private void eee()
	{
		Vector2Int fieldSize = fieldDataManager.GetFieldSize();
		if (playerTransform == null)
		{
			GameObject playerObj = GameObject.FindWithTag(playerTag);
			if (playerObj != null)
			{
				playerTransform = playerObj.transform;

				// プレイヤー初期位置が左端 or 右端か確認し、カメラ初期位置を調整
				fieldSize = fieldDataManager.GetFieldSize();
				float playerX = playerTransform.position.x;

				if (playerX < fieldSize.x / 2)
				{
					// 左端スタート：右に5マス離す
					transform.position = playerTransform.position + new Vector3(5, offsetPosition.y, offsetPosition.z);
				}
				else
				{
					// 右端スタート：左に5マス離す
					transform.position = playerTransform.position + new Vector3(-5, offsetPosition.y, offsetPosition.z);
				}

				// カメラ角度維持
				transform.eulerAngles = fixedRotation;
			}
			else
			{
				Debug.LogWarning("プレイヤーが見つかりません。タグを確認してください。");
				return;
			}
		}

		// 通常のカメラ追従処理
		Vector2Int fieldSizeDay = fieldDataManager.GetFieldSize();
		float playerXPos = playerTransform.position.x;
		Vector3 targetPosDay = playerTransform.position + offsetPosition;

		if (playerXPos < 5 || playerXPos > fieldSizeDay.x - 6)
		{
			targetPosDay.x = transform.position.x;
		}

		transform.position = targetPosDay;
	}
}

