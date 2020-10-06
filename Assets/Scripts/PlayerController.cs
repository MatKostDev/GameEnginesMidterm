﻿using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] Camera mainCamera = null;
    
    PlayerMotor m_playerMotor;

    void Start()
    {
        m_playerMotor = GetComponent<PlayerMotor>();
    }

    void Update()
    {
        bool forward  = Input.GetKey(KeyCode.W);
        bool backward = Input.GetKey(KeyCode.S);
        bool right    = Input.GetKey(KeyCode.D);
        bool left     = Input.GetKey(KeyCode.A);

        bool jump = Input.GetKeyDown(KeyCode.Space);

        m_playerMotor.Move(forward, backward, right, left, jump);
    }
}