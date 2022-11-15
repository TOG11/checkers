/* Copyright {C} 2022 Aiden Desjarlais */


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checker : MonoBehaviour
{

    public LayerMask oob_layer;
    public LayerMask board_layer;
    private Vector3 previous_position;
    private Vector3 start_position;

    public Vector3 checker_current_square;
    public Vector3 checker_last_square;

    public uint checker_index;
    public bool checker_ovr;
    public bool checker_clicked;
    //private bool is_king = false; TODO: add king checkers

    public float cell_size;
    public CheckerController.cell_info[] cell_info = new CheckerController.cell_info[8*8];

    private void
        Start()
    {
        start_position = transform.position;
    }

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
        //out of bounds
        if (Physics.Raycast(transform.position, new Vector3(0, -25, 0), out var hit, Mathf.Infinity))
        {
            if (hit.collider.tag == "OOB")
            {
                //print("OOB");
                transform.position = start_position;
                checker_clicked = false;
            }
            Debug.DrawLine(transform.position, hit.point, Color.cyan);
        }

        //movement
        if (checker_clicked)
        {

            Vector3 mouse = Input.mousePosition;
            Ray castPoint = Camera.main.ScreenPointToRay(mouse);
            if (Physics.Raycast(castPoint, out var _hit, Mathf.Infinity, board_layer))
            {
                gameObject.transform.position = new Vector3(_hit.point.x, transform.position.y, _hit.point.z - 0.3f);
            }
        }

        //reset
        if (checker_ovr && Input.GetMouseButtonDown(0))
        {
            if (checker_clicked)
                checker_clicked = false;
            else
                checker_clicked = true;
        }
        else if (Input.GetMouseButtonDown(0))
        {
            checker_clicked = false;
        }

        //check position
        if (transform.position != previous_position)
        {
            if (transform.position.z > previous_position.z + (cell_size * 0.5f))
            {
                print("Outside Home Square");
            }
        }
        previous_position = transform.position;
    }
}
