using System.Collections.Generic;
using UnityEngine;

public class CharacterMoveController : MonoBehaviour
{
    [Header("キャラクタータイプ")]
    [SerializeField] private _FieldDataManager.E_FIELDSTATE charaState;

    [Header("移動速度")]
    [SerializeField] private float moveSpeed = 2.0f;

    private _FieldDataManager   fieldData;      // _FieldDataManager
    private SecurityController  security;       // SecurityController
    private GameSystem          system;         // GameSystem
    private TimeManager         timeManager;    // TimeManager
    private RouteSearch         routeSearch;    // routeSearch
    private Vector2Int          currentPos;     // 現在のマス
    private Vector2Int          prevPos;        // 過去マス
    private new Transform       transform;      // Transform
    private Vector3             targetPos;      // 目標座標
    private bool                isMove;         // 移動するか
    private Vector2Int          fieldSize;      // フィールドサイズ
    private Queue<Vector2Int>   moveRoute;      // 移動経路
    private bool                isAutoMoving;   // 自動移動中か
    private bool                isStop;         // 動きを止めるか
    private bool                isFrontChara;   // 前方にキャラがいるか
    private int                 characterID;    // キャラクターID

    void Start()
    {
        fieldData = GameObject.Find("Field").GetComponent<_FieldDataManager>();
        if (!fieldData)
        {
            Debug.LogError(
               "Script:CharacterMoveController.cs \n" +
               "fieldDataがnullです"
            );
        }

        routeSearch = GetComponent<RouteSearch>();
        if (!routeSearch)
        {
            Debug.LogError(
               "Script:CharacterMoveController.cs \n" +
               "routeSearchがnullです"
            );
        }

        // 移動系変数の初期化
        transform = GetComponent<Transform>();
        prevPos = currentPos = new Vector2Int((int)transform.position.x, (int)transform.position.z);
        isMove = false;
        fieldSize = fieldData.GetFieldSize();
        moveRoute = new Queue<Vector2Int>();
        security = GetComponent<SecurityController>();
        isAutoMoving = false;
        isStop = false;
        isFrontChara = false;

        // nullチェック
        if (!transform)
        {
            Debug.LogError(
               "Script:CharacterMoveController.cs \n" +
               "transformがnullです"
            );
        }

		// 時間関係
		timeManager = FindFirstObjectByType<TimeManager>();
	}

    // Update is called once per frame
    private void Update()
    {
        // 止めるフラグが立っていたら終了
        if (isStop) return;

        if(isFrontChara)
        {
            MoveNextStep();
            return;
        }

        // 移動するなら
        if (isMove)
        {
            // 移動方向を計算
            Vector3 moveDirection = (targetPos - transform.position).normalized;

            // 移動方向がある場合
            if (moveDirection.sqrMagnitude > 0.001f)
            {
                // 現在の向き
                Vector3 currentForward = transform.forward;

                // 移動方向との角度を計算
                float angle = Vector3.SignedAngle(currentForward, moveDirection, Vector3.up);

                // 角度を90度の倍数に丸める
                float roundedAngle = Mathf.Round(angle / 90.0f) * 90.0f;

                // 現在の回転角度
                float currentYAngle = transform.eulerAngles.y;

                // 目標の回転角度を計算
                float targetYAngle = currentYAngle + roundedAngle;

                // 回転を適用
                transform.rotation = Quaternion.Euler(0, targetYAngle, 0);
            }

            // 指定速度で移動
            transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);

            // 移動終了
            if (Vector3.Distance(transform.position, targetPos) <= 0.001f)
            {
                // 移動フラグを下げる
                isMove = false;

                // 座標をぴったりにする
                this.transform.position = targetPos;

                if(security)
                {
                    if(security.StartMoveFoundPos())
                    {
                        return;
                    }
                }

                MoveNextStep();
            }
        }
    }
    private void UpdateTargetPosition()
    {
        // 移動先更新
        targetPos = new Vector3(currentPos.x, 0, currentPos.y);
    }

    public void AddPosX(int num)
    {
        // 移動中・自動移動中なら終了
        if (isMove || isAutoMoving) return;

        // 目標座標
        Vector2Int targetPos = new Vector2Int(currentPos.x + num, currentPos.y);

        // 通れるか判定
        if(!fieldData.GetIsThrough(targetPos))
        {
            return;
        }

        // 移動先にキャラがいたら終了
        if (IsFrontChara(targetPos))
        {
            return;
        }

        // 各座標更新
        prevPos = currentPos;
        currentPos.x += num;

        // 移動先に自身の情報登録
        fieldData.MoveInfo(prevPos, currentPos, charaState, characterID);

        // 移動情報更新
        isMove = true;
        UpdateTargetPosition();
    }
    public void AddPosY(int num)
    {
        // 移動中・自動移動中なら終了
        if (isMove || isAutoMoving) return;

        // 目標座標
        Vector2Int targetPos = new Vector2Int(currentPos.x, currentPos.y + num);

        // 通れるか判定
        if (!fieldData.GetIsThrough(targetPos))
        {
            return;
        }

        // 移動先にキャラがいたら終了
        if (IsFrontChara(targetPos))
        {
            return;
        }

        // 各座標更新
        prevPos = currentPos;
        currentPos.y += num;

        // 移動先に自身の情報登録
        fieldData.MoveInfo(prevPos, currentPos, charaState, characterID);

        // 移動情報更新
        isMove = true;
        UpdateTargetPosition();
    }

    public void SetPos(Vector2Int pos)
    {
        if (pos.x < 0 || pos.x >= fieldSize.x || pos.y < 0 || pos.y >= fieldSize.y)
        {
            Debug.Log(
              "Script:CharacterMoveController.cs \n" +
              gameObject.name + "の座標が範囲外です"
            );
        }

        // 移動可能か判定
        if (!fieldData.GetIsThrough(pos))
        {
            Debug.Log(
             "Script:CharacterMoveController.cs \n" +
             gameObject.name + "の座標が設定不可です"
            );

            return;
        }

        // 過去座標更新
        prevPos = currentPos;

        // 現在位置更新
        currentPos = pos;

        // 座標設定
        transform.position = new Vector3(currentPos.x, 0, currentPos.y);

        // 移動先に自身の情報登録
        fieldData.MoveInfo(prevPos, currentPos, charaState, characterID);

        isMove = false;
        isAutoMoving = false;
        moveRoute.Clear();
    }

    public void StartAutoMove(Vector2Int goal)
    {
        // 移動中の場合は終了
        if (isMove) return;

        // 自動移動中フラグをあげる
        isAutoMoving = true;

        // 経路探索
        List<Vector2Int> route = new List<Vector2Int>(routeSearch.MoveRouteSearch(currentPos, goal));

        // 探索経路セット
        SetMoveRoute(route);
    }

    public void SetMoveRoute(List<Vector2Int> route)
    {
        // 新しいルート
        moveRoute.Clear();
        moveRoute = new Queue<Vector2Int>(route);
        MoveNextStep();
    }
    private void MoveNextStep()
    {
        // ゴール到達
        if (moveRoute.Count == 0)
        {
            // 移動終了
            isMove = false;
            isAutoMoving = false;

            // 警備員なら
            if(security)
            {
                security.EndMovement();
            }

            return;
        }

        // 移動先にキャラがいたら終了
        if (IsFrontChara(moveRoute.Peek()))
        {
            isFrontChara = true;
            return;
        }

        // 前方にキャラがいるフラグを下げる
        isFrontChara = false;

        // 目標マスを取得
        Vector2Int next = moveRoute.Dequeue();
        
        // 過去座標を更新
        prevPos = currentPos;

        // 次のマスをセット
        currentPos = next;

        // 移動先に自身の情報登録
        fieldData.MoveInfo(prevPos, currentPos, charaState, characterID);

        // 座標更新
        isMove = true;
        UpdateTargetPosition();

        // 移動SE
        if (charaState == _FieldDataManager.E_FIELDSTATE.princess)
        {
            AudioManager.Instance.PlaySE(0);
        }
        else if (charaState == _FieldDataManager.E_FIELDSTATE.butler)
        {
            AudioManager.Instance.PlaySE(1);
        }
        else if (charaState == _FieldDataManager.E_FIELDSTATE.securityGuard_N)
        {
            // 足音が聞こえるフラグが立っていれば
            if (security.GetIsFoodStepsFlg())
            {
                AudioManager.Instance.PlaySE(2);
            }
        }
    }
 
    public bool GetAutoMove()
    {
        return isAutoMoving;
    }

    public Vector2Int GetCurrentPos()
    {
        return currentPos;
    }

    public Vector2Int GetPrevPos()
    {
        return prevPos;
    }
    public void Stop()
    {
        isStop = true;
    }

    public void ReStart()
    {
        isStop = false;
    }

    public bool GetIsMove()
    {
        return isMove;
    }

    public bool GetIsStop()
    {
        return isStop;
    }

    public void SetID(int id)
    {
        characterID = id;
    }

    private bool IsFrontChara(Vector2Int nextPos)
    {
        // 移動先のオブジェクト取得
        var objList = fieldData.GetInfoList(nextPos);
        foreach (var obj in objList)
        {
            // 朝はお嬢様と執事をかぶってもいいようにする
            if (timeManager.GetCurState() == CommonSE_Proto.E_TIMEOFDAY.afternoon && 
                charaState == _FieldDataManager.E_FIELDSTATE.butler     && obj.state == _FieldDataManager.E_FIELDSTATE.princess ||
                charaState == _FieldDataManager.E_FIELDSTATE.princess   && obj.state == _FieldDataManager.E_FIELDSTATE.butler)
            {
                return false;
            }

            // 夜は執事の座標を通れるようにする
            if(timeManager.GetCurState() == CommonSE_Proto.E_TIMEOFDAY.night && obj.state == _FieldDataManager.E_FIELDSTATE.butler)
            {
                return false;
            }

            if (obj.state == _FieldDataManager.E_FIELDSTATE.butler ||
                obj.state == _FieldDataManager.E_FIELDSTATE.princess ||
                obj.state == _FieldDataManager.E_FIELDSTATE.securityGuard_N)
            {
                return true;
            }
        }
        return false;
    }
}