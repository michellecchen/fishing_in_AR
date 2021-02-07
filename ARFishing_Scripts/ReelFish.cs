using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vuforia;

public class ReelFish : MonoBehaviour, IVirtualButtonEventHandler
{

    public GameObject vButtonObj;
    public GameObject fishingPole;
    public int numFishCaught;
    public FishManager fishManager;

    public StartFishing startButton;

    public EffectsManager effectsManager;

    // public GameObject reelLog;

    // Start is called before the first frame update
    void Start()
    {
        vButtonObj = GameObject.Find("ReelButton");
        vButtonObj.GetComponent<VirtualButtonBehaviour>().RegisterEventHandler(this);
        numFishCaught = 0;
        // reelLog.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnButtonPressed(VirtualButtonBehaviour vButt) {
        effectsManager.Splash();
        bool fishCaught = fishManager.CheckIfCaught();
        if (fishCaught) {
            numFishCaught += 1;
        }
        else {
            numFishCaught = 0; // reset
        }
        fishingPole.SetActive(false);
        startButton.SetFishing(false);
        
        // StartCoroutine(DisplayLog());
    }

    // IEnumerator DisplayLog() {
    //     reelLog.SetActive(true);
    //     yield return new WaitForSeconds(4.0f);
    //     reelLog.SetActive(false);
    // }

    public void OnButtonReleased(VirtualButtonBehaviour vButt) {
        // do nothing
    }
}
