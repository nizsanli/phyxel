using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Camera headCam;
    public Vector3 headRotation;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.visible = false;
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
            headCam.transform.rotation * new Vector3(xButton, 0f, zButton) * 1.5f);

        float xMouse = Input.GetAxisRaw("Mouse X");
        float yMouse = Input.GetAxisRaw("Mouse Y");

        //headRotation += new Vector3(-yMouse, xMouse, 0f) * 3f;
        headRotation += new Vector3(0f, xMouse, 0f) * 3f;
        headCam.transform.rotation = Quaternion.Euler(headRotation);
        headCam.transform.position = transform.position;
    }
}
