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

        Cursor.lockState = CursorLockMode.Locked;

        headRotation = transform.rotation.eulerAngles;
    }

    // Update is called once per frame
    void Update()
    {
        float zButton = Input.GetAxisRaw("Vertical");
        float xButton = Input.GetAxisRaw("Horizontal");

        GetComponent<CharacterController>().Move(
          Quaternion.Euler(0f, headRotation.y, 0f) * new Vector3(xButton, 0f, zButton).normalized * moveSpeed * Time.deltaTime);

        //GetComponent<CharacterController>().Move(
        //    headCam.transform.rotation * new Vector3(xButton, 0f, zButton) * moveSpeed);

        float xMouse = Input.GetAxisRaw("Mouse X");
        float yMouse = Input.GetAxisRaw("Mouse Y");

        headRotation += new Vector3(-yMouse, xMouse, 0f) * rotateSpeed * Time.deltaTime;
        //headRotation += Input.GetMouseButton(1) ? new Vector3(-yMouse, xMouse, 0f) * 3f : Vector3.zero;
        //headRotation += new Vector3(0f, xMouse, 0f) * rotateSpeed;
        headCam.transform.rotation = Quaternion.Euler(headRotation);
        headCam.transform.position = transform.position;

        if (Input.GetMouseButton(0))
        {
            Ray shootRay = new Ray(headCam.transform.position + headCam.transform.forward * transform.GetComponent<CharacterController>().radius, headCam.transform.forward);
            RaycastHit hitInfo;

            if (Physics.Raycast(shootRay, out hitInfo, 500))
            {
                //Debug.Log(hitInfo.transform.name);
                hitInfo.transform.GetComponent<ChunkGroup>().RegisterHit(hitInfo, shootRay);
            }
        }

        if (Input.GetKeyUp(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
        }
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
