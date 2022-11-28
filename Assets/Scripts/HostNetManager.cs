using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class HostNetManager : NetworkManager
{

    public override void OnServerConnect(NetworkConnectionToClient conn)
    {
        print("Conenction "+conn.connectionId+" has connected");
        foreach (var c in BoardController.signleton.checkers)
        {
            NetworkServer.Spawn(c.obj, conn);
        }
        base.OnServerConnect(conn);
    }

    public override void OnServerReady(NetworkConnectionToClient conn)
    {

        base.OnServerReady(conn);
    }


    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        GameObject client = new GameObject();
        client.name = "ClientPlayer " + conn.connectionId;
        GameObject client_camera = new GameObject();
        client_camera.name = "ClientCamera";
        client_camera.AddComponent<Camera>();
        client_camera.transform.SetParent(client.transform);

        client_camera.SetActive(false);

        NetworkServer.AddPlayerForConnection(conn, client);
        base.OnServerAddPlayer(conn);
    }

}
