using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class ClientNetManager : NetworkManager
{

    public override void OnClientConnect()
    {
        NetworkClient.Ready();
        base.OnClientConnect();
    }

}
