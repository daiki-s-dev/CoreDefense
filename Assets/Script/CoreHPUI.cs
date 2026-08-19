using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// CoreのHPをUIに表示する。
/// CoreのCurrentHPと連動してHPバーを更新する。
/// </summary>
public class CoreHPUI : MonoBehaviour
{
    [Header("Core")]
    [Tooltip("HPを表示するCore")]
    [SerializeField]
    private Core core;


    [Header("HPバー")]
    [Tooltip("HPバーのFill Image")]
    [SerializeField]
    private Image hpFill;


    [Header("HPテキスト")]
    [Tooltip("現在HPを表示するText")]
    [SerializeField]
    private TMP_Text hpText;


    private void Start()
    {
        // 初期表示
        UpdateHPUI();
    }


    private void Update()
    {
        // Coreが設定されていなければ処理しない
        if (core == null)
            return;


        UpdateHPUI();
    }


    /// <summary>
    /// CoreのHPをUIに反映する。
    /// </summary>
    private void UpdateHPUI()
    {
        if (core == null)
            return;


        // =========================================
        // HPバー
        // =========================================

        if (hpFill != null)
        {
            hpFill.fillAmount = core.GetHPRatio();
        }


        // =========================================
        // HPテキスト
        // =========================================

        if (hpText != null)
        {
            hpText.text =
                $"{core.CurrentHP} / {core.maxHP}";
        }
    }
}