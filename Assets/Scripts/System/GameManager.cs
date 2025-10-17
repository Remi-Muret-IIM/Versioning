using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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

    public GameObject winScreen;
    public int winsToShowWinScreen = 5;
    private int successCount = 0;
    private bool winScreenShown = false;

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

        if (winScreen != null)
            winScreen.SetActive(false);

    }

    public IEnumerator StartNewRound()
    {
        if (winScreenShown)
            yield break;

        targetQuantities.Clear();

        int numBottles = Random.Range(minBottles, maxBottles + 1);
        float remainingFill = totalTargetFill;

        List<GameObject> selectedBottles = new List<GameObject>();
        List<GameObject> availableBottles = new List<GameObject>(bottles);

        for (int i = 0; i < numBottles && availableBottles.Count > 0; i++)
        {
            int index = Random.Range(0, availableBottles.Count);
            selectedBottles.Add(availableBottles[index]);
            availableBottles.RemoveAt(index);
        }

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
        }

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
            if (audioSource != null && WinSound != null)
            {
                audioSource.clip = WinSound;
                audioSource.Play();
            }

            successCount++;
            if (successCount >= winsToShowWinScreen)
            {
                ShowWinScreen();
                return; 
            }
        }
        else
        {
            if (audioSource != null && LooseSound != null)
            {
                audioSource.clip = LooseSound;
                audioSource.Play();
            }
        }

        if (win)
        {
            var hand = GameObject.FindAnyObjectByType<HandController>(FindObjectsInactive.Exclude);
            if (hand != null)
                hand.IncreaseDrunk(drunkIncreaseOnWin);
        }

        StartCoroutine(StartNewRound());
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    private void ShowWinScreen()
    {
        winScreenShown = true;
        if (winScreen != null)
            winScreen.SetActive(true);
            Time.timeScale = 0f;
    }
    public System.Action<Glass> OnGlassSpawned;
}
