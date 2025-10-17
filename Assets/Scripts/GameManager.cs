using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Audio;

public class GameManager : MonoBehaviour
{
    public AudioClip WinSound;
    private AudioSource audioSource;

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

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        audioSource.playOnAwake = false;
    
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
        foreach (var bottle in bottles)
        {
            bottle.GetComponent<Bottle>().RegisterGlass(g);
        }
    }

    private void GenerateNewTarget()
    {
        targetQuantities.Clear();

        int bottleCount = Random.Range(minBottles, maxBottles + 1);
        List<string> allBottles = new List<string> { "Bottle1", "Bottle2", "Bottle3", "Bottle4" };

        List<string> chosenBottles = new List<string>();
        for (int i = 0; i < bottleCount; i++)
        {
            int index = Random.Range(0, allBottles.Count);
            chosenBottles.Add(allBottles[index]);
            allBottles.RemoveAt(index);
        }

        float remaining = totalTargetFill;
        for (int i = 0; i < chosenBottles.Count; i++)
        {
            string bottle = chosenBottles[i];
            float amount;

            if (i == chosenBottles.Count - 1)
                amount = remaining;
            else
                amount = Random.Range(10f, remaining - 10f * (chosenBottles.Count - i - 1));

            remaining -= amount;
            targetQuantities[bottle] = Mathf.Round(amount);
        }

        string debugMsg = "Objectif :\n";
        foreach (var kvp in targetQuantities)
        {
            debugMsg += $"{kvp.Key} → {kvp.Value}%\n";
        }

        Debug.Log(debugMsg);
    }

    public float drunkIncreaseOnWin = 0.1f;

    public void OnRoundEnd(bool win)
    {
        if (win)
        {
            Debug.Log("Victoire !");
            audioSource.clip = WinSound;
            audioSource.Play();
        }
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
}
