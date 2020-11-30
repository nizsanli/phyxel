using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Vector3 headRotation;

    public float moveSpeed;
    public float rotateSpeed;

    bool firstUpdateCall;

    private void Awake()
    {
        //QualitySettings.vSyncCount = 0;
        //Application.targetFrameRate = -1;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Start is called before the first frame update
    void Start()
    {
        headRotation = transform.rotation.eulerAngles;

        firstUpdateCall = true;
    }

    // Update is called once per frame
    void Update()
    {
        float forwardInput = Input.GetAxisRaw("Vertical");
        float strafeInput = Input.GetAxisRaw("Horizontal");

        float xMouseInput = Input.GetAxisRaw("Mouse X");
        float yMouseInput = Input.GetAxisRaw("Mouse Y");

        // locking the cursor moves it to center and is logged in the input axis
        // we need to perform a check to prevent a jump in the viewport; will only branch one time
        if (firstUpdateCall && MouseMoved())
        {
            firstUpdateCall = false;
            xMouseInput = yMouseInput = 0f;
        }

        Vector3 moveVec = Quaternion.Euler(0f, headRotation.y, 0f) * new Vector3(strafeInput, 0f, forwardInput).normalized * moveSpeed;
        GetComponent<CharacterController>().Move(moveVec * Time.deltaTime);

        headRotation += new Vector3(-yMouseInput, xMouseInput, 0f) * rotateSpeed * Time.deltaTime;
        transform.rotation = Quaternion.Euler(headRotation);

        if (Input.GetKeyUp(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
        }
    }

    private void FixedUpdate()
    {
    }

    private bool MouseMoved()
    {
        return Mathf.Abs(Input.GetAxisRaw("Mouse X")) > 0f || Mathf.Abs(Input.GetAxisRaw("Mouse Y")) > 0f;
    }
}
