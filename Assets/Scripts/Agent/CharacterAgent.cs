using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;

public class CharacterAgent : Agent
{
    [SerializeField] private GameObject platform;
    [SerializeField] private GameObject buttonPlatform;

    [SerializeField] private Button platformButton;

    private Vector3 startPosition;
    private CharacterController characterController;
    new private Rigidbody rigidbody;

    public override void Initialize()
    {
        startPosition = transform.position;
        characterController = GetComponent<CharacterController>();
        rigidbody = GetComponent<Rigidbody>();
    }

    public override void OnEpisodeBegin()
    {
        transform.position = startPosition;
        transform.rotation = Quaternion.Euler(Vector3.up * Random.Range(0f, 360f));
        rigidbody.velocity = Vector3.zero;

        platform.transform.position = startPosition + Quaternion.Euler(Vector3.up * Random.Range(0f, 360f)) * Vector3.forward * 5f;
        buttonPlatform.transform.position = startPosition + Quaternion.Euler(Vector3.up * Random.Range(0f, 360f)) * Vector3.forward * 5f;
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
        if (Vector3.Distance(startPosition, transform.position) > 10f)
        {
            AddReward(-1f);
            EndEpisode();
        }

        float vertical = actions.DiscreteActions[0] <= 1 ? actions.DiscreteActions[0] : -1;
        float horizontal = actions.DiscreteActions[1] <= 1 ? actions.DiscreteActions[1] : -1;
        bool jump = actions.DiscreteActions[2] > 0;
        bool interact = actions.DiscreteActions[3] == 1;


        characterController.ForwardInput = vertical;
        characterController.TurnInput = horizontal;
        characterController.JumpInput = jump;
        characterController.ButtonInput = interact;

        if (interact)
        {
            float interactZone = .5f;
            Collider[] collider3DArray = Physics.OverlapBox(transform.position, Vector3.one * interactZone);
            foreach (Collider collider in collider3DArray)
            {
                if (collider.TryGetComponent(out Button activatePlatform))
                {
                    if (activatePlatform.CanUseButton())
                    {
                        //Debug.Log("Collison with button, adding reward");
                        activatePlatform.UseButton();
                        AddReward(1f);
                        activatePlatform.ResetButton();
                        EndEpisode();
                    }
                }
            }
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        //if (other.tag == "coin")
        //{
        //    AddReward(1f);
        //    platformButton.ResetButton();
        //    EndEpisode();
        //}
    }
}
