using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public int minBottles = 1;
    public int maxBottles = 3;
    public float totalTargetFill = 100f;
    public List<GameObject> bottles;
    public GameObject glassPrefab;
    public float delayBeforeNextRound = 1f;

    [HideInInspector] public Dictionary<string, float> targetQuantities = new Dictionary<string, float>();
    private GameObject currentGlass;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        StartCoroutine(StartNewRound());
    }

    public IEnumerator StartNewRound()
    {
        yield return new WaitForSeconds(delayBeforeNextRound);

        if (currentGlass != null)
        {
            Destroy(currentGlass);
            yield return new WaitForSeconds(0.2f);
        }

        GenerateNewTarget();

        currentGlass = Instantiate(glassPrefab, glassPrefab.transform.position, Quaternion.identity);
        Glass g = currentGlass.GetComponent<Glass>();
        
        // Notifier les observateurs du nouveau verre
        OnGlassSpawned?.Invoke(g);

        foreach (var bottle in bottles)
        {
            bottle.GetComponent<Bottle>().RegisterGlass(g);
        }
    }

    private void GenerateNewTarget()
    {
        targetQuantities.Clear();

        int bottleCount = Random.Range(minBottles, Mathf.Min(maxBottles + 1, bottles.Count));
        List<GameObject> available = new List<GameObject>(bottles);

        List<GameObject> chosen = new List<GameObject>();
        for (int i = 0; i < bottleCount; i++)
        {
            int index = Random.Range(0, available.Count);
            chosen.Add(available[index]);
            available.RemoveAt(index);
        }

        float remaining = totalTargetFill;
        foreach (var bottleObj in chosen)
        {
            var bottle = bottleObj.GetComponent<Bottle>();
            string id = bottle.tag;

            float amount;
            if (bottleObj == chosen[^1])
                amount = remaining;
            else
                amount = Random.Range(10f, remaining - 10f * (chosen.Count - chosen.IndexOf(bottleObj) - 1));

            remaining -= amount;
            targetQuantities[id] = Mathf.Round(amount);
        }

        string debugMsg = "Objectif :\n";
        foreach (var kvp in targetQuantities)
            debugMsg += $"{kvp.Key} → {kvp.Value}%\n";

        Debug.Log(debugMsg);
    }

    public float drunkIncreaseOnWin = 0.1f;

    public void OnRoundEnd(bool win)
    {
        if (win)
            Debug.Log("Victoire !");
        else
            Debug.Log("Défaite...");

        if (win)
        {
            var hand = GameObject.FindAnyObjectByType<HandController>(FindObjectsInactive.Exclude);
            if (hand != null)
                hand.IncreaseDrunk(drunkIncreaseOnWin);
        }

        StartCoroutine(StartNewRound());
    }

    public System.Action<Glass> OnGlassSpawned;
}
