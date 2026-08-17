using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ƒ^ƒ[‚ÌUŒ‚ˆ—‚ğŠÇ—‚·‚éB
/// ”ÍˆÍ“à‚Ì“G‚©‚çAÅ‚ài‚ñ‚Å‚¢‚é“G‚ğƒ^[ƒQƒbƒg‚É‚µ‚ÄUŒ‚‚·‚éB
/// </summary>
public class Tower : MonoBehaviour
{
    [Header("ƒ^ƒ[ƒf[ƒ^")]
    public TowerData towerData;

    // UŒ‚”ÍˆÍ“à‚É‚¢‚é“G
    private readonly List<Enemy> enemiesInRange =
        new List<Enemy>();

    // Œ»İ‚Ìƒ^[ƒQƒbƒg
    private Enemy currentTarget;

    // Ÿ‚ÉUŒ‚‚Å‚«‚éŠÔ
    private float nextAttackTime;


    private CircleCollider2D rangeCollider;


    private void Awake()
    {
        rangeCollider =
            GetComponent<CircleCollider2D>();

        if (rangeCollider == null)
        {
            Debug.LogError(
                $"{gameObject.name}: Circle Collider 2D‚ª‚ ‚è‚Ü‚¹‚ñB",
                this
            );

            return;
        }

        rangeCollider.isTrigger = true;

        UpdateAttackRange();
    }


    private void Update()
    {
        if (towerData == null)
            return;

        RemoveInvalidEnemies();

        currentTarget = FindMostAdvancedEnemy();

        if (currentTarget == null)
            return;

        if (Time.time >= nextAttackTime)
        {
            Attack();
        }
    }


    /// <summary>
    /// ƒ^ƒ[‚ÌUŒ‚”ÍˆÍ‚ğİ’è‚·‚éB
    /// </summary>
    private void UpdateAttackRange()
    {
        if (rangeCollider == null)
            return;

        if (towerData == null)
            return;

        rangeCollider.radius =
            towerData.attackRange;
    }


    /// <summary>
    /// UŒ‚”ÍˆÍ‚É“G‚ª“ü‚Á‚½‚Æ‚«B
    /// </summary>
    private void OnTriggerEnter2D(Collider2D other)
    {
        Enemy enemy =
            other.GetComponentInParent<Enemy>();

        if (enemy == null)
            return;

        if (!enemiesInRange.Contains(enemy))
        {
            enemiesInRange.Add(enemy);
        }
    }


    /// <summary>
    /// UŒ‚”ÍˆÍ‚©‚ç“G‚ªo‚½‚Æ‚«B
    /// </summary>
    private void OnTriggerExit2D(Collider2D other)
    {
        Enemy enemy =
            other.GetComponentInParent<Enemy>();

        if (enemy == null)
            return;

        enemiesInRange.Remove(enemy);

        if (currentTarget == enemy)
        {
            currentTarget = null;
        }
    }


    /// <summary>
    /// null‚É‚È‚Á‚½“G‚â€–S‚µ‚½“G‚ğƒŠƒXƒg‚©‚çíœ‚·‚éB
    /// </summary>
    private void RemoveInvalidEnemies()
    {
        enemiesInRange.RemoveAll(
            enemy =>
                enemy == null ||
                !enemy.gameObject.activeInHierarchy
        );
    }


    /// <summary>
    /// ”ÍˆÍ“à‚ÅÅ‚ài‚ñ‚Å‚¢‚é“G‚ğ’T‚·B
    /// </summary>
    private Enemy FindMostAdvancedEnemy()
    {
        Enemy bestEnemy = null;

        float bestProgress = float.MinValue;

        foreach (Enemy enemy in enemiesInRange)
        {
            if (enemy == null)
                continue;

            if (!enemy.gameObject.activeInHierarchy)
                continue;

            float progress =
                enemy.RouteProgress;

            if (progress > bestProgress)
            {
                bestProgress = progress;
                bestEnemy = enemy;
            }
        }

        return bestEnemy;
    }


    /// <summary>
    /// “G‚ğUŒ‚‚·‚éB
    /// </summary>
    private void Attack()
    {
        if (currentTarget == null)
            return;

        currentTarget.TakeDamage(
            towerData.attackDamage
        );

        nextAttackTime =
            Time.time +
            towerData.attackInterval;
    }
}