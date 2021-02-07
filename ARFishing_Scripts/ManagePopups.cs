using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ManagePopups : MonoBehaviour
{
    public GameObject SuccessUI;
    public GameObject FailUI;
    public TextMeshProUGUI textSuccess;
    public string[] sentencesSuccess;

    // Start is called before the first frame update
    void Start()
    {
        SuccessUI.SetActive(false);
        FailUI.SetActive(false);
    }

    public void PopupStart(int whichFish, bool succeeded) {
        if (succeeded) {
            textSuccess.text = "Congratulations!\n\nYou caught a " + sentencesSuccess[whichFish] + ".";
            SuccessUI.SetActive(true);
        }
        else {
            FailUI.SetActive(true);
        }
    }

    public void PopupEnd(bool succeeded) {
        if (succeeded) {
            SuccessUI.SetActive(false);
        }
        else {
            FailUI.SetActive(false);
        }
    }
}
