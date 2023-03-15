using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainingArea : MonoBehaviour, IButton
{
    public CharacterAgent characterAgent;

    public GameObject button;
    public GameObject platform;

    private bool isPressed = false;
    public void SetPlatformActive()
    {
        platform.SetActive(true);
    }

    public void SetPlatformInactive()
    {
        platform.SetActive(false);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TogglePlatform()
    {
        isPressed = !isPressed;
        if(isPressed)
        {
            SetPlatformActive();
        }
        else
        {
            SetPlatformInactive();
        }
    }
}
