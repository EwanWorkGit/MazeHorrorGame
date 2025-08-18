using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] Camera Camera;

    [SerializeField] float Speed = 5, SprintSpeed;

    CharacterController Controller;

    private void Start()
    {
        Controller = GetComponent<CharacterController>();
    }

    private void Update()
    {
        Vector3 forward = Camera.transform.forward;
        forward.y = 0f;
        Vector3 right = Camera.transform.right;
        right.y = 0f;

        Vector2 moveAxis;

        if (Input.GetKey(KeyCode.LeftShift))
        {
            moveAxis = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized * SprintSpeed * Time.deltaTime;
        }
        else
        {
            moveAxis = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized * Speed * Time.deltaTime;
        }
        
        Vector3 movement = right * moveAxis.x + forward * moveAxis.y;

        Controller.Move(movement);
    }
}
