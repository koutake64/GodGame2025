using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 新しいFieldDataManager
/// </summary>
public class _FieldDataManager : MonoBehaviour
{
    /// <summary>
    /// マスに何があるかを表す列挙型
    /// </summary>
    public enum E_FIELDSTATE
    {
        // --- 基本ステータス
        outOfRange = -1,    // ステージ範囲外
        start,              // ゲームの開始地点
        goal,               // お宝(ゴール)の場所

        // --- 障害物
        pillar,             // 柱(単独のオブジェクトで監視カメラを設置できる)
        wall,               // 壁(監視カメラの監視範囲を遮断できるオブジェクト)
        exhibitionStand,    // 展示台(横か縦に長いオブジェクト)

        // --- キャラクター
        butler,             // 執事(プレイヤー)
        princess,           // お嬢様
        securityGuard_N,    // 通常の警備員(ノーマル)

        // --- ギミックオブジェクト
        surveillanceCamera, // 監視カメラ

        // --- ギミック範囲
        sc_searchRange,     // 監視カメラの監視範囲
        sg_searchRange,     // 警備員の監視範囲

    }

    /// <summary>
    /// マスにある情報
    /// </summary>
    [System.Serializable]
    public struct S_FIELDINFO
    {
        public int alignmentID;        // 連携番号
        public Vector2Int pos;         // 位置
        public CommonSE_Proto.E_DIRECTION dir;  // 方向
        public E_FIELDSTATE state;     // 状態
    }

    /// <summary>
    /// どのマスに何があるのかを管理する配列
    /// </summary>
    private List<S_FIELDINFO>[,] fieldData;

    private bool reLoadFlg = true;

    [Header("床の親オブジェクト(空のオブジェクトでOK)\n" +
            "※このゲームオブジェクトにFieldDataManagerをつける")]
    [SerializeField] private GameObject field;

    [Header("ステージのサイズ(マス) X:横, Y:奥\n" +
            "※範囲は1～50マスに制限してます")]
    [SerializeField, Range(1, 50)] private int fieldSizeX;
    [SerializeField, Range(1, 50)] private int fieldSizeY;

    [Header("床にするプレハブ(2種類)")]
    [SerializeField] private GameObject tileA;
    [SerializeField] private GameObject tileB;

    [Header("お姫様のプレハブ")]
    [SerializeField] private GameObject princess;

    [Header("プレイヤーのプレハブ")]
    [SerializeField] private GameObject player;

    [Header("警備員(通常)のプレハブ")]
    [SerializeField] private GameObject securityGuard_N;

    [Header("監視カメラのプレハブ")]
    [SerializeField] private GameObject surveillanceCamera;

    [Header("障害物のプレハブ")]
    [Header("柱(単独のオブジェクトで監視カメラを設置できる)")]
    [SerializeField] private GameObject pillar;
    [Header("壁(監視カメラの監視範囲を遮断できるオブジェクト)")]
    [SerializeField] private GameObject wall;
    [Header("展示台(横か縦に長いオブジェクト)")]
    [SerializeField] private GameObject exhibitionStand;

    private void Start()
    {
        // --- ヌルチェック
        // TODO 後で

        // --- フィールドデータの作成
        Vector2Int fieldSize = new Vector2Int(fieldSizeX, fieldSizeY);
        fieldData = new List<S_FIELDINFO>[fieldSize.x, fieldSize.y];
        for (int y = 0; y < fieldSize.y; ++y)
        {
            for (int x = 0; x < fieldSize.x; ++x)
            {
                fieldData[x, y] = new List<S_FIELDINFO>();
            }
        }

        // ---床の生成
        GameObject obj = null;
        bool tileType = false;
        bool evenNumSizeX = false;
        if (fieldSizeX % 2 == 0)
        {
            evenNumSizeX = true;
        }
        for (int y = 0; y < fieldSizeY; ++y)
        {
            for (int x = 0; x < fieldSizeY; ++x)
            {
                if (tileType)
                {
                   // タイルA生成
                   obj = Instantiate(
                       tileA,
                       new Vector3(x, 0, y),
                       Quaternion.identity
                       );
                }
                else
                {
                   // タイルB生成
                   obj = Instantiate(
                       tileB,
                       new Vector3(x, 0, y),
                       Quaternion.identity
                       );
                }

                // 生成したタイルを親オブジェクトにつける
                if (obj)
                {
                    obj.transform.SetParent(field.transform);
                }

                // 生成タイルを反転
                tileType ^= true;

            }

            // 横のサイズが偶数の場合、生成タイルを反転
            if (evenNumSizeX)
            {
                tileType ^= true;
            }

        }

        // プロトタイプ用のステージ作成


    }

    public List<Vector2Int> GetStatePos(E_FIELDSTATE state)
    {
        List<Vector2Int> list = new List<Vector2Int>();

        Vector2Int fieldSize = new Vector2Int(fieldSizeX, fieldSizeY);

        for (int y = 0; y < fieldSize.y; ++y)
        {
            for (int x = 0; x < fieldSize.x; ++x)
            {
                List<S_FIELDINFO> info = fieldData[x, y];

                if (info.Count == 0)
                {
                    continue;
                }

                for (int i = 0; i < info.Count; ++i)
                {
                    if (info[i].state != state)
                    {
                        continue;
                    }

                    list.Add(info[i].pos);

                }

            }

        }

        return list;

    }

    public List<S_FIELDINFO> GetInfoList(Vector2Int pos)
    {
        return fieldData[pos.x, pos.y];

    }

    public void AddInfo(Vector2Int pos, E_FIELDSTATE state, CommonSE_Proto.E_DIRECTION dir = CommonSE_Proto.E_DIRECTION.down, int id = 0)
    {
        S_FIELDINFO info = new S_FIELDINFO();

        info.alignmentID = id;
        info.pos = pos;
        info.dir = dir;
        info.state = state;

        fieldData[pos.x, pos.y].Add(info);

    }

    public void RemoveInfo(Vector2Int pos, E_FIELDSTATE state, int id = 0)
    {
        List<S_FIELDINFO> list = fieldData[pos.x, pos.y];

        for (int i = 0; i < list.Count; ++i)
        {
            if (list[i].state == state)
            {
                fieldData[pos.x, pos.y].Remove(list[i]);

            }

        }

    }

    public void ChangeDirection(Vector2Int pos, E_FIELDSTATE state, CommonSE_Proto.E_DIRECTION dir)
    {
        List<S_FIELDINFO> list = fieldData[pos.x, pos.y];

        for (int i = 0; i < list.Count; ++i)
        {
            if (list[i].state == state)
            {
                S_FIELDINFO temp = list[i];
                temp.dir = dir;
                list[i] = temp;

            }

        }

    }

    public void MoveInfo(Vector2Int prevPos, Vector2Int nextPos, E_FIELDSTATE state, int id = 0)
    {
        List<S_FIELDINFO> prevList = fieldData[prevPos.x, prevPos.y];

        for (int i = 0; i < prevList.Count; ++i)
        {
            if (prevList[i].state == state)
            {
                RemoveInfo(prevPos, state);
                AddInfo(nextPos, state);

            }

        }

    }

    private void FixedUpdate()
    {
        if (!reLoadFlg)
        {
            return;
        }

        // TODO フィールドデータをもとにオブジェクト生成

        //Vector2Int fieldSize = new Vector2Int(fieldSizeX, fieldSizeY);

        //for (int y = 0; y < fieldSize.y; ++y)
        //{
        //    for (int x = 0; x < fieldSize.x; ++x)
        //    {
                

        //    }

        //}

    }

}
