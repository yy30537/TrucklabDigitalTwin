using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceMenu : MonoBehaviour
{

    public GameObject spacesView;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Toggle()
    {
        spacesView.SetActive(!spacesView.activeSelf);
    }
}
