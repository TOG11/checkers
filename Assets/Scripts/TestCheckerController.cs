/* Copyright (C) 2022 Aiden Desjarlais
 * Copyright (C) 2022 Keir Yurkiw */

using UnityEngine;

namespace CheckerController
{
    public class TestCheckerController : MonoBehaviour
    {
        public GameObject board;
        public GameObject checker_prefab;

        private Vector3[] checker_pos = new Vector3[8*8];

        private bool
        is_even(uint i)
        {
            return i % 2 == 0;
        }

        private void
        Awake()
        {
            const float num_cells = 8.0f;
            float cell_size = board.GetComponent<Renderer>().bounds.size.x / num_cells;

            Vector3 bottom_left_cell_pos = new Vector3(
                    /* get position of the bottom-left corner of board and add cell
                     * offset to move to the center of the bottom-left cell */
                    -board.GetComponent<Renderer>().bounds.size.x/2.0f + cell_size/2.0f,
                    0.0f,
                    -board.GetComponent<Renderer>().bounds.size.z/2.0f + cell_size/2.0f);

            for (uint i = 0; i < num_cells; i++) {
                for (uint j = 0; j < num_cells; j++) {
                    checker_pos[i+j].x = bottom_left_cell_pos.x + (cell_size * i);
                    checker_pos[i+j].y = bottom_left_cell_pos.y;
                    checker_pos[i+j].z = bottom_left_cell_pos.z + (cell_size * j);

                    if (!is_even(i+j) && (j != 3 && j != 4))
                        Instantiate(checker_prefab).transform.position = checker_pos[i+j];
                }
            }
        }
    }
}
