using System.Reflection;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class CharacterMoveController : MonoBehaviour
{
    [Header("GameSystemObject.script")]
    [SerializeField] private GameSystem system;

    [Header("FileDataManager.script")]
    [SerializeField] private FieldDataManager fieldData;

    [Header("スタート座標")]
    [SerializeField] private Vector2 startPos;

    private float           moveSpeed;    // 移動速度
    private float           rotateSpeed;  // 回転速度
    private int             currentPosX;  // 現在のXマス
    private int             currentPosY;  // 現在のYマス
    private new Transform   transform;    // Transform
    private Vector3         targetPos;    // 目標座標
    bool                    isMove;       // 移動するか

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

        // 移動系変数の初期化
        moveSpeed = system.GetCharacterMoveSpeed();
        rotateSpeed = system.GetCharacterRotateSpeed();
        currentPosX = currentPosY = 0;
        transform = GetComponent<Transform>();
        isMove = false;

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
            if (Vector3.Distance(transform.position, targetPos) <= 0.1f) isMove = false;
        }
    }
    private void UpdateTargetPosition()
    {

        // 移動先の情報取得
        FieldDataManager.S_FIELDINFO state = fieldData.GetInfo(new Vector2(currentPosX, currentPosY));

        // 移動可能か判定
        if (state.state != FieldDataManager.E_FIELDSTATE.none)
        {
            isMove = false;
            return;
        }

        // 移動先更新
        targetPos = state.obj.transform.position;
    }

    public void AddPosX(int num)
    {
        // 移動中なら終了
        if (isMove) return;

        currentPosX += num;
        isMove = true;
        if (currentPosX < 0)
        {
            currentPosX = 0;
            isMove = false;
            return;
        }

        // 移動情報更新
        UpdateTargetPosition();
    }
    public void AddPosY(int num)
    {
        // 移動中なら終了
        if (isMove) return;

        currentPosY += num;
        isMove = true;
        if (currentPosY < 0)
        {
            currentPosY = 0;
            isMove = false;
            return;
        }

        // 移動情報更新
        UpdateTargetPosition();
    }

    private void SetPos(Vector2 pos)
    {
        if (pos.x < 0 || pos.y < 0)
        {
            Debug.LogError(
              "Script:CharacterMoveController.cs \n" +
              gameObject.name + "のスタート座標が範囲外です"
              );
        }

        // 現在位置更新
        currentPosX = (int)pos.x;
        currentPosY = (int)pos.y;

        // 移動先の情報取得
        FieldDataManager.S_FIELDINFO state = fieldData.GetInfo(new Vector2(currentPosX, currentPosY));

        // 移動可能か判定
        if (state.state != FieldDataManager.E_FIELDSTATE.none)
        {
            Debug.LogError(
             "Script:CharacterMoveController.cs \n" +
             gameObject.name + "のスタート座標が設定不可です"
             );

            return;
        }

        // 座標設定
        transform.position = state.obj.transform.position;
    }
}