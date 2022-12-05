/* Copyright (C) 2022 Aiden Desjarlais
 * Copyright (C) 2022 Keir Yurkiw */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Player : NetworkBehaviour
{
    public static Camera localCamera;

    public void Start()
    {
        if (Camera.main != null)
            Destroy(Camera.main.gameObject);
        localCamera = GetComponentInChildren<Camera>();
        print(isServer);
        if (!isServer)
            localCamera.enabled = true;
    }

}
