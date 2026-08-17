using UnityEngine;

/// <summary>
/// タワーの攻撃範囲を常時表示する。
///
/// 攻撃判定用のCircleCollider2Dとは完全に分離して使用する。
/// </summary>
public class TowerRangeDisplay : MonoBehaviour
{
    [Header("表示設定")]
    [Tooltip("攻撃範囲を表示するSpriteRenderer")]
    [SerializeField]
    private SpriteRenderer rangeRenderer;


    [Tooltip("攻撃範囲の表示倍率")]
    [SerializeField]
    private float displayScale = 1f;


    /// <summary>
    /// 攻撃範囲の表示サイズを変更する。
    /// </summary>
    public void SetRange(float range)
    {
        if (rangeRenderer == null)
            return;


        // Spriteの元サイズを基準に拡大する
        transform.localScale =
            Vector3.one *
            range *
            displayScale;
    }


    private void Awake()
    {
        if (rangeRenderer == null)
        {
            rangeRenderer =
                GetComponent<SpriteRenderer>();
        }


        if (rangeRenderer == null)
        {
            Debug.LogError(
                $"{gameObject.name}: " +
                "SpriteRendererがありません。",
                this
            );
        }
    }
}