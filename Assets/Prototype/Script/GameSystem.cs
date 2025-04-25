using UnityEngine;

public class GameSystem : MonoBehaviour
{
    [Header("床の親オブジェクト(空のオブジェクトでOK)\n" +
            "※このゲームオブジェクトにFieldDataManagerをつける")]
    [SerializeField] private GameObject field;

    [Header("床にするプレハブ(2種類)")]
    [SerializeField] private GameObject tileA;
    [SerializeField] private GameObject tileB;

    [Header("ステージのサイズ(マス) X:横, Y:奥\n" + 
            "※範囲は1～50マスに制限してます")]
    [SerializeField, Range(1, 50)] private int fieldSizeX;
    [SerializeField, Range(1, 50)] private int fieldSizeY;

    [Header("お姫様のプレハブ")]
    [SerializeField] private GameObject princess;

    [Header("プレイヤーのプレハブ")]
    [SerializeField] private GameObject player;

    [Header("監視カメラのプレハブ")]
    [SerializeField] private GameObject surveillanceCamera;

    [Header("キャラクターの移動速度")]
    [SerializeField] private float moveSpeed;

    [Header("キャラクターの回転速度")]
    [SerializeField] private float rotateSpeed;

    /// <summary>
    /// GameSystem.csのStart関数が正常に通った場合にtrueになる
    /// </summary>
    [HideInInspector] public bool isStart = false;
    private bool notStartLogOnce = true;

    private void Start()
    {
        // --- ゲームオブジェクトのヌルチェック
        if (!GOUtils_Proto.CheckGameObject(field))
        {
            Debug.LogError(
                "Script:GameSystem.cs \n" +
                "field が null になっています"
                );
            return;
        }
        if (!GOUtils_Proto.CheckGameObject(tileA))
        {
            Debug.LogError(
                "Script:GameSystem.cs \n" +
                "tileA が null になっています"
                );
            return;
        }
        if (!GOUtils_Proto.CheckGameObject(tileB))
        {
            Debug.LogError(
                "Script:GameSystem.cs \n" +
                "tileB が null になっています"
                );
            return;
        }
        if (!GOUtils_Proto.CheckGameObject(princess))
        {
            Debug.LogError(
                "Script:GameSystem.cs \n" +
                "princess が null になっています"
                );
            return;
        }
        if (!GOUtils_Proto.CheckGameObject(player))
        {
            Debug.LogError(
                "Script:GameSystem.cs \n" +
                "player が null になっています"
                );
            return;
        }
        if (!GOUtils_Proto.CheckGameObject(surveillanceCamera))
        {
            Debug.LogError(
                "Script:GameSystem.cs \n" +
                "surveillanceCamera が null になっています"
                );
            return;
        }

        // --- フィールドデータマネージャーの取得と初期化
        FieldDataManager fdMng = field.GetComponent<FieldDataManager>();
        if (!fdMng)
        {
            Debug.LogError(
                "Script:GameSystem.cs \n" +
                "field に FieldDataManager がついていません"
                );
            return;
        }
        fdMng.InitArray(new Vector2(fieldSizeX, fieldSizeY));

        // --- 床の生成
        GameObject obj = null;
        FieldDataManager.S_FIELDINFO info = new FieldDataManager.S_FIELDINFO();
        bool tileType = false;
        bool evenNumSizeX = false;
        if (fieldSizeX % 2 == 0)
        {
            evenNumSizeX = true;
        }
        for(int y = 0; y < fieldSizeY; ++y)
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

                // 情報の追加
                info.obj = obj;
                info.state = FieldDataManager.E_FIELDSTATE.none;
                // TODO 仕様確認必要
                info.dir = CommonSE_Proto.E_DIRECTION.up;
                fdMng.AddFieldInfo(new Vector2(x, y), info);

                // 生成タイルを反転
                tileType ^= true;

            }

            // 横のサイズが偶数の場合、生成タイルを反転
            if (evenNumSizeX)
            {
                tileType ^= true;
            }

        }

        // --- お姫様の生成
        Instantiate(
            princess,
            Vector3.zero,
            Quaternion.identity
            );

        // --- プレイヤーの生成
        Instantiate(
            player,
            new Vector3(1.0f, 0.0f, 0.0f),
            Quaternion.identity
            );

        // --- スタート関数正常終了
        isStart = true;

    }

    private void FixedUpdate()
    {
        // --- スタート関数を正常に通っているかチェック
        if (!isStart)
        {
            if (notStartLogOnce)
            {
                Debug.LogError(
                    "Script:GameSystem.cs \n" +
                    "Start関数が正常に通っていないため、FixedUpdateを実行しません"
                    );
                notStartLogOnce = false;
            }
            return;
        }

        // TODO 常に動かすゲームシステムを実装


    }

    public float GetCharacterMoveSpeed()
    {
        return moveSpeed;
    }
    public float GetCharacterRotateSpeed()
    {
        return rotateSpeed;
    }
}
