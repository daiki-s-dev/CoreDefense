using UnityEngine;

public class TitleAudioController : MonoBehaviour
{
    private void Start()
    {
        if (AudioManager.Instance == null)
            return;

        AudioManager.Instance.PlayBGM(
            AudioManager.BGMType.Title
        );
    }
}