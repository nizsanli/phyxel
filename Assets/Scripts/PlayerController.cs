using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Camera headCam;
    public Vector3 headRotation;

    public float moveSpeed;
    public float rotateSpeed;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.visible = false;
        headCam.transform.position = transform.position;
        headCam.transform.rotation = transform.rotation;

        headRotation = transform.rotation.eulerAngles;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        float zButton = Input.GetAxisRaw("Vertical");
        float xButton = Input.GetAxisRaw("Horizontal");

        GetComponent<CharacterController>().Move(
            Quaternion.Euler(0f, headRotation.y, 0f) * new Vector3(xButton, 0f, zButton) * moveSpeed);

        float xMouse = Input.GetAxisRaw("Mouse X");
        float yMouse = Input.GetAxisRaw("Mouse Y");

        headRotation += new Vector3(-yMouse, xMouse, 0f) * rotateSpeed;
        //headRotation += Input.GetMouseButton(1) ? new Vector3(-yMouse, xMouse, 0f) * 3f : Vector3.zero;
        //headRotation += new Vector3(0f, xMouse, 0f) * rotateSpeed;
        headCam.transform.rotation = Quaternion.Euler(headRotation);
        headCam.transform.position = transform.position;
    }
}
