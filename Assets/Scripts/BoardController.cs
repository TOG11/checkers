/* Copyright (C) 2022 Aiden Desjarlais
 * Copyright (C) 2022 Keir Yurkiw */

using UnityEngine;
using System;
using System.Collections.Generic;

public class checker
{
    public GameObject obj = null;
    public int list_index = -1, pos_index = -1;

    public
    checker(GameObject prefab, int in_index, int in_list_index)
    {
        obj = MonoBehaviour.Instantiate(prefab, BoardController.cell_infos[in_index].pos, Quaternion.identity);
        pos_index = in_index;
        list_index = in_list_index;
    }

    public bool
    move(int idx)
    {
        if (BoardController.cell_infos[idx].populated)
            return false;

        obj.transform.position = BoardController.cell_infos[idx].pos;
        BoardController.cell_infos[idx].populated = true;

        BoardController.cell_infos[pos_index].populated = false;
        pos_index = idx;

        return true;
    }

    public bool
    move(int x, int y)
    {
        var idx = (x == 0 || y == 0) ? x*8 + y : x + y*8;
        return move(idx);
    }

    public void
    kill()
    {
        MonoBehaviour.Destroy(obj);

        BoardController.cell_infos[pos_index].populated = false;
        BoardController.checkers.RemoveAt(list_index);

        /* update list index for other checkers  */
        for (var i = list_index; i < BoardController.checkers.Count; i++)
            BoardController.checkers[i].list_index = i;
    }
}

public class BoardController : MonoBehaviour
{
    public struct cell_info    {
        /* center position of each cell  */
        public Vector3 pos;
        /* corner positions of each cell  */
        public Vector2 bl, br, tl, tr;

        /* there is a checker in the cell  */
        public bool populated;
    }

    public static cell_info[] cell_infos = new cell_info[8*8];
    public static List<checker> checkers = new List<checker>();
    public GameObject player_checker = null;
    public GameObject enemy_checker = null;
    public float checker_y_pos = 0.15f;

    public Material original_checker_material = null;
    public Material flashing_checker_material = null;

    private Camera main_camera = null;
    private bool checker_select = false;
    private bool checker_selected = false;
    private GameObject selected_checker = null;

    public static bool
    is_even(int i)
    {
        return i % 2 == 0;
    }

    /* instantiate checkers and initialize cell_infos array  */
    private void
    Awake()
    {
        main_camera = Camera.main;

        float cell_size = GetComponent<Collider>().bounds.size.x / 8.0f;

        Vector3 bottom_left_cell_pos = new Vector3(
                /* get position of the bottom-left corner of board and add cell
                 * offset to move to the center of the bottom-left cell */
                GetComponent<Collider>().bounds.size.x * -0.5f + cell_size * 0.5f,
                checker_y_pos,
                GetComponent<Collider>().bounds.size.z * -0.5f + cell_size * 0.5f);

        Vector3 bottom_left_board_coord = new Vector3(
                GetComponent<Collider>().bounds.size.x * -0.5f,
                checker_y_pos,
                GetComponent<Collider>().bounds.size.z * -0.5f);

        for (var i = 0; i < 8; i++)
        {
            for (var j = 0; j < 8; j++)
            {
                int idx = i*8 + j;

                cell_infos[idx].pos.z = bottom_left_cell_pos.x + cell_size*i;
                cell_infos[idx].pos.y = bottom_left_cell_pos.y;
                cell_infos[idx].pos.x = bottom_left_cell_pos.z + cell_size*j;

                float x_off = cell_size*j, y_off = cell_size*i;
                cell_infos[idx].bl.x = bottom_left_board_coord.x + x_off;
                cell_infos[idx].bl.y = bottom_left_board_coord.z + y_off;
                cell_infos[idx].br.x = bottom_left_board_coord.x + cell_size + x_off;
                cell_infos[idx].br.y = bottom_left_board_coord.z + y_off;
                cell_infos[idx].tl.x = bottom_left_board_coord.x + x_off;
                cell_infos[idx].tl.y = bottom_left_board_coord.z + cell_size + y_off;
                cell_infos[idx].tr.x = bottom_left_board_coord.x + cell_size + x_off;
                cell_infos[idx].tr.y = bottom_left_board_coord.z + cell_size + y_off;

                /* instantiate checkers */
                if (!is_even(i + j) && (i != 3 && i != 4))
                {
                    checker c = new checker(i < 3 ? player_checker : enemy_checker, idx, checkers.Count);
                    checkers.Add(c);

                    cell_infos[idx].populated = true;
                }
            }
        }
    }

    private checker
    get_checker(int x, int y)
    {
        var idx = (x == 0 || y == 0) ? x*8 + y : x + y*8;
        for (var i = 0; i < checkers.Count; i++)
            if (checkers[i].pos_index == idx)
                return checkers[i];

        return null;
    }

    private void
    Update()
    {
        Ray mouse_ray = main_camera.ScreenPointToRay(Input.mousePosition);

        if (Input.GetMouseButtonDown(0))
        {
            if (Physics.Raycast(mouse_ray, out RaycastHit hit))
            {
                if (!checker_selected)
                {
                    foreach (var c in checkers)
                        if (c.obj.gameObject.tag == "PlayerChecker")
                            c.obj.GetComponent<MeshRenderer>().material = original_checker_material;

                    if (hit.transform.gameObject.tag == "PlayerChecker")
                    {
                        checker_select = !checker_select;
                        checker_select = true;
                        if (checker_select)
                        {
                            selected_checker = hit.transform.gameObject;
                            checker_selected = true;
                            hit.transform.gameObject.GetComponent<MeshRenderer>().material = flashing_checker_material;
                        }
                        else
                        {
                            selected_checker = null;
                            checker_selected = false;
                            hit.transform.gameObject.GetComponent<MeshRenderer>().material = original_checker_material;
                        }
                    }

                    if (checker_select && hit.transform.gameObject.tag != "PlayerChecker")
                        foreach (var c in checkers)
                            if (c.obj.gameObject.tag == "PlayerChecker")
                                c.obj.GetComponent<MeshRenderer>().material = original_checker_material;
                }
                else if (checker_selected)
                {
                    selected_checker.transform.position = hit.transform.position;
                }
            }
        }

        flashing_checker_material.SetFloat("_Metallic", (float)Math.Cos(Time.unscaledTime * 7.5f) * 0.35f);
    }
}
