using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vuforia;

public class StartFishing : MonoBehaviour, IVirtualButtonEventHandler
{

    public GameObject vButtonObj;
    public GameObject fishingPole;
    public Animator fishingPoleAnim;
    public bool startedFishing;
    public SoundManager sfx;
    // public GameObject startLog;

    // Start is called before the first frame update
    void Start()
    {
        vButtonObj = GameObject.Find("StartButton");
        vButtonObj.GetComponent<VirtualButtonBehaviour>().RegisterEventHandler(this);
        fishingPole.SetActive(false);
        startedFishing = false;

        // UI for debugging purposes
        // startLog.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnButtonPressed(VirtualButtonBehaviour vButt) {
        startedFishing = true;
        fishingPole.SetActive(startedFishing);
        fishingPoleAnim.Play("CastRod");
        sfx.PlayPlop();

        // UI for debugging purposes
        // StartCoroutine(DisplayLog());
    }

    // IEnumerator DisplayLog() {
    //     startLog.SetActive(true);
    //     yield return new WaitForSeconds(4.0f);
    //     startLog.SetActive(false);
    // }

    public void OnButtonReleased(VirtualButtonBehaviour vButt) {
        // do nothing
    }

    public void SetFishing(bool isFishing) {
        startedFishing = isFishing;
    }
}
