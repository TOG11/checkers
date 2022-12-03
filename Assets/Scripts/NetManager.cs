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

    private IEnumerator SetCameras(Camera h, Camera c)
    {
        yield return new WaitForSeconds(0.200f);
        checkerSpawner.SetCameras(h, c);
    }

    public override void OnClientConnect()
    {
        NetworkClient.Ready();
        NetworkClient.AddPlayer();
    }

    public Camera host_cam;
    public Camera client_cam;

    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        if (conn.connectionId == 0)//host connected
        {
            GameObject host = new GameObject();
            host.name = "HostPlayer " + conn.connectionId;
            host.AddComponent<NetworkIdentity>();
            GameObject host_camera = new GameObject();
            host_camera.name = "HostCamera";
            host_camera.AddComponent<Camera>();
            host_camera.transform.SetParent(host.transform);
            host_camera.transform.position = new Vector3(0, 7.71f, -8.44f);
            host_cam = host_camera.GetComponent<Camera>();
            NetworkServer.AddPlayerForConnection(conn, host);
        }
        else 
        { //client connected

            Camera.main.enabled = false;

            GameObject client = new GameObject();
            client.name = "ClientPlayer " + conn.connectionId;
            client.AddComponent<NetworkIdentity>();
            GameObject client_camera = new GameObject();
            client_camera.name = "ClientCamera";
            client_camera.AddComponent<Camera>();
            client_camera.transform.SetParent(client.transform);
            client_camera.transform.eulerAngles = new Vector3(45.056f, 180, 0);
            client_camera.transform.position = new Vector3(0, 7.71f, 8.44f);
            client_cam = client_camera.GetComponent<Camera>();
            NetworkServer.AddPlayerForConnection(conn, client);
            StartCoroutine(SetCameras(host_cam, client_cam));
        }
    }


}
