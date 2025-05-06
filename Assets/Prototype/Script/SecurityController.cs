using UnityEngine;
using System.Collections.Generic;

public class SecurityController : MonoBehaviour
{
    [Header("移動ターゲットリスト")]
    [SerializeField] private List<Vector2Int> targetArray = new List<Vector2Int>();

    [Header("前方監視範囲")]
    [SerializeField] private int monitoringRange;

    private CharacterMoveController moveController; // CharacterMoveController
    private _FieldDataManager       fieldData;      // _FieldDataManager
    private bool                    isEndMovement;  // 目標座標までの移動終了したか
    private int                     currentIndex;   // 配列の何番目か
    private int                     addNum;         // 加算する値
    Vector2Int                      fieldSize;      // フィールドサイズ

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        moveController = GetComponent<CharacterMoveController>();
        if (!moveController)
        {
            Debug.LogError(
               "Script:SecurityController.cs \n" +
               "moveControllerがnullです"
            );
        }

        fieldData = GameObject.Find("Field").GetComponent<_FieldDataManager>();
        if (!fieldData)
        {
            Debug.LogError(
               "Script:CharacterMoveController.cs \n" +
               "fieldDataがnullです"
            );
        }

        isEndMovement = true;
        currentIndex = 0;
        addNum = 1;
        fieldSize = fieldData.GetFieldSize();
    }

    // Update is called once per frame
    void Update()
    {
        // 進行方向に対してチェックを行う
        ForwardMonitoring();

        // 移動が終了していたら
        if (isEndMovement && targetArray.Count > 0)
        {
            // 移動先を指定
            moveController.StartAutoMove(moveController.GetCurrentPos(), targetArray[currentIndex]);

            // 移動終了フラグを下げる
            isEndMovement = false;
        }
    }

    public void EndMovement()
    {
        // 巡回するように配列番号を更新
        currentIndex = (currentIndex + addNum) % targetArray.Count;

        if(currentIndex < 0)
        {
            currentIndex = targetArray.Count - 1;
        }

        isEndMovement = true;
    }

    public void InverseArray()
    {
        addNum *= -1;
    }

    private void ForwardMonitoring()
    {
        // 向いている方向
        Vector3 forward = transform.forward;
        forward.Normalize();
        Vector2Int direction;
        if(Mathf.Abs(forward.x) > Mathf.Abs(forward.z))
        {
            direction = forward.x > 0 ? Vector2Int.right : Vector2Int.left;
        }
        else
        {
            direction = forward.z > 0 ? Vector2Int.up : Vector2Int.down;
        }

        // 監視
        Vector2Int currentPos = moveController.GetCurrentPos();
        for(int i = 0; i < monitoringRange; ++i)
        {
            // マス目の情報取得
            Vector2Int pos = currentPos + direction * (i + 1);

            // 範囲外判定
            if(pos.x < 0 || pos.x >= fieldSize.x || pos.y < 0 || pos.y >= fieldSize.y)
            {
                continue;
            }

            var info = fieldData.GetInfoList(pos);
            for(int j = 0; j < info.Count; ++j)
            {
                // お姫様を発見
                if (info[j].state == _FieldDataManager.E_FIELDSTATE.princess)
                {

                }

                // 貫通しないオブジェクトの場合
                if (info[j].state == _FieldDataManager.E_FIELDSTATE.wall || 
                    info[j].state == _FieldDataManager.E_FIELDSTATE.pillar ||
                    info[j].state == _FieldDataManager.E_FIELDSTATE.exhibitionStand)
                {
                    break;
                }
            }
        }
    }
}