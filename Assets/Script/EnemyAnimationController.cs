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
    // SpriteRenderer
    // =====================================================

    [Header("被ダメージ演出")]
    [Tooltip("被ダメージ時に赤くするSpriteRenderer")]
    [SerializeField]
    private SpriteRenderer spriteRenderer;


    [Tooltip("赤くする時間")]
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
        // -------------------------------------------------
        // SpriteRenderer
        // -------------------------------------------------

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
        else
        {
            Debug.LogWarning(
                $"{gameObject.name}: " +
                "SpriteRendererが見つかりません。",
                this
            );
        }


        // -------------------------------------------------
        // Animator
        // -------------------------------------------------

        if (animator == null)
        {
            animator =
                GetComponentInChildren<Animator>();
        }


        if (animator == null)
        {
            Debug.LogWarning(
                $"{gameObject.name}: " +
                "Animatorが見つかりません。",
                this
            );
        }
    }


    // =====================================================
    // 被ダメージ
    // =====================================================

    /// <summary>
    /// 被ダメージ時の赤点滅。
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


        damageFlashCoroutine =
            StartCoroutine(
                DamageFlashCoroutine()
            );
    }


    /// <summary>
    /// Spriteを赤くする。
    /// </summary>
    private IEnumerator DamageFlashCoroutine()
    {
        // 赤色
        spriteRenderer.color =
            Color.red;


        // 待機
        yield return new WaitForSeconds(
            damageFlashDuration
        );


        // 元に戻す
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
                $"{gameObject.name}: " +
                "Animatorが設定されていません。",
                this
            );

            return;
        }


        Debug.Log(
            $"{gameObject.name}: 死亡アニメーション開始"
        );


        animator.SetTrigger("Die");
    }


    // =====================================================
    // Animation Event用
    // =====================================================

    /// <summary>
    /// 死亡アニメーションの最後に
    /// Animation Eventから呼び出す。
    ///
    /// Enemy.csのFinishDeathAnimation()を呼び出す。
    /// </summary>
    public void OnDeathAnimationFinished()
    {
        Enemy enemy =
            GetComponent<Enemy>();


        if (enemy == null)
        {
            enemy =
                GetComponentInParent<Enemy>();
        }


        if (enemy == null)
        {
            Debug.LogError(
                $"{gameObject.name}: " +
                "Enemy.csが見つかりません。",
                this
            );

            return;
        }


        Debug.Log(
            $"{gameObject.name}: " +
            "Animation Event → 死亡アニメーション終了"
        );


        enemy.FinishDeathAnimation();
    }


    // =====================================================
    // 死亡アニメーション長さ
    // =====================================================

    /// <summary>
    /// 死亡アニメーションの長さを取得する。
    ///
    /// Animation Event方式を使う場合は
    /// 基本的に使用しない。
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