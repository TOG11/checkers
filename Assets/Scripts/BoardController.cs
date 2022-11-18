/* Copyright (C) 2022 Aiden Desjarlais
 * Copyright (C) 2022 Keir Yurkiw */

using UnityEngine;
using System.Collections.Generic;

public class checker
{
    private GameObject obj = null;
    public int pos_index = -1, list_index = -1;

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
        return move(x + y*8);
    }

    public void
    kill()
    {
        MonoBehaviour.Destroy(obj);

        BoardController.cell_infos[pos_index].populated = false;
        BoardController.checkers.RemoveAt(list_index);

        /* update list index for other checkers */
        for (var i = list_index; i < BoardController.checkers.Count; i++)
            BoardController.checkers[i].list_index = i;
    }
}

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

    private const int num_cells = 8;

    public static cell_info[] cell_infos = new cell_info[num_cells*num_cells];
    public static List<checker> checkers = new List<checker>();
    public GameObject player_checker = null;
    public GameObject enemy_checker = null;
    public float checker_y_pos = 0.15f;

    public static bool
    is_even(int i)
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

        for (var i = 0; i < num_cells; i++)
        {
            for (var j = 0; j < num_cells; j++)
            {
                int idx = i*num_cells + j;

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
                    checker c = new checker(i < 3 ? player_checker : enemy_checker, idx, checkers.Count);
                    checkers.Add(c);

                    cell_infos[idx].populated = true;
                }
            }
        }
    }

    private void
    Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            checkers[8].move(2, 3);
        else if (Input.GetKeyUp(KeyCode.Space))
            checkers[8].move(0, 0);

        if (Input.GetKeyDown(KeyCode.Backspace))
            checkers[0].kill();
    }
}
