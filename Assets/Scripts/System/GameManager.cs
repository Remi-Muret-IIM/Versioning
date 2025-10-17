using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Audio;

public class GameManager : MonoBehaviour
{
    public AudioClip WinSound;
    public AudioClip LooseSound;
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
        // Reset previous targets
        targetQuantities.Clear();

        // Generate new random targets
        int numBottles = Random.Range(minBottles, maxBottles + 1);
        float remainingFill = totalTargetFill;

        // Randomly select bottles and assign quantities
        List<GameObject> selectedBottles = new List<GameObject>();
        List<GameObject> availableBottles = new List<GameObject>(bottles);

        for (int i = 0; i < numBottles && availableBottles.Count > 0; i++)
        {
            int index = Random.Range(0, availableBottles.Count);
            selectedBottles.Add(availableBottles[index]);
            availableBottles.RemoveAt(index);
        }

        // Distribute fill amounts
        for (int i = 0; i < selectedBottles.Count; i++)
        {
            float targetFill;
            if (i == selectedBottles.Count - 1)
            {
                targetFill = remainingFill;
            }
            else
            {
                targetFill = Random.Range(10f, remainingFill - (10f * (selectedBottles.Count - i - 1)));
            }
            
            string bottleTag = selectedBottles[i].tag;
            targetQuantities[bottleTag] = targetFill;
            remainingFill -= targetFill;

            Debug.Log($"Target généré: {bottleTag} -> {targetFill}%");
        }

        // Spawn new glass
        if (currentGlass != null)
            Destroy(currentGlass);

        yield return new WaitForSeconds(delayBeforeNextRound);

        currentGlass = Instantiate(glassPrefab, glassPrefab.transform.position, Quaternion.identity);
        Glass g = currentGlass.GetComponent<Glass>();
        OnGlassSpawned?.Invoke(g);

        foreach (var bottle in bottles)
        {
            bottle.GetComponent<Bottle>().RegisterGlass(g);
        }
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
        audioSource.clip = LooseSound;
        audioSource.Play();

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
