using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Mixer")]
    [SerializeField] private AudioMixer audioMixer;

    [Header("Sources")]
    [SerializeField] private AudioSource bgmSource;     //BGM用
    [SerializeField] private AudioSource seSource;    //SE用

    [Header("BGM Clips")]
    [SerializeField] private AudioClip[] BGMs;

    [Header("SE Clips")]
    [SerializeField] private AudioClip[] SEs;

    private void Start()
    {
        //PlayBGM();
    }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

        }
        else Destroy(gameObject);
    }

    /// <summary>
    /// BGM　再生
    /// </summary>
    /// <param name="clip">BGMのクリップ</param>
    public void PlayBGM(int no = 0)
    {
        if (no >= BGMs.Length) return; // 配列外指定
        if (BGMs[no] == null) return; // clipがない

        bgmSource.Stop();
        bgmSource.clip = BGMs[no];
        bgmSource.Play();
    }

    /*
    public void PlayGameBGM(E_TIMEOFDAY state.)
    {

    }
    */


    /// <summary>
    /// SE再生
    /// </summary>
    /// <param name="clip"></param>
    public void PlaySE(int no = 0)
    {
        if (no >= SEs.Length) return; // 配列外指定
        if (SEs[no] == null) return; // clipがない

        seSource.PlayOneShot(SEs[no]);
    }

    /// <summary>
    /// BGM 音量設定
    /// </summary>
    /// <param name="volume"></param>
    public void SetBGMVolume(float volume)
    {
       audioMixer.SetFloat("BGMVolume", Mathf.Log10(Mathf.Clamp(volume, 0.0001f, 1f))*20);
    }

    /// <summary>
    /// SE音量
    /// </summary>
    /// <param name="volume"></param>
    public void SetSEVolume(float volume)
    {
        audioMixer.SetFloat("SEVolume", Mathf.Log10(Mathf.Clamp(volume, 0.0001f, 1f)) * 20);
    }
}
