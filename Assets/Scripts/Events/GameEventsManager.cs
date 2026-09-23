using UnityEngine;

public class GameEventsManager : MonoBehaviour
{
    public static GameEventsManager instance {  get; private set; }

    public GoldEvents goldEvents;
    public MiscEvents miscEvents;
    public QuestEvents questEvents;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Debug.LogError("Found more than one GameEventsManager. Destroying duplicate: " + gameObject.name);
            Destroy(gameObject);
            return;
        }
        instance = this;

        goldEvents = new GoldEvents();
        miscEvents = new MiscEvents();
        questEvents = new QuestEvents();
    }
}
