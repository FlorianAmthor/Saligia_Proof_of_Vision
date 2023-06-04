using SaligiaProofOfVision;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyScaling", menuName = "GamePlay/EnemyScaling")]
public class EnemyScalingSO : ScriptableObject
{
    public int maxWrathEnemyCount;
    public AnimationCurve curveWrath;
    [HideInInspector]
    public int currentWrathEnemyCount = 0;
    public int maxGluttonyEnemyCount;
    public AnimationCurve curveGluttony;
    [HideInInspector]
    public int currentGluttonyEnemyCount = 0;
    public int maxPrideEnemyCount;
    public AnimationCurve curvePride;
    [HideInInspector]
    public int currentPrideEnemyCount = 0;

    private void OnEnable()
    {
        GameEvents.resetGameEvent += ResetValues;
        GameEvents.trashEnemyDeathEvent += OnTrashEnemyDeath;
        GameEvents.bossEnemyDeathEvent += OnBossEnemyDeath;
    }

    private void OnDisable()
    {
        GameEvents.resetGameEvent -= ResetValues;
        GameEvents.trashEnemyDeathEvent -= OnTrashEnemyDeath;
        GameEvents.bossEnemyDeathEvent -= OnBossEnemyDeath;
    }


    private void OnTrashEnemyDeath(EnemyUnit enemy)
    {
        switch (enemy.sinType)
        {
            case SinType.Gula:
                currentGluttonyEnemyCount = Mathf.Clamp(currentGluttonyEnemyCount + 1, 0, maxGluttonyEnemyCount); break;
            case SinType.Ira:
                currentWrathEnemyCount = Mathf.Clamp(currentWrathEnemyCount + 1, 0, maxWrathEnemyCount); break;
            case SinType.Superbia:
                currentPrideEnemyCount = Mathf.Clamp(currentPrideEnemyCount + 1, 0, maxPrideEnemyCount); break;
            default:
                break;
        }
    }

    private void OnBossEnemyDeath(EnemyUnit enemy)
    {
        switch (enemy.sinType)
        {
            case SinType.Gula:
                currentGluttonyEnemyCount = maxGluttonyEnemyCount; break;
            case SinType.Ira:
                currentWrathEnemyCount = maxWrathEnemyCount; break;
            case SinType.Superbia:
                currentPrideEnemyCount = maxPrideEnemyCount; break;
            default:
                break;
        }
    }

    public float GetSinMod(SinType sinType)
    {
        switch (sinType)
        {
            case SinType.Gula:
                return curveGluttony.Evaluate((float)currentGluttonyEnemyCount / maxGluttonyEnemyCount);
            case SinType.Ira:
                return curveWrath.Evaluate((float)currentWrathEnemyCount / maxWrathEnemyCount);
            case SinType.Superbia:
                return curvePride.Evaluate((float)currentPrideEnemyCount / maxPrideEnemyCount);
            default:
                return 0;
        }
    }

    private void ResetValues()
    {
        currentGluttonyEnemyCount = 0;
        currentPrideEnemyCount = 0;
        currentWrathEnemyCount = 0;
    }
}
