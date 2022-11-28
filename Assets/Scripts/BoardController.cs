/* Copyright (C) 2022 Aiden Desjarlais
 * Copyright (C) 2022 Keir Yurkiw */

using UnityEngine;
using System;
using System.Collections.Generic;
using Mirror;

public class BoardController : NetworkBehaviour
{
    public List<checker> checkers = new List<checker>();
    public static BoardController signleton;

    private void Awake()
    {
        signleton = this;
    }







}
