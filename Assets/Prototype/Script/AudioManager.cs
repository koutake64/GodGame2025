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
    /// SEの再生ループを開始（SEが終わったら再び再生）
    /// </summary>
    public void PlaySE(int no = 0)
    {
        if (no >= SEs.Length || SEs[no].clip == null) return;

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
}