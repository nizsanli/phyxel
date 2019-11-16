using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Camera headCam;
    public Vector3 headRotation;

    public float moveSpeed;
    public float rotateSpeed;

    private void Awake()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = -1;
    }

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
        float zButton = Input.GetAxisRaw("Vertical") * Time.deltaTime;
        float xButton = Input.GetAxisRaw("Horizontal") * Time.deltaTime;

        GetComponent<CharacterController>().Move(
          Quaternion.Euler(0f, headRotation.y, 0f) * new Vector3(xButton, 0f, zButton) * moveSpeed);

        //GetComponent<CharacterController>().Move(
        //    headCam.transform.rotation * new Vector3(xButton, 0f, zButton) * moveSpeed);

        float xMouse = Input.GetAxisRaw("Mouse X") * Time.deltaTime;
        float yMouse = Input.GetAxisRaw("Mouse Y") * Time.deltaTime;

        headRotation += new Vector3(-yMouse, xMouse, 0f) * rotateSpeed;
        //headRotation += Input.GetMouseButton(1) ? new Vector3(-yMouse, xMouse, 0f) * 3f : Vector3.zero;
        //headRotation += new Vector3(0f, xMouse, 0f) * rotateSpeed;
        headCam.transform.rotation = Quaternion.Euler(headRotation);
        headCam.transform.position = transform.position;
    }

    private void FixedUpdate()
    {
        /*float zButton = Input.GetAxisRaw("Vertical");
        float xButton = Input.GetAxisRaw("Horizontal");

        GetComponent<CharacterController>().Move(
          Quaternion.Euler(0f, headRotation.y, 0f) * new Vector3(xButton, 0f, zButton) * moveSpeed);

        //GetComponent<CharacterController>().Move(
        //    headCam.transform.rotation * new Vector3(xButton, 0f, zButton) * moveSpeed);

        float xMouse = Input.GetAxisRaw("Mouse X");
        float yMouse = Input.GetAxisRaw("Mouse Y");

        headRotation += new Vector3(-yMouse, xMouse, 0f) * rotateSpeed;
        //headRotation += Input.GetMouseButton(1) ? new Vector3(-yMouse, xMouse, 0f) * 3f : Vector3.zero;
        //headRotation += new Vector3(0f, xMouse, 0f) * rotateSpeed;
        headCam.transform.rotation = Quaternion.Euler(headRotation);
        headCam.transform.position = transform.position;*/
    }
}
