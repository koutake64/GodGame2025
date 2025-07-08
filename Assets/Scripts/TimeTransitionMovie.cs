using UnityEngine;
using System.Collections;
using System.Threading.Tasks;

public class TimeTransitionMovie : MonoBehaviour
{
    private TimeManager timeManager;
    private _FieldDataManager fieldManager;
    private float gameTime;

    private GameObject player;
    private GameObject princess;
    private GameObject cameraObj;

    private Vector3 initialCamPos;
    private Vector3 initialPlayerPos;
    private Vector3 initialPrincessPos;

    private float noonMovieStart;
    private float nightMovieStart;
    private float duration = 5f;
    private float moveSpeed = 2.5f;
    private float cameraMoveSpeed = 5.0f;

    private bool isNoonMoviePlaying = false;
    private bool isNightMoviePlaying = false;
    private bool isMoviePlaying = false;

    private Vector3 offScreenOffset = new Vector3(5, 0, 0); // キャラが入場する位置のオフセット

    void Start()
    {
        timeManager = GameObject.Find("Canvas").GetComponent<TimeManager>();
        if (timeManager == null)
        {
            Debug.LogError("TimeManager not found on Canvas.");
            return;
        }

        fieldManager = GameObject.Find("Field").GetComponent<_FieldDataManager>();
        if (!fieldManager)
        {
            Debug.Log(
                "Script : ModelShiftByTime.cs \n" +
                "_FieldDataManagerが見つかりません"
                );
        }

        // 演出開始時間を設定
        noonMovieStart = timeManager.GetTime(CommonSE_Proto.E_TIMEOFDAY.noon) - duration;
        nightMovieStart = timeManager.GetTime(CommonSE_Proto.E_TIMEOFDAY.night) - duration;

        StartCoroutine(DelayedInit());
    }

    void Update()
    {
        gameTime = timeManager.GetCurrentTime();

        if (!isNoonMoviePlaying && gameTime >= noonMovieStart && gameTime < nightMovieStart)
        {
            isNoonMoviePlaying = true;
            isMoviePlaying = true; // 映像が再生中であることを示すフラグを設定
            _ = PlayNoonTransitionMovie();
        }

        if (!isNightMoviePlaying && gameTime >= nightMovieStart)
        {
            isNightMoviePlaying = true;
            isMoviePlaying = true; // 映像が再生中であることを示すフラグを設定
            _ = PlayNightTransitionMovie();
        }
    }

    private IEnumerator DelayedInit()
    {
        yield return null; // 1フレーム待つ

        // 各オブジェクトを取得
        player = GameObject.FindGameObjectWithTag("Player");
        princess = GameObject.FindGameObjectWithTag("Princess");
        cameraObj = GameObject.FindGameObjectWithTag("MainCamera");
        
        // 初期位置を保持
        if (player && princess && cameraObj)
        {
            initialCamPos = cameraObj.transform.position;
            initialPlayerPos = player.transform.position;
            initialPrincessPos = princess.transform.position;

            Debug.Log("初期ポジション取得完了\n" + initialCamPos + initialPlayerPos + initialPrincessPos);
        }
        else
        {
            Debug.LogWarning("初期化に失敗しました。タグの確認をしてください。");
        }
    }

    async Task PlayNoonTransitionMovie()
    {
        Debug.Log("昼の演出開始");

        // カメラを初期位置へ
        await MoveTo(cameraObj.transform, initialCamPos, cameraMoveSpeed);

        // 執事のポジションを (0,1) にセット
        var butlerMoveController = player.GetComponent<CharacterMoveController>();
        if (butlerMoveController != null)
        {
            butlerMoveController.SetPos(new Vector2Int(0, 1));
        }

        // お嬢様と執事を画面外に移動
        princess.transform.position = initialPrincessPos - offScreenOffset * 10;
        player.transform.position = initialPlayerPos - offScreenOffset;

        // 見た目を初期位置に向けておく
        player.transform.LookAt(initialPlayerPos);

        isMoviePlaying = false; // 演出終了
    }

    async Task PlayNightTransitionMovie()
    {
        Debug.Log("夜の演出開始");

        // カメラを初期位置へ
        await MoveTo(cameraObj.transform, initialCamPos, cameraMoveSpeed);

        // 執事のコンポーネントを取得
        var butlerMoveController = player.GetComponent<CharacterMoveController>();
        if (butlerMoveController != null)
        {
            // 執事のポジションを画面にセット
            butlerMoveController.SetPos(new Vector2Int(5, 1));
            // 執事を初期位置に移動
            butlerMoveController.AddPosX(-5);
        }

        // ここで夜用のPrincess（怪盗ver）に切り替えるなら、別の GameObject を有効化・切り替え等が必要
        isMoviePlaying = false; // 映像の再生が終了したことを示すフラグをリセット
    }

    async Task MoveTo(Transform target, Vector3 destination, float moveSpeed)
    {
        while (Vector3.Distance(target.position, destination) > 0.01f)
        {
            target.position = Vector3.MoveTowards(target.position, destination, moveSpeed * Time.deltaTime);
            await Task.Yield();
        }
        target.position = destination; // 最終位置補正
    }

    async Task FadeOut(GameObject obj)
    {
        var renderer = obj.GetComponentInChildren<Renderer>();
        if (renderer == null)
        {
            Debug.LogWarning($"Renderer not found in {obj.name}.");
            return;
        }

        Material mat = renderer.material;
        if (!mat.HasProperty("_BaseColor")) return;

        // マテリアルをTransparent設定へ
        SetMaterialToTransparent(mat);

        Color color = mat.color;
        float duration = 1.0f;
        float t = 0f;

        while (t < duration)
        {
            t += Time.deltaTime;
            color.a = Mathf.Lerp(1f, 0f, t / duration);
            mat.color = color;
            await Task.Yield();
        }

        color.a = 0f;
        mat.color = color;
    }

    async Task FadeIn(GameObject obj)
    {
        var renderer = obj.GetComponentInChildren<Renderer>();
        if (renderer == null)
        {
            Debug.LogWarning($"Renderer not found in {obj.name}.");
            return;
        }

        Material mat = renderer.material;
        if (!mat.HasProperty("_BaseColor")) return;

        // Transparent設定のままでフェードイン
        SetMaterialToTransparent(mat);

        Color color = mat.color;
        float duration = 1.0f;
        float t = 0f;

        while (t < duration)
        {
            t += Time.deltaTime;
            color.a = Mathf.Lerp(0f, 1f, t / duration);
            mat.color = color;
            await Task.Yield();
        }

        color.a = 1f;
        mat.color = color;

        // 最後にOpaqueに戻す
        SetMaterialToOpaque(mat);
    }

    void SetMaterialToTransparent(Material mat)
    {
        mat.SetFloat("_Surface", 1f); // Transparent
        mat.SetFloat("_ZWrite", 0f);
        mat.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
    }

    void SetMaterialToOpaque(Material mat)
    {
        mat.SetFloat("_Surface", 0f); // Opaque
        mat.SetFloat("_ZWrite", 1f);
        mat.renderQueue = -1;
    }

    /// <summary>
    /// ムービーが再生中かどうかを取得するメソッド
    /// </summary>
    /// <returns>ムービーが再生中か判定するフラグ</returns>
    public bool GetIsMoviePlaying()
    {
        return isMoviePlaying;
    }
}
