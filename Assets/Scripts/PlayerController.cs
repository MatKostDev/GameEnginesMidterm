using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] PlayerRotationController rotationController = null;
    [SerializeField] CheckpointManager        checkpointManager  = null;
    
    PlayerMotor m_playerMotor;

    Transform m_transform;

    void Start()
    {                 
        m_playerMotor = GetComponent<PlayerMotor>();
        m_transform   = transform;

        checkpointManager.SetCheckpoint(m_transform.position, rotationController.GetEulerRotation(), false);
    }

    void Update()
    {
        bool forward  = Input.GetKey(KeyCode.W);
        bool backward = Input.GetKey(KeyCode.S);
        bool right    = Input.GetKey(KeyCode.D);
        bool left     = Input.GetKey(KeyCode.A);

        bool jump = Input.GetKeyDown(KeyCode.Space);

        m_playerMotor.Move(forward, backward, right, left, jump);

        //dirty check for if the player gets out of bounds
        if (m_transform.position.y < -4f || m_transform.position.y > 10f)
        {
            RespawnAtLastCheckpoint();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.CompareTag("Spikes"))
        {
            RespawnAtLastCheckpoint();
        }
        else if (other.transform.CompareTag("Checkpoint"))
        {
            checkpointManager.SetCheckpoint(m_transform.position, rotationController.GetEulerRotation());

            Destroy(other.gameObject);
        }
    }

    void RespawnAtLastCheckpoint()
    {
        m_playerMotor     .ResetVelocity();
        m_playerMotor     .TeleportToPosition(checkpointManager.GetLastCheckpointPosition());
        rotationController.SetEulerRotation(checkpointManager.GetLastCheckpointRotation());
    }
}