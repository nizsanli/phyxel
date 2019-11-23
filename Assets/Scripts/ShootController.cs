using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootController : MonoBehaviour
{
    public Gun gun;

    float firedLast = float.MinValue;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(0) && Time.time - firedLast >= 1f / gun.fireRate)
        {
            gun.Shoot(transform.position + transform.forward * transform.localScale.z, transform.forward);
            firedLast = Time.time;
        }
    }
}
