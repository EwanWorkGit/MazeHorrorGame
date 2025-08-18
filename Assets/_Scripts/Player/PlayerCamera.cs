using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    [SerializeField] Transform PlayerBody;

    [SerializeField] float Sensitivity = 5f;

    float RotX, RotY;

    private void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked; 
    }

    private void Update()
    {
        Vector2 mouseAxis = new Vector2(Input.GetAxisRaw("Mouse X") * Sensitivity, Input.GetAxisRaw("Mouse Y") * Sensitivity);

        RotX -= mouseAxis.y;
        RotY += mouseAxis.x;
    }

    private void LateUpdate()
    {
        transform.rotation = Quaternion.Euler(RotX, PlayerBody.transform.eulerAngles.y, 0f);
        PlayerBody.rotation = Quaternion.Euler(PlayerBody.eulerAngles.x, RotY, 0f);
    }
}
