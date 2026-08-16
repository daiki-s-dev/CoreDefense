using UnityEngine;

/// <summary>
/// 拠点となるコアを管理する。
/// 敵が到達するとダメージを受け、HPが0になるとゲームオーバーにする。
/// </summary>
public class Core : MonoBehaviour
{
    [Header("コアHP")]
    [Tooltip("コアの最大HP")]
    public int maxHP = 10;

    // 現在のHP
    public int CurrentHP { get; private set; }

    // ゲームオーバー済みか
    private bool isDestroyed = false;


    private void Awake()
    {
        // ゲーム開始時に最大HPまで回復
        CurrentHP = maxHP;
    }


    /// <summary>
    /// 敵からダメージを受ける。
    /// Enemy.csから呼び出される。
    /// </summary>
    public void TakeDamage(int damage)
    {
        // すでにゲームオーバーなら処理しない
        if (isDestroyed)
            return;

        // 0以下のダメージは無視
        if (damage <= 0)
            return;

        // HPを減らす
        CurrentHP -= damage;

        // HPがマイナスにならないようにする
        CurrentHP = Mathf.Max(CurrentHP, 0);

        Debug.Log(
            $"Coreが{damage}ダメージを受けました。 " +
            $"残りHP：{CurrentHP}/{maxHP}"
        );

        // HPが0になったか確認
        if (CurrentHP <= 0)
        {
            GameOver();
        }
    }


    /// <summary>
    /// ゲームオーバー処理。
    /// </summary>
    private void GameOver()
    {
        if (isDestroyed)
            return;

        isDestroyed = true;

        Debug.Log("Coreが破壊されました。GAME OVER");

        // ここで後ほどゲームオーバーUIを表示する。
        // WaveManagerなどからゲームを停止する処理も追加予定。
    }


    /// <summary>
    /// 現在のHPの割合を取得する。
    /// HPバーなどで使用できる。
    /// </summary>
    public float GetHPRatio()
    {
        if (maxHP <= 0)
            return 0f;

        return (float)CurrentHP / maxHP;
    }


    /// <summary>
    /// コアが破壊されているか取得する。
    /// </summary>
    public bool IsDestroyed()
    {
        return isDestroyed;
    }
}