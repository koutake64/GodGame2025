using System.Reflection;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class CharacterMoveController : MonoBehaviour
{
    [Header("GameSystemObject")]
    [SerializeField] private GameSystem system;

    [Header("FileDataManager")]
    [SerializeField] private FieldDataManager fieldData;

    private float           moveSpeed;    // 移動速度
    private float           rotateSpeed;  // 回転速度
    private int             currentPosX;  // 現在のXマス
    private int             currentPosY;  // 現在のYマス
    private new Transform   transform;    // Transform
    private Transform       targetPos;    // 目標座標
    bool                    isMove;       // 移動するか

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // nullチェック
        if(!system)
        {
            Debug.LogError(
               "Script:CharacterMoveController.cs \n" +
               "systemがnullです"
               );
        }
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

    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        // 移動するなら
        if (isMove)
        {
            // 指定速度で移動
            transform.position = Vector3.MoveTowards(transform.position, targetPos.position, moveSpeed * Time.deltaTime);

            // 移動終了
            if (Vector3.Distance(transform.position, targetPos.position) <= 0.1f) isMove = false;
        }
    }
    private void UpdateTargetPosition()
    {
        // 移動先の情報取得
        FieldDataManager.S_FIELDINFO state = fieldData.GetInfo(new Vector2(currentPosX, currentPosY));

        // 移動可能か判定
        if(state.state != FieldDataManager.E_FIELDSTATE.none)
        {
            isMove = false;
            return;
        }

        // 移動先更新
        targetPos.position = state.obj.transform.position;
    }

    public void AddPosX(int num)
    {
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
}