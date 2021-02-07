using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateManager : MonoBehaviour
{

    public GameObject[] fishToAdd;

    // Start is called before the first frame update
    void Start()
    {
        ClearFish();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Add fish to crate
    public void AddFish(int fishIndex) {
        fishToAdd[fishIndex].SetActive(true);
    }

    public void ClearFish() {
        for (int i=0; i < fishToAdd.Length; i++) {
            fishToAdd[i].SetActive(false);
        }
    }
}
