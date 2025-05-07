using UnityEngine;

public class CameraManager : MonoBehaviour
{
	[SerializeField, Header("追従対象のタグ")] private string playerTag = "Player";
	[SerializeField, Header("プレイヤーとの相対位置")] private Vector3 offsetPosition;
	[SerializeField, Header("カメラの固定角度")] private Vector3 fixedRotation;

	private Transform playerTransform;

	void Start()
	{
		

		// カメラの角度を固定
		transform.eulerAngles = fixedRotation;
	}

	void LateUpdate()
	{
		// To Do 
		// 後にStartで探すように
		// プレイヤーをタグで探す（プレハブは "Player" タグをつけておくこと）
		GameObject playerObj = GameObject.FindWithTag(playerTag);
		if (playerObj != null)
		{
			playerTransform = playerObj.transform;
		}
		else
		{
			Debug.LogWarning("プレイヤーが見つかりませんでした。タグを確認してください。");
		}

		if (playerTransform != null)
		{
			// プレイヤーに追従（角度は固定）
			transform.position = playerTransform.position + offsetPosition;
		}
	}
}
