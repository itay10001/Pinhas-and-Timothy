using JetBrains.Annotations;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(Rigidbody2D))]
public class Enemy : MonoBehaviour
{

    private Vector2 enemymoves;

    private bool enemyIsAlive = false;
    public bool IsAlive => enemyIsAlive;

    private float spawnRate;

    private int currentEnemyType;

    private double enemyHP;
    private double enemyDMG;
    private double enemySPD;
    private double enemyCD;

    private Rigidbody2D rb;
    private SpriteRenderer enemyRenderer;
    private Collider2D enemyCollider;

    void Start()
    {
        
    }
    

    private void EnemyType(int enemyType)
    {
        int buff = 1;
        int quick = 2;
        int balanced = 3;
        int dummyType = 4;


        
        double buffHP = Random.Range(150.0f, 250.0f);
        double buffDMG = Random.Range(10.0f, 20.0f);
        double buffSPD = Random.Range(1.0f, 3.0f);
        double buffCD = Random.Range(1.0f, 3.0f);

        
        double quickHP = Random.Range(25.0f, 75.0f);
        double quickDMG = Random.Range(5.0f, 10.0f);
        double quickSPD = Random.Range(5.0f, 8.0f);
        double quickCD = Random.Range(0.25f, 0.75f);

        

        double balancedHP = Random.Range(75.0f, 150.0f);
        double balancedDMG = Random.Range(10.0f, 15.0f);
        double balancedSPD = Random.Range(3.0f, 5.0f);
        double balancedCD = Random.Range(0.75f, 1.5f);

        double dummyHP =
            90.0 * 90.0 * 90.0 * 90.0;

        double dummyDMG = 0;
        double dummySPD = 0;

        double dummyCD =
            90.0 * 90.0 * 90.0 * 90.0 *
            90.0 * 90.0 * 90.0 * 90.0 *
            90.0 * 90.0 * 90.0 * 90.0;

        currentEnemyType = enemyType;

        if (enemyType == buff)
        {
            enemyHP = buffHP;
            enemyDMG = buffDMG;
            enemySPD = buffSPD;
            enemyCD = buffCD;
        }
        else if (enemyType == quick)
        {
            enemyHP = quickHP;
            enemyDMG = quickDMG;
            enemySPD = quickSPD;
            enemyCD = quickCD;
        }
        else if (enemyType == balanced)
        {
            enemyHP = balancedHP;
            enemyDMG = balancedDMG;
            enemySPD = balancedSPD;
            enemyCD = balancedCD;
        }
        else if (enemyType == dummyType)
        {
            enemyHP = dummyHP;
            enemyDMG = dummyDMG;
            enemySPD = dummySPD;
            enemyCD = dummyCD;
        }

        Debug.Log(
            "Enemy Type: " + currentEnemyType +
            " | HP: " + enemyHP +
            " | DMG: " + enemyDMG +
            " | SPD: " + enemySPD +
            " | CD: " + enemyCD
        );
    }


    public double GetEnemyHP()
    {
        return enemyHP;
    }

    public double GetEnemySpeed()
    {
        return enemySPD;
    }

    public double GetEnemyDamage()
    {
        return enemyDMG;
    }

    public double GetEnemyCooldown()
    {
        return enemyCD;
    }

    public double EnemyTakesDamage(int damage)
    {
        if (!enemyIsAlive || damage <= 0)
        {
            return enemyHP;
        }

        enemyHP -= damage;

        enemyHP = Mathf.Max((float)enemyHP, 0f);

        return enemyHP;
    }
}
