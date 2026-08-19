using UnityEngine;

public class ClearAudioController : MonoBehaviour
{
    private void Start()
    {
        if (AudioManager.Instance == null)
            return;

        AudioManager.Instance.PlayBGM(
            AudioManager.BGMType.Clear
        );
    }
}