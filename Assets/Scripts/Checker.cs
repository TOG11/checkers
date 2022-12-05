/* Copyright (C) 2022 Aiden Desjarlais
 * Copyright (C) 2022 Keir Yurkiw */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;


// base class for all checker functions and utilitys
public class CheckerUtilitys
{
    public Checker checker;

    public bool CanMoveHere(Vector3 hit)
    {
        Vector3 square_left = CheckerSpawner.singleton.cell_infos[checker.data.pos_index + 8 - 1].pos;
        Vector3 square_right = CheckerSpawner.singleton.cell_infos[checker.data.pos_index + 8 + 1].pos;

        if (hit == square_left)
        {
            checker.data.obj.transform.position = square_left;
        }
        else if (hit == square_right)
        {
            checker.data.obj.transform.position = square_right;
        }
        else
            return false;

        return true;
    }
    private bool Move(int idx)
    {
        if (CheckerSpawner.singleton.cell_infos[idx].populated || !CheckerSpawner.singleton.cell_infos[idx].is_black)
        {
            foreach (var c in BoardController.singleton.checkers)
                if (c.obj.gameObject.CompareTag("PlayerChecker"))
                    c.obj.GetComponent<MeshRenderer>().material = checker.originalCheckerMaterial;

            return false;
        }

        CheckerSpawner.singleton.cell_infos[idx].populated = true;

        CheckerSpawner.singleton.cell_infos[checker.data.pos_index].populated = false;
        checker.data.pos_index = idx;

        return true;
    }

    public bool MoveTo(Vector3 vec)
    {
        Vector3 vec3 = new Vector3(vec.x, CheckerSpawner.singleton.checker_y_pos, vec.z);
        for (int i = 0; i < 64; i++)
        {
            if (vec3 == CheckerSpawner.singleton.cell_infos[i].pos)
            {
                return Move(i);
            }
        }
        return false;
    }
}

//base checker data class
public class CheckerData
{
    public bool selected;
    public int list_index = -1, pos_index = -1;
    private bool is_king = false;
    public int type;
    public GameObject obj;
    public CheckerUtilitys utils;

    
    public CheckerData CreateChecker(GameObject prefab, int in_index, int in_list_index, int in_type, NetworkConnectionToClient conn)
    {
        type = in_type;
        obj = MonoBehaviour.Instantiate(prefab, CheckerSpawner.singleton.cell_infos[in_index].pos, Quaternion.identity);
        utils = new CheckerUtilitys { checker = obj.GetComponent<Checker>() };
        obj.GetComponent<Checker>().utils = utils;
        obj.GetComponent<Checker>().data = this;
        if (type == 1)
        {
            NetworkServer.Spawn(obj, conn);
        }
        else if (type == 0)
            NetworkServer.Spawn(obj);

        pos_index = in_index;
        list_index = in_list_index;
        return this;
    }
}

//base checker control class
public class Checker : NetworkBehaviour
{

    public Material flashingCheckerMaterial;
    public Material originalCheckerMaterial;

    [SyncVar]
    public CheckerUtilitys utils;
    [SyncVar]
    public CheckerData data;

    public bool isEnemy;

    private void Update()
    {
        if (data == null || utils == null || Player.localCamera == null)
            return;

        Ray mouse_ray;

        //checker selection
        if (NetManager.host != null)
            mouse_ray = NetManager.host.ScreenPointToRay(Input.mousePosition);
        else
            mouse_ray = Player.localCamera.ScreenPointToRay(Input.mousePosition);

        if (Input.GetMouseButtonDown(0) && isServer)//HOST SELECTION
        {
            if (Physics.Raycast(mouse_ray, out RaycastHit hit) && !data.selected)
            {
                if (hit.transform.gameObject == gameObject && !hit.transform.gameObject.CompareTag("EnemyChecker"))
                { // clicked this checker
                    
                    HostSelected(true);
                    GetComponent<MeshRenderer>().material = flashingCheckerMaterial;
                    return;
                }
                else
                {
                    HostSelected(false);
                    GetComponent<MeshRenderer>().material = originalCheckerMaterial;

                    foreach (var i in BoardController.singleton.checkers)
                        if (i.obj.CompareTag("PlayerChecker") && i.obj == gameObject)
                        {
                            i.obj.GetComponent<MeshRenderer>().material = originalCheckerMaterial;
                            i.obj.GetComponent<Checker>().HostSelected(false);
                        }
                }
            }
         else if (Physics.Raycast(mouse_ray, out RaycastHit _hit))
            { // this checker has been deselected by pressing another checker

                if (!_hit.transform.gameObject.CompareTag("PlayerChecker"))
                    return;
                HostSelected(false);
                GetComponent<MeshRenderer>().material = originalCheckerMaterial;
            }
            else
            { //clicked something thats not a checker
                HostSelected(false);
                GetComponent<MeshRenderer>().material = originalCheckerMaterial;

                foreach (var i in BoardController.singleton.checkers)
                    if (i.obj.CompareTag("PlayerChecker") && i.obj == gameObject)
                    {
                        i.obj.GetComponent<MeshRenderer>().material = originalCheckerMaterial;
                        i.obj.GetComponent<Checker>().HostSelected(false);
                    }
            }
        }
        else if (Input.GetMouseButtonDown(0) && !isServer) //CLIENT SELECTION
        {
            print("client");
            if (Physics.Raycast(mouse_ray, out RaycastHit hit) && !data.selected)
            { 
                if (hit.transform.gameObject == gameObject && hit.transform.gameObject.CompareTag("EnemyChecker"))
                { // clicked this checker
                    ClientSelected(true);
                    GetComponent<MeshRenderer>().material = flashingCheckerMaterial;
                    return;
                }
                else
                {
                    ClientSelected(false);
                    GetComponent<MeshRenderer>().material = originalCheckerMaterial;

                    foreach (var i in BoardController.singleton.checkers)
                        if (i.obj.CompareTag("EnemyChecker") && i.obj == gameObject)
                        {
                            i.obj.GetComponent<MeshRenderer>().material = originalCheckerMaterial;
                            i.obj.GetComponent<Checker>().ClientSelected(false);
                        }
                }
            }
            else if (Physics.Raycast(mouse_ray, out RaycastHit _hit))
            { // this checker has been deselected by pressing another checker

                if (!_hit.transform.gameObject.CompareTag("EnemyChecker"))
                    return;
                ClientSelected(false);
                GetComponent<MeshRenderer>().material = originalCheckerMaterial;
            }
            else
            { //clicked something thats not a checker
                ClientSelected(false);
                GetComponent<MeshRenderer>().material = originalCheckerMaterial;

                foreach (var i in BoardController.singleton.checkers)
                    if (i.obj.CompareTag("EnemyChecker") && i.obj == gameObject)
                    {
                        i.obj.GetComponent<MeshRenderer>().material = originalCheckerMaterial;
                        i.obj.GetComponent<Checker>().ClientSelected(false);
                    }
            }
        }

    }


    [Command(requiresAuthority = false)]
    public void ClientSelected(bool select)
    {
        if (select)
        {
            data.selected = true;
            gameObject.GetComponent<MeshRenderer>().material = flashingCheckerMaterial;
        }
        else
        {
            data.selected = false;
            gameObject.GetComponent<MeshRenderer>().material = originalCheckerMaterial;
        }
    }

    internal void HostSelected(bool select)
    {
        if (select)
        {
            data.selected = true;
            gameObject.GetComponent<MeshRenderer>().material = flashingCheckerMaterial;
            HostSelect(true);
        }
        else
        {
            data.selected = false;
            gameObject.GetComponent<MeshRenderer>().material = originalCheckerMaterial;
            HostSelect(false);
        }
    }

    [ClientRpc]
    public void HostSelect(bool select)
    {
        if (select)
            gameObject.GetComponent<MeshRenderer>().material = flashingCheckerMaterial;
        else
            gameObject.GetComponent<MeshRenderer>().material = originalCheckerMaterial;
    }


    [Command(requiresAuthority = false)]
    public void SpawnWithAuthority(GameObject c, NetworkConnectionToClient sender = null)
    {
        NetworkServer.Spawn(c, sender);
    }

    [Command(requiresAuthority = false)]
    public void Spawn(GameObject c)
    {
        NetworkServer.Spawn(c);
    }

    [Command(requiresAuthority = false)]
    public void
    Kill()
    {
        MonoBehaviour.Destroy(data.obj);

        CheckerSpawner.singleton.cell_infos[data.pos_index].populated = false;
        BoardController.singleton.checkers.RemoveAt(data.list_index);

        /* update list index for other checkers  */
        for (var i = data.list_index; i < BoardController.singleton.checkers.Count; i++)
            BoardController.singleton.checkers[i].list_index = i;
    }
}
