using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseYTest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float xMouseInput = Input.GetAxisRaw("Mouse X");
        float yMouseInput = Input.GetAxisRaw("Mouse Y");

        if (Mathf.Abs(yMouseInput) > 1) Debug.Log(xMouseInput + " " + yMouseInput);
    }
}
