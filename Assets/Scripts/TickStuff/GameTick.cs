using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The class used to tick for whenever new game events should occur
/// </summary>
public class GameTick : MonoBehaviour
{
    public static GameTick Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<GameTick>();
                if (_instance == null)
                {
                    _instance = new GameObject("Game Ticker").AddComponent<GameTick>();
                }
            }
            return _instance;
        }
    }

    private static GameTick _instance;

    public System.Action Ticked;

    [SerializeField] private float tickRate = 300;
    private WaitForSeconds tickPeriod;

    public bool stopTick;

    private bool performTickOnUnpause;


    [SerializeField] private int performTickAmount = 0;

    public UpdateRelationships UpdateRelationships;
    public NpcPlacements NpcPlacements;
    private void Awake()
    {
        if (Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
        tickPeriod = new WaitForSeconds(tickRate);

        StartCoroutine(DelayInitialTick());
    }
    private void PerformMultipleTicks()
    {
        for (int i = 0; i < performTickAmount; i++)
        {
            Debug.Log("Performed Tick!");
            UpdateRelationships.UpdateFromTick();
            NpcPlacements.UpdateFromTick();

            Ticked?.Invoke();
            GameManager.Instance.SaveLoader.SaveData();
        }
    }
    private IEnumerator DelayInitialTick()
    {
        yield return null;
        yield return null;
        if (performTickAmount > 0)
        {
            PerformMultipleTicks();
        }
        PerformTick();
    }

    public void ForceTick()
    {
        PerformTick(true);
    }
    private void PerformTick(bool force = false)
    {

        if (stopTick && !force)
        {
            performTickOnUnpause = true;
            return;
        }
        Debug.Log("Performed Tick!");
        UpdateRelationships.UpdateFromTick();
        NpcPlacements.UpdateFromTick();

        Ticked?.Invoke();
        GameManager.Instance.SaveLoader.SaveData();
        
        StartCoroutine(DelayNextTick());
    }
    private IEnumerator DelayNextTick()
    {
        yield return tickPeriod;
        PerformTick();
    }

    public void ToggleTickSave(bool enable)
    {
        if(enable)
        {
            stopTick = false;
            if (performTickOnUnpause)
            {
                performTickOnUnpause = false;
                PerformTick();
            }
        }
        else 
        {
            stopTick = true;  
        }

    }
}
