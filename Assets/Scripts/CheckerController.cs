/* Copyright (C) 2022 Aiden Desjarlais
 * Copyright (C) 2022 Keir Yurkiw */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckerController : MonoBehaviour
{
    public LayerMask oob_layer;
    public LayerMask board_layer;

    public uint checker_index;
    public bool checker_ovr;
    public bool checker_clicked;
    //private bool is_king = false; TODO: add king checkers

    public float board_cell_size;
    public BoardController.cell_info[] board_cell_infos;

    private void
    OnMouseEnter()
    {
        checker_ovr = true;
    }

    private void
    OnMouseExit()
    {
        checker_ovr = false;
    }

    private void
    Update()
    {
        // out of bounds
        if (Physics.Raycast(transform.position, new Vector3(0, -25, 0), out var hit, Mathf.Infinity))
        {
            if (hit.collider.tag == "OOB")
            {
                //print("OOB");

                transform.position = board_cell_infos[checker_index].pos;
                checker_clicked = false;
            }
            Debug.DrawLine(transform.position, hit.point, Color.cyan);
        }

        // movement
        if (checker_clicked)
        {

            Vector3 mouse = Input.mousePosition;
            Ray castPoint = Camera.main.ScreenPointToRay(mouse);
            if (Physics.Raycast(castPoint, out var _hit, Mathf.Infinity, board_layer))
            {
                gameObject.transform.position = new Vector3(_hit.point.x, transform.position.y, _hit.point.z - 0.3f);
            }
        }

        // reset
        if (checker_ovr && Input.GetMouseButtonDown(0))
            checker_clicked = !checker_clicked;
        else if (Input.GetMouseButtonDown(0))
            checker_clicked = false;
    }
}
