/* Copyright (C) 2022 Aiden Desjarlais
 * Copyright (C) 2022 Keir Yurkiw */

using UnityEngine;
using System;
using System.Collections.Generic;
using Mirror;

public class BoardController : NetworkBehaviour
{
    public readonly SyncList<CheckerData> checkers = new SyncList<CheckerData>();
    public static BoardController singleton;

    [SyncVar]
    public char WhosTurnIsIt = 'C';


    private void Awake()
    {
        singleton = this;
    }


    public char WhosTurn()
    {
        return WhosTurnIsIt;
    }

    public void SetTurn(char c)
    {
        SetTurnFromServer(c);
    }

    [Command]
    public void SetTurnFromServer(char c)
    {
        if (c == 'C')
        {
            WhosTurnIsIt = 'C';
        }
        else if (c == 'H')
        {
            WhosTurnIsIt = 'H';
        }
    }


    public bool HasTurn()
    {
        if (Player.instance.data.hasTurn)
            return true;
        else
            return false;
    }

}
