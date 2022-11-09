/* Copyright {C} 2022 Aiden Desjarlais
 * Copyright {C} 2022 Keir Yurkiw
*/



using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEditor;

namespace CheckerController
{

    public class TestCheckerController : MonoBehaviour
    {
        public GameObject board;
        public GameObject checker_prefab;

        public float next_x = 0;
        private Vector3[] checker_pos = new Vector3[8 * 8];


        private bool
        is_even(int i)
        {
            return (i%2) == 0;
        }

        private void Awake()
        {
            const float num_cells = 8.0f;
            float cell_size = board.GetComponent<Renderer>().bounds.size.x / num_cells;

            float board_offset = board.GetComponent<Renderer>().bounds.size.x / 2.0f;
            GameObject temp_checker = Instantiate(checker_prefab);
            float checker_offset = temp_checker.GetComponent<Renderer>().bounds.size.x / 2.0f;
            Destroy(temp_checker);

            for (var i = 0; i < num_cells; i++)
            {
                for (var j = 0; j < num_cells; j++)
                {

                    // if ((j == 6 || j == 3) || is_even(j))
                    //     continue;

                    switch (j)
                    {
                        case 0:
                        case 2:
                        case 4:
                        case 5:
                        case 6:
                        case 8:
                            continue;
                    }

                    checker_pos[i].x = cell_size * i - board_offset + checker_offset + 0.1f;
                    checker_pos[i].z = cell_size * j - board_offset + checker_offset + 0.1f;
                    checker_pos[i].y = 0.233f;

                    if (!is_even(i))
                    {
                        checker_pos[i].z -= cell_size;
                    }
                        

                    //print(checker_pos[i].x + " " + checker_pos[i].y + " " + checker_pos[i].z);
                    Instantiate(checker_prefab).transform.position = checker_pos[i];
                }
            }
        }
    }
}
