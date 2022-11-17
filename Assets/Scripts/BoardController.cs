/* Copyright (C) 2022 Aiden Desjarlais
 * Copyright (C) 2022 Keir Yurkiw */

using UnityEngine;
using System.Collections.Generic;

public class BoardController : MonoBehaviour
{
    public struct cell_info
    {
        /* center position of each cell */
        public Vector3 pos;
        /* corner positions of each cell */
        public Vector2 bl, br, tl, tr;

        /* there is a checker in the cell */
        public bool populated;
    }

    private const uint num_cells = 8;

    private cell_info[] cell_infos = new cell_info[num_cells*num_cells];
    private List<GameObject> checkers = new List<GameObject>();

    public GameObject player_checker = null;
    public GameObject enemy_checker = null;
    public float checker_y_pos = 0.15f;

    private bool
    is_even(uint i)
    {
        return i % 2 == 0;
    }

    /* instantiate checkers and initialize cell_infos array */
    private void
    Awake()
    {
        float cell_size = GetComponent<Renderer>().bounds.size.x / (float)num_cells;

        Vector3 bottom_left_cell_pos = new Vector3(
                /* get position of the bottom-left corner of board and add cell
                 * offset to move to the center of the bottom-left cell */
                GetComponent<Renderer>().bounds.size.x * -0.5f + cell_size * 0.5f,
                checker_y_pos,
                GetComponent<Renderer>().bounds.size.z * -0.5f + cell_size * 0.5f);

        Vector3 bottom_left_board_coord = new Vector3(
                GetComponent<Renderer>().bounds.size.x * -0.5f,
                checker_y_pos,
                GetComponent<Renderer>().bounds.size.z * -0.5f);

        for (uint i = 0; i < num_cells; i++)
        {
            for (uint j = 0; j < num_cells; j++)
            {
                uint idx = i*num_cells + j;

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

                if (!is_even(i + j) && (i != 3 && i != 4))
                {
                    GameObject c = Instantiate(i < 3 ? player_checker : enemy_checker, cell_infos[idx].pos, Quaternion.identity);
                    checkers.Add(c);

                    cell_infos[idx].populated = true;
                }
            }
        }
    }

    private GameObject
    find_checker(int idx)
    {
        for (var i = 0; i < checkers.Count; i++)
            if (checkers[i].transform.position == cell_infos[idx].pos)
                return checkers[i];

        return null;
    }

    private bool
    move_checker(GameObject c, int idx)
    {
        if (!c)
            return false;

        c.transform.position = cell_infos[idx].pos;
        return true;
    }

    private void
    select_checker()
    {
        RaycastHit hit = new RaycastHit();
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit) && Input.GetMouseButton(0))
            hit.transform.gameObject.transform.position = new Vector3(hit.point.x, checker_y_pos, hit.point.z + 0.3f);
    }

    private void
    Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            move_checker(find_checker(1), 0);
        if (Input.GetKeyUp(KeyCode.Space))
            move_checker(find_checker(0), 1);

        select_checker();
    }
}
