using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class CharacterMoveController : MonoBehaviour
{
    [Header("スタート座標")]
    [SerializeField] private Vector2Int startPos;

    [Header("自動移動")]
    [SerializeField] private bool isAutoMove = true;


    private FieldDataManager    fieldData;      // FieldDataManager
    GameSystem                  system;         // GameSystem
    RouteSearch                 routeSearch;    // routeSearch
    private float               moveSpeed;      // 移動速度
    private float               rotateSpeed;    // 回転速度
    private Vector2Int          currentPos;     // 現在のマス
    private new Transform       transform;      // Transform
    private Vector3             targetPos;      // 目標座標
    bool                        isMove;         // 移動するか
    Vector2Int                  fieldSize;      // フィールドサイズ
    private Queue<Vector2Int>   moveRoute;      // 移動経路

    // Start is called once before the first execution of Update after the MonoBehaviour is created
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

        fieldData = GameObject.Find("Field").GetComponentInChildren<FieldDataManager>();
        if (!fieldData)
        {
            Debug.LogError(
               "Script:CharacterMoveController.cs \n" +
               "fieldDataがnullです"
            );
        }

        routeSearch = this.GetComponent<RouteSearch>();
        if (!routeSearch)
        {
            Debug.LogError(
               "Script:CharacterMoveController.cs \n" +
               "routeSearchがnullです"
            );
        }

        // 移動系変数の初期化
        moveSpeed = system.GetCharacterMoveSpeed();
        rotateSpeed = system.GetCharacterRotateSpeed();
        currentPos = new Vector2Int( 0, 0);
        transform = GetComponent<Transform>();
        isMove = false;
        fieldSize = system.GetFieldSize();
        moveRoute = new Queue<Vector2Int>();

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
    private void FixedUpdate()
    {
        // 移動するなら
        if (isMove)
        {
            // 指定速度で移動
            transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);

            // 移動終了
            if (Vector3.Distance(transform.position, targetPos) <= 0.1f)
            {
                isMove = false;

                // 自動移動trueなら次のマスをセット
                if(isAutoMove)
                {
                    MoveNextStep();
                }
            }
        }
    }
    private void UpdateTargetPosition()
    {
        // 移動先の情報取得
        var info = fieldData.GetInfo(new Vector2(currentPos.x, currentPos.y));

        // 移動先更新
        targetPos = info.obj.transform.position;
    }

    public void AddPosX(int num)
    {
        // 移動中・自動移動中なら終了
        if (isMove || isAutoMove) return;

        currentPos.x += num;
        isMove = true;

        // 通れるか判定
        var info = fieldData.GetInfo(new Vector2(currentPos.x, currentPos.y));
        if (info.state != FieldDataManager.E_FIELDSTATE.none)
        {
            currentPos.x -= num;
            isMove = false;
            return;
        }
        if (currentPos.x < 0)
        {
            currentPos.x = 0;
            isMove = false;
            return;
        }
        if (currentPos.x > fieldSize.x - 1)
        {
            currentPos.x = fieldSize.x - 1;
            isMove = false;
            return;
        }

        // 移動情報更新
        UpdateTargetPosition();
    }
    public void AddPosY(int num)
    {
        // 移動中・自動移動中なら終了
        if (isMove || isAutoMove) return;

        currentPos.y += num;
        isMove = true;

        // 通れるか判定
        var info = fieldData.GetInfo(new Vector2(currentPos.x, currentPos.y));
        if (info.state != FieldDataManager.E_FIELDSTATE.none)
        {
            currentPos.y -= num;
            isMove = false;
            return;
        }

        if (currentPos.y < 0)
        {
            currentPos.y = 0;
            isMove = false;
            return;
        }
        if (currentPos.y > fieldSize.y - 1)
        {
            currentPos.y = fieldSize.y - 1;
            isMove = false;
            return;
        }

        // 移動情報更新
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

    public void StartAutoMove(Vector2Int start, Vector2Int goal)
    {
        if (isAutoMove) return;
        isAutoMove = true;
        // 経路探索
        SetMoveRoute(routeSearch.MoveRouteSearch(start, goal));
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
            isAutoMove = false;

            // TODO プロト終わったら消す
            routeSearch.ResetTileColor();

            return;
        }

        // 次ルートをセット
        Vector2Int next = moveRoute.Dequeue();
        currentPos.x = next.x;
        currentPos.y = next.y;

        // 座標更新
        isMove = true;
        UpdateTargetPosition();
    }

    public void IsAutoMove(bool flg)
    {
        isAutoMove = flg;
    }

    public Vector2Int GetCurrentPos()
    {
        return currentPos;
    }
}