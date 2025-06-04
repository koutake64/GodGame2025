using UnityEngine;
using UnityEngine.Audio;
using System.Collections;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [System.Serializable]
    public class SEData
    {
        public AudioClip clip;
        [Range(0f, 1f)]
        public float volume = 1f;
        public bool loop = false; // ← インスペクターからループ設定可能
    }

    [Header("Mixer")]
    [SerializeField] private AudioMixer audioMixer;

    [Header("Sources")]
    [SerializeField] private AudioSource bgmSource;
    [SerializeField] private AudioSource seSource;

    [Header("BGM Clips")]
    [SerializeField] private AudioClip[] BGMs;

    [Header("SE Data")]
    [SerializeField] private SEData[] SEs;

    private Coroutine bgmLoopCoroutine;
    private Coroutine seLoopCoroutine;

    private void Awake()
    {
        // シングルトン化
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// BGMの再生ループを開始（曲が終わったら再び再生）
    /// </summary>
    public void PlayBGM(int no = 0)
    {
        if (no >= BGMs.Length || BGMs[no] == null) return;

        if (bgmLoopCoroutine != null)
            StopCoroutine(bgmLoopCoroutine);

        bgmLoopCoroutine = StartCoroutine(BGMRepeatCoroutine(no));
    }

    private IEnumerator BGMRepeatCoroutine(int no)
    {
        while (true)
        {
            bgmSource.clip = BGMs[no];
            bgmSource.Play();
            yield return new WaitForSeconds(bgmSource.clip.length);
        }
    }

    /// <summary>
    /// SEの再生。ループフラグに応じて再生処理を分岐。
    /// </summary>
    public void PlaySE(int no = 0)
    {
        if (no >= SEs.Length || SEs[no].clip == null) return;

        // ループしない場合は一度だけ再生
        if (!SEs[no].loop)
        {
            seSource.PlayOneShot(SEs[no].clip, SEs[no].volume);
            return;
        }

        // ループする場合はコルーチンで管理
        if (seLoopCoroutine != null)
            StopCoroutine(seLoopCoroutine);

        seLoopCoroutine = StartCoroutine(SERepeatCoroutine(no));
    }

    private IEnumerator SERepeatCoroutine(int no)
    {
        while (true)
        {
            seSource.clip = SEs[no].clip;
            seSource.volume = SEs[no].volume;
            seSource.Play();
            yield return new WaitForSeconds(seSource.clip.length);
        }
    }

    /// <summary>
    /// BGM音量設定（ミキサー連動）
    /// </summary>
    public void SetBGMVolume(float volume)
    {
        audioMixer.SetFloat("BGMVolume", Mathf.Log10(Mathf.Clamp(volume, 0.0001f, 1f)) * 20);
    }

    /// <summary>
    /// SE音量設定（ミキサー連動）
    /// </summary>
    public void SetSEVolume(float volume)
    {
        audioMixer.SetFloat("SEVolume", Mathf.Log10(Mathf.Clamp(volume, 0.0001f, 1f)) * 20);
    }

    /// <summary>
    /// SE個別音量を設定
    /// </summary>
    public void SetSEIndividualVolume(int index, float volume)
    {
        if (index < 0 || index >= SEs.Length) return;
        SEs[index].volume = Mathf.Clamp01(volume);
    }

    /// <summary>
    /// SE個別音量を取得
    /// </summary>
    public float GetSEIndividualVolume(int index)
    {
        if (index < 0 || index >= SEs.Length) return 1f;
        return SEs[index].volume;
    }

    /// <summary>
    /// SEループの停止（任意で使用）
    /// </summary>
    public void StopSELoop()
    {
        if (seLoopCoroutine != null)
        {
            StopCoroutine(seLoopCoroutine);
            seLoopCoroutine = null;
        }

        seSource.Stop();
    }

    /// <summary>
    /// BGMループの停止（任意で使用）
    /// </summary>
    public void StopBGMLoop()
    {
        if (bgmLoopCoroutine != null)
        {
            StopCoroutine(bgmLoopCoroutine);
            bgmLoopCoroutine = null;
        }

        bgmSource.Stop();
    }
}