using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using System;
using Unity.MLAgents.Sensors;

public class CharacterAgent : Agent
{
    //public event EventHandler OnCollectCoin;
    //public event EventHandler OnEpisodeBeginEvent;

    [SerializeField] private GameObject platform;
    [SerializeField] private GameObject buttonPlatform;

    [SerializeField] private Button platformButton;

    private Vector3 startPositionPlatform;
    private Vector3 startPositionButton;
    private CharacterController characterController;
    new private Rigidbody rigidbody;

    public bool pressButton;
    public bool collectCoin;
    public bool buttonUsed;

    public override void Initialize()
    {
        startPositionPlatform = transform.position;
        startPositionButton = transform.position;
        characterController = GetComponent<CharacterController>();
        rigidbody = GetComponent<Rigidbody>();
    }

    public override void OnEpisodeBegin()
    {
        if (pressButton)
        {
            transform.position = startPositionButton;
            transform.rotation = Quaternion.Euler(Vector3.up * UnityEngine.Random.Range(0f, 360f));
            rigidbody.velocity = Vector3.zero;

            buttonPlatform.transform.position = startPositionButton + Quaternion.Euler(Vector3.up * UnityEngine.Random.Range(0f, 360f)) * Vector3.forward * 5f;
        }

        if (collectCoin)
        {
            transform.position = startPositionPlatform;
            transform.rotation = Quaternion.Euler(Vector3.up * UnityEngine.Random.Range(0f, 360f));
            rigidbody.velocity = Vector3.zero;

            platform.transform.position = startPositionPlatform + Quaternion.Euler(Vector3.up * UnityEngine.Random.Range(0f, 360f)) * Vector3.forward * 5f;
        }


        if (collectCoin && pressButton)
        {
            platformButton.ResetButton();
            buttonPlatform.SetActive(true);
        }

    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        int vertical = Mathf.RoundToInt(Input.GetAxisRaw("Vertical"));
        int horizontal = Mathf.RoundToInt(Input.GetAxisRaw("Horizontal"));
        bool jump = Input.GetKey(KeyCode.Space);
        bool interact = Input.GetKey(KeyCode.LeftShift);

        ActionSegment<int> actions = actionsOut.DiscreteActions;
        actions[0] = vertical >= 0 ? vertical : 2;
        actions[1] = horizontal >= 0 ? horizontal : 2;
        actions[2] = jump ? 1 : 0;
        actions[3] = interact ? 1 : 0;
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        if (pressButton)
        {
            if (Vector3.Distance(startPositionButton, transform.position) > 10f)
            {
                AddReward(-1f);
                platformButton.ResetButton();
                EndEpisode();
            }
        }

        if (collectCoin)
        {
            if (Vector3.Distance(startPositionPlatform, transform.position) > 10f)
            {
                AddReward(-1f);
                platformButton.ResetButton();
                EndEpisode();
            }
        }


        float vertical = actions.DiscreteActions[0] <= 1 ? actions.DiscreteActions[0] : -1;
        float horizontal = actions.DiscreteActions[1] <= 1 ? actions.DiscreteActions[1] : -1;
        bool jump = actions.DiscreteActions[2] > 0;
        bool interact = actions.DiscreteActions[3] == 1;


        characterController.ForwardInput = vertical;
        characterController.TurnInput = horizontal;
        characterController.JumpInput = jump;
        characterController.ButtonInput = interact;

        if (pressButton)
        {
            if (interact)
            {
                float interactZone = .2f;
                Collider[] collider3DArray = Physics.OverlapBox(transform.position, Vector3.one * interactZone);
                foreach (Collider collider in collider3DArray)
                {
                    if (collider.TryGetComponent(out Button activatePlatform))
                    {
                        if (activatePlatform.CanUseButton())
                        {
                            activatePlatform.UseButton();
                            AddReward(1f);
                            buttonPlatform.SetActive(false);
                        }
                    }
                }

            }
        }

        AddReward(-1f / MaxStep);

    }


    private void OnTriggerEnter(Collider other)
    {

        if (other.tag == "coin")
        {
            AddReward(1f);
            if (pressButton)
            {
                platformButton.ResetButton();
                EndEpisode();
            }



        }


    }
}
