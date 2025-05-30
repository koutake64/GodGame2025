using UnityEditor.Build;
using UnityEngine;

public class LightObject : MonoBehaviour
{
    public enum LightDirection
    {
        Left = -1,  // 左にずらした際に左に回転させたいから-1スタート
        Center,
        Right,
    }

    [Header("正面を向いている際に照らすエリア")]
    [SerializeField] private Vector2Int illuminateRange;

    [Header("方向をずらした際の回転角度")]
    [SerializeField, Range(0, 90)] private float rotateAngle;

    private _FieldDataManager   fieldData;  // _FieldDataManager
    private LightDirection      direction;  // 現在の方向
    private Vector2Int          pos;        // オブジェクトのマス
    private Vector2Int          fieldSize;  // フィールドサイズ

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        fieldData = GameObject.Find("Field").GetComponentInChildren<_FieldDataManager>();
        if (!fieldData)
        {
            Debug.LogError(
                "Script:LightObject.cs \n" +
                "fieldがnullです"
            );
        }

        direction = LightDirection.Center;
        pos = new Vector2Int((int)transform.position.x, (int)transform.position.z);
        fieldSize = fieldData.GetFieldSize();
    }

    // Update is called once per frame
    void Update()
    {


    }

    public void Action(Transform playerTransform)
    {
        // プレイヤーがライトに対してどの位置にいるか計算
        Vector2 lightForward    = new Vector2(transform.forward.x, transform.forward.z).normalized;
        Vector2 toPlayer        = new Vector2(playerTransform.position.x - transform.position.x, playerTransform.position.z - transform.position.z).normalized;
        float dot   = Vector2.Dot(lightForward, toPlayer);
        float cross = lightForward.x * toPlayer.x - lightForward.y * toPlayer.y;

        if (dot > 0) // ライトの前方
        {
            if(cross > 0) // ライトの左側
            {
                ChangeDirection((int)direction + LightDirection.Left);
            }
            else if(cross < 0) // ライトの右側
            {
                ChangeDirection((int)direction + LightDirection.Right);
            }
        }
        else if(dot < 0) // ライトの後方
        {
            if (cross > 0) // ライトの左側
            {
                ChangeDirection((int)direction + LightDirection.Right);
            }
            else if (cross < 0) // ライトの右側
            {
                ChangeDirection((int)direction + LightDirection.Left);
            }
        }
        else // 真横   
        {
            if (cross > 0) // ライトの左側
            {
                ChangeDirection((int)direction + LightDirection.Right);
            }
            else if (cross < 0) // ライトの右側
            {
                ChangeDirection((int)direction + LightDirection.Left);
            }
        }
    }
    public void ChangeDirection(LightDirection newDirection)
    {
        // 現在の方向と新しい方向で計算して方向を変えていいか計算する
        int num = (int)(direction) + (int)(newDirection);
        if(num < -1 || num > 1)
        {
            return;
        }

        // 角度を更新
        direction = newDirection;
        transform.rotation = Quaternion.Euler(0.0f, (float)direction * rotateAngle, 0.0f);
        
        // 影の位置を再計算
        CalcShadow();
    }

    private void CalcShadow()
    {
        if (direction == LightDirection.Center)
        {
            // 向いている方向
            Vector3 forward = transform.forward.normalized;
            Vector2Int lightDir;
            if (Mathf.Abs(forward.x) > Mathf.Abs(forward.z))
            {
                lightDir = forward.x > 0 ? Vector2Int.right : Vector2Int.left;
            }
            else
            {
                lightDir = forward.z > 0 ? Vector2Int.up : Vector2Int.down;
            }

            // 障害物があった際の影フラグ
            bool isShadow = false;

            for (int i = -1; i < illuminateRange.x - 1; ++i)
            {
                for(int j = 0; j < illuminateRange.y; ++j)
                {
                    // 横方向を取得
                    Vector2Int lateralDir = new Vector2Int(-lightDir.y, lightDir.x);

                    // 対象マスの座標を計算
                    Vector2Int targetPos = pos + lightDir * (j + 1) + lateralDir * i;

                    // 範囲外チェック
                    if (targetPos.x < 0 || targetPos.x >= fieldSize.x || targetPos.y < 0 || targetPos.y >= fieldSize.y)
                    {
                        continue;
                    }

                    // 影フラグが立っていたら
                    if(isShadow)
                    {
                        // フィールドに影情報を登録する


                        isShadow = false;
                    }

                    // マスの情報を取得
                    var infoArray = fieldData.GetInfoList(targetPos);
                    foreach (var info in infoArray)
                    {
                        if(info.state == _FieldDataManager.E_FIELDSTATE.pillar)
                        {
                            // このマスの影設定を解除

                            isShadow = true;
                        }
                    }
                }
            }
        }
    }
}