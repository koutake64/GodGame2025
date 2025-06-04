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
        light,              // ライト

        // --- ギミック範囲
        sc_searchRange,     // 監視カメラの監視範囲
        sg_searchRange,     // 警備員の監視範囲
        shadow,             // 影

        // --- その他
        talk                // お花しするマス

    }

    /// <summary>
    /// マスにある情報
    /// </summary>
    [System.Serializable]
    public struct S_FIELDINFO
    {
        public GameObject obj;          // ゲームオブジェクト
        public int alignmentID;         // 連携番号
        public Vector2Int pos;          // 位置
        public CommonSE_Proto.E_DIRECTION dir;  // 方向
        public E_FIELDSTATE state;      // 状態
    }

    private struct S_TAILPREHUBINFO
    {
        public GameObject obj;
        public bool typeFlag;
    }

    /// <summary>
    /// どのマスに何があるのかを管理する配列
    /// </summary>
    private List<S_FIELDINFO>[,] fieldData;

    private S_TAILPREHUBINFO[,] fieldGameObj;
    private bool changeColorFlag = true;

    private int updateCnt = 0;

    private List<GameObject> moveGameObjList = new List<GameObject>();

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

    [Header("フィールドを囲う壁のプレハブ")]
    [SerializeField] private GameObject fieldWall;
    [SerializeField] private GameObject fieldWallWindow;

    [Header("お姫様のプレハブ")]
    [SerializeField] private GameObject princess;

    [Header("プレイヤーのプレハブ")]
    [SerializeField] private GameObject player;

    [Header("警備員(通常)のプレハブ")]
    [SerializeField] private GameObject securityGuard_N;

    [Header("監視カメラのプレハブ")]
    [SerializeField] private GameObject surveillanceCamera;

    [Header("光のプレハブ")]
    [SerializeField] private GameObject lightObj;

    [Header("障害物のプレハブ")]
    [Header("柱(単独のオブジェクトで監視カメラを設置できる)")]
    [SerializeField] private List<GameObject> pillars;
    [Header("壁(監視カメラの監視範囲を遮断できるオブジェクト)")]
    [SerializeField] private List<GameObject> walls;
    [Header("展示台(横か縦に長いオブジェクト)")]
    [SerializeField] private GameObject exhibitionStand;

    [Header("お宝のプレハブ")]
    [SerializeField] private GameObject goal;

    [Header("警備員巡回ルート(IDはこちらで指定)")]
    [SerializeField] private Dictionary<int, List<Vector2Int>> route = new Dictionary<int, List<Vector2Int>>();

    [Header("ポール")]
    [SerializeField] private GameObject pole;

    [Header("ロープ")]
    [SerializeField] private GameObject rope;

    [Header("場外床")]
    [SerializeField] private GameObject outOfRangeTile;

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

        fieldGameObj = new S_TAILPREHUBINFO[fieldSizeX, fieldSizeY];

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
            for (int x = 0; x < fieldSizeX; ++x)
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

                S_TAILPREHUBINFO tpi = new S_TAILPREHUBINFO();
                tpi.obj = obj;
                tpi.typeFlag = tileType;
                fieldGameObj[x, y] = tpi;

                // 生成タイルを反転
                tileType ^= true;

            }

            // 横のサイズが偶数の場合、生成タイルを反転
            if (evenNumSizeX)
            {
                tileType ^= true;
            }

        }


        Vector3 fwPos = Vector3.zero;
        Vector3 fwScl = Vector3.one;

        // 奥左
        fwPos = new Vector3 ((fieldSizeX / 2.0f) - 0.5f, 0.0f, fieldSizeY);

        obj = Instantiate(
            fieldWall,
            fwPos,
            Quaternion.identity
            );
        
        fwPos = new Vector3 ((-fieldSizeX / 2) - 0.5f, 0.0f, fieldSizeY);

        obj = Instantiate(
            fieldWall,
            fwPos,
            Quaternion.identity
            );

        // 奥右
        fwPos = new Vector3(fieldSizeX, 0.0f, (fieldSizeY / 2.0f) - 0.5f);

        obj = Instantiate(
            fieldWallWindow,
            fwPos,
            Quaternion.identity
            );

        // ----- ポールとロープの生成
        Vector3 basePos = new Vector3(-1.0f, 0.0f, -1.0f);
        Vector3 hRopeScale = new Vector3(0.8f, 0.8f, 0.85f);
        Vector3 wRopeScale = new Vector3(0.8f, 0.8f, 0.95f);
        Quaternion wRot = Quaternion.Euler(0.0f, 90.0f, 0.0f);

        Instantiate(pole, basePos, Quaternion.identity);
        Instantiate(pole, basePos, wRot);

        // --- 縦
        for (int i = 0; i < fieldSizeY / 3; ++i)
        {
            Vector3 ipPos = new Vector3(basePos.x, basePos.y, basePos.z + ((i + 1) * 3));
            Instantiate(pole, ipPos, Quaternion.identity);
            Vector3 irPos = new Vector3(ipPos.x, ipPos.y + 0.75f, ipPos.z - 1.625f);
            GameObject _rope = Instantiate(rope, irPos, Quaternion.identity);
            _rope.transform.localScale = hRopeScale;
        }

        // --- 横
        for (int i = 0; i < fieldSizeX / 3; ++i)
        {
            Vector3 ipPos = new Vector3(basePos.x + ((i + 1) * 3) + ((i +1) * 0.33f), basePos.y, basePos.z);
            Instantiate(pole, ipPos, wRot);
            Vector3 irPos = new Vector3(ipPos.x - 1.8f, ipPos.y + 0.75f, ipPos.z);
            GameObject _rope = Instantiate(rope, irPos, wRot);
            _rope.transform.localScale = wRopeScale;
        }

        // --- 床外の床を生成
        GameObject _outOfRangeTile1 = Instantiate(outOfRangeTile, new Vector3(-11, -0.5f, 7), Quaternion.identity);
        _outOfRangeTile1.transform.localScale = new Vector3(21, 1, 15);
        GameObject _outOfRangeTile2 = Instantiate(outOfRangeTile, new Vector3(0, -0.5f, -9), Quaternion.identity);
        _outOfRangeTile2.transform.localScale = new Vector3(43, 1, 17);
        GameObject _outOfRangeTile3 = Instantiate(outOfRangeTile, new Vector3(31, -0.5f, -1), Quaternion.identity);
        _outOfRangeTile3.transform.localScale = new Vector3(21, 1, 33);

        // プロトタイプ用のステージ作成
        AddInfo(new Vector2Int(0, 1), E_FIELDSTATE.start, CommonSE_Proto.E_DIRECTION.right);
        S_FIELDINFO startInfo = GetInfoList(GetStatePos(E_FIELDSTATE.start)[0])[0];
        AddInfo(new Vector2Int(0, 1), E_FIELDSTATE.butler, startInfo.dir);
        AddInfo(new Vector2Int(0, 1), E_FIELDSTATE.princess, startInfo.dir);

        AddInfo(new Vector2Int(0, 2), E_FIELDSTATE.pillar, CommonSE_Proto.E_DIRECTION.down, 0);
        AddInfo(new Vector2Int(1, 2), E_FIELDSTATE.pillar, CommonSE_Proto.E_DIRECTION.down, 0);
        AddInfo(new Vector2Int(2, 2), E_FIELDSTATE.pillar, CommonSE_Proto.E_DIRECTION.down, 0);
        AddInfo(new Vector2Int(3, 2), E_FIELDSTATE.pillar, CommonSE_Proto.E_DIRECTION.down, 0);
        AddInfo(new Vector2Int(4, 2), E_FIELDSTATE.pillar, CommonSE_Proto.E_DIRECTION.down, 0);
        AddInfo(new Vector2Int(5, 2), E_FIELDSTATE.pillar, CommonSE_Proto.E_DIRECTION.down, 0);

        AddInfo(new Vector2Int(8, 0), E_FIELDSTATE.pillar, CommonSE_Proto.E_DIRECTION.down, 0);
        AddInfo(new Vector2Int(9, 0), E_FIELDSTATE.pillar, CommonSE_Proto.E_DIRECTION.down, 0);
        AddInfo(new Vector2Int(10, 0), E_FIELDSTATE.pillar, CommonSE_Proto.E_DIRECTION.down, 0);

        AddInfo(new Vector2Int(13, 0), E_FIELDSTATE.pillar, CommonSE_Proto.E_DIRECTION.down, 0);

        AddInfo(new Vector2Int(15, 2), E_FIELDSTATE.pillar, CommonSE_Proto.E_DIRECTION.down, 0);

        AddInfo(new Vector2Int(17, 3), E_FIELDSTATE.wall, CommonSE_Proto.E_DIRECTION.down, 0);

        AddInfo(new Vector2Int(17, 6), E_FIELDSTATE.pillar, CommonSE_Proto.E_DIRECTION.down, 0);

        AddInfo(new Vector2Int(15, 10), E_FIELDSTATE.pillar, CommonSE_Proto.E_DIRECTION.down, 0);

        AddInfo(new Vector2Int(14, 12), E_FIELDSTATE.wall, CommonSE_Proto.E_DIRECTION.down, 0);

        AddInfo(new Vector2Int(11, 10), E_FIELDSTATE.pillar, CommonSE_Proto.E_DIRECTION.down, 0);

        AddInfo(new Vector2Int(9, 8), E_FIELDSTATE.pillar, CommonSE_Proto.E_DIRECTION.down, 0);

        AddInfo(new Vector2Int(9, 14), E_FIELDSTATE.wall, CommonSE_Proto.E_DIRECTION.down, 0);

        AddInfo(new Vector2Int(5, 8), E_FIELDSTATE.pillar, CommonSE_Proto.E_DIRECTION.down, 0);

        AddInfo(new Vector2Int(1, 10), E_FIELDSTATE.wall, CommonSE_Proto.E_DIRECTION.down, 0);

        AddInfo(new Vector2Int(1, 11), E_FIELDSTATE.wall, CommonSE_Proto.E_DIRECTION.down, 0);

        AddInfo(new Vector2Int(3, 11), E_FIELDSTATE.pillar, CommonSE_Proto.E_DIRECTION.down, 0);

        AddInfo(new Vector2Int(1, 12), E_FIELDSTATE.wall, CommonSE_Proto.E_DIRECTION.down, 0);

        AddInfo(new Vector2Int(2, 13), E_FIELDSTATE.wall, CommonSE_Proto.E_DIRECTION.down, 0);
        AddInfo(new Vector2Int(3, 13), E_FIELDSTATE.wall, CommonSE_Proto.E_DIRECTION.down, 0);
        AddInfo(new Vector2Int(4, 13), E_FIELDSTATE.wall, CommonSE_Proto.E_DIRECTION.down, 0);
        AddInfo(new Vector2Int(5, 13), E_FIELDSTATE.wall, CommonSE_Proto.E_DIRECTION.down, 0);
        AddInfo(new Vector2Int(6, 13), E_FIELDSTATE.pillar, CommonSE_Proto.E_DIRECTION.down, 0);


        AddInfo(new Vector2Int(17, 4), E_FIELDSTATE.surveillanceCamera, CommonSE_Proto.E_DIRECTION.up);
        AddInfo(new Vector2Int(2, 10), E_FIELDSTATE.surveillanceCamera, CommonSE_Proto.E_DIRECTION.right);
        AddInfo(new Vector2Int(15, 12), E_FIELDSTATE.surveillanceCamera, CommonSE_Proto.E_DIRECTION.right);
        AddInfo(new Vector2Int(9, 13), E_FIELDSTATE.surveillanceCamera, CommonSE_Proto.E_DIRECTION.down);

        AddInfo(new Vector2Int(1, 13), E_FIELDSTATE.securityGuard_N, CommonSE_Proto.E_DIRECTION.up, 10);
        List<Vector2Int> route1 = new List<Vector2Int>();
        route1.Add(new Vector2Int(1, 14));
        route1.Add(new Vector2Int(1, 13));
        route.Add(10, route1);

        AddInfo(new Vector2Int(15, 14), E_FIELDSTATE.securityGuard_N, CommonSE_Proto.E_DIRECTION.right, 11);
        List<Vector2Int> route2 = new List<Vector2Int>();
        route2.Add(new Vector2Int(11, 14));
        route2.Add(new Vector2Int(19, 14));
        route.Add(11, route2);

        AddInfo(new Vector2Int(0, 14), E_FIELDSTATE.goal);

        AddInfo(new Vector2Int(13, 1), E_FIELDSTATE.talk);
        AddInfo(new Vector2Int(12, 10), E_FIELDSTATE.talk);

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

    public List<GameObject> GetGameObjectList(E_FIELDSTATE state)
    {
        List<GameObject> list = new List<GameObject>();

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

                    list.Add(info[i].obj);

                }

            }

        }

        return list;

    }

    public bool GetIsThrough(Vector2Int pos)
    {
        if (pos.x < 0 || pos.y < 0 || pos.x >= fieldData.GetLength(0) || pos.y >= fieldData.GetLength(1))
        {
            return false;
        }

        int cnt = fieldData[pos.x, pos.y].Count;

        for (int i = 0; i < cnt; ++i)
        {
            switch (fieldData[pos.x, pos.y][i].state)
            {
                case E_FIELDSTATE.outOfRange:
                    return false;
                case E_FIELDSTATE.pillar:
                    return false;
                case E_FIELDSTATE.wall:
                    return false;
                case E_FIELDSTATE.exhibitionStand:
                    return false;
                //case E_FIELDSTATE.butler:
                //    return false;
                //case E_FIELDSTATE.princess:
                //    return false;
                //case E_FIELDSTATE.securityGuard_N:
                //    return false;
                default:
                    break;
            }
        }

        return true;
    }

    public void AddInfo(Vector2Int pos, E_FIELDSTATE state, CommonSE_Proto.E_DIRECTION dir = CommonSE_Proto.E_DIRECTION.down, int id = -1)
    {
        S_FIELDINFO info = new S_FIELDINFO();

        info.alignmentID = id;
        info.pos = pos;
        info.dir = dir;
        info.state = state;

        fieldData[pos.x, pos.y].Add(info);

    }

    public void RemoveInfo(Vector2Int pos, E_FIELDSTATE state, int id = -1)
    {
        List<S_FIELDINFO> list = fieldData[pos.x, pos.y];

        for (int i = 0; i < list.Count; ++i)
        {
            if (list[i].state == state && list[i].alignmentID == id)
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
            if (prevList[i].state == state && prevList[i].alignmentID == id)
            {
                RemoveInfo(prevPos, state);
                AddInfo(nextPos, state);

            }

        }

    }

    private void FixedUpdate()
    {
        if (updateCnt > 3 && changeColorFlag)
        {
            ChangeTail_debug();
            changeColorFlag = false;
        }

        updateCnt++;

        if (updateCnt > 2)
        {
            return;
        }

        // TODO フィールドデータをもとにオブジェクト生成
        if (updateCnt == 1)
        {
            Vector2Int fieldSize = new Vector2Int(fieldSizeX, fieldSizeY);
            GameObject obj = null;

            for (int y = 0; y < fieldSize.y; ++y)
            {
                for (int x = 0; x < fieldSize.x; ++x)
                {
                    if (fieldData[x, y].Count == 0)
                    {
                        continue;

                    }

                    for (int i = 0; i < fieldData[x, y].Count; ++i)
                    {
                        Vector3 instPos = new Vector3(fieldData[x, y][i].pos.x, 0.0f, fieldData[x, y][i].pos.y);

                        float angle = 0.0f;

                        switch (fieldData[x, y][i].dir)
                        {
                            case CommonSE_Proto.E_DIRECTION.up:
                                angle = 0.0f;
                                break;
                            case CommonSE_Proto.E_DIRECTION.right:
                                angle = 90.0f;
                                break;
                            case CommonSE_Proto.E_DIRECTION.down:
                                angle = 180.0f;
                                break;
                            case CommonSE_Proto.E_DIRECTION.left:
                                angle = 270.0f;
                                break;
                            default:
                                break;
                        }

                        Quaternion instRot = Quaternion.Euler(0, angle, 0);

                        switch (fieldData[x, y][i].state)
                        {
                            case E_FIELDSTATE.outOfRange:
                                break;
                            case E_FIELDSTATE.start:
                                break;
                            case E_FIELDSTATE.goal:
                                obj = Instantiate(
                                    goal,
                                    instPos,
                                    Quaternion.identity
                                    );
                                break;
                            case E_FIELDSTATE.pillar:
                                obj = Instantiate(
                                    pillars[fieldData[x, y][i].alignmentID],
                                    instPos,
                                    instRot
                                    );
                                break;
                            case E_FIELDSTATE.wall:
                                obj = Instantiate(
                                    walls[fieldData[x, y][i].alignmentID],
                                    instPos,
                                    instRot
                                    );
                                break;
                            case E_FIELDSTATE.exhibitionStand:
                                obj = Instantiate(
                                    exhibitionStand,
                                    instPos,
                                    instRot
                                    );
                                break;
                            case E_FIELDSTATE.butler:
                                obj = Instantiate(
                                    player,
                                    instPos,
                                    instRot
                                    );

                                GameObject child = GameObject.FindGameObjectWithTag("Player");
                                ButlerController bc = child.GetComponent<ButlerController>();
                                bc.SetInitPos(new Vector2Int((int)instPos.x, (int)instPos.z));

                                break;
                            case E_FIELDSTATE.princess:
                                obj = Instantiate(
                                    princess,
                                    instPos,
                                    instRot
                                    );
                                break;
                            case E_FIELDSTATE.securityGuard_N:
                                obj = Instantiate(
                                    securityGuard_N,
                                    instPos,
                                    instRot
                                    );

                                obj.GetComponent<SecurityController>().SetInitPos(new Vector2Int((int)instPos.x, (int)instPos.z));
                                foreach (var r in route)
                                {
                                    if (fieldData[x, y][i].alignmentID == r.Key)
                                    {
                                        foreach(var q in r.Value)
                                        {
                                            obj.GetComponent<SecurityController>().AddTargetPos(q);
                                        }
                                    }
                                }

                                break;
                            case E_FIELDSTATE.surveillanceCamera:
                                obj = Instantiate(
                                    surveillanceCamera,
                                    instPos,
                                    instRot
                                    );

                                SurveillanceCamera sc = obj.transform.GetComponent<SurveillanceCamera>();
                                if ( sc != null )
                                {
                                    sc.SetCameraDir(fieldData[x, y][i].dir);
                                }

                                break;
                            case E_FIELDSTATE.sc_searchRange:
                                break;
                            case E_FIELDSTATE.sg_searchRange:
                                break;
                            case E_FIELDSTATE.light:
                                Instantiate(
                                    lightObj,
                                    instPos,
                                    instRot
                                    );
                                break;
                            default:
                                break;
                        }

                        if (obj)
                        {
                            S_FIELDINFO temp = fieldData[x, y][i];
                            temp.obj = obj;
                            fieldData[x, y][i] = temp;



                            obj.transform.SetParent(field.transform);
                        }

                        if (obj.GetComponent<CharacterMoveController>() != null)
                        {
                            moveGameObjList.Add(obj);
                        }

                    }

                }

            }

        }
    }

    public Vector2Int GetFieldSize()
    {
        return new Vector2Int(fieldSizeX, fieldSizeY);
    }

    private void ChangeTail_debug()
    {
        Vector2Int fieldSize = new Vector2Int(fieldSizeX, fieldSizeY);
        E_FIELDSTATE[,] state = new E_FIELDSTATE[fieldSizeX, fieldSizeY];

        for (int y = 0; y < fieldSize.y; ++y)
        {
            for (int x = 0; x < fieldSize.x; ++x)
            {
                state[x, y] = E_FIELDSTATE.outOfRange;
            }

        }

        for (int y = 0; y < fieldSize.y; ++y)
        {
            for (int x = 0; x < fieldSize.x; ++x)
            {
                for(int i = 0; i < fieldData[x, y].Count; ++i)
                {
                    if (fieldData[x, y][i].state == E_FIELDSTATE.surveillanceCamera)
                    {
                        SurveillanceCamera sc = fieldData[x, y][i].obj.GetComponent<SurveillanceCamera>();

                        List<Vector2Int> posList = sc.GetSearchedTileList();

                        for (int j = 0; j < posList.Count; ++j)
                        {
                            state[posList[j].x, posList[j].y] = E_FIELDSTATE.sc_searchRange;
                        }
                    }
                    else if (fieldData[x, y][i].state == E_FIELDSTATE.shadow)
                    { 
                        // TODO 影の位置をもらい、ステータスを更新
                    }


                }

            }

        }

        for (int y = 0; y < fieldSize.y; ++y)
        {
            for (int x = 0; x < fieldSize.x; ++x)
            {
                MeshRenderer mr = fieldGameObj[x, y].obj.GetComponent<MeshRenderer>();

                if (state[x, y] == E_FIELDSTATE.sc_searchRange)
                {
                    var renderer = fieldGameObj[x, y].obj.GetComponent<MeshRenderer>();
                    renderer.material = new Material(renderer.sharedMaterial);
                    renderer.material.color = Color.red;
                }
                else if (state[x, y] == E_FIELDSTATE.shadow)
                {
                    var renderer = fieldGameObj[x, y].obj.GetComponent<MeshRenderer>();
                    renderer.material = new Material(renderer.sharedMaterial);
                    renderer.material.color = Color.black;
                }
                else
                {
                    Color baseColor = fieldGameObj[x, y].typeFlag
                        ? tileA.GetComponent<MeshRenderer>().sharedMaterial.color
                        : tileB.GetComponent<MeshRenderer>().sharedMaterial.color;

                    mr.material.color = baseColor;
                }
            }
        }
    }

    public void ChangeColor()
    {
        changeColorFlag = true;
    }

}
