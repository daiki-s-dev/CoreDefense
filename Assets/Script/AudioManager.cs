using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ゲーム全体のBGM・SEを管理するAudioManager。
/// TitleScene、GameScene、ClearSceneをまたいで使用する。
/// </summary>
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }


    // =====================================================
    // BGM
    // =====================================================

    public enum BGMType
    {
        None,

        Title,

        // GameScene
        GamePreparation,
        GameWave,

        Clear
    }


    // =====================================================
    // SE
    // =====================================================

    public enum SEType
    {
        ButtonClick,
        ButtonHover,

        TowerBuild,
        TowerUpgrade,
        TowerSell,
        TowerAttack,

        EnemyDamage,
        EnemyDeath,

        WaveStart,
        CoreDamage,

        GameClear,
        GameOver
    }


    // =====================================================
    // BGMデータ
    // =====================================================

    [System.Serializable]
    public class BGMData
    {
        public BGMType type;

        public AudioClip clip;
    }


    // =====================================================
    // SEデータ
    // =====================================================

    [System.Serializable]
    public class SEData
    {
        public SEType type;

        public AudioClip clip;

        [Range(0f, 1f)]
        public float volume = 1f;
    }


    // =====================================================
    // AudioSource
    // =====================================================

    [Header("Audio Source")]

    [SerializeField]
    private AudioSource bgmSource;

    [SerializeField]
    private AudioSource seSource;


    // =====================================================
    // AudioClip
    // =====================================================

    [Header("BGM")]

    [SerializeField]
    private List<BGMData> bgmList = new List<BGMData>();


    [Header("SE")]

    [SerializeField]
    private List<SEData> seList = new List<SEData>();


    // =====================================================
    // 音量
    // =====================================================

    [Header("音量")]

    [Range(0f, 1f)]
    [SerializeField]
    private float bgmVolume = 1f;

    [Range(0f, 1f)]
    [SerializeField]
    private float seVolume = 1f;


    // =====================================================
    // 現在のBGM
    // =====================================================

    private BGMType currentBGM = BGMType.None;


    // =====================================================
    // Awake
    // =====================================================

    private void Awake()
    {
        // -------------------------------------------------
        // Singleton
        // -------------------------------------------------

        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);

            return;
        }


        Instance = this;


        // -------------------------------------------------
        // シーンを移動しても残す
        // -------------------------------------------------

        DontDestroyOnLoad(gameObject);


        // -------------------------------------------------
        // AudioSourceを自動取得
        // -------------------------------------------------

        if (bgmSource == null)
        {
            bgmSource =
                GetComponent<AudioSource>();
        }


        if (seSource == null)
        {
            AudioSource[] sources =
                GetComponents<AudioSource>();


            if (sources.Length >= 2)
            {
                seSource =
                    sources[1];
            }
        }


        // -------------------------------------------------
        // BGM設定
        // -------------------------------------------------

        if (bgmSource != null)
        {
            bgmSource.loop = true;
            bgmSource.playOnAwake = false;
            bgmSource.volume = bgmVolume;
        }


        // -------------------------------------------------
        // SE設定
        // -------------------------------------------------

        if (seSource != null)
        {
            seSource.loop = false;
            seSource.playOnAwake = false;
            seSource.volume = seVolume;
        }
    }


    // =====================================================
    // BGM再生
    // =====================================================

    /// <summary>
    /// 指定したBGMを再生する。
    /// </summary>
    public void PlayBGM(BGMType type)
    {
        if (type == BGMType.None)
            return;


        // 現在と同じBGMなら何もしない
        if (currentBGM == type &&
            bgmSource != null &&
            bgmSource.isPlaying)
        {
            return;
        }


        AudioClip clip =
            GetBGMClip(type);


        if (clip == null)
        {
            Debug.LogWarning(
                $"AudioManager: BGMが設定されていません。Type = {type}"
            );

            return;
        }


        if (bgmSource == null)
        {
            Debug.LogWarning(
                "AudioManager: BGM用AudioSourceが設定されていません。"
            );

            return;
        }


        currentBGM = type;


        bgmSource.clip = clip;
        bgmSource.volume = bgmVolume;
        bgmSource.loop = true;
        bgmSource.Play();
    }


    // =====================================================
    // BGM停止
    // =====================================================

    /// <summary>
    /// BGMを停止する。
    /// </summary>
    public void StopBGM()
    {
        if (bgmSource == null)
            return;


        bgmSource.Stop();

        bgmSource.clip = null;

        currentBGM = BGMType.None;
    }


    // =====================================================
    // BGM一時停止
    // =====================================================

    /// <summary>
    /// BGMを一時停止する。
    /// </summary>
    public void PauseBGM()
    {
        if (bgmSource == null)
            return;


        if (bgmSource.isPlaying)
        {
            bgmSource.Pause();
        }
    }


    // =====================================================
    // BGM再開
    // =====================================================

    /// <summary>
    /// 一時停止したBGMを再開する。
    /// </summary>
    public void ResumeBGM()
    {
        if (bgmSource == null)
            return;


        if (!bgmSource.isPlaying &&
            bgmSource.clip != null)
        {
            bgmSource.UnPause();
        }
    }


    // =====================================================
    // SE再生
    // =====================================================

    /// <summary>
    /// 指定したSEを再生する。
    /// </summary>
    public void PlaySE(SEType type)
    {
        PlaySE(type, 1f);
    }


    /// <summary>
    /// 指定したSEを音量倍率付きで再生する。
    /// </summary>
    public void PlaySE(
        SEType type,
        float volumeMultiplier)
    {
        AudioClip clip =
            GetSEClip(type);


        if (clip == null)
        {
            Debug.LogWarning(
                $"AudioManager: SEが設定されていません。Type = {type}"
            );

            return;
        }


        if (seSource == null)
        {
            Debug.LogWarning(
                "AudioManager: SE用AudioSourceが設定されていません。"
            );

            return;
        }


        float volume =
            GetSEVolume(type);


        volume *= volumeMultiplier;


        volume = Mathf.Clamp01(volume);


        seSource.PlayOneShot(
            clip,
            volume
        );
    }


    // =====================================================
    // BGM取得
    // =====================================================

    private AudioClip GetBGMClip(
        BGMType type)
    {
        for (int i = 0; i < bgmList.Count; i++)
        {
            if (bgmList[i] == null)
                continue;


            if (bgmList[i].type == type)
            {
                return bgmList[i].clip;
            }
        }


        return null;
    }


    // =====================================================
    // SE取得
    // =====================================================

    private AudioClip GetSEClip(
        SEType type)
    {
        for (int i = 0; i < seList.Count; i++)
        {
            if (seList[i] == null)
                continue;


            if (seList[i].type == type)
            {
                return seList[i].clip;
            }
        }


        return null;
    }


    // =====================================================
    // SE音量取得
    // =====================================================

    private float GetSEVolume(
        SEType type)
    {
        for (int i = 0; i < seList.Count; i++)
        {
            if (seList[i] == null)
                continue;


            if (seList[i].type == type)
            {
                return seList[i].volume;
            }
        }


        return 1f;
    }


    // =====================================================
    // BGM音量変更
    // =====================================================

    public void SetBGMVolume(
        float volume)
    {
        bgmVolume =
            Mathf.Clamp01(volume);


        if (bgmSource != null)
        {
            bgmSource.volume =
                bgmVolume;
        }
    }


    // =====================================================
    // SE音量変更
    // =====================================================

    public void SetSEVolume(
        float volume)
    {
        seVolume =
            Mathf.Clamp01(volume);


        if (seSource != null)
        {
            seSource.volume =
                seVolume;
        }
    }


    // =====================================================
    // 音量取得
    // =====================================================

    public float GetBGMVolume()
    {
        return bgmVolume;
    }


    public float GetSEVolume()
    {
        return seVolume;
    }
}