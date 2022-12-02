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

    public override void Start()
    {
        checkerSpawner.board = board;
        base.Start();
    }

    public override void OnServerConnect(NetworkConnectionToClient conn)
    {
        print("Conenction " + conn.connectionId + " has connected");
        if (conn.connectionId != 0)
            StartCoroutine(SpawnCheckers(conn));

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

    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        GameObject client = new GameObject();
        client.name = "ClientPlayer " + conn.connectionId;
        client.AddComponent<NetworkIdentity>();
        GameObject client_camera = new GameObject();
        client_camera.name = "ClientCamera";
        client_camera.AddComponent<Camera>();
        client_camera.transform.SetParent(client.transform);

        client_camera.SetActive(false);

        NetworkServer.AddPlayerForConnection(conn, client);
    }


}
