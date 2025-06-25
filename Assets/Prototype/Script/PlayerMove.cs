using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMove : MonoBehaviour
{
	private CharacterMoveController moveController;
	private TimeManager timeManager;
	private _FieldDataManager fieldData;
	private UIManager uiManager;

	private Vector2 moveInput; // 入力値
	private float inputCooldown = 0f; // 入力間隔
	private float inputTimer = 0f;
	private Vector2Int fieldSize;

	public void OnMove(InputAction.CallbackContext context)
	{
		moveInput = context.ReadValue<Vector2>();
	}

	void Start()
	{
		moveController = GetComponent<CharacterMoveController>();
		timeManager = FindFirstObjectByType<TimeManager>();
		uiManager = FindFirstObjectByType<UIManager>();

        fieldData = GameObject.Find("Field").GetComponent<_FieldDataManager>();
        if (!fieldData)
        {
            Debug.LogError(
               "Script:PlayerMove.cs \n" +
               "fieldDataがnullです"
            );
        }
		fieldSize = fieldData.GetFieldSize();
    }

	void Update()
	{
		if (timeManager.CurrentState != CommonSE_Proto.E_TIMEOFDAY.noon || timeManager.GetCurState() == CommonSE_Proto.E_TIMEOFDAY.afternoon || uiManager.Getいずちぇんじないとあにめーしょんず())
			return;

		inputTimer -= Time.deltaTime;
		if (inputTimer > 0) return;

		// 優先順位：上下→左右
		if (moveInput.y > 0.5f)
		{
			moveController.AddPosY(1);
			inputTimer = inputCooldown;
		}
		else if (moveInput.y < -0.5f)
		{
			moveController.AddPosY(-1);
			inputTimer = inputCooldown;
		}
		else if (moveInput.x > 0.5f)
		{
			moveController.AddPosX(1);
			inputTimer = inputCooldown;
		}
		else if (moveInput.x < -0.5f)
		{
			moveController.AddPosX(-1);
			inputTimer = inputCooldown;
		}

		if (Input.GetKeyDown(KeyCode.Return))
		{
			ChangeObjectDirection();
		}
	}

	private void ChangeObjectDirection()
	{
		// 現在の座標を取得
		Vector2Int pos = moveController.GetCurrentPos();

		// 方向
		Vector2Int[] directions = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };

        // 自身の周り4マスにカメラ・ライトがあるか判定
		foreach(var dir in directions)
		{
			// 取得座標がフィールドの範囲内か判定
			Vector2Int targetPos = pos + dir;
            if (targetPos.x < 0 || targetPos.x >= fieldSize.x || targetPos.y < 0 || targetPos.y >= fieldSize.y)
            {
				continue;
			}

			// 対象マスの情報取得
			var infoList = fieldData.GetInfoList(targetPos);
			foreach (var info in infoList) 
			{
				// オブジェクトがない場合は次へ
				if (!info.obj) continue;

				var camera = info.obj.GetComponent<SurveillanceCamera>();
				if(camera)
				{
					Vector3 objPos = info.obj.GetComponent<Transform>().position;
					if(objPos != null)
					{
						// 対象オブジェクトの方を見る
						LookAtObject(objPos);
                    }
                    AudioManager.Instance.PlaySE(6);
                    camera.CameraAction(this.transform);
					break;
				}
                var light = info.obj.GetComponent<LightObject>();
                if (light)
                {
                    Vector3 objPos = info.obj.GetComponent<Transform>().position;
                    if (objPos != null)
                    {
                        // 対象オブジェクトの方を見る
                        LookAtObject(objPos);
                    }
                    AudioManager.Instance.PlaySE(6);
                    light.Action(this.transform);
                    break;
                }
            }
		}
	}

	private void LookAtObject(Vector3 target)
	{
		// 高さの要素は自身のを使用
		Vector3 lookPos = target;
		lookPos.y = this.transform.position.y;

		this.transform.LookAt(target);
	}
}