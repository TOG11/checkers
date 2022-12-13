/* Copyright (C) 2022 Aiden Desjarlais
 * Copyright (C) 2022 Keir Yurkiw */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Player : NetworkBehaviour
{
    public static Camera localCamera;
    public PlayerData data = new();
    public static Player instance;

    public void Start()
    {
        instance = this;

        if (Camera.main != null)
            Destroy(Camera.main.gameObject);
        localCamera = GetComponentInChildren<Camera>();
        if (!isServer)
        {
            localCamera.enabled = true;
            data.client = true;
            data.hasTurn = true;
        }
    }

}

public class PlayerData
{
    public bool client = false;
    public bool hasTurn = false;


}