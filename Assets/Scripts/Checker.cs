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
        if (data == null || utils == null)
            return;

        //checker selection
        Ray mouse_ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Input.GetMouseButtonDown(0) && NetworkClient.connection != null)
        {
            if (Physics.Raycast(mouse_ray, out RaycastHit hit) && !data.selected)
            {

                if (hit.transform.gameObject == gameObject)
                { // clicked this checker
                    GetComponent<MeshRenderer>().material = flashingCheckerMaterial;
                    return;
                }
                else
                {
                    GetComponent<MeshRenderer>().material = originalCheckerMaterial;

                    foreach (var i in BoardController.singleton.checkers)
                        if (i.obj.CompareTag("PlayerChecker") && i.obj == gameObject)
                            i.obj.GetComponent<MeshRenderer>().material = originalCheckerMaterial;
                }
            }
         else if (Physics.Raycast(mouse_ray, out RaycastHit _hit))
            { // this checker has been deselected by pressing another checker

                if (!_hit.transform.gameObject.CompareTag("PlayerChecker"))
                    return;
                GetComponent<MeshRenderer>().material = originalCheckerMaterial;
            }
            else
            { //clicked something thats not a checker
                GetComponent<MeshRenderer>().material = originalCheckerMaterial;

                foreach (var i in BoardController.singleton.checkers)
                    if (i.obj.CompareTag("PlayerChecker") && i.obj == gameObject)
                        i.obj.GetComponent<MeshRenderer>().material = originalCheckerMaterial;
            }
        }
        else if (Input.GetMouseButtonDown(0))
        {
            if (Physics.Raycast(mouse_ray, out RaycastHit hit) && !data.selected)
            { 
                if (hit.transform.gameObject == gameObject)
                { // clicked this checker
                    GetComponent<MeshRenderer>().material = flashingCheckerMaterial;
                    return;
                }
                else
                {
                    GetComponent<MeshRenderer>().material = originalCheckerMaterial;

                    foreach (var i in BoardController.singleton.checkers)
                        if (i.obj.CompareTag("EnemyChecker") && i.obj == gameObject)
                            i.obj.GetComponent<MeshRenderer>().material = originalCheckerMaterial;
                }
            }
            else if (Physics.Raycast(mouse_ray, out RaycastHit _hit))
            { // this checker has been deselected by pressing another checker

                if (!_hit.transform.gameObject.CompareTag("EnemyChecker"))
                    return;
                GetComponent<MeshRenderer>().material = originalCheckerMaterial;
            }
            else
            { //clicked something thats not a checker
                GetComponent<MeshRenderer>().material = originalCheckerMaterial;

                foreach (var i in BoardController.singleton.checkers)
                    if (i.obj.CompareTag("EnemyChecker") && i.obj == gameObject)
                        i.obj.GetComponent<MeshRenderer>().material = originalCheckerMaterial;
            }
        }

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

    [ClientRpc]
    public void AddUtilsAndData(CheckerData c, CheckerUtilitys util)
    {
        utils = util;
        data = c;
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
