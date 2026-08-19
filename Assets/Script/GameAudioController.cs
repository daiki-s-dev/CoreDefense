using UnityEngine;

/// <summary>
/// GameSceneのBGMを管理する。
/// Wave開始前とWave中でBGMを切り替える。
/// </summary>
public class GameAudioController : MonoBehaviour
{
    private void Start()
    {
        PlayPreparationBGM();
    }


    /// <summary>
    /// Wave開始前のBGMを再生する。
    /// </summary>
    public void PlayPreparationBGM()
    {
        if (AudioManager.Instance == null)
            return;


        AudioManager.Instance.PlayBGM(
            AudioManager.BGMType.GamePreparation
        );
    }


    /// <summary>
    /// Wave中のBGMを再生する。
    /// </summary>
    public void PlayWaveBGM()
    {
        if (AudioManager.Instance == null)
            return;


        AudioManager.Instance.PlayBGM(
            AudioManager.BGMType.GameWave
        );
    }
}