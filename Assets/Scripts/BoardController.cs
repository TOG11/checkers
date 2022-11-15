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
    };

    private const uint num_cells = 8;
    private float cell_size;

    public GameObject board = null;
    public GameObject checker_prefab = null;
    public float checker_y_pos = 0.233f;
    public cell_info[] cell_infos = new cell_info[num_cells*num_cells];

    public cell_info[]
    get_board_info()
    {
        return cell_infos;
    }

    private bool
    is_even(uint i)
    {
        return i % 2 == 0;
    }

    private void
    Awake()
    {
        cell_size = board.GetComponent<Renderer>().bounds.size.x / (float)num_cells;

        Vector3 bottom_left_cell_pos = new Vector3(
                /* get position of the bottom-left corner of board and add cell
                 * offset to move to the center of the bottom-left cell */
                board.GetComponent<Renderer>().bounds.size.x * -0.5f + cell_size * 0.5f,
                checker_y_pos,
                board.GetComponent<Renderer>().bounds.size.z * -0.5f + cell_size * 0.5f);

        Vector3 bottom_left_board_coord = new Vector3(
                board.GetComponent<Renderer>().bounds.size.x * -0.5f,
                checker_y_pos,
                board.GetComponent<Renderer>().bounds.size.z * -0.5f);

        for (uint i = 0; i < num_cells; i++)
        {
            for (uint j = 0; j < num_cells; j++)
            {
                uint idx = i*num_cells + j;

                cell_infos[idx].pos.x = bottom_left_cell_pos.x + cell_size*i;
                cell_infos[idx].pos.y = bottom_left_cell_pos.y;
                cell_infos[idx].pos.z = bottom_left_cell_pos.z + cell_size*j;

                float x_off = cell_size*j, y_off = cell_size*i;
                cell_infos[idx].bl.x = bottom_left_board_coord.x + x_off;
                cell_infos[idx].bl.y = bottom_left_board_coord.z + y_off;
                cell_infos[idx].br.x = bottom_left_board_coord.x + cell_size + x_off;
                cell_infos[idx].br.y = bottom_left_board_coord.z + y_off;
                cell_infos[idx].tl.x = bottom_left_board_coord.x + x_off;
                cell_infos[idx].tl.y = bottom_left_board_coord.z + cell_size + y_off;
                cell_infos[idx].tr.x = bottom_left_board_coord.x + cell_size + x_off;
                cell_infos[idx].tr.y = bottom_left_board_coord.z + cell_size + y_off;

                if (!is_even(i + j) && (j != 3 && j != 4))
                {
                    GameObject new_checker = Instantiate(checker_prefab);
                    new_checker.transform.position = cell_infos[idx].pos;
                    new_checker.GetComponent<CheckerController>().checker_index = idx;
                    new_checker.GetComponent<CheckerController>().board_cell_infos = cell_infos;
                    new_checker.GetComponent<CheckerController>().board_cell_size = cell_size;
                }
            }
        }
    }
}