using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class CharacterMoveController : MonoBehaviour
{
    [Header("スタート座標")]
    [SerializeField] private Vector2Int startPos;

    private FieldDataManager   fieldData;      // _FieldDataManager
    private SecurityController  security;       // SecurityController
    private GameSystem          system;         // GameSystem
    private RouteSearch         routeSearch;    // routeSearch
    private float               moveSpeed;      // 移動速度
    private Vector2Int          currentPos;     // 現在のマス
    private new Transform       transform;      // Transform
    private Vector3             targetPos;      // 目標座標
    private bool                isMove;         // 移動するか
    private Vector2Int          fieldSize;      // フィールドサイズ
    private Queue<Vector2Int>   moveRoute;      // 移動経路
    private bool                isAutoMoving;   // 自動移動中か   

    void Start()
    {
        system = GameObject.Find("GameSystem").GetComponent<GameSystem>();
        // nullチェック
        if(!system)
        {
            Debug.LogError(
               "Script:CharacterMoveController.cs \n" +
               "systemがnullです"
            );
        }

        fieldData = GameObject.Find("Field").GetComponent<FieldDataManager>();
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
        moveSpeed = system.GetCharacterMoveSpeed();
        currentPos = new Vector2Int(0, 0);
        transform = GetComponent<Transform>();
        isMove = false;
        fieldSize = system.GetFieldSize();
        moveRoute = new Queue<Vector2Int>();
        security = GetComponent<SecurityController>();
        isAutoMoving = false;

        // nullチェック
        if (!transform)
        {
            Debug.LogError(
               "Script:CharacterMoveController.cs \n" +
               "transformがnullです"
            );
        }

        // プレイヤー配置
        SetPos(startPos);
    }

    // Update is called once per frame
    private void Update()
    {
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
            if (Vector3.Distance(transform.position, targetPos) <= 0.1f)
            {
                isMove = false;

                MoveNextStep();
            }
        }
    }
    private void UpdateTargetPosition()
    {
        // 移動先の情報取得
        var info = fieldData.GetInfo(new Vector2(currentPos.x, currentPos.y));

        // 移動先更新
        targetPos = info.obj.transform.position;

        // TODO いるマスの更新
        // ここに自身の情報とcurrentPosでいるマスを設定する

    }

    public void AddPosX(int num)
    {
        // 移動中・自動移動中なら終了
        if (isMove || isAutoMoving) return;

        // フィールド情報リセット
        ResetFieldData();

        currentPos.x += num;

        // 通れるか判定
        var info = fieldData.GetInfo(new Vector2(currentPos.x, currentPos.y));
        if (info.state != FieldDataManager.E_FIELDSTATE.none && 
            info.state != FieldDataManager.E_FIELDSTATE.cameraRange)
        {
            currentPos.x -= num;
            return;
        }
        if (currentPos.x < 0)
        {
            currentPos.x = 0;
            return;
        }
        if (currentPos.x > fieldSize.x - 1)
        {
            currentPos.x = fieldSize.x - 1;
            return;
        }

        // 移動情報更新
        isMove = true;
        UpdateTargetPosition();
    }
    public void AddPosY(int num)
    {
        // 移動中・自動移動中なら終了
        if (isMove || isAutoMoving) return;

        // フィールド情報リセット
        ResetFieldData();

        currentPos.y += num;

        // 通れるか判定
        var info = fieldData.GetInfo(new Vector2(currentPos.x, currentPos.y));
        if (info.state != FieldDataManager.E_FIELDSTATE.none &&
            info.state != FieldDataManager.E_FIELDSTATE.cameraRange)
        {
            currentPos.y -= num;
            return;
        }

        if (currentPos.y < 0)
        {
            currentPos.y = 0;
            return;
        }
        if (currentPos.y > fieldSize.y - 1)
        {
            currentPos.y = fieldSize.y - 1;
            return;
        }

        // 移動情報更新
        isMove = true;
        UpdateTargetPosition();
    }

    private void SetPos(Vector2Int pos)
    {
        if (pos.x < 0 || pos.y < 0)
        {
            Debug.LogError(
              "Script:CharacterMoveController.cs \n" +
              gameObject.name + "のスタート座標が範囲外です"
            );
        }

        // 現在位置更新
        currentPos = pos;

        // 移動先の情報取得
        var info = fieldData.GetInfo(new Vector2(currentPos.x, currentPos.y));

        // 移動可能か判定
        if (info.state != FieldDataManager.E_FIELDSTATE.none)
        {
            Debug.LogError(
             "Script:CharacterMoveController.cs \n" +
             gameObject.name + "のスタート座標が設定不可です"
            );

            return;
        }

        // 座標設定
        transform.position = info.obj.transform.position;
    }

    public bool StartAutoMove(Vector2Int start, Vector2Int goal)
    {
        // 自動移動中フラグをあげる
        isAutoMoving = true;

        // 経路探索
        List<Vector2Int> route = new List<Vector2Int>(routeSearch.MoveRouteSearch(start, goal));

        // ルートがなかった場合
        if(route.Count == 0)
        {
            return false;
        }

        // 探索経路セット
        SetMoveRoute(route);

        return true;
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
        if(moveRoute.Count == 0)
        {
            // 移動終了
            isMove = false;
            isAutoMoving = false;

            // TODO プロト終わったら消す
            routeSearch.ResetTileColor();

            // 警備員なら
            if(security)
            {
                security.EndMovement();
            }

            return;
        }

        // いるマスの情報をリセット
        ResetFieldData();

        // 次ルートをセット
        Vector2Int next = moveRoute.Dequeue();
        currentPos.x = next.x;
        currentPos.y = next.y;

        // 座標更新
        isMove = true;
        UpdateTargetPosition();
    }

    private void ResetFieldData()
    {
        // TODO 
        // ここで一度currentPosを使っている座標をnoneで上書き
    }

    public void IsAutoMove(bool flg)
    {
        isAutoMoving = flg;
    }

    public Vector2Int GetCurrentPos()
    {
        return currentPos;
    }
}