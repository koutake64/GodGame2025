using UnityEngine;
using UnityEngine.Audio;

/// <summary>
/// ゲーム全体のBGMおよびSE（効果音）を管理するシングルトンクラス。
/// BGM/SEの個別音量や全体音量をMixerと連携して管理できる。
/// </summary>
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance; // シングルトンインスタンス

    /// <summary>
    /// BGMごとのデータ（AudioClipと個別音量）
    /// </summary>
    [System.Serializable]
    public class BGMData
    {
        public AudioClip clip; // BGM音源
        [Range(0f, 1f)] public float volume = 1f; // 個別音量
    }

    /// <summary>
    /// SEごとのデータ（AudioClipと個別音量）
    /// </summary>
    [System.Serializable]
    public class SEData
    {
        public AudioClip clip; // SE音源
        [Range(0f, 1f)] public float volume = 1f; // 個別音量
    }

    [Header("Mixer")]
    [SerializeField] private AudioMixer audioMixer; // AudioMixer（BGMVolume, SEVolumeのExposeが必要）

    [Header("Sources")]
    [SerializeField] private AudioSource bgmSource; // BGM再生用AudioSource
    [SerializeField] private AudioSource seSource;  // SE再生用AudioSource

    [Header("BGM Data")]
    [SerializeField] private BGMData[] BGMs; // 登録するBGMデータ

    [Header("SE Data")]
    [SerializeField] private SEData[] SEs;   // 登録するSEデータ

    private void Awake()
    {
        // シングルトンの確立と重複破棄
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // シーンをまたいで保持
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnValidate()
    {
        // インスペクター上で変更されたBGMの個別音量を、再生中なら即反映
        if (bgmSource != null && bgmSource.isPlaying && bgmSource.clip != null)
        {
            for (int i = 0; i < BGMs.Length; i++)
            {
                if (BGMs[i].clip == bgmSource.clip)
                {
                    bgmSource.volume = BGMs[i].volume;
                    break;
                }
            }
        }
    }

    /// <summary>
    /// 指定番号のBGMを再生（個別音量も設定）
    /// </summary>
    public void PlayBGM(int no = 0)
    {
        if (no >= BGMs.Length || BGMs[no].clip == null) return;

        bgmSource.Stop();
        bgmSource.clip = BGMs[no].clip;
        bgmSource.volume = BGMs[no].volume;
        bgmSource.Play();
    }

    /// <summary>
    /// 指定番号のSEを再生（個別音量で）
    /// </summary>
    public void PlaySE(int no = 0)
    {
        if (no >= SEs.Length || SEs[no].clip == null) return;

        seSource.PlayOneShot(SEs[no].clip, SEs[no].volume);
    }

    /// <summary>
    /// ミキサーを使ったBGM全体音量の設定
    /// </summary>
    /// <param name="volume">0〜1の範囲</param>
    public void SetBGMVolume(float volume)
    {
        // ミキサーにはdB単位で設定する必要があるため、Logスケールに変換
        audioMixer.SetFloat("BGMVolume", Mathf.Log10(Mathf.Clamp(volume, 0.0001f, 1f)) * 20);
    }

    /// <summary>
    /// ミキサーを使ったSE全体音量の設定
    /// </summary>
    public void SetSEVolume(float volume)
    {
        audioMixer.SetFloat("SEVolume", Mathf.Log10(Mathf.Clamp(volume, 0.0001f, 1f)) * 20);
    }

    /// <summary>
    /// 特定のSEだけ個別に音量を変更
    /// </summary>
    public void SetSEIndividualVolume(int index, float volume)
    {
        if (index < 0 || index >= SEs.Length) return;

        SEs[index].volume = Mathf.Clamp01(volume);
    }

    /// <summary>
    /// 特定のSEの現在の音量を取得
    /// </summary>
    public float GetSEIndividualVolume(int index)
    {
        if (index < 0 || index >= SEs.Length) return 1f;

        return SEs[index].volume;
    }

    /// <summary>
    /// 特定のBGMの個別音量を設定し、再生中なら即反映
    /// </summary>
    public void SetBGMIndividualVolume(int index, float volume)
    {
        if (index < 0 || index >= BGMs.Length) return;

        BGMs[index].volume = Mathf.Clamp01(volume);

        // 現在の再生BGMなら反映
        if (bgmSource.isPlaying && bgmSource.clip == BGMs[index].clip)
        {
            bgmSource.volume = volume;
        }
    }

    /// <summary>
    /// 特定のBGMの個別音量を取得
    /// </summary>
    public float GetBGMIndividualVolume(int index)
    {
        if (index < 0 || index >= BGMs.Length) return 1f;

        return BGMs[index].volume;
    }
}