using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button : MonoBehaviour
{
    //[SerializeField] private GameObject platformGameObject;
    [SerializeField] private Transform agentTransform;

    public event EventHandler OnUsed;

    public GameObject platform;

    //[SerializeField] private Transform buttonTransform;
    private bool canUseButton;

    private void Awake()
    {
        //buttonTransform = transform.Find("Button");
        canUseButton = true;
    }

    public bool CanUseButton()
    {
        return canUseButton;
    }

    public void UseButton()
    {
        if(canUseButton)
        {
            canUseButton= false;

            OnUsed?.Invoke(this, EventArgs.Empty);

            //platform.SetActive(true);
        }
    }

    public void ResetButton()
    {
        canUseButton = true;
        //platform.SetActive(false);
    }

}
