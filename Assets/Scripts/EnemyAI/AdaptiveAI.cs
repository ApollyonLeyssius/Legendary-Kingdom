using UnityEngine;

public class AdaptiveAI : MonoBehaviour
{
    public enum PlayerAction
    {
        LightAttack,
        HeavyAttack
    }

    [Header("Adaptive AI")]
    [SerializeField] private int repeatedActionsRequired = 3;

    private PlayerAction lastAction;
    private int repeatedActionCount;
    private bool hasPreviousAction;

    private EnemyCounter enemyCounter;

    private void Awake()
    {
        enemyCounter = GetComponent<EnemyCounter>();

        if (enemyCounter == null)
        {
            Debug.LogError("AdaptiveAI: EnemyCounter is missing!");
        }
    }

    public void RegisterAction(PlayerAction action)
    {
        if (hasPreviousAction && action == lastAction)
        {
            repeatedActionCount++;
        }
        else
        {
            lastAction = action;
            repeatedActionCount = 1;
            hasPreviousAction = true;
        }

        Debug.Log(
            "Player used " + action +
            " | Repeated: " + repeatedActionCount
        );

        if (repeatedActionCount >= repeatedActionsRequired)
        {
            TriggerCounter(action);
        }
    }

    private void TriggerCounter(PlayerAction action)
    {
        if (enemyCounter == null)
        {
            Debug.LogError("Cannot counter: EnemyCounter missing!");
            return;
        }

        Debug.Log("Enemy recognized repeated " + action);

        bool counterSucceeded = enemyCounter.CounterAttack();

        if (counterSucceeded)
        {
            repeatedActionCount = 0;
        }
    }
}