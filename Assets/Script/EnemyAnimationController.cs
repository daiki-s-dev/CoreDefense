using System.Collections;
using UnityEngine;

/// <summary>
/// 敵のアニメーション・被ダメージ演出・死亡演出を管理する。
/// </summary>
public class EnemyAnimationController : MonoBehaviour
{
    // =====================================================
    // Animator
    // =====================================================

    [Header("Animator")]
    [Tooltip("敵のAnimator")]
    [SerializeField]
    private Animator animator;


    // =====================================================
    // 被ダメージ演出
    // =====================================================

    [Header("被ダメージ演出")]
    [Tooltip("被ダメージ時に赤くするSpriteRenderer")]
    [SerializeField]
    private SpriteRenderer spriteRenderer;

    [Tooltip("赤く点滅する時間")]
    [SerializeField]
    private float damageFlashDuration = 0.1f;


    // =====================================================
    // 色
    // =====================================================

    private Color originalColor;

    private Coroutine damageFlashCoroutine;


    // =====================================================
    // 初期化
    // =====================================================

    private void Awake()
    {
        // SpriteRendererが設定されていない場合
        if (spriteRenderer == null)
        {
            spriteRenderer =
                GetComponentInChildren<SpriteRenderer>();
        }


        if (spriteRenderer != null)
        {
            originalColor =
                spriteRenderer.color;
        }


        // Animatorが設定されていない場合
        if (animator == null)
        {
            animator =
                GetComponent<Animator>();
        }
    }


    // =====================================================
    // 被ダメージ
    // =====================================================

    /// <summary>
    /// 被ダメージ時の演出。
    /// </summary>
    public void PlayDamageEffect()
    {
        if (spriteRenderer == null)
            return;


        // すでに点滅中なら停止
        if (damageFlashCoroutine != null)
        {
            StopCoroutine(
                damageFlashCoroutine
            );
        }


        // 赤点滅開始
        damageFlashCoroutine =
            StartCoroutine(
                DamageFlashCoroutine()
            );
    }


    /// <summary>
    /// 赤く点滅させる。
    /// </summary>
    private IEnumerator DamageFlashCoroutine()
    {
        // 赤色
        spriteRenderer.color =
            Color.red;


        // 少し待つ
        yield return new WaitForSeconds(
            damageFlashDuration
        );


        // 元の色に戻す
        spriteRenderer.color =
            originalColor;


        damageFlashCoroutine = null;
    }


    // =====================================================
    // 死亡
    // =====================================================

    /// <summary>
    /// 死亡アニメーションを再生する。
    /// </summary>
    public void PlayDeathAnimation()
    {
        if (animator == null)
        {
            Debug.LogWarning(
                $"{gameObject.name}: Animatorが設定されていません。",
                this
            );

            return;
        }


        animator.SetTrigger("Die");
    }


    /// <summary>
    /// 死亡アニメーションの長さを取得する。
    /// </summary>
    public float GetDeathAnimationLength()
    {
        if (animator == null)
            return 0f;


        RuntimeAnimatorController controller =
            animator.runtimeAnimatorController;


        if (controller == null)
            return 0f;


        foreach (
            AnimationClip clip
            in controller.animationClips)
        {
            if (clip.name == "Enemy_Death")
            {
                return clip.length;
            }
        }


        return 0f;
    }


    // =====================================================
    // 色リセット
    // =====================================================

    /// <summary>
    /// 色を元に戻す。
    /// </summary>
    public void ResetColor()
    {
        if (spriteRenderer == null)
            return;


        if (damageFlashCoroutine != null)
        {
            StopCoroutine(
                damageFlashCoroutine
            );

            damageFlashCoroutine = null;
        }


        spriteRenderer.color =
            originalColor;
    }
}