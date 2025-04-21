using UnityEngine;

public class Player : MonoBehaviour
{
	[SerializeField, Header("行動クールタイム(秒)")] private float moveCooldown;
	[SerializeField,Header("プレイヤー生成位置")] private Vector2Int startPos;
	[SerializeField,Header("FieldDataManagerを設定")] private FieldDataManager fieldManager;

	private Vector2Int currentPos;
	private float moveTimer;

	void Start()
	{
		currentPos = startPos;
		transform.position = fieldManager.GetInfo(currentPos).obj.transform.position;
	}

	void Update()
	{
		moveTimer += Time.deltaTime;

		// 移動処理
		if (moveTimer >= moveCooldown)
		{
			Vector2Int direction = Vector2Int.zero;

			if (Input.GetKey(KeyCode.W)) direction = Vector2Int.up;
			else if (Input.GetKey(KeyCode.S)) direction = Vector2Int.down;
			else if (Input.GetKey(KeyCode.D)) direction = Vector2Int.right;
			else if (Input.GetKey(KeyCode.A)) direction = Vector2Int.left;

			if (direction != Vector2Int.zero)
			{
				TryMove(direction);
				moveTimer = 0;
			}
		}
	}

	void TryMove(Vector2Int direction)
	{
		Vector2Int nextPos = currentPos + direction;

		if (CanMoveTo(nextPos))
		{
			currentPos = nextPos;
			transform.position = fieldManager.GetInfo(currentPos).obj.transform.position;

			var state = fieldManager.GetInfo(currentPos).state;
			if (state == FieldDataManager.E_FIELDSTATE.goal)
			{
				Debug.Log(" ゴールしました！");
			}
		}
	}

	bool CanMoveTo(Vector2Int pos)
	{
		if (fieldManager == null) return false;

		int maxX = fieldManager.fieldInfoArray.GetLength(0);
		int maxY = fieldManager.fieldInfoArray.GetLength(1);

		if (pos.x < 0 || pos.y < 0 || pos.x >= maxX || pos.y >= maxY)
			return false;

		var state = fieldManager.GetInfo(pos).state;
		return state == FieldDataManager.E_FIELDSTATE.none || state == FieldDataManager.E_FIELDSTATE.goal;
	}
}
