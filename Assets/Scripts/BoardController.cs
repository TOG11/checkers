/* Copyright (C) 2022 Aiden Desjarlais
 * Copyright (C) 2022 Keir Yurkiw */

using UnityEngine;
using System;
using System.Collections.Generic;
using Mirror;

public class BoardController : NetworkBehaviour
{
    [SyncVar]
    public Checker SelectedChecker;

    public readonly SyncList<CheckerData> checkers = new SyncList<CheckerData>();
    public static BoardController singleton;

    private void Awake()
    {
        singleton = this;
    }

    public CheckerData GetSelectedChecker()
    {
        foreach (var c in checkers)
        {
            if (c.selected)
                return c;
        }
        return null;
    }
}
