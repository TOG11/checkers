/* Copyright (C) 2022 Aiden Desjarlais
 * Copyright (C) 2022 Keir Yurkiw */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class CheckerSpawner : NetworkBehaviour
{
    public static CheckerSpawner singleton;
    public float checker_y_pos = 0.15f;
    public Material flashingCheckerMaterial;
    public Material client_flashingCheckerMaterial;
    public GameObject board;

    public static bool
    is_even(int i)
    {
        return i % 2 == 0;
    }


    private void Awake()
    {
        singleton = this;
    }

    private void Update()
    {
        flashingCheckerMaterial.SetFloat("_Metallic", (float)Mathf.Sin(Time.unscaledTime * 7.5f) * 0.35f);
        client_flashingCheckerMaterial.SetFloat("_Metallic", (float)Mathf.Sin(Time.unscaledTime * 7.5f) * 0.35f);
    }

    /* instantiate checkers and initialize cell_infos array  */
    public void
    SpawnCheckers(NetworkConnectionToClient conn)
    {
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


                float x_off = cell_size * j, y_off = cell_size * i;
                cell_infos[idx].bl.x = bottom_left_board_coord.x + x_off;
                cell_infos[idx].bl.y = bottom_left_board_coord.z + y_off;
                cell_infos[idx].br.x = bottom_left_board_coord.x + cell_size + x_off;
                cell_infos[idx].br.y = bottom_left_board_coord.z + y_off;
                cell_infos[idx].tl.x = bottom_left_board_coord.x + x_off;
                cell_infos[idx].tl.y = bottom_left_board_coord.z + cell_size + y_off;
                cell_infos[idx].tr.x = bottom_left_board_coord.x + cell_size + x_off;
                cell_infos[idx].tr.y = bottom_left_board_coord.z + cell_size + y_off;

                /* instantiate checkers */
                if (!is_even(i + j))
                {
                    cell_infos[idx].is_black = true;
                    if (i != 3 && i != 4)
                    {
                        CheckerData c;

                        if (i < 3)
                            c = new CheckerData().CreateChecker(player_checker, idx, BoardController.singleton.checkers.Count, 0, conn);
                        else
                            c = new CheckerData().CreateChecker(enemy_checker, idx, BoardController.singleton.checkers.Count, 1, conn);

                        BoardController.singleton.checkers.Add(c);
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

        public Vector2 bl, br, tl, tr;

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
}
