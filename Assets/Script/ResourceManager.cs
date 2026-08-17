using System;
using UnityEngine;

/// <summary>
/// ゲーム内のお金を管理する。
/// </summary>
public class ResourceManager : MonoBehaviour
{
    public static ResourceManager Instance { get; private set; }


    [Header("初期所持金")]
    [SerializeField]
    private int startingMoney = 200;


    private int currentMoney;


    /// <summary>
    /// 所持金が変更されたときに呼ばれる。
    /// </summary>
    public Action<int> OnMoneyChanged;


    /// <summary>
    /// 現在の所持金。
    /// </summary>
    public int CurrentMoney => currentMoney;


    private void Awake()
    {
        if (Instance != null &&
            Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }


    private void Start()
    {
        currentMoney = startingMoney;

        OnMoneyChanged?.Invoke(currentMoney);
    }


    /// <summary>
    /// お金が足りているか確認する。
    /// </summary>
    public bool CanAfford(int amount)
    {
        return currentMoney >= amount;
    }


    /// <summary>
    /// お金を消費する。
    /// </summary>
    public bool SpendMoney(int amount)
    {
        if (amount < 0)
            return false;

        if (!CanAfford(amount))
            return false;

        currentMoney -= amount;

        OnMoneyChanged?.Invoke(currentMoney);

        return true;
    }


    /// <summary>
    /// お金を追加する。
    /// </summary>
    public void AddMoney(int amount)
    {
        if (amount <= 0)
            return;

        currentMoney += amount;

        OnMoneyChanged?.Invoke(currentMoney);
    }
}