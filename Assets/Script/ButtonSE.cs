using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// ボタンのホバー時・クリック時にSEを再生する。
/// </summary>
public class ButtonSE : MonoBehaviour,
    IPointerEnterHandler,
    IPointerClickHandler
{
    // =====================================================
    // マウスカーソルがボタンに入ったとき
    // =====================================================

    public void OnPointerEnter(
        PointerEventData eventData)
    {
        if (AudioManager.Instance == null)
            return;

        AudioManager.Instance.PlaySE(
            AudioManager.SEType.ButtonHover
        );
    }


    // =====================================================
    // ボタンをクリックしたとき
    // =====================================================

    public void OnPointerClick(
        PointerEventData eventData)
    {
        if (AudioManager.Instance == null)
            return;

        AudioManager.Instance.PlaySE(
            AudioManager.SEType.ButtonClick
        );
    }
}