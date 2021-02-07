using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FishManager : MonoBehaviour
{
    public GameObject fishingPole;
    public Animator fishingPoleAnim;
    public GameObject[] fish; // size=4
    // GameObject[fishLevel-1]

    // public GameObject SuccessfulCatchUI;
    // public GameObject FailedCatchUI;
    // public GameObject ExclamationUI;

    public GameObject ExclamationQuad;
    public Animator ExclAnim;

    float bobTimer;
    int bobTimes; // between 3 to 5
    int fishLevel;
    public float timeWindow;
    bool fishIsReady;

    GameObject startButton;

    public int fishCaught;
    public int fishOverall;
    public Image[] fishImages;
    public Sprite fullSardine;
    public Sprite fullWhale;
    public Sprite emptySardine;
    public Sprite emptyWhale;

    public GameObject[] fish3D;
    public Animator[] fishAnimators;
    public Animation whaleAnim;

    public ManagePopups popupManager;
    public SoundManager soundManager;
    public EffectsManager effectsManager;
    public CreateManager crateManager;

    // Start is called before the first frame update
    void Start()
    {
        fishLevel = 1;
        bobTimer = 0f;
        bobTimes = 0;
        timeWindow = 2.0f;
        fishIsReady = false;
        // SuccessfulCatchUI.SetActive(false);
        // FailedCatchUI.SetActive(false);
        ExclamationQuad.SetActive(false);
        startButton = GameObject.Find("StartButton");
        fishCaught = 0;

        for (int i=0; i < fish3D.Length; i++) {
            fish3D[i].SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {

        StartFishing startButtonScript = startButton.GetComponent<StartFishing>();
        bool isFishing = startButtonScript.startedFishing;

        if (isFishing) {
            bobTimer += Time.deltaTime; // +1 second every IRL second

            // Animate bobbing fishing pole
            if (bobTimer < 5.0f && bobTimer >= 3.0f) {
                // 50% chance for bobbing event before 5 seconds have passed
                if (Random.value > 0.5f) {
                    soundManager.PlaySplash();
                    effectsManager.Ripple();
                    StartCoroutine(BobPole());
                    bobTimes+=1;
                    bobTimer=0f;
                }
            }

            // It's been long enough - bob event now must be triggered
            if (bobTimer >= 5.0f) {
                soundManager.PlaySplash();
                effectsManager.Ripple();
                StartCoroutine(BobPole());
                bobTimes+=1;
                bobTimer=0f;
            }

            if (bobTimes < 5 && bobTimes >= 3) {
                // 50% chance for fish to be ready before 5 times
                if (Random.value > 0.5f) {
                    Debug.Log("Bobbed " + bobTimes + " times for fish level = " + fishLevel);
                    effectsManager.Ripple();
                    ReadyFish();
                    bobTimes=0;
                    // StartCoroutine(WindowTimeout());
                }
            }

            // It's been long enough - fishIsReady now must turn true
            if (bobTimes >= 5) {
                Debug.Log("Bobbed " + bobTimes + " times for fish level = " + fishLevel);
                effectsManager.Ripple();
                ReadyFish();
                bobTimes=0;
                // StartCoroutine(WindowTimeout());
            }
        }

        // Fish UI
        if (fishCaught > fishOverall) {
            fishCaught = fishOverall;
        }

        for (int i = 0; i < fish.Length; i++) {
            if (i < fishCaught) {
                if (i == 3) {
                    fishImages[i].sprite=fullWhale;
                }
                else {
                    fishImages[i].sprite=fullSardine;
                }
            }
            else {
                if (i == 3) {
                    fishImages[i].sprite=emptyWhale;
                }
                else {
                    fishImages[i].sprite=emptySardine;
                }
            }

            // Ensuring that we don't try to display more fish than we can catch.
            if (i < fishOverall) {
                fishImages[i].enabled = true; // visible
            }
            else {
                fishImages[i].enabled = false;
            }
        }

    }

    // Buggy?
    IEnumerator WindowTimeout() {
        Debug.Log("Window timed out! No fish for you.");
        yield return new WaitForSeconds(timeWindow);
        fishIsReady = false;
    }

    IEnumerator BobPole() {
        // Debug.Log("Bobbing pole");
        fishingPoleAnim.SetBool("PoleBobbing", true);
        yield return new WaitForSeconds(Random.Range(2.5f, 4.5f)); // how long to bob?
        fishingPoleAnim.SetBool("PoleBobbing", false);
    }

    public void ReadyFish() {
        // Debug.Log("Readying fish");
        fishIsReady = true;
        fishingPoleAnim.SetBool("FishBiting", true);
        ExclamationQuad.SetActive(true);
        ExclAnim.Play("EnlargeQuad");
    }

    // USAGE: Called when user presses ReelButton.
    // FUNCTIONALITY: If user presses ReelButton when fishIsReady, then it's a successful catch.
    // Unsucessful otherwise --> causing fishLevel to drop back down to 1.
    public bool CheckIfCaught() {
        Debug.Log("Checking if fish was caught (ReelFish button pressed...)");
        fishingPoleAnim.Play("ReelIn");
        
        // Whether or not the fish was caught successfully, the fishing pole has to stop moving.
        fishingPoleAnim.SetBool("FishBiting", false);
        
        // NEED REELBUTTON TO CHECK TIMEWINDOW DURING BUTTONPRESS
        if (fishIsReady) {
            Debug.Log("Fish was caught");
            fishCaught += 1;
            soundManager.PlaySuccess(); // SFX
            effectsManager.Confetti();
            StartCoroutine(FishCaughtSuccessfully());
            
            fishIsReady = false;
            return true;
        }
        else {
            Debug.Log("Fish wasn't caught");
            fishCaught = 0;
            StartCoroutine(FishNotCaught());
            return false;
        }
    }

    IEnumerator FishCaughtSuccessfully() {
        ExclamationQuad.SetActive(false);
        // SuccessfulCatchUI.SetActive(true);

        int fishIndex = fishLevel-1;
        popupManager.PopupStart(fishIndex, true);
        crateManager.AddFish(fishIndex);

        fish3D[fishIndex].SetActive(true);
        if (fishIndex < 3) {
            fishAnimators[fishIndex].Play("Swim");
        }
        else {
            whaleAnim.Play();
        }

        UpdateFishLevel();
        yield return new WaitForSeconds(4);
        // SuccessfulCatchUI.SetActive(false);
        
        popupManager.PopupEnd(true);
        fish3D[fishIndex].SetActive(false);
        if (fishIndex<3) {
            fishAnimators[fishIndex].enabled = false;
        }
        else {
            whaleAnim.enabled = false;
        }
    }

    IEnumerator FishNotCaught() {
        ExclamationQuad.SetActive(false);
        popupManager.PopupStart(0, false); // whichFish doesn't matter -- fail UI is same
        ResetFishLevel();
        yield return new WaitForSeconds(4);
        popupManager.PopupEnd(false);
    }

    // FUNCTIONALITY: Updates fish level, and with it, the corresponding timing window for catching them.
    // Time window, in other words, inversely correlates with fish level (difficulty mechanic).
    // USAGE: Called when user successfully catches a fish.
    public void UpdateFishLevel() {
        
        fishLevel += 1;
        Debug.Log("Updated fish level to " + fishLevel);
        
        // Small sardine
        if (fishLevel == 1) {
            timeWindow = 2.0f;
        }
        // Medium sardine
        else if (fishLevel == 2) {
            timeWindow = 1.5f;
        }
        // Large sardine
        else if (fishLevel == 3) {
            timeWindow = 1.0f;
        }
        // Humpback whale, AKA huge sardine
        else if (fishLevel == 4) {
            timeWindow = 0.5f;
        }
        // You've caught all the fish there is to catch!
        else {
            ResetFishLevel();
        }
    }

    // FUNCTIONALITY: Drops the fish level back down to 1. Small fish only can be caught.
    // USAGE: Called by CheckIfCaught() when user unsuccessfully catches a fish;
    // either the timing window timed out or the user tried to catch a fish when
    // it wasn't 'ready' yet.
    public void ResetFishLevel() {
        Debug.Log("Resetting fish level");
        crateManager.ClearFish();
        fishLevel = 1;
    }
}
