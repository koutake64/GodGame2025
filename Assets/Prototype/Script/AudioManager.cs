using UnityEditor.Build.Content;
using UnityEngine;
using UnityEngine.Audio;
using static CommonSE_Proto;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Mixer")]
    [SerializeField] AudioMixer audioMixer;

    [Header("Sources")]
    [SerializeField] AudioSource bgmSource;     //B(GM用
    [SerializeField] AudioSource seSource2D;    //UI・２D　SE用

    [Header("BGM Clips")]
    [SerializeField] AudioClip mornigBGM;       //朝
    [SerializeField] AudioClip noonBGM;         //昼
    [SerializeField] AudioClip afternoonBGM;    //夕
    [SerializeField] AudioClip nightBGM;        //夜
    [SerializeField] AudioClip titleBGM;        //タイトル
    [SerializeField] AudioClip goalBGM;         //ゴール
    [SerializeField] AudioClip gameOverBGM;     //ゲームオーバー


    [Header("UI SE Clips")]
    [SerializeField] AudioClip sCameraSE;           //監視カメラの向き変更時クリックSE
    [SerializeField] AudioClip sCameraHitSE;        //監視カメラ発見SE
    [SerializeField] AudioClip movePrincessSE;      //姫移動SE
    [SerializeField] AudioClip moveButlerSE;        //執事移動SE
    [SerializeField] AudioClip moveSecuritySE;      //警備員移動SE

    private TimeManager timeManager;

    private void Start()
    {
        timeManager = GameObject.Find("Canvas").GetComponent<TimeManager>();
        if(timeManager == null)
        {
            Debug.Log("タイムマネージャが見つかりませんAudioManagerで！");
        }
        PlaySceneBGM();
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
    public void PlayBGM(AudioClip clip)
    {
        if (clip == null || bgmSource.clip == clip) return;
        bgmSource.Stop();
        bgmSource.clip = clip;
        bgmSource.Play();

    }

    /// <summary>
    /// 2D UI SE再生
    /// </summary>
    /// <param name="clip"></param>
    public void PlaySE2D(AudioClip clip)
    {
        if (clip == null) return;
        seSource2D.PlayOneShot(clip);
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

    /// <summary>
    /// シーンに応じたBGM再生
    /// </summary>
    public void PlaySceneBGM()
    {
        if (timeManager.GetCurState() == E_TIMEOFDAY.morning)
        {
            PlayBGM(mornigBGM);
        }
        else if (timeManager.GetCurState() == E_TIMEOFDAY.noon)
        {
            PlayBGM(noonBGM);
        }
        else if (timeManager.GetCurState() == E_TIMEOFDAY.afternoon)
        {
            PlayBGM(afternoonBGM);
        }
        else if (timeManager.GetCurState() == E_TIMEOFDAY.night)
        {
            PlayBGM(nightBGM);
        }
    }

    /// <summary>
    /// クリックSE再生
    /// </summary>
    public void PlayCmeraSE()
    {
        PlaySE2D(sCameraSE);
    }

    /// <summary>
    /// 決定ボタンクリック時のSE再生
    /// </summary>
    public void PlayCameraHitSE()
    {
        PlaySE2D(sCameraHitSE);
    }

    /// <summary>
    /// キャンセルボタンクリック時のSE再生
    /// </summary>
    public void PlayCancelSE()
    {
        PlaySE2D(movePrincessSE);
    }
}
