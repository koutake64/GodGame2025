using UnityEngine;
using System.Collections;

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
    private GameSystem gameSystem;
    private UIManager uiManager;
    private TreasureEffect treasureEffect;
    private bool initNightPos = false;

    private Camera mainCamera;
    private Transform princess;
    private Transform treasure;
    private Transform security;

    [Header("***ゴール演出***")]

    [Header("演出中のカリングマスク")]
    [SerializeField] private LayerMask cinematicCullingMask;

    [Header("プリンセスとのオフセット")]
    [SerializeField] private Vector3 cameraOffset = new Vector3(0, 3f, -10f);

    [Header("お宝の頭上オフセット")]
    [SerializeField] private Vector3 treasureOffset0 = new Vector3(0, 3f, 0);
    [SerializeField] private Vector3 treasureOffset1 = new Vector3(0, 1f, -1f);

    [Header("宝物の回転速度")]
    [SerializeField] private float spinSpeed = 60f;

    [Header("ズームイン設定")]
    [SerializeField] private float targetFOV = 20f;
    [SerializeField] private float zoomSpeed = 5f;

    private bool isCinematic = false;
    private bool isZooming = false;
    private float cameraMoveSpeed = 5f;
    private float treasureMoveSpeed = 3f;

    void Start()
    {
        mainCamera = Camera.main;
        transform.eulerAngles = fixedRotation;
        timeManager = FindFirstObjectByType<TimeManager>();
        fieldDataManager = FindFirstObjectByType<_FieldDataManager>();
        gameSystem = FindFirstObjectByType<GameSystem>();
        uiManager = FindFirstObjectByType<UIManager>();
    }

    void LateUpdate()
	{
        if (gameSystem.GetIsGoal()) return; // ゴール演出中はカメラの動きを停止

        switch (timeManager.CurrentState)
		{
			case CommonSE_Proto.E_TIMEOFDAY.night:
                Vector2Int fieldSize = fieldDataManager.GetFieldSize();
                if (princessTransform == null)
                {
                    GameObject princessObj = GameObject.FindWithTag(princessTag);
                    if (princessObj != null)
                    {
                        princessTransform = princessObj.transform;

                        // プレイヤー初期位置が左端 or 右端か確認し、カメラ初期位置を調整
                        fieldSize = fieldDataManager.GetFieldSize();
                        float princessX = princessTransform.position.x;

                        if (princessX < fieldSize.x / 2)
                        {
                            // 左端スタート：右に5マス離す
                            transform.position = princessTransform.position + new Vector3(5, offsetPosition.y, offsetPosition.z);
                        }
                        else
                        {
                            // 右端スタート：左に5マス離す
                            transform.position = princessTransform.position + new Vector3(-5, offsetPosition.y, offsetPosition.z);
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
                float princessXPos = princessTransform.position.x;
                Vector3 targetPosDay = princessTransform.position + offsetPosition;

                if (princessXPos < 5 || princessXPos > fieldSizeDay.x - 6)
                {
                    targetPosDay.x = transform.position.x;
                }

                transform.position = targetPosDay;
                break;

			case CommonSE_Proto.E_TIMEOFDAY.morning:
				PlayerStartPos();
				break;
			case CommonSE_Proto.E_TIMEOFDAY.noon:
				PlayerStartPos();
				break;
			case CommonSE_Proto.E_TIMEOFDAY.afternoon:
				PlayerStartPos();
				break;
		}
	}

	private void PlayerStartPos()
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

    void Update()
    {
        if (!isCinematic || princess == null || treasure == null) return;

        Vector3 lookTarget = transform.position;
        lookTarget.y = princess.position.y; // 高さを同じにしてY軸回転だけにする
        princess.transform.LookAt(lookTarget);

        // お宝を頭上へ移動
        Vector3 targetPos1 = princess.position + treasureOffset1;
        treasure.position = Vector3.MoveTowards(treasure.position, targetPos1, treasureMoveSpeed * Time.deltaTime);

        // お宝が頭上に到達したらズームイン
        if (!isZooming && Vector3.Distance(treasure.position, targetPos1) < 0.1f)
        {
            isZooming = true;
        }

        // カメラズーム
        if (isZooming)
        {
            mainCamera.fieldOfView = Mathf.MoveTowards(mainCamera.fieldOfView, targetFOV, zoomSpeed * Time.deltaTime);
            // 宝物をくるくる回転
            treasure.Rotate(Vector3.up * spinSpeed * Time.deltaTime, Space.World);
            Invoke("GameClear", 6.0f); // ゲームクリアUI表示)
        }
    }

    public void StartCinematic()
    {
        // カリングマスク変更
        Camera.main.cullingMask = cinematicCullingMask;

        // Princess取得
        GameObject princessObj = GameObject.FindWithTag("Princess");
        if (princessObj != null)
            princess = princessObj.transform;
        else
            Debug.LogError("Princessタグがついたオブジェクトが見つかりません");

        // Treasure取得
        GameObject treasureObj = GameObject.FindWithTag("Treasure");
        if (treasureObj != null)
        {
            treasure = treasureObj.transform;

            // 子オブジェクトも含めて TreasureEffect を取得
            treasureEffect = treasure.GetComponentInChildren<TreasureEffect>();
            if (treasureEffect == null)
            {
                Debug.LogError("TreasureEffect コンポーネントが Treasure またはその子に見つかりません");
            }
        }
        else
        {
            Debug.LogError("Treasureタグがついたオブジェクトが見つかりません");
        }

        // 警備員削除
        int securityLayer = LayerMask.NameToLayer("Security");
        var allObjects = GameObject.FindObjectsByType<Transform>(FindObjectsSortMode.None);
        foreach (var obj in allObjects)
        {
            if (obj.gameObject.layer == securityLayer)
                Destroy(obj.gameObject);
        }

        // エフェクト再生
        treasureEffect?.StartGetEffect();

        // カメラ演出開始
        StartCoroutine(MoveCameraToPrincess());
    }

    private IEnumerator MoveCameraToPrincess()
    {
        yield return new WaitForSeconds(1.0f); // 少し待機してから開始

        Vector3 targetPos = princess.position + cameraOffset;

        // カメラ移動が完了するまでループ
        while (Vector3.Distance(transform.position, targetPos) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, cameraMoveSpeed * Time.deltaTime);
            transform.LookAt(new Vector3(princess.position.x, princess.position.y + 2.0f, princess.position.z));
            Vector3 targetPos0 = princess.position + treasureOffset0;
            treasure.position = Vector3.MoveTowards(treasure.position, targetPos0, treasureMoveSpeed * Time.deltaTime);
            yield return null;
        }

        // 最終調整（ズレ補正）
        transform.position = targetPos;
        transform.LookAt(new Vector3(princess.position.x, princess.position.y + 2.0f, princess.position.z));

        // 演出フラグON
        isCinematic = true;
    }

    private void GameClear()
    {
        if (uiManager != null)
        {
            uiManager.SetUIActive(UIManager.E_UI_KIND.gameClear, true);
        }
        else
        {
            Debug.LogWarning("UIManagerが見つかりません。ゲームクリアUIを表示できません。");
        }
        // カメラのカリングマスクを元に戻す
        Camera.main.cullingMask = -1; // 全レイヤーを表示
        isCinematic = false;
        isZooming = false;
    }
}

