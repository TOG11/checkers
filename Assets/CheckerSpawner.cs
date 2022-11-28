using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckerSpawner : MonoBehaviour
{
    public static CheckerSpawner singleton;
    private Camera main_camera = null;
    public float checker_y_pos = 0.15f;
    public Material original_checker_material = null;
    public Material flashing_checker_material = null;
    private bool checker_selected;
    private checker selected_checker;
    public GameObject board;

    public static bool
    is_even(int i)
    {
        return i % 2 == 0;
    }

    public bool spawn;
    private void
Update()
    {

        if (spawn) { SpawnCheckers(); spawn = false; }
        //movement
        Ray mouse_ray = main_camera.ScreenPointToRay(Input.mousePosition);

        if (Input.GetMouseButtonDown(0))
        {
            if (Physics.Raycast(mouse_ray, out RaycastHit hit))
            {

                if (hit.transform.gameObject.tag == "PlayerChecker" || hit.transform.gameObject.tag == "EnemyChecker")
                    checker_selected = true;

                if (checker_selected)
                {
                    foreach (var c in BoardController.signleton.checkers)
                        if (c.obj.gameObject.tag == "PlayerChecker")
                            c.obj.GetComponent<MeshRenderer>().material = original_checker_material;


                    if (hit.transform.gameObject.tag == "PlayerChecker")
                    {
                        hit.transform.gameObject.GetComponent<MeshRenderer>().material = flashing_checker_material;
                        foreach (var c in BoardController.signleton.checkers)
                        {
                            if (c.obj == hit.transform.gameObject)
                            {
                                selected_checker = c;
                                break;
                            }
                        }
                    }
                    else if (selected_checker != null)
                    {
                    }
                }
            }
        }
        flashing_checker_material.SetFloat("_Metallic", (float)Mathf.Sin(Time.unscaledTime * 7.5f) * 0.35f);
    }

    private void Awake()
    {
        SpawnCheckers();
    }

    /* instantiate checkers and initialize cell_infos array  */
    private void
    SpawnCheckers()
    {
        singleton = this;
        main_camera = Camera.main;

        float cell_size = board.GetComponent<Collider>().bounds.size.x / 8.0f;

        Vector3 bottom_left_cell_pos = new Vector3(
                /* get position of the bottom-left corner of board and add cell
                 * offset to move to the center of the bottom-left cell */
                board.GetComponent<Collider>().bounds.size.x * -0.5f + cell_size * 0.5f,
                checker_y_pos,
                board.GetComponent<Collider>().bounds.size.z * -0.5f + cell_size * 0.5f);

        Vector3 bottom_left_board_coord = new Vector3(
                board.GetComponent<Collider>().bounds.size.x * -0.5f,
                checker_y_pos,
                board.GetComponent<Collider>().bounds.size.z * -0.5f);

        for (var i = 0; i < 8; i++)
        {
            for (var j = 0; j < 8; j++)
            {
                int idx = i * 8 + j;

                cell_infos[idx].pos.z = bottom_left_cell_pos.x + cell_size * i;
                cell_infos[idx].pos.y = bottom_left_cell_pos.y;
                cell_infos[idx].pos.x = bottom_left_cell_pos.z + cell_size * j;

                /* instantiate checkers */
                if (!is_even(i + j))
                {
                    cell_infos[idx].is_black = true;
                    if (i != 3 && i != 4)
                    {
                        checker c;
                        if (i < 3)
                            c = new checker(player_checker, idx, BoardController.signleton.checkers.Count, 0);
                        else
                            c = new checker(enemy_checker, idx, BoardController.signleton.checkers.Count, 1);

                        BoardController.signleton.checkers.Add(c);

                        cell_infos[idx].populated = true;
                    }
                }
            }
        }
    }
    public struct cell_info
    {
        /* center position of each cell  */
        public Vector3 pos;
        public bool is_black;
        /* there is a checker in the cell  */
        public bool populated;
    }

    public GameObject player_checker = null;
    public GameObject enemy_checker = null;

    public cell_info[] cell_infos = new cell_info[8 * 8];

    public Vector3 find_hitpoint(Vector3 hit)
    {
        Vector3 min = Vector3.zero;
        print(hit);

        float min_dist = 1000000f;

        for (int i = 0; i < 64; i++)
        {
            float dist = Vector3.Distance(hit, cell_infos[i].pos);
            if (dist < min_dist)
            {
                min = cell_infos[i].pos;
                min_dist = dist;
            }
        }

        return min;
    }

    private checker
get_checker(int x, int y)
    {
        var idx = (x == 0 || y == 0) ? x * 8 + y : x + y * 8;
        for (var i = 0; i < BoardController.signleton.checkers.Count; i++)
            if (BoardController.signleton.checkers[i].pos_index == idx)
                return BoardController.signleton.checkers[i];

        return null;
    }

}

public class checker
{
    public GameObject obj = null;
    public int list_index = -1, pos_index = -1;
    private bool is_king = false;
    public int type;


    public
    checker(GameObject prefab, int in_index, int in_list_index, int in_type)
    {
        type = in_type;
        obj = MonoBehaviour.Instantiate(prefab, CheckerSpawner.singleton.cell_infos[in_index].pos, Quaternion.identity);
        pos_index = in_index;
        list_index = in_list_index;
    }

    public bool
    check_move_validity(Vector3 hit)
    {
        Vector3 square_left = CheckerSpawner.singleton.cell_infos[pos_index + 8 - 1].pos;
        Vector3 square_right = CheckerSpawner.singleton.cell_infos[pos_index + 8 + 1].pos;

        if (hit == square_left)
        {
            obj.transform.position = square_left;
        }
        else if (hit == square_right)
        {
            obj.transform.position = square_right;
        }
        else
            return false;

        return true;
    }



    public bool
    move(int idx)
    {
        if (CheckerSpawner.singleton.cell_infos[idx].populated || !CheckerSpawner.singleton.cell_infos[idx].is_black)
        {
            foreach (var c in BoardController.signleton.checkers)
                if (c.obj.gameObject.tag == "PlayerChecker")
                    c.obj.GetComponent<MeshRenderer>().material = CheckerSpawner.singleton.original_checker_material;

            return false;
        }

        CheckerSpawner.singleton.cell_infos[idx].populated = true;

        CheckerSpawner.singleton.cell_infos[pos_index].populated = false;
        pos_index = idx;

        return true;
    }

    public bool
    move(int x, int y)
    {
        var idx = (x == 0 || y == 0) ? x * 8 + y : x + y * 8;
        return move(idx);
    }

    public bool
    move(Vector3 vec)
    {
        Vector3 vec3 = new Vector3(vec.x, CheckerSpawner.singleton.checker_y_pos, vec.z);
        for (int i = 0; i < 64; i++)
        {
            if (vec3 == CheckerSpawner.singleton.cell_infos[i].pos)
            {
                return move(i);
            }
        }
        return false;
    }

    public void
    kill()
    {
        MonoBehaviour.Destroy(obj);

        CheckerSpawner.singleton.cell_infos[pos_index].populated = false;
        BoardController.signleton.checkers.RemoveAt(list_index);

        /* update list index for other checkers  */
        for (var i = list_index; i < BoardController.signleton.checkers.Count; i++)
            BoardController.signleton.checkers[i].list_index = i;
    }

}
