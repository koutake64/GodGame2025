using UnityEngine;
using System.Collections.Generic;

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

    [Header("この値を超えたら斜めにしか影を作らない距離")]
    [SerializeField] private float diagonalShadowDistance;

    [Header("この距離以上になったら後ろにしか影にしない")]
    [SerializeField] private float backShadowDistance;

    [Header("影が2つになる角度")]
    [SerializeField] private float shadowAngle;


    private _FieldDataManager   fieldData;      // _FieldDataManager
    private LightDirection      direction;      // 現在の方向
    private Vector2Int          pos;            // オブジェクトのマス
    private Vector2Int          fieldSize;      // フィールドサイズ
    private List<Vector2Int>    shadowList;     // 影にする座標配列
    private Vector2             lightForward;   // ライトの進行方向
    private Vector2Int          lightDir;       // ライトの向き


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
        shadowList = new List<Vector2Int>();
        lightForward = new Vector2(transform.forward.x, transform.forward.z).normalized;


        // 向いている方向
        if (Mathf.Abs(lightForward.x) > Mathf.Abs(lightForward.y))
        {
            lightDir = lightForward.x > 0 ? Vector2Int.right : Vector2Int.left;
        }
        else
        {
            lightDir = lightForward.y > 0 ? Vector2Int.up : Vector2Int.down;
        }

        // 初期位置の影を計算
        CalcShadow();
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void Action(Transform playerTransform)
    {
        // プレイヤーがライトに対してどの位置にいるか計算
        Vector2 toPlayer = new Vector2(playerTransform.position.x - this.transform.position.x, playerTransform.position.z - this.transform.position.z).normalized;
        
        // 内積の計算により、ライトの向きに対しての位置関係を計算
        float dot = Vector2.Dot(lightForward, toPlayer);

        // しきい値で横にいても0.0fにならない場合に対応
        if(Mathf.Abs(dot) < 0.01)
        {
            dot = 0.0f;
        }
        
        // 外積の計算を用いてライトに対して左右どちらにいるか判定
        float cross = lightForward.x * toPlayer.y - lightForward.y * toPlayer.x;

        // 横にいる場合にのみ処理を行う
        if(dot == 0)
        {
            if (cross > 0) // ライトの左側
            {
                ChangeDirection(LightDirection.Right);
            }
            else if (cross < 0) // ライトの右側
            {
                ChangeDirection(LightDirection.Left);
            }
        }
    }
    public void ChangeDirection(LightDirection changeDirection)
    {
        LightDirection newDirection = (int)direction + changeDirection;

        // 現在の方向と新しい方向で計算して方向を変えていいか計算する
        if(Mathf.Abs((int)newDirection) > 1)
        {
            return;
        }

        // 角度を更新
        Vector3 currentAngle = transform.eulerAngles;

        // 向きを更新
        direction = newDirection;

        // 新しい方向を適用
        currentAngle.y += (int)changeDirection * rotateAngle;
        transform.eulerAngles = currentAngle;
        
        // 影の位置を再計算
        CalcShadow();
    }

    private void CalcShadow()
    {
        // リストに要素があれば削除処理を実行
        if (shadowList.Count > 0)
        {
            // 影を一度削除
            foreach (var pos in shadowList)
            {
                fieldData.RemoveInfo(pos, _FieldDataManager.E_FIELDSTATE.shadow);
            }
            shadowList.Clear();
        }

        // 方向に応じて加算する値を変更する
        Vector2Int lateralDir = new Vector2Int();
        switch (direction)
        {
            case LightDirection.Left:
                lateralDir = new Vector2Int(-lightDir.y, lightDir.x);
                break;
            case LightDirection.Right:
                lateralDir = new Vector2Int(lightDir.y, -lightDir.x);
                break;
            default:  // Center
                lateralDir = new Vector2Int(-lightDir.y, lightDir.x);
                break;
        }

        // 障害物があった際の影フラグ
        bool isShadow = false;

        // 向いている方向に応じた影の生成処理
        if (direction == LightDirection.Center)
        {
            for (int i = -1; i < illuminateRange.x - 1; ++i)
            {
                // 違う列に影が行かないようにフラグを下げる
                isShadow = false;

                for(int j = 0; j < illuminateRange.y; ++j)
                {
                    // 対象マスの座標を計算
                    Vector2Int targetPos = pos + lightDir * (j + 1) + lateralDir * i;

                    // 範囲外チェック
                    if (targetPos.x < 0 || targetPos.x >= fieldSize.x || targetPos.y < 0 || targetPos.y >= fieldSize.y)
                    {
                        continue;
                    }

                    // このマスが影になったかフラグ
                    bool isThisShadow = false;

                    // 影フラグが立っていたら
                    if(isShadow)
                    {
                        // フィールドに影情報を登録する
                        shadowList.Add(targetPos);

                        isThisShadow = true;
                        isShadow = false;
                    }

                    // マスの情報を取得
                    var infoArray = fieldData.GetInfoList(targetPos);
                    foreach (var info in infoArray)
                    {
                        if(info.state == _FieldDataManager.E_FIELDSTATE.pillar)
                        {
                            if (isThisShadow)
                            {
                                // このマスの影設定を解除
                                shadowList.Remove(targetPos);
                            }

                            // 次のマスを影マスにするためにフラグを立てる
                            isShadow = true;
                        }

                        // 柱があったら他を処理する必要はないので終了
                        break;
                    }
                }
            }
        }
        else
        {
            // 目標座標配列
            List<Vector2Int> targetPosList = new List<Vector2Int>();

            for (int i = 0; i < illuminateRange.x; ++i)
            {
                for (int j = 0; j < illuminateRange.y; ++j)
                {
                    // 対象マスの座標を計算
                    Vector2Int targetPos = pos + lightDir * (j + 1) + lateralDir * i;

                    // 範囲外チェック
                    if (targetPos.x < 0 || targetPos.x >= fieldSize.x || targetPos.y < 0 || targetPos.y >= fieldSize.y)
                    {
                        continue;
                    }

                    // マスの情報を取得
                    var infoArray = fieldData.GetInfoList(targetPos);
                    foreach (var info in infoArray)
                    {
                        if (info.state == _FieldDataManager.E_FIELDSTATE.pillar)
                        {
                            // 目標座標リストに追加
                            targetPosList.Add(targetPos);

                            // 自身とターゲット座標の差分を計算
                            float distance = Vector2Int.Distance(targetPos, pos);
                            
                            // 影にする候補の座標を計算
                            Vector2Int back         = targetPos + lightDir;

                            // カメラの向きによって影の左右が変わらないように二乗して全て+にする
                            Vector2Int backLeft     = back + new Vector2Int(lateralDir.x * lateralDir.x, lateralDir.y * lateralDir.y);
                            Vector2Int backRight    = back - new Vector2Int(lateralDir.x * lateralDir.x, lateralDir.y * lateralDir.y);

                            // 右奥と左奥の遠い方を計算
                            float distLeft = Vector2Int.Distance(pos, backLeft);
                            float distRight = Vector2Int.Distance(pos, backRight);
                            Vector2Int fartherSide = new Vector2Int();
                            if (distLeft == distRight)
                            {
                                if (direction == LightDirection.Left)
                                    fartherSide = backLeft;
                                else if (direction == LightDirection.Right)
                                    fartherSide = backRight;
                            }
                            else
                            {
                                fartherSide = distLeft > distRight ? backLeft : backRight;
                            }

                            // オブジェクトとの角度を計算して角度に応じた影の位置を算出
                            Vector3 toTargetVector = (info.obj.transform.position - transform.position).normalized;
                            float angle = Vector3.Angle(lightForward, toTargetVector);

                            // オブジェクトの後ろを影に
                            if (back.x >= 0 && back.x < fieldSize.x && back.y >= 0 && back.y < fieldSize.y)
                            {
                                shadowList.Add(back);
                            }

                            // 指定角度以上になったら
                            if (angle > shadowAngle)
                            {
                                if (diagonalShadowDistance < distance)
                                {
                                    if (fartherSide.x >= 0 && fartherSide.x < fieldSize.x && fartherSide.y >= 0 && fartherSide.y < fieldSize.y)
                                    {
                                        shadowList.Add(fartherSide);
                                    }

                                    // 斜めにしか影を作りたくないので後ろを削除
                                    if (shadowList.Contains(back))
                                    {
                                        shadowList.Remove(back);
                                    }
                                }
                                else
                                {
                                    if (fartherSide.x >= 0 && fartherSide.x < fieldSize.x && fartherSide.y >= 0 && fartherSide.y < fieldSize.y)
                                    {
                                        shadowList.Add(fartherSide);
                                    }
                                }
                            }

                            // 柱があったらこのマスの後ろを処理する必要はないので終了
                            break;
                        }
                    }
                }
            }

            // オブジェクトの座標と被らないように被っていたら破棄
            foreach (var pos in targetPosList)
            {
                if (shadowList.Contains(pos))
                {
                    shadowList.Remove(pos);
                }
            }
        }

        // 影の情報を登録
        foreach (var shadow in shadowList)
        {
            fieldData.AddInfo(shadow, _FieldDataManager.E_FIELDSTATE.shadow);
        }

        // フィールドの色を変更
        fieldData.ChangeColor();
    }

    public List<Vector2Int> GetShadowList()
    { 
        return shadowList; 
    }
}