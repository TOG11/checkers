/* Copyright (C) 2022 Aiden Desjarlais
 * Copyright (C) 2022 Keir Yurkiw */

using UnityEngine;
using System;
using System.Collections.Generic;
using Mirror;

public class BoardController : NetworkBehaviour
{
    [SyncVar]
    public GameObject SelectedChecker = null;

    public readonly SyncList<CheckerData> checkers = new SyncList<CheckerData>();
    public static BoardController singleton;

    private void Awake()
    {
        singleton = this;
    }







}
