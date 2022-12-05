/* Copyright (C) 2022 Aiden Desjarlais
 * Copyright (C) 2022 Keir Yurkiw */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;

public class NetManager : NetworkManager
{
    public GameObject board;
    public BoardController boardControl;
    public CheckerSpawner checkerSpawner;
    public GameObject clientPrefab;
    public GameObject hostPrefab;

    public override void Start()
    {
        Application.targetFrameRate = 60;
        checkerSpawner.board = board;
        base.Start();
    }

    public override void OnServerConnect(NetworkConnectionToClient conn)
    {
        print("Conenction " + conn.connectionId + " has connected");
        if (conn.connectionId != 0) //all players loaded in, spawn checkers (0 = host)
        {
            StartCoroutine(SpawnCheckers(conn));
        } else
        {// host connected

        }

        base.OnServerConnect(conn);
    }


    private IEnumerator SpawnCheckers(NetworkConnectionToClient conn)
    {
        yield return new WaitForSeconds(0.200f);
        checkerSpawner.SpawnCheckers(conn);
    }


    public override void OnClientConnect()
    {
        NetworkClient.Ready();
        NetworkClient.AddPlayer();
    }

    public static Camera host;

    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        if (conn.connectionId == 0)//host connected
        {
            Destroy(Camera.main.gameObject);
            var hostc = Instantiate(hostPrefab);
            host = hostc.GetComponentInChildren<Camera>();
            host.enabled = true;
            NetworkServer.AddPlayerForConnection(conn, hostc);
        }
        else 
        { //client connected
            var clientc = Instantiate(clientPrefab);
            NetworkServer.AddPlayerForConnection(conn, clientc);
        }

    }


}
